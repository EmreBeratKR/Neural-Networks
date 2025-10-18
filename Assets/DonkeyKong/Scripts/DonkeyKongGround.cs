using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongGround : MonoBehaviour
    {
        [SerializeField] private GameObject _visual;


        private void Awake()
        {
            _visual.SetActive(false);
        }


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