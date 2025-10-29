using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Value_Match
{
    public class ValueMatchGame : MonoBehaviour, IGeneticAlgorithmEnvironment
    {
        [SerializeField] private ValueMatchPlayer _playerPrefab;
        
        
        public event Action OnSimulationDone;


        private List<ValueMatchPlayer> m_Players;
        private GeneticAlgorithmParameters m_Parameters;
        private int m_Value;
        private float m_Time;


        private void Update()
        {
            var elapsedTime = Time.time - m_Time;
            
            if (elapsedTime < 1f) return;
            
            m_Time = Time.time;
            OnSimulationDone?.Invoke();
        }
        

        public void Initialize(GeneticAlgorithmParameters parameters)
        {
            m_Parameters = parameters;
            m_Players = new List<ValueMatchPlayer>();
            m_Value = Random.Range(0, 256);
            
            var player = Instantiate(_playerPrefab);
            
            player.SetGame(this);
            player.SetPosition(new Vector2(-7f, 0f));
            player.SetValue(m_Value);
        }

        public void ResetState()
        {
            var oldPopulation = m_Players.Take(m_Parameters.populationCount);
            var newPopulation = m_Players.Skip(m_Parameters.populationCount).ToList();
            foreach (var player in oldPopulation)
            {
                Destroy(player.gameObject);
            }
            m_Players = newPopulation;
        }

        public IGeneticAlgorithmEntity CreateEntityWithBrainSize(int brainSize)
        {
            var brain = BasicGeneticAlgorithmBrain.New(1, 256);
            return CreateEntityWithBrain(brain);
        }

        public IGeneticAlgorithmEntity CreateEntityWithBrain(IGeneticAlgorithmBrain brain)
        {
            var player = Instantiate(_playerPrefab);
            
            player.SetGame(this);
            player.SetBrain(brain);
            m_Players.Add(player);
            
            return player;
        }

        public void Simulate()
        {
            var scale = 0.9f * 100f / m_Parameters.populationCount;
            var offset = Mathf.RoundToInt(Mathf.Sqrt(m_Parameters.populationCount)) * scale * 0.5f;

            for (var i = 0; i < m_Players.Count; i++)
            {
                var x = i / 10 - offset;
                var y = i % 10 - offset;
                var position = new Vector2(x, y) * scale;
            
                m_Players[i].SetPosition(position);
                m_Players[i].SetScale(scale);
            }
        }
        
        public float GetValue()
        {
            return m_Value;
        }
    }
}