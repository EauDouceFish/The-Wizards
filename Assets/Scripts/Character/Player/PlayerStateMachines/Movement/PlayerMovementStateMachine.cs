using MovementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : StateMachine 
{
    // 只用get，设置为只读属性
    public Player Player { get; }

    public PlayerStateReusableData ReusableData { get; }

    // Grounded
    public PlayerIdlingState IdlingState { get; }
    public PlayerDashingState DashingState { get; }
    public PlayerWalkingState WalkingState { get; }
    public PlayerRunningState RunningState { get; }
    public PlayerSprintingState SprintingState{ get; }

    public PlayerLightStoppingState LightStoppingState { get; }
    public PlayerMediumStoppingState MediumStoppingState { get; }
    public PlayerHardStoppingState HardStoppingState { get; }

    // Airborne
    public PlayerJumpingState JumpingState { get; }
    public PlayerFallingState FallingState { get; }

    // Swimming

    // 在状态机注册所有可能的状态
    public PlayerMovementStateMachine(Player player)
    {
        Player = player;

        ReusableData = new PlayerStateReusableData();

        IdlingState = new PlayerIdlingState(this);
        DashingState = new PlayerDashingState(this);
        WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        SprintingState = new PlayerSprintingState(this);

        LightStoppingState = new PlayerLightStoppingState(this);    
        MediumStoppingState = new PlayerMediumStoppingState(this);  
        HardStoppingState = new PlayerHardStoppingState(this);  

        JumpingState = new PlayerJumpingState(this);
        FallingState = new PlayerFallingState(this);
    }

    #region Encapsulated Methods For Players
    /// <summary>
    /// 给玩家添加力的方法: Player.Rigidbody.AddForce()
    /// </summary>
    /// <param name="force"></param>
    /// <param name="forceMode"></param>
    public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
    {
        Player.Rigidbody.AddForce(force, forceMode);
    }
    
    // 获得玩家速度
    public Vector3 GetPlayerVelocity()
    {
        return Player.Rigidbody.velocity;
    }

    // 获得玩家旋转
    public Quaternion GetPlayerRotation()
    {
        return Player.Rigidbody.rotation;
    } 
    
    // 获得玩家旋转--欧拉角
    public Vector3 GetPlayerRotationEular()
    {
        return Player.Rigidbody.rotation.eulerAngles;
    }

    #endregion
}
