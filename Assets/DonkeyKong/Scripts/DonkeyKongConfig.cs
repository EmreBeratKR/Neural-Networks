using System;
using UnityEngine;

namespace DonkeyKong
{
    [Serializable]
    public struct DonkeyKongConfig
    {
        [Header("============PLAYER============")]
        public float playerHorizontalSpeed;
        public float playerGravity;
        public float playerJumpHeight;
        public float playerLadderClimbSpeed;
        [Header("============BARREL============")]
        public float barrelHorizontalSpeed;
        public float barrelLadderFallSpeed;
        public float barrelGravity;
        [Range(0f, 1f)]
        public float barrelLadderFallChance;
        public float barrelThrowInterval;


        public static DonkeyKongConfig Default => new()
        {
            playerHorizontalSpeed = 1.6f,
            playerGravity = 1.3f,
            playerJumpHeight = 0.6f,
            playerLadderClimbSpeed = 1.3f,
            barrelHorizontalSpeed = 2.5f,
            barrelLadderFallSpeed = 2.5f,
            barrelGravity = 1.5f,
            barrelLadderFallChance = 0.2f,
            barrelThrowInterval = 4.5f,
        };
    }
}