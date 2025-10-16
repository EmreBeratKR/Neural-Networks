using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace Capture_the_Flag
{
    public class GeneticAlgorithmRunner : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagGame _game;
        [SerializeField] private GeneticAlgorithmParameters _parameters;
        [SerializeField] private TMP_Text _generationNumberText;
        [SerializeField] private TMP_Text _brainSizeText;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _fitnessGraphSampleCount;
        [SerializeField] private Vector2 _fitnessGraphScale;


        private int m_GenerationNumber;
        
        
        private void Start()
        {
            var brains = CreateInitialGenerationBrains();

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
            float CalculateFitness(Vector2 position, bool collectedFirst, bool collectedSecond)
            {
                var flagPosition = _game.GetState().flagPosition;
                var distance = Vector2.Distance(flagPosition, position);

                var distanceFitness = 1f / (distance + 1f);
                var firstBonusFitness = collectedFirst ? 1f : 0f;
                var secondBonusFitness = collectedSecond ? 1f : 0f;

                return distanceFitness * 0.8f + firstBonusFitness * 0.1f + secondBonusFitness * 0.1f;
            }

            var isFirst = false;
            var isSecond = false;
            for (var i = 0; i < _fitnessGraphSampleCount; i++)
            {
                var t = i / (float) (_fitnessGraphSampleCount - 1);
                var localPoint = _splineContainer.Spline.EvaluatePosition(t);
                var worldPoint = _splineContainer.transform.TransformPoint(localPoint);

                if (!isFirst && Vector2.Distance(_splineContainer.transform.TransformPoint(_splineContainer.Spline.EvaluatePosition(0.2f)), worldPoint) < 0.5f)
                {
                    isFirst = true;
                }
                
                if (!isSecond && Vector2.Distance(_splineContainer.transform.TransformPoint(_splineContainer.Spline.EvaluatePosition(0.4f)), worldPoint) < 0.5f)
                {
                    isSecond = true;
                }
                
                var fitness = CalculateFitness(worldPoint, isFirst, isSecond);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(new Vector3(t * _fitnessGraphScale.x, fitness * _fitnessGraphScale.y, 0f), 0.01f);
            }
        }


        private void OnGameDone()
        {
            var bestPlayers = _game.GetBestPlayers();
            var nextGenBrains = CreateNextGenerationBrains(bestPlayers);

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
                brain.Mutate(_parameters.brainBatchSize, _parameters.mutationRate);
            }

            if (m_GenerationNumber % _parameters.genPerBrainSizeIncrease == 0)
            {
                foreach (var brain in brains)
                {
                    brain.IncreaseSize(_parameters.brainBatchSize);
                }
                UpdateBrainSizeText(brains[0].GetSize());
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