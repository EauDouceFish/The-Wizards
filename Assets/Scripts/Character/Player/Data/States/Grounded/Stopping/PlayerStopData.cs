using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [Serializable]
    public class PlayerStopData
    {
        [field: SerializeField]
        [field: Range(0f, 15f)]
        public float LightDecelerationForce { get; private set; } = 1.0f;

        [field: SerializeField]
        [field: Range(0f, 15f)]
        public float MediumDecelerationForce { get; private set; } = 3.5f;

        [field: SerializeField]
        [field: Range(0f, 15f)]
        public float HardDecelerationForce { get; private set; } = 5.0f;
    }
}
