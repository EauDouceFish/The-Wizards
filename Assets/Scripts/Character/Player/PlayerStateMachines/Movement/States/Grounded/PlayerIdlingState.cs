using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdlingState : PlayerGroundedState
{
    public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    // 静止状态玩家无法移动
    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = 0.0f;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StationaryForce;

        ResetVelocity();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            return;
        }

        // 有移动输入，进入移动状态
        OnMove();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!IsMovingHorizontally())
        {
            return;
        }

        ResetVelocity();
    }

    #endregion
}
