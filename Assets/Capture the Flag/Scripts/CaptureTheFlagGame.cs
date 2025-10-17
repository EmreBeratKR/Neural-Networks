using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

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
            Application.targetFrameRate = parameters.framesPerSeconds;
            m_GaParameters = parameters;
            m_Players = new List<CaptureTheFlagPlayer>();
        }

        public void ResetState()
        {
            foreach (var player in m_Players)
            {
                Destroy(player.gameObject);
            }
            m_Players.Clear();
            m_IsStarted = false;
        }

        public void SetPopulation(IGeneticAlgorithmEntity[] population)
        {
            foreach (var entity in population)
            {
                m_Players.Add((CaptureTheFlagPlayer) entity);
            }
        }

        public IGeneticAlgorithmEntity CreateEntityWithBrainSize(int brainSize)
        {
            var brain = CaptureTheFlagPlayerBrain.New(brainSize);
            return CreateEntityWithBrain(brain);
        }
        
        public IGeneticAlgorithmEntity CreateEntityWithBrain(IGeneticAlgorithmBrain brain)
        {
            var startPosition = _start.position;
            var player = Instantiate(_playerPrefab);
            var input = CaptureTheFlagPlayerBotInput.New(player);
            player.SetGame(this);
            player.SetInput(input);
            player.SetBrain(brain);
            player.SetPosition(startPosition);
            return player;
        }

        public IGeneticAlgorithmEntity[] GetPopulationOfBestEntities()
        {
            var entities = new IGeneticAlgorithmEntity[m_GaParameters.bestPopulationCount];
            m_Players.Sort((a, b) =>
            {
                var fitA = a.CalculateFitness();
                var fitB = b.CalculateFitness();
                return fitB.CompareTo(fitA);
            });
            
            for (var i = 0; i < entities.Length; i++)
            {
                entities[i] = m_Players[i];
            }

            return entities;
        }
        
        public float GetAverageFitnessOfCurrentPopulation()
        {
            return m_Players.Average(e => e.CalculateFitness());
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