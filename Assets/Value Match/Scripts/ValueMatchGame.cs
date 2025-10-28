using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Value_Match.Scripts
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
            foreach (var player in m_Players)
            {
                Destroy(player.gameObject);
            }
            m_Players.Clear();
        }

        public void SetPopulation(IGeneticAlgorithmEntity[] population)
        {
            var scale = 0.9f;
            var offset = Mathf.RoundToInt(Mathf.Sqrt(population.Length)) * scale * 0.5f;
            
            foreach (var entity in population)
            {
                var x = m_Players.Count / 10 - offset;
                var y = m_Players.Count % 10 - offset;
                var player = (ValueMatchPlayer) entity;
                var position = new Vector2(x, y) * scale;
                player.SetPosition(position);
                player.SetScale(scale);
                m_Players.Add(player);
            }
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
            return player;
        }

        public IGeneticAlgorithmEntity[] GetPopulationOfBestEntities()
        {
            var entities = new IGeneticAlgorithmEntity[m_Parameters.bestPopulationCount];
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
            
        }
        
        public float GetValue()
        {
            return m_Value;
        }
    }
}