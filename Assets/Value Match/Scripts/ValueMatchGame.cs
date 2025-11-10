using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Value_Match
{
    public class ValueMatchGame : MonoBehaviour, 
        IGeneticAlgorithmEnvironment
    {
        [SerializeField] private ValueMatchPlayer _playerPrefab;
        [SerializeField] private float _timer;
        
        
        public event Action OnSimulationDone;


        private List<ValueMatchPlayer> m_Players;
        private GeneticAlgorithmParameters m_Parameters;
        private int m_Value;
        private float m_Time;


        private void Update()
        {
            var elapsedTime = Time.time - m_Time;
            
            if (elapsedTime < _timer) return;
            
            m_Time = Time.time;
            OnSimulationDone?.Invoke();
        }
        

        public void Initialize(GeneticAlgorithmParameters parameters)
        {
            m_Parameters = parameters;
            m_Players = new List<ValueMatchPlayer>();
            m_Value = Random.Range(0, 256);
            
            var dummy = Instantiate(_playerPrefab, transform);
            
            dummy.SetGame(this);
            dummy.SetPosition(new Vector2(-7f, 0f));
            dummy.SetValue(m_Value);

            for (var i = 0; i < parameters.populationCount; i++)
            {
                var player = Instantiate(_playerPrefab);
            
                player.SetGame(this);
                m_Players.Add(player);
            }
        }

        public void ResetState()
        {
            
        }

        public IGeneticAlgorithmEntity[] GetPopulationPool()
        {
            return m_Players.Cast<IGeneticAlgorithmEntity>().ToArray();
        }

        public IGeneticAlgorithmBrain CreateBrainWithSize(int brainSize)
        {
            return BasicGeneticAlgorithmBrain.New(1, 256);
        }

        public void Simulate()
        {
            var edge = Mathf.RoundToInt(Mathf.Sqrt(m_Parameters.populationCount));
            var scale = 0.9f * 10f / edge;
            var offset = (edge - 1) * 0.5f;

            m_Players.Sort((a, b) =>
            {
                var val = -1 * b.GetFitness().CompareTo(a.GetFitness());

                if (val == 0)
                {
                    return -1 * b.GetValue().CompareTo(a.GetValue());
                }

                return val;
            });
            
            for (var i = 0; i < m_Players.Count; i++)
            {
                var x = i % edge - offset;
                var y = i / edge - offset;
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