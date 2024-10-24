using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }

        // 1 左移layer位，与layerMask掩码做&比较，同位为1才返回1，否则返回0代表不包含该Layer
        public bool ContainsLayer(LayerMask layerMask, int layer)
        {
            return (1 << layer & layerMask) != 0;
        }

        public bool IsGroundLayer(int layer)
        {
            bool isGroundLayer = ContainsLayer(GroundLayer, layer);
            Logger.Log("IsGroundLayer: " + isGroundLayer);
            return isGroundLayer;
        }

    }

}
