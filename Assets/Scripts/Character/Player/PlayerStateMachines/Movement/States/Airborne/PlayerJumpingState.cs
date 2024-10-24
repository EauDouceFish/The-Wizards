using MovementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerAirborneState
{
    // ���������Ծ����Ҫ�ڿ�����ת�򰴼�����
    private bool shouldKeepRotating;

    private PlayerJumpData jumpData;

    private bool canStartFalling;

    public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        // ��airborne������Ծ���ݣ�֮��ͬ����״̬��
        jumpData = airborneData.JumpData;
        stateMachine.ReusableData.RotationData = jumpData.RotationData;
    }

    #region Reusable Methods

    // ��д������������ShouldSprintΪfalse
    protected override void ResetSprintState()
    {
    }

    #endregion

    #region IState Methods

    public override void Enter()
    {
        base.Enter(); 

        // Jumpʱ���������ƶ��ٶȣ����������������
        stateMachine.ReusableData.MovementSpeedModifier = 0;

        // Jump�ļ��٣��ý�ɫ���쵽�ﶥ�㣬�Եø�����Ȼ���ή����Ծ�߶ȣ�
        stateMachine.ReusableData.MovementDecelerateForce = jumpData.DecelerationForce;

        // �ƶ��ٶȲ�Ϊ0
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

        // �������y<0���ʹ���ʼ���䣬�л�������״̬�����򷵻�
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

    // slope�����ػ�ı�Jump�����ȣ�������Ҫ�ɶ���
    private void Jump()
    {
        Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

        // ��������ҳ�����Ծ
        Vector3 jumpDirection = stateMachine.Player.transform.forward;

        if (shouldKeepRotating)
        {
            Logger.Log("Should Keep Rotating!");
            jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
        }

        jumpForce.x *= jumpDirection.x;
        jumpForce.z *= jumpDirection.z;

        // ����б���ϵ�Jump�߼�:��ȡCollider���ģ����·���Ray����Ƿ���б����
        Vector3 capsuleColliderCenterInWorldSpace = 
            stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, jumpData.JumpToGroundRayDistance,
            stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {
            // ��ȡ�͵���֮��ļн�
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            if (IsMovingUp())
            {
                // ����Ƕȣ����ݽǶȶ�ȡ������Ϣ
                float forceModifier = jumpData.JumpForcedModifierOnSlopeUpwards.Evaluate(groundAngle);

                // ���µ���ˮƽ��Ծ����ˮƽ�ƶ�������ѣ�
                jumpForce.x *= forceModifier;
                // jumpForce.y *= forceModifier;
                jumpForce.z *= forceModifier;
            }

            if (IsMovingDown())
            {
                float forceModifier = jumpData.JumpForcedModifierOnSlopeDownwards.Evaluate(groundAngle);

                // ���µ�����ɫ��Ծ�߶ȣ�����Ȼ
                jumpForce.y *= forceModifier;
            }
        }

        // Jump��Ҫ�ܵ�����״̬Ӱ����ǵ�������
        ResetVelocity();

        // Logger.Log("JumpForce: " + jumpForce);
        stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    #endregion
}
