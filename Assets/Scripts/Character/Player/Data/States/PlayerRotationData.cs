using System;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerRotationData
    {
        // 平滑转向时长
        [field: SerializeField]
        public Vector3 TargetRotationReachTime { get; private set; } 
    }
}
