using UnityEngine;

namespace MovementSystem
{
    /// <summary>
    /// 纯数据管理类，提供可以更改值的引用
    /// </summary>
    public class PlayerStateReusableData
    {
        public Vector2 MovementInput {  get; set; }
        public float MovementSpeedModifier { get; set; } = 1.0f;
        
        // 斜坡时候会略微减速
        public float MovementOnSlopesSpeedModifier { get; set; } = 1.0f;
        
        // 停止时候减速
        public float MovementDecelerateForce { get; set; } = 1.0f;    

        public bool ShouldWalk { get; set; }

        public bool ShouldSprint { get; set; }

        // 当前玩家希望的朝向
        private Vector3 currentTargetRotation;
        // 旋转所需时间:几个通道分别存储
        private Vector3 timeToReachTargetRotation;
        // 阻尼旋转当前的角速度，用于实现平滑旋转
        private Vector3 dampedTargetRotationCurrentVelocity;
        // 记录旋转所经过的时间，与总时长结合来做插值
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

        // 当前控制玩家的旋转信息
        public PlayerRotationData RotationData { get; set; }
    }
}
