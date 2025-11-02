using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithmModel
    {
        public event Action<int> OnGenerationNumberChanged;
        public event Action<int> OnGenerationEvaluated; 
        public event Action<int> OnBrainSizeChanged;


        private IGeneticAlgorithmEntity[] m_Population;
        private IGeneticAlgorithmEnvironment m_Environment;
        private GeneticAlgorithmParameters m_Parameters;
        private List<float> m_AverageFitnessValues;
        private int m_GenerationNumber;
        private float m_FitnessSum;
        private float[] m_FitnessValues;
        private bool m_ManualTerminate;
        private bool m_IsTerminated;
        
        
        public static GeneticAlgorithmModel New()
        {
            return new GeneticAlgorithmModel();
        }

        public GeneticAlgorithmModel SetEnvironment(IGeneticAlgorithmEnvironment environment)
        {
            m_Environment = environment;
            return this;
        }

        public GeneticAlgorithmModel SetParameters(GeneticAlgorithmParameters parameters)
        {
            m_Parameters = parameters;
            return this;
        }
        
        public void Run()
        {
            m_Environment.OnSimulationDone += OnSimulationDone;
            m_Environment.Initialize(m_Parameters);

            m_Population = m_Environment.GetPopulationPool();
            m_FitnessValues = new float[m_Population.Length];
            CreateInitialPopulation();
            m_AverageFitnessValues = new List<float>();
            SetGenerationNumber(1);
            OnBrainSizeChanged?.Invoke(m_Population[0].GetBrain().GetSize());
            
            m_Environment.Simulate();
        }

        [ContextMenu("Terminate")]
        public void ManualTerminate()
        {
            m_ManualTerminate = true;
        }

        public List<float> GetFitnessValues()
        {
            return m_AverageFitnessValues;
        }
        
        public PopulationSaveData GetCurrentPopulationSaveData()
        {
            var entities = new EntitySaveData[m_Population.Length];

            for (var i = 0; i < entities.Length; i++)
            {
                var brain = m_Population[i].GetBrain();
                var brainSize = brain.GetSize();
                var actions = new int[brainSize];

                for (var j = 0; j < actions.Length; j++)
                {
                    actions[j] = brain.GetAction(j);
                }
                entities[i] = new EntitySaveData()
                {
                    fitness = m_FitnessValues[i],
                    actions = actions
                };
            }

            return new PopulationSaveData()
            {
                bestMeanFitness = m_AverageFitnessValues.Max(),
                entities = entities
            };
        }


        private void OnSimulationDone()
        {
            CacheValues();
            OnGenerationEvaluated?.Invoke(m_GenerationNumber + 1);
            
            if (m_IsTerminated) return;

            if (ShouldTerminate())
            {
                Terminate();
                return;
            }
            
            CreateNextPopulation();
            SetGenerationNumber(m_GenerationNumber + 1);
            OnBrainSizeChanged?.Invoke(m_Population[0].GetBrain().GetSize());
            m_Environment.ResetState();
            m_Environment.Simulate();
        }
        

        private void SetGenerationNumber(int value)
        {
            m_GenerationNumber = value;
            OnGenerationNumberChanged?.Invoke(value);
        }
        
        private void CreateInitialPopulation()
        {
            for (var i = 0; i < m_Parameters.populationCount; i++)
            {
                var brain = m_Environment.CreateBrainWithSize(m_Parameters.brainSize);
                m_Population[i].SetBrain(brain);
            }
        }

        private void Terminate()
        {
            m_IsTerminated = true;
            Debug.Log("terminated");
        }

        private bool ShouldTerminate()
        {
            if (TerminateIfManualTrigger()) return true;
            
            switch (m_Parameters.terminationCondition)
            {
                case TerminationConditionType.LowFitnessValueVariance:
                    return TerminateIfLowFitnessValueVariance();
                case TerminationConditionType.UntilGeneration:
                    return TerminateIfGenerationReached();
                case TerminationConditionType.UntilFitnessValue:
                    return TerminateIfFitnessValueReached();
                default:
                    return TerminateIfManualTrigger();
            }
        }

        private bool TerminateIfManualTrigger()
        {
            return m_ManualTerminate;
        }
        
        private bool TerminateIfLowFitnessValueVariance()
        {
            var threshold = m_Parameters.lowVarianceThreshold;
            var sampleCount = m_Parameters.lowVarianceSampleCount;

            if (sampleCount >= m_AverageFitnessValues.Count) return false;
            
            var sampleMean = m_AverageFitnessValues
                .TakeLast(sampleCount)
                .Average();
            var numerator = 0f;
            for (var i = m_AverageFitnessValues.Count - sampleCount; i < m_AverageFitnessValues.Count; i++)
            {
                var dif = m_AverageFitnessValues[i] - sampleMean;
                numerator += dif * dif;
            }

            var variance = numerator / (sampleCount - 1);
            var scaledVariance = variance * 1000000;

            Debug.Log(scaledVariance);
            
            return scaledVariance <= threshold;
        }

        private bool TerminateIfGenerationReached()
        {
            return m_GenerationNumber >= m_Parameters.generationThreshold;
        }

        private bool TerminateIfFitnessValueReached()
        {
            return m_AverageFitnessValues[^1] >= m_Parameters.fitnessValueThreshold;
        }
        
        private void CreateNextPopulation()
        {
            var eliteCount = Mathf.RoundToInt(m_Population.Length * m_Parameters.elitismRate);
            var elites = m_Population.OrderByDescending(e => e.GetFitness()).Take(eliteCount).ToArray();

            for (var i = 0; i < eliteCount; i++)
            {
                var brain = elites[i].GetBrain().Copy();
                m_Population[i].SetBrain(brain);
                m_Population[i].ResetState();
            }
            
            for (var i = eliteCount; i < m_Population.Length; i++)
            {
                var brain = ReproduceNewBrainFromPopulation(m_Population, m_Parameters);
                m_Population[i].SetBrain(brain);
                m_Population[i].ResetState();
            }
        }

        private void CacheValues()
        {
            var averageFitness = m_Population.Average(e => e.GetFitness());
            
            m_AverageFitnessValues.Add(averageFitness);
            m_FitnessSum = m_Population.Sum(e => e.GetFitness());

            for (var i = 0; i < m_Population.Length; i++)
            {
                m_FitnessValues[i] = m_Population[i].GetFitness();
            }
        }


        private IGeneticAlgorithmEntity SelectParent()
        {
            if (m_Parameters.parentSelectionOperator is ParentSelectionOperatorType.RouletteWheel)
            {
                return RouletteWheelSelection();
            }

            return RouletteWheelSelection();
        }
        
        private IGeneticAlgorithmEntity RouletteWheelSelection()
        {
            var value = Random.Range(0f, m_FitnessSum);
            var cumulative = 0f;

            for (var i = 0; i < m_Population.Length; i++)
            {
                var entity = m_Population[i];
                cumulative += m_FitnessValues[i];

                if (value <= cumulative)
                {
                    return entity;
                }
            }

            return m_Population[^1];
        }

        private IGeneticAlgorithmBrain Crossover(IGeneticAlgorithmBrain a, IGeneticAlgorithmBrain b)
        {
            if (m_Parameters.crossoverOperator is CrossoverOperatorType.SinglePoint)
            {
                return SinglePointCrossover(a, b);
            }

            return SinglePointCrossover(a, b);
        }
        
        private IGeneticAlgorithmBrain SinglePointCrossover(IGeneticAlgorithmBrain a, IGeneticAlgorithmBrain b)
        {
            var brain = a.Copy();
            var brainSize = brain.GetSize();
            var crossoverPoint = Random.Range(0, brainSize);
            
            for (var i = 0; i < brainSize; i++)
            {
                if (i < crossoverPoint)
                {
                    brain.SetAction(b.GetAction(i), i);
                }
            }

            return brain;
        }

        private IGeneticAlgorithmBrain ReproduceNewBrainFromPopulation(IGeneticAlgorithmEntity[] population, GeneticAlgorithmParameters parameters)
        {
            IGeneticAlgorithmBrain newBrain;
            var a = SelectParent().GetBrain();
            var b = SelectParent().GetBrain();

            if (Random.Range(0f, 1f) <= parameters.crossoverRate)
            {
                newBrain = Crossover(a, b);
            }
            else
            {
                newBrain = Random.Range(0f, 1f) <= 0.5f ? a.Copy() : b.Copy();
            }

            if (Random.Range(0f, 1f) <= parameters.mutationRate)
            {
                newBrain.Mutate();
            }

            return newBrain;
        }
    }
}