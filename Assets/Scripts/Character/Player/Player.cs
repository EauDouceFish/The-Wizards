using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem 
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        // 玩家各个状态所用数据的封装
        [field: Header("References")]
        [field : SerializeField] public PlayerSO Data {  get; private set; }
        [field : SerializeField] public PlayerCapsuleColliderUtility ColliderUtility {  get; private set; }
        [field : SerializeField] public PlayerLayerData LayerData { get; private set; }

        public PlayerInput Input { get; private set; }
        
        public Rigidbody Rigidbody { get; private set; }

        public Transform MainCameraTransform { get; private set; }

        private PlayerMovementStateMachine movementStateMachine;

        private void Awake()
        {
            // 给私有属性赋值：刚体，输入，相机，并使用移动状态机
            Rigidbody = GetComponent<Rigidbody>();

            Input = GetComponent<PlayerInput>();

            // 用玩家gameObject初始化胶囊碰撞体特性
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();

            MainCameraTransform = Camera.main.transform;

            movementStateMachine = new PlayerMovementStateMachine(this);
        }

        // 字段更改的时候也会同时更新Collider特性
        private void OnValidate()
        {
            // 用玩家gameObject初始化胶囊碰撞体特性
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }

        private void Start()
        {
            // 进入后默认为Idle状态
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }

        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            movementStateMachine.OnTriggerExit(collider);
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }
    }
}


