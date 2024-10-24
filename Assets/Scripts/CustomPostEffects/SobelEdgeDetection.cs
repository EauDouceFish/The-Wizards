using System;

namespace UnityEngine.Rendering.Universal
{
    [VolumeComponentMenu("Custom Post-processing/Sobel Edge Detection")]
    sealed class SobelEdgeDetection : VolumeComponent, IPostProcessComponent
    {

        [Range(0.0f, 1.0f), Tooltip("������ʾǿ��")]
        public ClampedFloatParameter edgesOnly = new ClampedFloatParameter(0f, 0.0f, 1.0f);

        [Tooltip("�����ɫ")]
        public ColorParameter edgeColor = new ColorParameter(Color.black);

        [Tooltip("������ɫ")]
        public ColorParameter backgroundColor = new ColorParameter(Color.white);

        [Range(0.0f, 10.0f), Tooltip("���ǿ��")]
        public ClampedFloatParameter outlineStrength = new ClampedFloatParameter(1.0f, 0.0f, 10.0f);

        public bool IsActive()=> edgesOnly.value > 0;

        public bool IsTileCompatible() => true;
    }
}
