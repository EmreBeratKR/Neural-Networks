using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Capture_the_Flag
{
    public class CaptureTheFlagGame : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagPlayer _playerPrefab;
        [SerializeField] private Flag _flag;
        [SerializeField] private Transform _start;


        public event Action OnDone;
        

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
                OnDone?.Invoke();
            }
        }

        private void CreatePlayerWithBrain(CaptureTheFlagPlayerBrain brain)
        {
            var startPosition = _start.position;
            var player = Instantiate(_playerPrefab);
            var input = CaptureTheFlagPlayerBotInput.New(player);
            player.SetGame(this);
            player.SetInput(input);
            player.SetBrain(brain);
            player.SetPosition(startPosition);
            m_Players.Add(player);
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

        public void Begin(List<CaptureTheFlagPlayerBrain> brains)
        {
            for (var i = 0; i < m_GaParameters.populationCount; i++)
            {
                CreatePlayerWithBrain(brains[i]);
            }

            m_IsStarted = true;
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

        public CaptureTheFlagGameState GetState()
        {
            return new CaptureTheFlagGameState
            {
                flagPosition = _flag.GetPosition(),
                isStarted = m_IsStarted
            };
        }

        public List<CaptureTheFlagPlayer> GetPlayers()
        {
            return m_Players;
        }

        public CaptureTheFlagPlayer[] GetBestPlayers()
        {
            var players = new CaptureTheFlagPlayer[m_GaParameters.bestPopulationCount];
            m_Players.Sort((a, b) =>
            {
                var fitA = a.CalculateFitness();
                var fitB = b.CalculateFitness();
                return fitB.CompareTo(fitA);
            });
            
            for (var i = 0; i < players.Length; i++)
            {
                players[i] = m_Players[i];
            }

            return players;
        }
    }
}