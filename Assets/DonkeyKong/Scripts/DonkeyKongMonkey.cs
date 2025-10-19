using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongMonkey : MonoBehaviour
    {
        [SerializeField] private DonkeyKongGame _game;
        [SerializeField] private DonkeyKongBarrel _barrelPrefab;
        [SerializeField] private Transform _barrelSpawnPoint;


        private float m_LastThrowTime;
        

        private void Update()
        {
            const float barrelThrowInterval = 2.1f;
            var elapsedTime = Time.time - m_LastThrowTime;

            if (elapsedTime > barrelThrowInterval)
            {
                m_LastThrowTime = Time.time;
                ThrowBarrel();
            }
        }

        private void ThrowBarrel()
        {
            var position = _barrelSpawnPoint.position;
            var barrel = Instantiate(_barrelPrefab);
            barrel.SetGame(_game);
            barrel.SetPosition(position);
        }
    }
}