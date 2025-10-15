using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagGame : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagPlayer _playerPrefab;
        [SerializeField] private Flag _flagPrefab;


        public Action OnDone;
        

        private GeneticAlgorithmParameters m_GaParameters;
        private List<CaptureTheFlagPlayer> m_Players;
        private Flag m_Flag;
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
        

        private Vector3 GetStartPosition()
        {
            return new Vector3(-4.3f, -4.5f, 0f);
        }
        
        private Vector3 GetRandomFlagPosition()
        {
            return new Vector3(4.7f, 4.5f, 0f);
        }

        private void CreatePlayerWithBrain(CaptureTheFlagPlayerBrain brain)
        {
            var startPosition = GetStartPosition();
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
            
            var flagPosition = GetRandomFlagPosition();
            var flag = Instantiate(_flagPrefab);
            flag.SetPosition(flagPosition);
            m_Flag = flag;

            m_IsStarted = true;
        }

        public void ResetState()
        {
            foreach (var player in m_Players)
            {
                Destroy(player.gameObject);
            }
            m_Players.Clear();
            Destroy(m_Flag.gameObject);
            m_Flag = null;
            m_IsStarted = false;
        }

        public CaptureTheFlagGameState GetState()
        {
            return new CaptureTheFlagGameState
            {
                flagPosition = m_Flag.GetPosition(),
                isStarted = m_IsStarted
            };
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