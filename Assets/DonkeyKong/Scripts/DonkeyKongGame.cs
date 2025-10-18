using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongGame : MonoBehaviour
    {
        [SerializeField] private DonkeyKongPlayer _playerPrefab;
        
        
        private DonkeyKongGround[] m_Grounds;


        private void Awake()
        {
            m_Grounds = GetComponentsInChildren<DonkeyKongGround>(true);
            CreatePlayer();
        }

        private void CreatePlayer()
        {
            var player = Instantiate(_playerPrefab);
            player.SetGame(this);
        }


        public DonkeyKongGround[] GetGrounds()
        {
            return m_Grounds;
        }
    }
}