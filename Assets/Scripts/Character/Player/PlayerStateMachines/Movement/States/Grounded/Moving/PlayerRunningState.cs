using MovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunningState : PlayerMovingState
{
    private PlayerSprintData sprintData;

    private float startTime;

    public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    #region IState Methods

    // running状态下恢复默认移动速度1.0f 而不是移动状态下的0.225f
    public override void Enter()
    {
        base.Enter();
        stateMachine.ReusableData.MovementSpeedModifier = movementData.RunData.SpeedModifier;
        stateMachine.ReusableData.CurrentJumpForce= airborneData.JumpData.MediumForce;
        startTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        // 如果是Run状态，就无需更新逻辑了
        if(!stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        // 如果是冲刺结束状态，看一下Run过渡之后切换为Walk还是Idle
        if (Time.time < startTime + sprintData.RunToWalkTime)
        {
            return;
        }
        StopRunning();
    }

    #endregion

    #region Main Methods

    // 停止Run之后没有输入就保持静止 不然就Walk
    public void StopRunning()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdlingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.WalkingState);
    }

    #endregion

    #region Input Methods


    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.MediumStoppingState);
    }

    // 切换到跑步状态
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);

        stateMachine.ChangeState(stateMachine.WalkingState);
    }


    #endregion
}