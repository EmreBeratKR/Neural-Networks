using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongGround : MonoBehaviour
    {
        [SerializeField] private GameObject _visual;
        [SerializeField] private bool _isPartOfLadder;


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

        public bool IsPartOfLadder()
        {
            return _isPartOfLadder;
        }
    }
}