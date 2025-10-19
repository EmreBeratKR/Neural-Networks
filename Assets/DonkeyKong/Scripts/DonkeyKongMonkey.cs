using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongMonkey : MonoBehaviour
    {
        [SerializeField] private DonkeyKongGame _game;
        [SerializeField] private DonkeyKongBarrel _barrelPrefab;
        [SerializeField] private Transform _barrelSpawnPoint;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                var position = _barrelSpawnPoint.position;
                var barrel = Instantiate(_barrelPrefab);
                barrel.SetGame(_game);
                barrel.SetPosition(position);
            }
        }
    }
}