using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlurRenderFeature : ScriptableRendererFeature
{
    // ʵ�ָ�Feature��Ҫʹ��һ��Pass
    GaussianBlurPass gaussianBlurPass;

    // ��ʼ�� SobelEdgeDetectionRenderFeature ����Դ
    public override void Create()
    {
        // ���ﶨ����renderPassEvent��ֵΪBeforeRenderingPostProcessing
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
    // ��Ҫ�������裺
    // ����һ��k_RenderTag�Ա���CommandBufferPool��ָ����ȡ
    static readonly string k_RenderTag = "Render GaussianBlur Effects";
    // ������Ҫ��Shader��ֵ���õı���������
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    // ��ʽ�洢һ����ʱĿ�꣬������Shader����û����ʽ����
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetGaussianBlur");

    // ��������Shader��Ҫ����������
    static readonly int BlurSizeId = Shader.PropertyToID("_BlurSize");

    // ����Ч�������
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

        // �������ر��˺�����ֱ�ӷ���
        if (!renderingData.cameraData.postProcessEnabled) return;

        // ������VolumeManager��ȡ�Զ���Volume��ʵ��
        var stack = VolumeManager.instance.stack;
        gaussianBlur = stack.GetComponent<GaussianBlur>();

        // ������volumeδ������߲����ھ�ֱ�ӷ���
        if (gaussianBlur == null) return;
        if (!gaussianBlur.IsActive()) return;

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
        int dest = TempTargetId;

        var width = cameraData.camera.scaledPixelWidth;
        var height = cameraData.camera.scaledPixelHeight;

        // ����OnRenderImage�и���ز�����ֵ
        gaussianBlurMat.SetFloat(BlurSizeId, gaussianBlur.blurSize.value);

        int verticalPass = 0;
        int horizontalPass = 1;

        // ��Ŀ��Shader���������óɵ�ǰ����
        cmdBuffer.SetGlobalTexture(MainTexId, src);

        // ����һ����ʱRT��������dest����Ϊwidth ��Ϊheight�������������д�룬��Ҫ���ؾ�ϸ��Point����ʹ��Ĭ�ϸ�ʽ�洢ͼ��
        cmdBuffer.GetTemporaryRT(dest, width, height, 0, FilterMode.Point, RenderTextureFormat.Default);

        // ����ǰ�����֡ͼ��vertģ��һ�����dest
        // ֮�ú�horizontalģ��һ�����»���
        // �ܹ�����blurSize��
        for(int i=0; i< gaussianBlur.blurSize.value; i++)
        {
            cmdBuffer.Blit(src, dest, gaussianBlurMat, verticalPass);
            cmdBuffer.Blit(dest, src, gaussianBlurMat, horizontalPass);
        }
    }
}

