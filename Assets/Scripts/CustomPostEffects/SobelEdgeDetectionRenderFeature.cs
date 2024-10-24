using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelEdgeDetectionRenderFeature : ScriptableRendererFeature
{
    // ʵ�ָ�Feature��Ҫʹ��һ��Pass
    SobelEdgeDetectionPass sobelEdgeDetectionPass;

    // ��ʼ�� SobelEdgeDetectionRenderFeature ����Դ
    public override void Create()
    {
        // ���ﶨ����renderPassEvent��ֵΪBeforeRenderingPostProcessing
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
    // ����һ��k_RenderTag�Ա���CommandBufferPool��ָ����ȡ
    static readonly string k_RenderTag = "Render SobelEdgeDetection Effects";

    // ������Ҫ��Shader��ֵ���õı���������
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");

    // ��ʽ�洢һ����ʱĿ�꣬������Shader����û����ʽ����
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetSobelEdgeDetection");
    
    // Shader����Ҫ����������
    static readonly int EdgesOnlyId = Shader.PropertyToID("_EdgesOnly");
    static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
    static readonly int BackgroundColorId = Shader.PropertyToID("_BackgroundColor");
    static readonly int OutlineStrengthId = Shader.PropertyToID("_OutlineStrength");

    // ������PassЧ����Ҫ�õ��ĳ�Ա����: Ч���࣬Ч������, currentTarget ָ����Ⱦ������Ŀ�꣨����һ������򻺳�����
    SobelEdgeDetection sobelEdgeDetection;
    Material sobelEdgeDetectionMaterial;
    // RenderTargetIdentifier currentTarget;

    // Pass�Ĺ��캯����������Passʱ����Ҫ�õ�Shader��Ҫ�����Ĳ���
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

    // ����һ��������Ⱦ����Ŀ��ķ���
/*    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }
*/
    // ����������ն����Pass�ľ���ִ�в���
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var src = renderingData.cameraData.renderer.cameraColorTargetHandle;
        if (sobelEdgeDetectionMaterial == null)
        {
            Logger.LogError("Related material is not created!");
        }

        // �������ر��˺�����ֱ�ӷ���
        if (!renderingData.cameraData.postProcessEnabled) return;

        // ������VolumeManager��ȡ�Զ���Volume��ʵ��
        var stack = VolumeManager.instance.stack;
        sobelEdgeDetection = stack.GetComponent<SobelEdgeDetection>();

        // ������volumeδ������߲����ھ�ֱ�ӷ���
        if (sobelEdgeDetection == null) return;
        if (!sobelEdgeDetection.IsActive()) return;

        // volume���ڣ����ʰ󶨺ã�
        var cmdBuffer = CommandBufferPool.Get(k_RenderTag);
        Render(cmdBuffer, ref renderingData, src);

        context.ExecuteCommandBuffer(cmdBuffer);
        CommandBufferPool.Release(cmdBuffer);
    }

    private void Render(CommandBuffer cmdBuffer, ref RenderingData renderingData, RenderTargetIdentifier src)
    {
        // ��ȡ��ǰ������ݣ�����������ǰ�����֡���Լ�����һ����ʱdest�洢������֡
        ref var cameraData = ref renderingData.cameraData;
        //var src = currentTarget;
        int dest = TempTargetId;

        var width = cameraData.camera.scaledPixelWidth;
        var height = cameraData.camera.scaledPixelHeight;

        // ����OnRenderImage�и���ز�����ֵ
        sobelEdgeDetectionMaterial.SetFloat(EdgesOnlyId, sobelEdgeDetection.edgesOnly.value);
        sobelEdgeDetectionMaterial.SetColor(EdgeColorId, sobelEdgeDetection.edgeColor.value);
        sobelEdgeDetectionMaterial.SetColor(BackgroundColorId, sobelEdgeDetection.backgroundColor.value);
        sobelEdgeDetectionMaterial.SetFloat(OutlineStrengthId,sobelEdgeDetection.outlineStrength.value);

        int shaderPass = 0;

        // ��Ŀ��Shader���������óɵ�ǰ����
        cmdBuffer.SetGlobalTexture(MainTexId, src);

        // ����һ����ʱRT��������dest����Ϊwidth ��Ϊheight�������������д�룬��Ҫ���ؾ�ϸ��Point����ʹ��Ĭ�ϸ�ʽ�洢ͼ��
        cmdBuffer.GetTemporaryRT(dest, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);

        // ����ǰ�����֡ͼ�����dest���ú�������Լ���ӦShader��shaderPass����ǰΪ�����Pass������֮�����»���
        cmdBuffer.Blit(src, dest);
        cmdBuffer.Blit(dest, src, sobelEdgeDetectionMaterial, shaderPass);
    }
}

