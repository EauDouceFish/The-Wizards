using System;

namespace UnityEngine.Rendering.Universal
{
    [VolumeComponentMenu("Custom Post-processing/Sobel Edge Detection")]
    sealed class SobelEdgeDetection : VolumeComponent, IPostProcessComponent
    {

        [Range(0.0f, 1.0f), Tooltip("±³¾°ÏÔÊ¾Ç¿¶È")]
        public ClampedFloatParameter edgesOnly = new ClampedFloatParameter(0f, 0.0f, 1.0f);

        [Tooltip("Ãè±ßÑÕÉ«")]
        public ColorParameter edgeColor = new ColorParameter(Color.black);

        [Tooltip("±³¾°ÑÕÉ«")]
        public ColorParameter backgroundColor = new ColorParameter(Color.white);

        [Range(0.0f, 10.0f), Tooltip("Ãè±ßÇ¿¶È")]
        public ClampedFloatParameter outlineStrength = new ClampedFloatParameter(1.0f, 0.0f, 10.0f);

        public bool IsActive()=> edgesOnly.value > 0;

        public bool IsTileCompatible() => true;
    }
}
