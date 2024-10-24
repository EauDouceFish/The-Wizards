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
        // 如果右键处于按住状态，持续触发 OnRightClickHold 事件
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
        // 标记右键按住状态为 true
        isRightClickHeld = true;
        OnRightClickAction?.Invoke(this, EventArgs.Empty);
    }

    private void RightClick_canceled(InputAction.CallbackContext obj)
    {
        // 标记右键按住状态为 false
        isRightClickHeld = false;
        OnRightClickRelease?.Invoke(this, EventArgs.Empty);
    }
}
