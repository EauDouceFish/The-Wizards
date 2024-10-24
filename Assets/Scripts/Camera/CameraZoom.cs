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

        // 当前使用的是framingTransposer模式
        private CinemachineFramingTransposer framingTransposer;
        // 支持外部input输入的inputProvider
        private CinemachineInputProvider inputProvider;

        // 当前记录的相机和所需要的目标距离
        private float currentTargetDistance;

        private void Awake()
        {
            // 获取virtualCamera的控制
            framingTransposer = GetComponent<CinemachineVirtualCamera>().
                GetCinemachineComponent<CinemachineFramingTransposer>();

            inputProvider = GetComponent<CinemachineInputProvider>();
            
            // 初始化为默认距离
            currentTargetDistance = defaultDistance;
        }

        private void Update()
        {
            // 实时调整相机距离
            Zoom();
        }

        private void Zoom()
        {
            float zoomValue = inputProvider.GetAxisValue(2) * zoomSensitivity;
            currentTargetDistance = Mathf.Clamp(currentTargetDistance + zoomValue, minimumDistance, maximumDistance);

            // 获取virtualCamera到目标的距离并且赋值
            float currentDistance = framingTransposer.m_CameraDistance;
            if (currentDistance == currentTargetDistance) return;

            float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);
            framingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }

}

