using MovementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerAirborneState
{
    // 控制玩家跳跃方向不要在空中旋转向按键方向
    private bool shouldKeepRotating;

    private PlayerJumpData jumpData;

    private bool canStartFalling;

    public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        // 从airborne类获得跳跃数据，之后同步给状态机
        jumpData = airborneData.JumpData;
        stateMachine.ReusableData.RotationData = jumpData.RotationData;
    }

    #region Reusable Methods

    // 覆写方法，不重置ShouldSprint为false
    protected override void ResetSprintState()
    {
    }

    #endregion

    #region IState Methods

    public override void Enter()
    {
        base.Enter(); 

        // Jump时候不允许有移动速度，而是依靠自身的力
        stateMachine.ReusableData.MovementSpeedModifier = 0;

        // Jump的减速，让角色更快到达顶点，显得更加自然（会降低跳跃高度）
        stateMachine.ReusableData.MovementDecelerateForce = jumpData.DecelerationForce;

        // 移动速度不为0
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        Jump();
    }

    public override void Exit()
    {
        base.Exit();
        SetBaseRotationData();

        canStartFalling = false;
    }

    public override void Update()
    {
        base.Update();

        if (!canStartFalling && IsMovingUp(0f))
        {
            canStartFalling = true;
        }

        // 如果空中y<0，就代表开始下落，切换到下落状态，否则返回
        if (!canStartFalling || GetPlayerVerticalVelocity().y > 0)
        {
            return;
        }
        stateMachine.ChangeState(stateMachine.FallingState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (shouldKeepRotating)
        {
            RotateTowardsTargetRotation();
        }

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    #endregion

    #region Main Methods

    // slope等因素会改变Jump的力度，所以需要可定义
    private void Jump()
    {
        Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

        // 限制向玩家朝向跳跃
        Vector3 jumpDirection = stateMachine.Player.transform.forward;

        if (shouldKeepRotating)
        {
            Logger.Log("Should Keep Rotating!");
            jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }

        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;

        // 处理斜坡上的Jump逻辑:获取Collider中心，向下发射Ray检测是否在斜坡上
        Vector3 capsuleColliderCenterInWorldSpace = 
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, jumpData.JumpToGroundRayDistance,
            stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // 获取和地面之间的夹角
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            if (IsMovingUp())
            {
                // 传入角度，根据角度读取曲线信息
                float forceModifier = jumpData.JumpForcedModifierOnSlopeUpwards.Evaluate(groundAngle);

                // 上坡调整水平跳跃力（水平移动变得困难）
                jumpForce.x *= forceModifier;
                // jumpForce.y *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            if (IsMovingDown())
            {
                float forceModifier = jumpData.JumpForcedModifierOnSlopeDownwards.Evaluate(groundAngle);

                // 下坡调整角色跳跃高度，更自然
                jumpForce.y *= forceModifier;
            }
        }

        // Jump不要受到其他状态影响而是单独处理
        ResetVelocity();

        // Logger.Log("JumpForce: " + jumpForce);
        stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    #endregion
}
