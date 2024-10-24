using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    // 需要高度，中心和半径即可定义Collider
    [Serializable]
    public class DefaultColliderData
    {
        [field: SerializeField] public float Height { get; private set; } = 1.0f;
        [field: SerializeField] public float CenterY { get; private set; } = 0.8f;
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
    }
}
