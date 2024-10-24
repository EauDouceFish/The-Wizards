using MovementSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashingState : PlayerGroundedState
{
    private PlayerDashData dashData;
    private float dashToSprintTime = 0.1f;
    private int consecutiveDashesUsed;

    private bool shouldKeepRotating;
    // ��̿�ʼ��ʱ��
    private float startTime;

    public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
        dashData = movementData.DashData;
    }

    #region IState Methods

    // ���ó���ٶ�
    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementSpeedModifier = dashData.SpeedModifier;

        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

        stateMachine.ReusableData.RotationData = dashData.RotationData;

        // 
        AddForceOnTransitionFromStationaryState();

        // ������������������ת��
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

    // ˿���л���һ����������ֹ���߿����ܲ�
    public override void OnAnimationTransitionEvent() 
    {
        // �����������ص���ֹ״̬�������������������ܲ�
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

    // ����ֱ�ӴӾ�ֹ��ʼDash���߼����Ǿ�ֹ�򷵻�
    // ���Dash�ټ����ͻᵼ�¶�����һ�γ�̼���
    private void AddForceOnTransitionFromStationaryState()
    {
        if (stateMachine.ReusableData.MovementInput != Vector2.zero)
        {
            return;
        }

        Vector3 characterRotationDirection = stateMachine.Player.transform.forward;

        characterRotationDirection.y = 0;

        // Dashʱ��ת��˴�����Ҫ�ƶ����
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

            // ��ʹ����������������֮�����һ��ʱ�����ȴ
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

        // �κ������붼�ἤ�����ɵ����뻹δ�ͷ�
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
