using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongGame : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private DonkeyKongPlayer _playerPrefab;
        
        
        private DonkeyKongGround[] m_Grounds;
        private DonkeyKongLadder[] m_Ladders;


        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            m_Grounds = GetComponentsInChildren<DonkeyKongGround>(true);
            m_Ladders = GetComponentsInChildren<DonkeyKongLadder>(true);
            CreatePlayer();
        }

        private void CreatePlayer()
        {
            var startPosition = _startPoint.position;
            var player = Instantiate(_playerPrefab);
            player.SetGame(this);
            player.SetPosition(startPosition);
        }


        public DonkeyKongGround[] GetGrounds()
        {
            return m_Grounds;
        }

        public DonkeyKongLadder[] GetLadders()
        {
            return m_Ladders;
        }
    }
}