using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongLadder : MonoBehaviour
    {
        [SerializeField] private Transform _bottom;
        [SerializeField] private Transform _top;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(GetTopRectCenter(), GetTopRectSize());
            
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(GetBottomRectCenter(), GetBottomRectSize());
        }


        public Vector2 GetTopRectCenter()
        {
            return _top.position;
        }
        
        public Vector2 GetTopRectSize()
        {
            return _top.localScale;
        }
        
        public Vector2 GetBottomRectCenter()
        {
            return _bottom.position;
        }
        
        public Vector2 GetBottomRectSize()
        {
            return _bottom.localScale;
        }
    }
}