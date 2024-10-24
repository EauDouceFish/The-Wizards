using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelEdgeDetectionRenderFeature : ScriptableRendererFeature
{
    // 实现该Feature需要使用一个Pass
    SobelEdgeDetectionPass sobelEdgeDetectionPass;

    // 初始化 SobelEdgeDetectionRenderFeature 的资源
    public override void Create()
    {
        // 这里定义了renderPassEvent的值为BeforeRenderingPostProcessing
        sobelEdgeDetectionPass = new SobelEdgeDetectionPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //sobelEdgeDetectionPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(sobelEdgeDetectionPass);
    }
}

public class SobelEdgeDetectionPass : ScriptableRenderPass
{
    // 宣告一个k_RenderTag以便在CommandBufferPool中指定获取
    static readonly string k_RenderTag = "Render SobelEdgeDetection Effects";

    // 创建需要给Shader赋值所用的变量缓冲区
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");

    // 隐式存储一张临时目标，可以在Shader里面没有显式定义
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetSobelEdgeDetection");
    
    // Shader所需要的其他属性
    static readonly int EdgesOnlyId = Shader.PropertyToID("_EdgesOnly");
    static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
    static readonly int BackgroundColorId = Shader.PropertyToID("_BackgroundColor");
    static readonly int OutlineStrengthId = Shader.PropertyToID("_OutlineStrength");

    // 声明该Pass效果需要用到的成员变量: 效果类，效果材质, currentTarget 指定渲染操作的目标（比如一个纹理或缓冲区）
    SobelEdgeDetection sobelEdgeDetection;
    Material sobelEdgeDetectionMaterial;
    // RenderTargetIdentifier currentTarget;

    // Pass的构造函数，定义了Pass时机，要用的Shader，要创建的材质
    public SobelEdgeDetectionPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        
        Shader shader = Shader.Find("PostProcess/SobelEdgeDetection");
        if(shader == null)
        {
            Logger.LogError("Target shader is not found.");
            return;
        }

        sobelEdgeDetectionMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    // 定义一个传入渲染操作目标的方法
/*    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }
*/
    // 这个函数最终定义该Pass的具体执行步骤
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        if (sobelEdgeDetectionMaterial == null)
        {
            Logger.LogError("Related material is not created!");
        }

        // 如果相机关闭了后处理则直接返回
        if (!renderingData.cameraData.postProcessEnabled) return;

        // 从内置VolumeManager获取自定义Volume的实例
        var stack = VolumeManager.instance.stack;
        sobelEdgeDetection = stack.GetComponent<SobelEdgeDetection>();

        // 如果这个volume未激活或者不存在就直接返回
        if (sobelEdgeDetection == null) return;
        if (!sobelEdgeDetection.IsActive()) return;

        // volume存在，材质绑定好，
        var cmdBuffer = CommandBufferPool.Get(k_RenderTag);
        Render(cmdBuffer, ref renderingData, src);

        context.ExecuteCommandBuffer(cmdBuffer);
        CommandBufferPool.Release(cmdBuffer);
    }

    private void Render(CommandBuffer cmdBuffer, ref RenderingData renderingData, RenderTargetIdentifier src)
    {
        // 获取当前相机数据，包括长宽，当前处理的帧，以及创建一个临时dest存储处理后的帧
        ref var cameraData = ref renderingData.cameraData;
        //var src = currentTarget;
        int dest = TempTargetId;

        var width = cameraData.camera.scaledPixelWidth;
        var height = cameraData.camera.scaledPixelHeight;

        // 类似OnRenderImage中给相关参数赋值
        sobelEdgeDetectionMaterial.SetFloat(EdgesOnlyId, sobelEdgeDetection.edgesOnly.value);
        sobelEdgeDetectionMaterial.SetColor(EdgeColorId, sobelEdgeDetection.edgeColor.value);
        sobelEdgeDetectionMaterial.SetColor(BackgroundColorId, sobelEdgeDetection.backgroundColor.value);
        sobelEdgeDetectionMaterial.SetFloat(OutlineStrengthId,sobelEdgeDetection.outlineStrength.value);

        int shaderPass = 0;

        // 将目标Shader的纹理设置成当前对象
        cmdBuffer.SetGlobalTexture(MainTexId, src);

        // 创建一个临时RT，保存在dest，宽为width 长为height，后处理不进行深度写入，需要像素精细（Point），使用默认格式存储图像
        cmdBuffer.GetTemporaryRT(dest, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);

        // 将当前处理的帧图像存入dest，用后处理材质以及对应Shader的shaderPass（当前为第零号Pass）处理之后再吐回来
        cmdBuffer.Blit(src, dest);
        cmdBuffer.Blit(dest, src, sobelEdgeDetectionMaterial, shaderPass);
    }
}

