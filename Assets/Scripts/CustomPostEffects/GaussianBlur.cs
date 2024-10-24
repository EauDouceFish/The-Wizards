using System;

namespace UnityEngine.Rendering.Universal
{
    [VolumeComponentMenu("Custom Post-processing/Gaussian Blur")]
    sealed class GaussianBlur : VolumeComponent, IPostProcessComponent
    {

        [Range(0.0f, 1.0f), Tooltip("ģ��ǿ��")]
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.0f, 0.0f, 5.0f);

        public bool IsActive() => blurSize.value > 0;

        // �ݲ�֧����Ƭ��Ⱦ������������Ƭ���ݵ��µ�
        public bool IsTileCompatible() => false;
    }
}
