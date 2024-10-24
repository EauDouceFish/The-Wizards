using System;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerTriggerColliderData
    {
        /// <summary>
        /// һ��BoxCollider���ڼ�������ײ,�������Ա��⿨��΢С��϶
        /// </summary>
        [field: SerializeField]
        public BoxCollider GroundCheckCollider { get; private set; }
    }
}
