using UnityEngine;

namespace Capture_the_Flag
{
    public class Flag : MonoBehaviour
    {
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}