using MovementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : IState
{
    // �ƶ�״̬��ʹ�� �����ƶ�״̬��״̬��
    protected PlayerMovementStateMachine stateMachine;

    protected PlayerGroundedData movementData;

    protected PlayerAirborneData airborneData;

    // ��������ƶ�״̬����״̬����ȡ������ƶ�״̬��
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
    // IState�ӿ�ʵ�ֵķ����������˵�ǰ״̬�߼�

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
    // ������Ҫ˽�з����������������MovementState�µ���Ҫ�ƶ��߼�
    // �����ƶ��¼��߼�����ȡ����ֵ�뷽�������

    private void ReadMovementInput()
    {
        stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();
    }

    // �ƶ���ҵķ���
    private void Move()
    {
        // �Ȳ鿴����Ƿ��������ٶȣ��������ú��ƶ�����
        if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0.0f)
        {
            return;
        }
        Vector3 movementDirection = GetMovementInputDirection();

        // ���ƶ�������ת���
        float targetRotationYAngle = Rotate(movementDirection);

        // ��������ҵ�Ŀ����ת�Ƕ�����һ���µ�ǰ������
        // ������ϵͳ����������泯������򣬽�����Ӧ������������ƶ�����
        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

        // ��ȡ����ƶ�����֮���ȡ�˶��ٶ�
        float movementSpeed = GetMovementSpeed();

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

        // Unity�ĵ����Ƽ�ֱ�����ø����ٶȣ�����AddForce
        // ������velocity��˲ʱ�ģ�Addforce����һ��PhysicsUpdate�У���˳ʱ
        // ���������Ϸ��˵��AddForce����ͬʱ�ܵ������ʹ������ϵͳ������չ�Ը���
        stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);

        //Logger.Log("ChangedVelocity: " + GetPlayerHorizontalVelocity() + " while cur:" + currentPlayerHorizontalVelocity);
    }

    /// <summary>
    /// ��ת��ң��ȸ������ϣ���ĳ���Ȼ�������ת���ó���
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);

        RotateTowardsTargetRotation();

        return directionAngle;
    }


    // �����ת���������ϣ���ĳ���ͬʱ��������ת����ʱ�䣨������ת��
    private void UpdateTargetRotationData(float targetAngle)
    {
        stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0.0f;
    }

    // ���㲢�������ӵ�ǰ�������ת�Ƕ�
    private float AddCameraRotationToAngle(float angle)
    {
        // Ϊ�ƶ������������ת�Ƕȣ�ŷ���Ƕ�����Ԫ��rotation��
        angle += stateMachine.Player.MainCameraTransform.eulerAngles.y;
        if (angle > 360f)
        {
            angle -= 360f;
        }
        return angle;
    }

    // ����ŷ���ǵ�yawֵ
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
    // �ɸ��÷������Ա�����̳�

    protected void SetBaseRotationData()
    {
        stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

        stateMachine.ReusableData.TimeToReachTargetRotation =
            stateMachine.ReusableData.RotationData.TargetRotationReachTime;
    }

    // ��ȡƽ���ٶ�
    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;
        //Debug.Log("GetHorizontalV: " + playerHorizontalVelocity);
        
        // ������ֱ����Ϊ0
        playerHorizontalVelocity.y = 0.0f;
        
        return playerHorizontalVelocity;
    }

    // ��ȡ��ֱ�ٶ�������yֵΪ�ٶ�
    protected Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f); 
    }

    // ��ȡ�ƶ��ٶ�
    protected float GetMovementSpeed()
    {
        return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * 
            stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
    }

    // ��ȡ�ƶ�����
    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(stateMachine.ReusableData.MovementInput.x, 0.0f, stateMachine.ReusableData.MovementInput.y);
    }

    // �����ƽ��ת��Ŀ�귽��
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

        // ����ֱ�����õ���ʧЧ����
        // stateMachine.Player.transform.rotation = targetRotation;
        stateMachine.Player.Rigidbody.MoveRotation(targetRotation);

    }

    // ����ϣ���ĳ���
    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if(shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        // ���ֵ�ǰ���ϣ���ĳ��������
        if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    // ���ϣ���ĳ���
    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
    }

    // ˲ʱ�ı���������Ҿ�ֹ
    protected void ResetVelocity()
    {
        stateMachine.Player.Rigidbody.velocity = Vector3.zero;
    }

    // ���ô�ֱ����:ֻ����ˮƽ����
    protected void ResetVerticalVelocity()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();
        stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
    }
      
    // ����ˮƽ����:ֻ���ִ�ֱ����
    protected void ResetHorizontalVelocity()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();
        stateMachine.Player.Rigidbody.velocity = playerVerticalVelocity;
    }

    // �����״̬������walk��Ϊ��ת��
    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;
    }


    protected virtual void RemoveInputActionsCallbacks()
    {
        stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;
    }

    // ��ö�Ӧ�ƶ��ٶ�Ȼ�������ʵ��ֹͣ�ƶ�Ч��
    protected void DecelerateHorizontally()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();   

        // ����mass����,���м���
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

        // ʹ��minimum���ƹ����������涶���ȵ��µ�΢С����
        Vector2 playerHorizontalMovement = new Vector2(playerHorizontalVelocity.x, playerHorizontalVelocity.z);

        // �����ֵ(����)���ʱ�򷵻��ж�����ˮƽ�ƶ�
        return playerHorizontalMovement.magnitude > minimumMagnitude;
    }

    // С��0.05f���������
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
