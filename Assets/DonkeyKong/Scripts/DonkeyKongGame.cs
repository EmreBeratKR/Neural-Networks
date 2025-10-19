using Physics;
using UnityEngine;
using Random = System.Random;

namespace DonkeyKong
{
    public class DonkeyKongGame : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private DonkeyKongPlayer _playerPrefab;


        private DonkeyKongPrincess m_Princess;
        private DonkeyKongMonkey m_Monkey;
        private DonkeyKongGround[] m_Grounds;
        private DonkeyKongWall[] m_Walls;
        private DonkeyKongLadder[] m_Ladders;
        private Random m_Random;


        private void Awake()
        {
            Application.targetFrameRate = 60;

            m_Random = new Random(256);
            m_Princess = GetComponentInChildren<DonkeyKongPrincess>(true);
            m_Monkey = GetComponentInChildren<DonkeyKongMonkey>(true);
            m_Grounds = GetComponentsInChildren<DonkeyKongGround>(true);
            m_Walls = GetComponentsInChildren<DonkeyKongWall>(true);
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


        public DonkeyKongPrincess GetPrincess()
        {
            return m_Princess;
        }
        
        public DonkeyKongMonkey GetMonkey()
        {
            return m_Monkey;
        }
        
        public DonkeyKongGround[] GetGrounds()
        {
            return m_Grounds;
        }

        public DonkeyKongWall[] GetWalls()
        {
            return m_Walls;
        }

        public DonkeyKongLadder[] GetLadders()
        {
            return m_Ladders;
        }

        public bool RandomBool(float possibility)
        {
            return m_Random.NextDouble() <= possibility;
        }
    }
}