using System;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerFallData
    {
        [field: SerializeField]
        [field: Range(1f, 15f)]
        public float FallSpeedLimit { get; private set; } = 15f;
    }

}
