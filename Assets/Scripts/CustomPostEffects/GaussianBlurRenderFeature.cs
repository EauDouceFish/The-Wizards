using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderFeature : ScriptableRendererFeature
{
    // 实现该Feature需要使用一个Pass
    GaussianBlurPass gaussianBlurPass;

    // 初始化 SobelEdgeDetectionRenderFeature 的资源
    public override void Create()
    {
        // 这里定义了renderPassEvent的值为BeforeRenderingPostProcessing
        gaussianBlurPass = new GaussianBlurPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //sobelEdgeDetectionPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(gaussianBlurPass);
    }
}

public class GaussianBlurPass : ScriptableRenderPass
{
    // 必要的三步骤：
    // 宣告一个k_RenderTag以便在CommandBufferPool中指定获取
    static readonly string k_RenderTag = "Render GaussianBlur Effects";
    // 创建需要给Shader赋值所用的变量缓冲区
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    // 隐式存储一张临时目标，可以在Shader里面没有显式定义
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetGaussianBlur");

    // 声明所用Shader需要的其他属性
    static readonly int BlurSizeId = Shader.PropertyToID("_BlurSize");

    // 声明效果与材质
    GaussianBlur gaussianBlur;
    Material gaussianBlurMat;

    public GaussianBlurPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        Shader shader = Shader.Find("PostProcess/GaussianBlur");
        if(shader == null)
        {
            Logger.LogError("Targer Shader is not found.");
            return;
        }
        gaussianBlurMat = CoreUtils.CreateEngineMaterial(shader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        if (gaussianBlurMat == null)
        {
            Logger.LogError("Related material is not created!");
        }

        // 如果相机关闭了后处理则直接返回
        if (!renderingData.cameraData.postProcessEnabled) return;

        // 从内置VolumeManager获取自定义Volume的实例
        var stack = VolumeManager.instance.stack;
        gaussianBlur = stack.GetComponent<GaussianBlur>();

        // 如果这个volume未激活或者不存在就直接返回
        if (gaussianBlur == null) return;
        if (!gaussianBlur.IsActive()) return;

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
        int dest = TempTargetId;

        var width = cameraData.camera.scaledPixelWidth;
        var height = cameraData.camera.scaledPixelHeight;

        // 类似OnRenderImage中给相关参数赋值
        gaussianBlurMat.SetFloat(BlurSizeId, gaussianBlur.blurSize.value);

        int verticalPass = 0;
        int horizontalPass = 1;

        // 将目标Shader的纹理设置成当前对象
        cmdBuffer.SetGlobalTexture(MainTexId, src);

        // 创建一个临时RT，保存在dest，宽为width 长为height，后处理不进行深度写入，需要像素精细（Point），使用默认格式存储图像
        cmdBuffer.GetTemporaryRT(dest, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);

        // 将当前处理的帧图像vert模糊一遍存入dest
        // 之用后horizontal模糊一遍再吐回来
        // 总共迭代blurSize次
        for(int i=0; i< gaussianBlur.blurSize.value; i++)
        {
            cmdBuffer.Blit(src, dest, gaussianBlurMat, verticalPass);
            cmdBuffer.Blit(dest, src, gaussianBlurMat, horizontalPass);
        }
    }
}

