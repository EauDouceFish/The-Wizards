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

        // 给一个游戏物体，初始化其胶囊碰撞体
        public void Initialize(GameObject gameObject)
        {
            // 避免重复初始化数据
            if (CapsuleColliderData != null)
            {
                return;
            }

            CapsuleColliderData = new CapsuleColliderData();    

            CapsuleColliderData.Initialize(gameObject);
        }

        // 设置胶囊体可交互范围
        public void CalculateCapsuleColliderDimensions()
        {
            // 先用默认Collider参数进行初始构造（设置半径，降低Collider高度）
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

        // 提供外部修改胶囊体碰撞盒数据的方法
        public void SetCapsuleColliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }

        public void SetCapsuleColliderHeight(float height)
        {
            CapsuleColliderData.Collider.height = height;
        }

        // 重新计算胶囊体中心,注意height过小（小于半径时候）需要修正
        // 以默认高度中心+高度差的一半为大小，修改胶囊体中心
        public void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + colliderHeightDifference / 2f, 0f);

            CapsuleColliderData.Collider.center = newColliderCenter;
        }
    }
}
