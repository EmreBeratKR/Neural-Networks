using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongGround : MonoBehaviour
    {
        public Vector2 GetCenter()
        {
            return transform.position;
        }

        public Vector2 GetSize()
        {
            return transform.localScale;
        }
    }
}