using UnityEngine;

namespace MovementSystem
{
    /// <summary>
    /// �����ݹ����࣬�ṩ���Ը���ֵ������
    /// </summary>
    public class PlayerStateReusableData
    {
        public Vector2 MovementInput {  get; set; }
        public float MovementSpeedModifier { get; set; } = 1.0f;
        
        // б��ʱ�����΢����
        public float MovementOnSlopesSpeedModifier { get; set; } = 1.0f;
        
        // ֹͣʱ�����
        public float MovementDecelerateForce { get; set; } = 1.0f;    

        public bool ShouldWalk { get; set; }

        public bool ShouldSprint { get; set; }

        // ��ǰ���ϣ���ĳ���
        private Vector3 currentTargetRotation;
        // ��ת����ʱ��:����ͨ���ֱ�洢
        private Vector3 timeToReachTargetRotation;
        // ������ת��ǰ�Ľ��ٶȣ�����ʵ��ƽ����ת
        private Vector3 dampedTargetRotationCurrentVelocity;
        // ��¼��ת��������ʱ�䣬����ʱ�����������ֵ
        private Vector3 dampedTargetRotationPassedTime;

        public ref Vector3 CurrentTargetRotation
        {
            get
            {
                return ref currentTargetRotation;
            }
        }        
        
        public ref Vector3 TimeToReachTargetRotation
        {
            get
            {
                return ref timeToReachTargetRotation;
            }
        }
        public ref Vector3 DampedTargetRotationCurrentVelocity
        {
            get
            {
                return ref dampedTargetRotationCurrentVelocity;
            }
        }        
        
        public ref Vector3 DampedTargetRotationPassedTime
        {
            get
            {
                return ref dampedTargetRotationPassedTime;
            }
        }

        public Vector3 CurrentJumpForce { get; set; }

        // ��ǰ������ҵ���ת��Ϣ
        public PlayerRotationData RotationData { get; set; }
    }
}
