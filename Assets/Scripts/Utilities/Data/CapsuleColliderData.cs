using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MovementSystem
{
    public class CapsuleColliderData
    {
        public CapsuleCollider Collider { get; private set; }

        public Vector3 ColliderCenterInLocalSpace { get; private set; }

        // 避免过于灵敏的空中状态,拓展一段检测距离
        public Vector3 ColliderVerticalExtents { get; private set; }

        // 从游戏对象获得CapsuleCollider并且初始化
        // 让这个部分可以复用
        public void Initialize(GameObject gameObject)
        {
            if (Collider != null) return;

            Collider = gameObject.GetComponent<CapsuleCollider>();

            UpdateColliderData();
        }

        public void UpdateColliderData()
        {
            ColliderCenterInLocalSpace = Collider.center;

            ColliderVerticalExtents = new Vector3(0f, Collider.bounds.extents.y, 0f);
        }
    }
}

