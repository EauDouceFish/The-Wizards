public class PlayerMediumStoppingState : PlayerStoppingState
{
    public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }
    #region IState Methods

    public override void Enter()
    {
        base.Enter();

        stateMachine.ReusableData.MovementDecelerateForce = movementData.StopData.MediumDecelerationForce;
    
        stateMachine.ReusableData.CurrentJumpForce= airborneData.JumpData.MediumForce;

    }

    #endregion

}
