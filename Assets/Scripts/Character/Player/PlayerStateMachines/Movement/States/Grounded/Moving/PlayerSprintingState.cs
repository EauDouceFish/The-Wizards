using MovementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSprintingState : PlayerMovingState
{

    private PlayerSprintData sprintData;
    private bool keepSprinting;
    private float startTime;

    // 用于控制Sprint参数在其他状态起作用
    private bool shouldResetSprintState;

    public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        sprintData = movementData.SprintData;
    }

    #region IState Methods
    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = sprintData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce= airborneData.JumpData.StrongForce;

        shouldResetSprintState = true;

        startTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();

        if (shouldResetSprintState)
        {
            keepSprinting = false;
            stateMachine.ReusableData.ShouldSprint = false;
        }

    }

    // 长按持续冲刺快跑
    public override void Update()
    {
        base.Update();
        if (keepSprinting)
        {
            return;
        }

        if (Time.time < startTime + sprintData.SprintToRunTime)
        {
            return;
        }

        StopSprinting();
    }

    #endregion

    #region Main Methods

    // 停止冲刺的时候如果有进一步输入，会先切换到RunningState再进行判断
    private void StopSprinting()
    {
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdlingState);

            return;
        }
        stateMachine.ChangeState(stateMachine.RunningState);
    }

    #endregion


    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();

        stateMachine.Player.Input.PlayerActions.Sprint.performed -= OnSprintPerformed;
    }

    protected override void OnFall()
    {
        shouldResetSprintState = false;

        base.OnFall();
    }

    #endregion

    #region Input Methods


    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.ChangeState(stateMachine.HardStoppingState);
    }

    private void OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        keepSprinting = true;   
        stateMachine.ReusableData.ShouldSprint = true;
    }

    protected override void OnJumpStarted(InputAction.CallbackContext context)
    {
        // 跳跃时候不要重置冲刺状态
        shouldResetSprintState = false;

        base.OnJumpStarted(context);
    }
    #endregion
}
