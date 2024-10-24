using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class CapsuleColliderUtility 
    {
        public CapsuleColliderData CapsuleColliderData { get; private set; }

        [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }

        [field: SerializeField] public SlopeData SlopeData { get; private set; }

        // ��һ����Ϸ���壬��ʼ���佺����ײ��
        public void Initialize(GameObject gameObject)
        {
            // �����ظ���ʼ������
            if (CapsuleColliderData != null)
            {
                return;
            }

            CapsuleColliderData = new CapsuleColliderData();    

            CapsuleColliderData.Initialize(gameObject);
        }

        // ���ý�����ɽ�����Χ
        public void CalculateCapsuleColliderDimensions()
        {
            // ����Ĭ��Collider�������г�ʼ���죨���ð뾶������Collider�߶ȣ�
            SetCapsuleColliderRadius(DefaultColliderData.Radius);
            SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage));

            RecalculateCapsuleColliderCenter();

            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;
            if (halfColliderHeight < CapsuleColliderData.Collider.radius)
            {
                SetCapsuleColliderRadius(halfColliderHeight);
            }

            CapsuleColliderData.UpdateColliderData();
        }

        // �ṩ�ⲿ�޸Ľ�������ײ�����ݵķ���
        public void SetCapsuleColliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }

        public void SetCapsuleColliderHeight(float height)
        {
            CapsuleColliderData.Collider.height = height;
        }

        // ���¼��㽺��������,ע��height��С��С�ڰ뾶ʱ����Ҫ����
        // ��Ĭ�ϸ߶�����+�߶Ȳ��һ��Ϊ��С���޸Ľ���������
        public void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + colliderHeightDifference / 2f, 0f);

            CapsuleColliderData.Collider.center = newColliderCenter;
        }
    }
}