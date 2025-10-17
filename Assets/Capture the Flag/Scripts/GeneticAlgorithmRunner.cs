using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Capture_the_Flag
{
    public class GeneticAlgorithmRunner : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagGame _game;
        [SerializeField] private TMP_Text _generationNumberText;
        [SerializeField] private TMP_Text _brainSizeText;
        [SerializeField] private GeneticAlgorithmParameters _parameters;
        
        [Header("Fitness Graph")]
        [SerializeField] private Vector2 _fitnessGraphScale;
        [SerializeField] private Vector2 _fitnessGraphOffset;
        [SerializeField] private float _fitnessGraphDotRadius;


        private List<float> m_Fitnesses;
        private int m_GenerationNumber;
        private int m_BrainSizeIncreaseCount;
        
        
        private void Start()
        {
            var brains = CreateInitialGenerationBrains();

            m_Fitnesses = new List<float>();
            m_GenerationNumber = 1;
            UpdateGenerationNumberText(m_GenerationNumber);
            UpdateBrainSizeText(brains[0].GetSize());
            _game.Initialize(_parameters);
            _game.Begin(brains);

            _game.OnDone += OnGameDone;
        }

        private void OnDestroy()
        {
            _game.OnDone -= OnGameDone;
        }

        private void OnDrawGizmos()
        {
            if (m_Fitnesses is not null)
            {
                Gizmos.color = Handles.xAxisColor;
                Gizmos.DrawLine(_fitnessGraphOffset, (Vector3) _fitnessGraphOffset + Vector3.right * _fitnessGraphScale.x);
                
                Gizmos.color = Handles.yAxisColor;
                Gizmos.DrawLine(_fitnessGraphOffset, (Vector3) _fitnessGraphOffset + Vector3.up * _fitnessGraphScale.y);
                
                for (var i = 0; i < m_Fitnesses.Count; i++)
                {
                    Gizmos.color = Color.white;
                    var x = i * _fitnessGraphScale.x * 0.01f + _fitnessGraphOffset.x;
                    var y = m_Fitnesses[i] * _fitnessGraphScale.y + _fitnessGraphOffset.y;
                    Gizmos.DrawSphere(new Vector3(x, y, 0f), _fitnessGraphDotRadius);
                }
            }
        }


        private void OnGameDone()
        {
            var bestPlayers = _game.GetBestPlayers();
            var nextGenBrains = CreateNextGenerationBrains(bestPlayers);

            m_Fitnesses.Add(_game.GetAveragePlayersFitness());
            m_GenerationNumber += 1;
            UpdateGenerationNumberText(m_GenerationNumber);
            _game.ResetState();
            _game.Begin(nextGenBrains);
        }


        private void UpdateGenerationNumberText(int value)
        {
            _generationNumberText.text = $"Generation {value}";
        }

        private void UpdateBrainSizeText(int value)
        {
            _brainSizeText.text = $"Brain Size: {value}";
        }

        private List<CaptureTheFlagPlayerBrain> CreateInitialGenerationBrains()
        {
            var brains = new List<CaptureTheFlagPlayerBrain>(_parameters.populationCount);
            
            for (var i = 0; i < _parameters.populationCount; i++)
            {
                brains.Add(CaptureTheFlagPlayerBrain.New(_parameters.brainBatchSize));
            }

            return brains;
        }
        
        private List<CaptureTheFlagPlayerBrain> CreateNextGenerationBrains(CaptureTheFlagPlayer[] players)
        {
            var brains = new List<CaptureTheFlagPlayerBrain>(_parameters.populationCount);
            var newBrainCount = _parameters.populationCount - players.Length;
            
            for (var i = 0; i < newBrainCount; i++)
            {
                const int maxIter = 100;
                var a = players[Random.Range(0, players.Length)].GetBrain();
                var j = 0;
                CaptureTheFlagPlayerBrain b;
                do
                {
                    b = players[Random.Range(0, players.Length)].GetBrain();
                    j++;
                } while (b == a && j < maxIter);

                var newBrain = ReproduceBrain(a, b);
                brains.Add(newBrain);
            }
            foreach (var player in players)
            {
                var brain = player.GetBrain();
                brains.Add(brain);
            }

            foreach (var brain in brains)
            {
                brain.Mutate(_parameters.mutationRate);
            }

            var brainMaxSizeNotReached = m_BrainSizeIncreaseCount < _parameters.maxBrainSizeIncreaseCount;
            var isGenEligableToIncreaseBrainSize = m_GenerationNumber % _parameters.genPerBrainSizeIncrease == 0; 
            if (brainMaxSizeNotReached && isGenEligableToIncreaseBrainSize)
            {
                foreach (var brain in brains)
                {
                    brain.IncreaseSize(_parameters.brainBatchSize);
                }
                UpdateBrainSizeText(brains[0].GetSize());
                m_BrainSizeIncreaseCount += 1;
            }

            return brains;
        }

        private CaptureTheFlagPlayerBrain ReproduceBrain(CaptureTheFlagPlayerBrain a, CaptureTheFlagPlayerBrain b)
        {
            var brain = CaptureTheFlagPlayerBrain.New(0);
            var size = Mathf.Min(a.GetSize(), b.GetSize());
            
            for (var i = 0; i < size; i++)
            {
                brain.AddAction(Random.Range(0f, 1f) < 0.5f ? a.GetAction(i) : b.GetAction(i));
            }

            return brain;
        }
    }
}