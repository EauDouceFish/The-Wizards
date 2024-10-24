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

    // running״̬�»ָ�Ĭ���ƶ��ٶ�1.0f �������ƶ�״̬�µ�0.225f
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

        // �����Run״̬������������߼���
        if(!stateMachine.ReusableData.ShouldWalk)
        {
            return;
        }

        // ����ǳ�̽���״̬����һ��Run����֮���л�ΪWalk����Idle
        if (Time.time < startTime + sprintData.RunToWalkTime)
        {
            return;
        }
        StopRunning();
    }

    #endregion

    #region Main Methods

    // ֹͣRun֮��û������ͱ��־�ֹ ��Ȼ��Walk
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

    // �л����ܲ�״̬
    protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        base.OnWalkToggleStarted(context);

        stateMachine.ChangeState(stateMachine.WalkingState);
    }


    #endregion
}