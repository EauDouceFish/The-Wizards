using System;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerRotationData
    {
        // ƽ��ת��ʱ��
        [field: SerializeField]
        public Vector3 TargetRotationReachTime { get; private set; } 
    }
}
