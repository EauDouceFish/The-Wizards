using System.Globalization;
using System;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerIsometric : NetworkBehaviour
{
    public static PlayerIsometric LocalInstance { get; private set; }

    private NavMeshAgent agent;
    private Camera mainCamera;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;

        GameInput.instance.OnRightClickAction += GameInput_OnRightClickAction;
        GameInput.instance.OnRightClickHold += GameInput_OnRightClickHold;
        GameInput.instance.OnRightClickRelease += GameInput_OnRightClickRelease;
    }

    private void GameInput_OnRightClickAction(object sender, EventArgs e)
    {
        MoveAgentToClickPosition();
        Debug.Log("RightClick");
    }

    private void GameInput_OnRightClickHold(object sender, EventArgs e)
    {
        // 长按时持续更新目标点
        MoveAgentToClickPosition();
    }

    private void GameInput_OnRightClickRelease(object sender, EventArgs e)
    {
        Logger.Log("Right Released");
    }

    private void MoveAgentToClickPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        throw new NotImplementedException();
    }
}
