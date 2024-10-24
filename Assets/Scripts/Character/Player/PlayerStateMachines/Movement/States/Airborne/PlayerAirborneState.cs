using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerMovementState
{
    public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        ResetSprintState();
    }

    #endregion

    #region Reusable Methods

    protected override void OnContactWithGorund(Collider collider)
    {
        base.OnContactWithGorund(collider);

        // һ���Ӵ���������л�ΪIdle״̬
        stateMachine.ChangeState(stateMachine.IdlingState);
    }

    // ͨ���ڿ���ʱ��Ҫ���ó��״̬
    protected virtual void ResetSprintState()
    {
        stateMachine.ReusableData.ShouldSprint = false;
    }

    #endregion
}
