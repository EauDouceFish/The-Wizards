using System;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility
    {
        [field : SerializeField] public CapsuleColliderUtility CapsuleColliderUtility { get; private set; }

        [field : SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }
    }
}
