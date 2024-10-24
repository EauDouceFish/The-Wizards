using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem 
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        // ��Ҹ���״̬�������ݵķ�װ
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
            // ��˽�����Ը�ֵ�����壬���룬�������ʹ���ƶ�״̬��
            Rigidbody = GetComponent<Rigidbody>();

            Input = GetComponent<PlayerInput>();

            // �����gameObject��ʼ��������ײ������
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();

            MainCameraTransform = Camera.main.transform;

            movementStateMachine = new PlayerMovementStateMachine(this);
        }

        // �ֶθ��ĵ�ʱ��Ҳ��ͬʱ����Collider����
        private void OnValidate()
        {
            // �����gameObject��ʼ��������ײ������
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimensions();
        }

        private void Start()
        {
            // �����Ĭ��ΪIdle״̬
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

