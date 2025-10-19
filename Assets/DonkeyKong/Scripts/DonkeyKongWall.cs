using System;
using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongWall : MonoBehaviour
    {
        public Vector2 GetCenter()
        {
            return transform.position;
        }

        public Vector2 GetSize()
        {
            return transform.localScale;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawCube(GetCenter(), GetSize());
        }
    }
}