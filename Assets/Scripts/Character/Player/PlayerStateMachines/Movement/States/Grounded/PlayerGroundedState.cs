using MovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ��װһ��Grounded״̬��״̬�ڼ���ʹ����ͬshouldWalk�߼�
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

    // ����Ҳ��ᱻС�¿�ס
    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace =
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        // Ͷ������ͬʱ���Դ�����
        if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, 
            slopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // �����¶�
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

    // ʹ��AnimationCurveʵ�ֹ��ɱ仯���ٶȿ���
    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

        stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    // ��غ�û�������ֹ���
    private void UpdateShouldSprintState()
    {
        // ��غ��ShouldSprint���������룬��������ShouldSprint��������̣�
        // ���ShouldSprint�����û�����룬�޸�״̬Ϊfalse
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

    // ʹ��OverlapBox��ȡ����һ����Χ�����е���ײ�壬�����������0˵�����ڵ���
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

        // ��¼�ƶ�״̬�л�
        stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;

        // ��¼���״̬��ʼ
        stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;

        // ��¼��Ծ״̬��ʼ
        stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;

    }

  

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;
        stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
    }


    // ��װ״̬�л�:����/�ܲ� shouldWalk�л�
    protected virtual void OnMove()
    {
        if (stateMachine.ReusableData.ShouldSprint)
        {
            stateMachine.ChangeState(stateMachine.SprintingState);

            return;
        }

        // ʹ��Data����
        if (stateMachine.ReusableData.ShouldWalk)
        {
            stateMachine.ChangeState(stateMachine.WalkingState);

            return;
        }
        stateMachine.ChangeState(stateMachine.RunningState);
    }

    // �뿪ƽ̨�������޽Ӵ�����״̬
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

    // �˳��ƶ�״̬ ���ؾ�ֹ
    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.IdlingState);
    }


    protected virtual void OnDashStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.DashingState);
    }

    // Grounded״̬������ʱ������Ծ״̬
    protected virtual void OnJumpStarted(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.JumpingState);
    }

    #endregion
}
