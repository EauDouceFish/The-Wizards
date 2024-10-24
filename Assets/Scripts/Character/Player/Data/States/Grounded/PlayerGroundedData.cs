using System;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField]
        [field: Range(0, 25f)]
        public float BaseSpeed { get; private set; } = 5.0f;   
        
        // 在此高度之上会被视为半空中,需要Fall
        [field: SerializeField]
        [field: Range(0, 5f)]
        public float GroundToFallRayDistance { get; private set; } = 1.0f;

        [field: SerializeField]
        public AnimationCurve SlopeSpeedAngles { get; private set; }

        [field: SerializeField]
        public PlayerRotationData BaseRotationData { get; private set; }

        [field: SerializeField]
        public PlayerWalkData WalkData { get; private set; }
        
        [field: SerializeField]
        public PlayerRunData RunData { get; private set; }

        [field: SerializeField]
        public PlayerSprintData SprintData { get; private set; }

        [field: SerializeField]
        public PlayerDashData DashData { get; private set; }

        [field: SerializeField]
        public PlayerStopData StopData { get; private set; }
    }
}

