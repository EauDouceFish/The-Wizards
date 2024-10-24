using MovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 封装一个Grounded状态，状态内即可使用相同shouldWalk逻辑
/// </summary>
public class PlayerGroundedState : PlayerMovementState
{
    private SlopeData slopeData;

    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        slopeData = stateMachine.Player.ColliderUtility.SlopeData;
    }
    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        UpdateShouldSprintState();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        FloatCapsule();
    }

    #endregion

    #region Main Methods

    // 让玩家不会被小坡卡住
    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // 投射射线同时忽略触发器
        if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, 
            slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // 计算坡度
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if(slopeSpeedModifier == 0f)
            {
                return;
            }

            float distanceToFloatPoint = stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y
                * stateMachine.Player.transform.localScale.y - hit.distance;

            if (distanceToFloatPoint == 0f)
            {
                return;
            }

            float amountToLift = distanceToFloatPoint * slopeData.StepReachForce - GetPlayerVerticalVelocity().y;
            
            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

            stateMachine.AddForce(liftForce, ForceMode.VelocityChange);
        }
    }

    // 使用AnimationCurve实现规律变化的速度控制
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    // 落地后没输入就终止冲刺
    private void UpdateShouldSprintState()
    {
        // 落地后非ShouldSprint或者有输入，都不更改ShouldSprint（继续冲刺）
        // 如果ShouldSprint但落地没有输入，修改状态为false
        if (!stateMachine.ReusableData.ShouldSprint)
        {
            return;
        }
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        stateMachine.ReusableData.ShouldSprint = false;
    }

    // 使用OverlapBox获取脚下一定范围内所有的碰撞体，如果数量大于0说明有在地上
    private bool ISThereGroundUnderneath()
    {
        BoxCollider groundCheckCollider = stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckCollider;
        Vector3 groundColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckCollider.bounds.center;

        Collider[] overlappedGroundColliders = Physics.OverlapBox(
            groundColliderCenterInWorldSpace, 
            groundCheckCollider.bounds.center,
            groundCheckCollider.transform.rotation,
            stateMachine.Player.LayerData.GroundLayer,
            QueryTriggerInteraction.Ignore
            );

        return overlappedGroundColliders.Length > 0;
    }


    #endregion

    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        // 记录移动状态切换
        stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

        // 记录冲刺状态开始
        stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;

        // 记录跳跃状态开始
        stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;

    }

  

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
        stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
    }


    // 封装状态切换:行走/跑步 shouldWalk切换
    protected virtual void OnMove()
    {
        if (stateMachine.ReusableData.ShouldSprint)
        {
            stateMachine.ChangeState(stateMachine.SprintingState);

            return;
        }

        // 使用Data数据
        if (stateMachine.ReusableData.ShouldWalk)
        {
            stateMachine.ChangeState(stateMachine.WalkingState);

            return;
        }
        stateMachine.ChangeState(stateMachine.RunningState);
    }

    // 离开平台，进入无接触地面状态
    protected override void OnContactWithGorundExited(Collider collider)
    {
        base.OnContactWithGorundExited(collider);

        if (ISThereGroundUnderneath())
        {
            return;
        }

        Vector3 capsuleColliderCenterWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleBottom = new Ray( capsuleColliderCenterWorldSpace - 
            stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

        if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _))
        {
            OnFall();
        }

    }

    protected virtual void OnFall()
    {
        stateMachine.ChangeState(stateMachine.FallingState);
    }

    #endregion

    #region Input Methods

    // 退出移动状态 返回静止
    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.IdlingState);
    }


    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.DashingState);
    }

    // Grounded状态可以随时进入跳跃状态
    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.JumpingState);
    }

    #endregion
}
