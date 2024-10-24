using MovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    // 移动状态下使用 管理移动状态的状态机
    protected PlayerMovementStateMachine stateMachine;

    protected PlayerGroundedData movementData;

    protected PlayerAirborneData airborneData;

    // 构造玩家移动状态，从状态机中取出玩家移动状态机
    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine;

        movementData = stateMachine.Player.Data.PlayerGroundedData;

        airborneData = stateMachine.Player.Data.PlayerAirborneData;

        InitializeData();
    }

    private void InitializeData()
    {
        SetBaseRotationData();
    }

    #region IState Methods
    // IState接口实现的方法，定义了当前状态逻辑

    public virtual void Enter()
    {
        Logger.Log("State: " + GetType().Name);

        AddInputActionsCallbacks();
    }


    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }


    public virtual void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }

    public virtual void OnAnimationEnterEvent()
    {
    }

    public virtual void OnAnimationExitEvent()
    {
    }

    public virtual void OnAnimationTransitionEvent()
    {
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGorund(collider);
            return;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGorundExited(collider);
            return;
        }
    }

    #endregion

    #region Main Methods
    // 父类主要私有方法，定义了玩家在MovementState下的主要移动逻辑
    // 处理移动事件逻辑：读取输入值与方向，添加力

    private void ReadMovementInput()
    {
        stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    // 移动玩家的方法
    private void Move()
    {
        // 先查看玩家是否有输入速度，有则设置好移动方向
        if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0.0f)
        {
            return;
        }
        Vector3 movementDirection = GetMovementInputDirection();

        // 向移动方向旋转玩家
        float targetRotationYAngle = Rotate(movementDirection);

        // 来根据玩家的目标旋转角度生成一个新的前进向量
        // 即告诉系统“玩家现在面朝这个方向，接下来应该往这个方向移动”。
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

        // 获取玩家移动方向之后获取运动速度
        float movementSpeed = GetMovementSpeed();

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

        // Unity文档不推荐直接设置刚体速度，而是AddForce
        // 但设置velocity是瞬时的，Addforce在下一个PhysicsUpdate中，不顺时
        // 对于这个游戏来说，AddForce可以同时受到多个力使用物理系统，可拓展性更好
        stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);

        //Logger.Log("ChangedVelocity: " + GetPlayerHorizontalVelocity() + " while cur:" + currentPlayerHorizontalVelocity);
    }

    /// <summary>
    /// 旋转玩家，先更新玩家希望的朝向，然后将玩家旋转到该朝向
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);

        RotateTowardsTargetRotation();

        return directionAngle;
    }


    // 相机旋转，更新玩家希望的朝向，同时重置已旋转过的时间（重新旋转）
    private void UpdateTargetRotationData(float targetAngle)
    {
        stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0.0f;
    }

    // 计算并给玩家添加当前相机的旋转角度
    private float AddCameraRotationToAngle(float angle)
    {
        // 为移动加上相机的旋转角度（欧拉角而非四元数rotation）
        angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;
        if (angle > 360f)
        {
            angle -= 360f;
        }
        return angle;
    }

    // 计算欧拉角的yaw值
    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        
        if (directionAngle < 0.0f)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    #endregion

    #region Reusable Methods
    // 可复用方法可以被子类继承

    protected void SetBaseRotationData()
    {
        stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

        stateMachine.ReusableData.TimeToReachTargetRotation =
            stateMachine.ReusableData.RotationData.TargetRotationReachTime;
    }

    // 获取平行速度
    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
        //Debug.Log("GetHorizontalV: " + playerHorizontalVelocity);
        
        // 修正竖直分量为0
        playerHorizontalVelocity.y = 0.0f;
        
        return playerHorizontalVelocity;
    }

    // 获取垂直速度向量，y值为速度
    protected Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f); 
    }

    // 获取移动速度
    protected float GetMovementSpeed()
    {
        return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * 
            stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
    }

    // 获取移动方向
    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(stateMachine.ReusableData.MovementInput.x, 0.0f, stateMachine.ReusableData.MovementInput.y);
    }

    // 让玩家平滑转向到目标方向
    protected void RotateTowardsTargetRotation()
    {
        float currentYAngle = stateMachine.GetPlayerRotationEular().y;
    
        if(currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(
            currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, 
            ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y,
            stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0.0f, smoothedYAngle, 0.0f);

        // 这里直接设置导致失效！！
        // stateMachine.Player.transform.rotation = targetRotation;
        stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

    }

    // 更新希望的朝向
    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if(shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        // 发现当前玩家希望的朝向更新了
        if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    // 获得希望的朝向
    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    // 瞬时改变速率让玩家静止
    protected void ResetVelocity()
    {
        stateMachine.Player.Rigidbody.velocity = Vector3.zero;
    }

    // 重置垂直分量:只保持水平分量
    protected void ResetVerticalVelocity()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
    }
      
    // 重置水平分量:只保持垂直分量
    protected void ResetHorizontalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
        stateMachine.Player.Rigidbody.velocity = playerVerticalVelocity;
    }

    // 基类的状态机控制walk行为的转换
    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
    }


    protected virtual void RemoveInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
    }

    // 获得对应移动速度然后减掉，实现停止移动效果
    protected void DecelerateHorizontally()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();   

        // 忽略mass属性,进行减速
        stateMachine.Player.Rigidbody.AddForce
            (-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerateForce, ForceMode.Acceleration);
        //Logger.Log("Decelerated: " + -playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerateForce);
    }
    
    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();   

        stateMachine.Player.Rigidbody.AddForce
            (-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerateForce, ForceMode.Acceleration);
        //Logger.Log("Decelerated: " + -playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerateForce);
    }

    protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

        // 使用minimum限制过滤物理引擎抖动等导致的微小噪音
        Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

        // 比最低值(噪音)大的时候返回判断正在水平移动
        return playerHorizontalMovement.magnitude > minimumMagnitude;
    }

    // 小于0.05f则忽略噪音
    protected bool IsMovingUp(float minimumVelocity = 0.05f)
    {
        return GetPlayerVerticalVelocity().y > minimumVelocity;
    }    
    
    protected bool IsMovingDown(float minimumVelocity = 0.05f)
    {
        return GetPlayerVerticalVelocity().y < -minimumVelocity;
    }

    protected virtual void OnContactWithGorund(Collider collider)
    {
    }


    protected virtual void OnContactWithGorundExited(Collider collider)
    {
    }

    #endregion


    #region Input Methods

    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;   
    }


    #endregion
}
