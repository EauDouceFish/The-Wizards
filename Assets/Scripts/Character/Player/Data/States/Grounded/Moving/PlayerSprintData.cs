using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerSprintData
    {
        [field : SerializeField]
        [field : Range(1f, 3f)]
        public float SpeedModifier { get; private set; } = 1.7f;
        
        [field : SerializeField]
        [field : Range(1f, 5f)]
        public float SprintToRunTime { get; private set; } = 1f;        

        // Sprint后如果ShouldWalk仍会短暂停留一段时间(0.5f)在Run状态
        [field : SerializeField]
        [field : Range(0f, 2f)]
        public float RunToWalkTime { get; private set; } = 0.5f;
    }
}
