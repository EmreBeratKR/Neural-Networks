using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = System.Random;

namespace DonkeyKong
{
    public class DonkeyKongGame : MonoBehaviour,
        IGeneticAlgorithmEnvironment
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private DonkeyKongPlayer _playerPrefab;
        [SerializeField] private bool _useHumanControls;
        [SerializeField] private DonkeyKongConfig _config = DonkeyKongConfig.Default;

        
        public event Action OnSimulationDone;


        private GeneticAlgorithmParameters m_GaParameters;
        private List<DonkeyKongPlayer> m_Players;
        private DonkeyKongPrincess m_Princess;
        private DonkeyKongMonkey m_Monkey;
        private DonkeyKongGround[] m_Grounds;
        private DonkeyKongWall[] m_Walls;
        private DonkeyKongLadder[] m_Ladders;
        private Random m_Random;
        private bool m_IsStarted;


        private void Awake()
        {
            DonkeyKongPlayer.OnAnyStop += OnAnyPlayerStop;
        }

        private void OnDestroy()
        {
            DonkeyKongPlayer.OnAnyStop -= OnAnyPlayerStop;
        }


        private void OnAnyPlayerStop(DonkeyKongPlayer player)
        {
            if (IsGameDone())
            {
                OnSimulationDone?.Invoke();
            }
        }

        private bool IsGameDone()
        {
            return m_Players.All(player => player.IsDone());
        }
        
        public void Initialize(GeneticAlgorithmParameters parameters)
        {
            var config = GetConfig();

            m_GaParameters = parameters;
            m_Random = new Random(config.seed);
            m_Players = new List<DonkeyKongPlayer>();
            
            for (var i = 0; i < parameters.populationCount; i++)
            {
                var startPosition = _startPoint.position;
                var player = Instantiate(_playerPrefab);
                var input = _useHumanControls 
                    ? (IDonkeyKongPlayerInput) new DonkeyKongPlayerHumanInput()
                    : DonkeyKongPlayerBotInput.New(player);
                player.SetGame(this);
                player.SetInput(input);
                player.SetPosition(startPosition);
                m_Players.Add(player);
            }
            
            m_Princess = GetComponentInChildren<DonkeyKongPrincess>(true);
            m_Monkey = GetComponentInChildren<DonkeyKongMonkey>(true);
            m_Grounds = GetComponentsInChildren<DonkeyKongGround>(true);
            m_Walls = GetComponentsInChildren<DonkeyKongWall>(true);
            m_Ladders = GetComponentsInChildren<DonkeyKongLadder>(true);
        }

        public void ResetState()
        {
            var config = GetConfig();
            
            m_IsStarted = false;
            m_Random = new Random(config.seed);
            m_Monkey.ResetState();
        }

        public IGeneticAlgorithmEntity[] GetPopulationPool()
        {
            return m_Players.Cast<IGeneticAlgorithmEntity>().ToArray();
        }

        public IGeneticAlgorithmBrain CreateBrainWithSize(int brainSize)
        {
            return BasicGeneticAlgorithmBrain.New(brainSize, 6);
        }

        public void Simulate()
        {
            m_IsStarted = true;
        }

        public float GetDeltaTime()
        {
            return 1f / m_GaParameters.framesPerSeconds;
        }

        public Vector2 GetStartPosition()
        {
            return _startPoint.position;
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

        public DonkeyKongConfig GetConfig()
        {
            return _config;
        }

        public bool IsStarted()
        {
            return m_IsStarted;
        }

        public bool RandomBool(float possibility)
        {
            return m_Random.NextDouble() <= possibility;
        }
    }
}