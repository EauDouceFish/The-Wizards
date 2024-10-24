using System;

namespace UnityEngine.Rendering.Universal
{
    [VolumeComponentMenu("Custom Post-processing/Gaussian Blur")]
    sealed class GaussianBlur : VolumeComponent, IPostProcessComponent
    {

        [Range(0.0f, 1.0f), Tooltip("模糊强度")]
        public ClampedFloatParameter blurSize = new ClampedFloatParameter(0.0f, 0.0f, 5.0f);

        public bool IsActive() => blurSize.value > 0;

        // 暂不支持瓦片渲染，访问其他瓦片数据导致的
        public bool IsTileCompatible() => false;
    }
}
