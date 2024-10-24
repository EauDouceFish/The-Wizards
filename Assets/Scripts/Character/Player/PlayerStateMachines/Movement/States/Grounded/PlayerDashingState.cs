using MovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private float dashToSprintTime = 0.1f;
    private int consecutiveDashesUsed;

    private bool shouldKeepRotating;
    // 冲刺开始的时间
    private float startTime;

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = movementData.DashData;
    }

    #region IState Methods

    // 设置冲刺速度
    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        // 
        AddForceOnTransitionFromStationaryState();

        // 如果有输入则可以立刻转向
        shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

        UpdateConsecutiveDashes();
        startTime = Time.time;

    }

    public override void Exit()
    {
        base.Exit();

        SetBaseRotationData();
    }

    public override void Update()
    {
        base.Update();

        if (Time.time < startTime + dashToSprintTime)
        {
            return;
        }

        stateMachine.ChangeState(stateMachine.SprintingState); 
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (!shouldKeepRotating)
        {
            return;
        }

        RotateTowardsTargetRotation();
    }

    // 丝滑切换下一个动画：静止或者快速跑步
    public override void OnAnimationTransitionEvent() 
    {
        // 如果无输入则回到静止状态，有输入则连续快速跑步
        base.OnAnimationTransitionEvent();
        if (stateMachine.ReusableData.MovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);
            return;
        }
        stateMachine.ChangeState(stateMachine.SprintingState);
    }

    #endregion

    #region Main Methods

    // 处理直接从静止开始Dash的逻辑，非静止则返回
    // 如果Dash再加力就会导致额外多出一段冲刺加速
    private void AddForceOnTransitionFromStationaryState()
    {
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        Vector3 characterRotationDirection = stateMachine.Player.transform.forward;

        characterRotationDirection.y = 0;

        // Dash时候转向此处不需要移动相机
        UpdateTargetRotation(characterRotationDirection, false);

        stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
    }

    private void UpdateConsecutiveDashes()
    {
        if (IsConsecutive())
        {
            consecutiveDashesUsed = 0;
        }

        ++consecutiveDashesUsed;

        if (consecutiveDashesUsed == dashData.ConsecutiveDashesLimitAmount)
        {
            consecutiveDashesUsed = 0;

            // 在使用完可连续冲刺数量之后进入一段时间的冷却
            stateMachine.Player.Input.DisableActionFor(
                stateMachine.Player.Input.PlayerActions.Dash,
                dashData.DashLimitReachedCooldown
                );
        }
    }

    private bool IsConsecutive()
    {
        return Time.time < startTime + dashData.TimeToBeConsideredConsecutive;
    }

    #endregion

    #region Reusable Methods

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        // 任何新输入都会激活，即便旧的输入还未释放
        stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();

        stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
    }

    #endregion

    #region Input Methods

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        shouldKeepRotating = true;
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {

    }

    protected override void OnDashStarted(InputAction.CallbackContext context)
    {
    }

    #endregion
}
