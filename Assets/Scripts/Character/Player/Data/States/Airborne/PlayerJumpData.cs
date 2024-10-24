using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerJumpData
    {
        // 跳跃时候的旋转信息
        [field: SerializeField]
        public PlayerRotationData RotationData { get; private set; }

        // 地面距离检测
        [field: SerializeField]
        [field: Range(0, 5f)]
        public float JumpToGroundRayDistance { get; private set; } = 2.0f;

        // 在斜坡上向上或向下跳跃，速度会随着斜坡角度改变
        [field: SerializeField]
        public AnimationCurve JumpForcedModifierOnSlopeUpwards { get; private set; }
        
        [field: SerializeField]
        public AnimationCurve JumpForcedModifierOnSlopeDownwards { get; private set; }

        // 不同初速度起跳的力度不同
        [field : SerializeField] public Vector3 StationaryForce
        { get; private set; } = new Vector3( 0f, 5f, 0f );

        [field : SerializeField] public Vector3 WeakForce 
        {  get; private set; } = new Vector3( 1f, 5f, 1f );

        [field : SerializeField] public Vector3 MediumForce
        { get; private set; } = new Vector3(3.5f, 5f, 3.5f);

        [field : SerializeField] public Vector3 StrongForce
        { get; private set; } = new Vector3(5f, 5f, 5f);

        // 跳跃的垂直方向减速值
        [field: SerializeField]
        [field: Range(0, 10f)]
        public float DecelerationForce { get; private set; } = 1.5f;
    }
}
