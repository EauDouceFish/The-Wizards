using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] [Range(0f, 10f)] private float defaultDistance = 6.0f; 
        [SerializeField] [Range(0f, 10f)] private float minimumDistance = 0.8f; 
        [SerializeField] [Range(0f, 10f)] private float maximumDistance = 6.0f; 

        [SerializeField] [Range(0f, 10f)] private float smoothing = 4.0f; 
        [SerializeField] [Range(0f, 10f)] private float zoomSensitivity = 1.0f;

        // ��ǰʹ�õ���framingTransposerģʽ
        private CinemachineFramingTransposer framingTransposer;
        // ֧���ⲿinput�����inputProvider
        private CinemachineInputProvider inputProvider;

        // ��ǰ��¼�����������Ҫ��Ŀ�����
        private float currentTargetDistance;

        private void Awake()
        {
            // ��ȡvirtualCamera�Ŀ���
            framingTransposer = GetComponent<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();

            inputProvider = GetComponent<CinemachineInputProvider>();
            
            // ��ʼ��ΪĬ�Ͼ���
            currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            // ʵʱ�����������
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;
            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minimumDistance, maximumDistance);

            // ��ȡvirtualCamera��Ŀ��ľ��벢�Ҹ�ֵ
            float currentDistance = framingTransposer.m_CameraDistance;
            if (currentDistance == currentTargetDistance) return;

            float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);
            framingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }

}

