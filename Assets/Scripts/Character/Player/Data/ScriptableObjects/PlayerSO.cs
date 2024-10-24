using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MovementSystem
{
    [CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
    public class PlayerSO : ScriptableObject
    {
        [field : SerializeField]
        public PlayerGroundedData PlayerGroundedData { get; private set; } 
        
        [field : SerializeField]
        public PlayerAirborneData PlayerAirborneData { get; private set; } 
    }
}
