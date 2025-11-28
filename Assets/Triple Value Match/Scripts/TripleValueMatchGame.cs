using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Triple_Value_Match
{
    public class TripleValueMatchGame : MonoBehaviour, 
        IGeneticAlgorithmEnvironment
    {
        [SerializeField] private TripleValueMatchPlayer _playerPrefab;
        [SerializeField] private float _timer;
        [SerializeField] private bool _useSeededValue;
        [SerializeField, Min(0)] private Vector3Int _seededValue;
        
        
        public event Action OnSimulationDone;


        private List<TripleValueMatchPlayer> m_Players;
        private GeneticAlgorithmParameters m_Parameters;
        private int m_Value0;
        private int m_Value1;
        private int m_Value2;
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
            m_Players = new List<TripleValueMatchPlayer>();

            if (_useSeededValue)
            {
                m_Value0 = _seededValue.x;
                m_Value1 = _seededValue.y;
                m_Value2 = _seededValue.z;
            }
            else
            {
                m_Value0 = Random.Range(0, 256);
                m_Value1 = Random.Range(0, 256);
                m_Value2 = Random.Range(0, 256);
            }
            
            var dummy = Instantiate(_playerPrefab, transform);
            
            dummy.SetGame(this);
            dummy.SetPosition(new Vector2(-7f, 0f));
            dummy.SetValue(m_Value0, m_Value1, m_Value2);

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
            return BasicGeneticAlgorithmBrain.New(3, 256);
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
                    return -1 * b.GetColorValue().CompareTo(a.GetColorValue());
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
        
        public (int, int, int) GetValues()
        {
            return (m_Value0, m_Value1, m_Value2);
        }
    }
}