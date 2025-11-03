using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagGame : MonoBehaviour,
        IGeneticAlgorithmEnvironment
    {
        [SerializeField] private CaptureTheFlagPlayer _playerPrefab;
        [SerializeField] private Flag _flag;
        [SerializeField] private Transform _start;


        public event Action OnSimulationDone;
        

        private GeneticAlgorithmParameters m_GaParameters;
        private List<CaptureTheFlagPlayer> m_Players;
        private bool m_IsStarted;


        private void Awake()
        {
            CaptureTheFlagPlayer.OnAnyStop += OnAnyPlayerStop;
        }

        private void OnDestroy()
        {
            CaptureTheFlagPlayer.OnAnyStop -= OnAnyPlayerStop;
        }


        private void OnAnyPlayerStop(CaptureTheFlagPlayer player)
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
            m_GaParameters = parameters;
            m_Players = new List<CaptureTheFlagPlayer>();
            
            for (var i = 0; i < parameters.populationCount; i++)
            {
                var startPosition = _start.position;
                var player = Instantiate(_playerPrefab);
                var input = CaptureTheFlagPlayerBotInput.New(player);
                player.SetGame(this);
                player.SetInput(input);
                player.SetPosition(startPosition);
                m_Players.Add(player);
            }
        }

        public void ResetState()
        {
            m_IsStarted = false;
        }

        public IGeneticAlgorithmEntity[] GetPopulationPool()
        {
            return m_Players.Cast<IGeneticAlgorithmEntity>().ToArray();
        }

        public IGeneticAlgorithmBrain CreateBrainWithSize(int brainSize)
        {
            return BasicGeneticAlgorithmBrain.New(brainSize, 4);
        }
        
        public void Simulate()
        {
            m_IsStarted = true;
        }

        public CaptureTheFlagGameState GetState()
        {
            return new CaptureTheFlagGameState
            {
                startPosition = _start.position,
                flagPosition = _flag.GetPosition(),
                isStarted = m_IsStarted
            };
        }

        public List<CaptureTheFlagPlayer> GetPlayers()
        {
            return m_Players;
        }
    }
}