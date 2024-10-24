using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementSystem;

public class PlayerFallingState : PlayerAirborneState
{
    private PlayerFallData fallData;

    public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        fallData = airborneData.FallData;
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = 0f;

        ResetVerticalVelocity();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        LimitVerticalVelocity();
    }
    #endregion

    #region Reusable Methods

    protected override void ResetSprintState()
    {   
        base.ResetSprintState();
    }

    #endregion

    #region Main Methods

    private void LimitVerticalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity(); 
        if(stateMachine.Player.Rigidbody.velocity.y >= -fallData.FallSpeedLimit)
        {
            return;
        }

        // ����������ٶ����ޣ���ԭ���ٶ�һ����������ʹ��ص�����
        Vector3 limitedVelocity = new Vector3(0f, -fallData.FallSpeedLimit - stateMachine.Player.Rigidbody.velocity.y, 0f);

        stateMachine.AddForce(limitedVelocity, ForceMode.VelocityChange);
    }

    #endregion
}