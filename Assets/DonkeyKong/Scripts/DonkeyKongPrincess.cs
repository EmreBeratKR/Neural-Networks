using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongPrincess : MonoBehaviour
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Vector2 _size;


        private void OnDrawGizmos()
        {
            var center = GetCenter();
            var size = GetSize();
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);
        }


        public Vector2 GetCenter()
        {
            return (Vector2) transform.position + _offset;
        }

        public Vector2 GetSize()
        {
            return _size;
        }
    }
}