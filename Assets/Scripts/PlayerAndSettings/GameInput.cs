using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnRightClickAction;
    public event EventHandler OnRightClickHold;
    public event EventHandler OnRightClickRelease;
    public event EventHandler OnPauseAction;

    private bool isRightClickHeld;

    private void Awake()
    {
        instance = this;

        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        playerInputActions.PlayerIsometric.Enable();
        playerInputActions.PlayerIsometric.RightClick.started += RightClick_started;
        playerInputActions.PlayerIsometric.RightClick.canceled += RightClick_canceled;
        playerInputActions.PlayerIsometric.Pause.performed += Pause_performed;
    }

    private void Update()
    {
        // ����Ҽ����ڰ�ס״̬���������� OnRightClickHold �¼�
        if (isRightClickHeld)
        {
            OnRightClickHold?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        playerInputActions.PlayerIsometric.RightClick.started -= RightClick_started;
        playerInputActions.PlayerIsometric.RightClick.canceled -= RightClick_canceled;
        playerInputActions.PlayerIsometric.Pause.performed -= Pause_performed;
        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void RightClick_started(InputAction.CallbackContext obj)
    {
        // ����Ҽ���ס״̬Ϊ true
        isRightClickHeld = true;
        OnRightClickAction?.Invoke(this, EventArgs.Empty);
    }

    private void RightClick_canceled(InputAction.CallbackContext obj)
    {
        // ����Ҽ���ס״̬Ϊ false
        isRightClickHeld = false;
        OnRightClickRelease?.Invoke(this, EventArgs.Empty);
    }
}
