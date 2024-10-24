using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MovementSystem
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();

            PlayerActions = InputActions.Player;  
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        // 使用协程禁止一段时间的输入
        public void DisableActionFor(InputAction inputAction, float seconds)
        {
            StartCoroutine(DisableAction(inputAction, seconds));
        }

        private IEnumerator DisableAction(InputAction inputAction, float seconds)
        {
            inputAction.Disable();

            yield return new WaitForSeconds(seconds);

            inputAction.Enable();
        }
    }
}