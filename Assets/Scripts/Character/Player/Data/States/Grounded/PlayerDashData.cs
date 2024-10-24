using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerDashData
    {
        [field: SerializeField]
        [field: Range(1f, 3f)]
        public float SpeedModifier { get; private set; } = 2f;
        
        [field: SerializeField]
        public PlayerRotationData RotationData { get; private set; }

        [field: SerializeField]
        [field: Range(0f, 2f)]
        public float TimeToBeConsideredConsecutive { get; private set; } = 1f;

        // 最多可支持连续冲刺的次数
        [field: SerializeField]
        [field: Range(1f, 10f)]
        public float ConsecutiveDashesLimitAmount { get; private set; } = 5f;
        
        // 连续冲刺后禁止使用冲刺的时间
        [field: Range(1f, 3f)]
        public float DashLimitReachedCooldown { get; private set; } = 1.75f;
    }

}