using Unity.Android.Types;

public class PlayerLightStoppingState : PlayerStoppingState
{
    public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementDecelerateForce = movementData.StopData.LightDecelerationForce;
    
        stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
    }

    #endregion
}
