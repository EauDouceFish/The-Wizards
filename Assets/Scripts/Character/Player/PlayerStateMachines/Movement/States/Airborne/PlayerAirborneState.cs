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

        // 一旦接触到地面就切换为Idle状态
        stateMachine.ChangeState(stateMachine.IdlingState);
    }

    // 通常在空中时候，要重置冲刺状态
    protected virtual void ResetSprintState()
    {
        stateMachine.ReusableData.ShouldSprint = false;
    }

    #endregion
}
