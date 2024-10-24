using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerJumpData
    {
        // ��Ծʱ�����ת��Ϣ
        [field: SerializeField]
        public PlayerRotationData RotationData { get; private set; }

        // ���������
        [field: SerializeField]
        [field: Range(0, 5f)]
        public float JumpToGroundRayDistance { get; private set; } = 2.0f;

        // ��б�������ϻ�������Ծ���ٶȻ�����б�½Ƕȸı�
        [field: SerializeField]
        public AnimationCurve JumpForcedModifierOnSlopeUpwards { get; private set; }
        
        [field: SerializeField]
        public AnimationCurve JumpForcedModifierOnSlopeDownwards { get; private set; }

        // ��ͬ���ٶ����������Ȳ�ͬ
        [field : SerializeField] public Vector3 StationaryForce
        { get; private set; } = new Vector3( 0f, 5f, 0f );

        [field : SerializeField] public Vector3 WeakForce 
        {  get; private set; } = new Vector3( 1f, 5f, 1f );

        [field : SerializeField] public Vector3 MediumForce
        { get; private set; } = new Vector3(3.5f, 5f, 3.5f);

        [field : SerializeField] public Vector3 StrongForce
        { get; private set; } = new Vector3(5f, 5f, 5f);

        // ��Ծ�Ĵ�ֱ�������ֵ
        [field: SerializeField]
        [field: Range(0, 10f)]
        public float DecelerationForce { get; private set; } = 1.5f;
    }
}
