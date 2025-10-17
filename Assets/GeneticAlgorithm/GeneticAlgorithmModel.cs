using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithmModel
    {
        public event Action<int> OnGenerationNumberChanged;
        public event Action<int> OnBrainSizeChanged; 
        
        
        private IGeneticAlgorithmEnvironment m_Environment;
        private GeneticAlgorithmParameters m_Parameters;
        private List<float> m_FitnessValues;
        private int m_GenerationNumber;
        private int m_BrainSizeIncreaseCount;
        
        
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
            var population = CreateInitialPopulation();

            m_FitnessValues = new List<float>();
            SetGenerationNumber(1);
            m_BrainSizeIncreaseCount = 0;
            OnBrainSizeChanged?.Invoke(population[0].GetBrain().GetSize());

            m_Environment.OnSimulationDone += OnSimulationDone;
            m_Environment.Initialize(m_Parameters);
            m_Environment.SetPopulation(population);
            m_Environment.Simulate();
        }

        public List<float> GetFitnessValues()
        {
            return m_FitnessValues;
        }


        private void OnSimulationDone()
        {
            var averageFitness = m_Environment.GetAverageFitnessOfCurrentPopulation();
            var bestEntities = m_Environment.GetPopulationOfBestEntities();
            var nextPopulation = CreateNextPopulation(bestEntities);
            
            m_FitnessValues.Add(averageFitness);
            SetGenerationNumber(m_GenerationNumber + 1);
            m_Environment.ResetState();
            m_Environment.SetPopulation(nextPopulation);
            m_Environment.Simulate();
        }
        

        private void SetGenerationNumber(int value)
        {
            m_GenerationNumber = value;
            OnGenerationNumberChanged?.Invoke(value);
        }
        
        private IGeneticAlgorithmEntity[] CreateInitialPopulation()
        {
            var entities = new IGeneticAlgorithmEntity[m_Parameters.populationCount];
            
            for (var i = 0; i < m_Parameters.populationCount; i++)
            {
                var entity = m_Environment.CreateEntityWithBrainSize(m_Parameters.brainBatchSize);
                entities[i] = entity;
            }

            return entities;
        }
        
        private IGeneticAlgorithmEntity[] CreateNextPopulation(IGeneticAlgorithmEntity[] currentGenEntities)
        {
            var entities = new IGeneticAlgorithmEntity[m_Parameters.populationCount];
            var newEntityCount = m_Parameters.populationCount - currentGenEntities.Length;
            
            for (var i = 0; i < newEntityCount; i++)
            {
                const int maxIter = 100;
                var a = currentGenEntities[Random.Range(0, currentGenEntities.Length)].GetBrain();
                var j = 0;
                IGeneticAlgorithmBrain b;
                do
                {
                    b = currentGenEntities[Random.Range(0, currentGenEntities.Length)].GetBrain();
                    j++;
                } while (b == a && j < maxIter);

                var newBrain = ReproduceBrain(a, b);
                entities[i] = m_Environment.CreateEntityWithBrain(newBrain);
            }

            for (var i = 0; i < currentGenEntities.Length; i++)
            {
                var brain = currentGenEntities[i].GetBrain();
                entities[newEntityCount + i] = m_Environment.CreateEntityWithBrain(brain);
            }

            foreach (var entity in entities)
            {
                entity.GetBrain().Mutate(m_Parameters.mutationRate);
            }

            var brainMaxSizeNotReached = m_BrainSizeIncreaseCount < m_Parameters.maxBrainSizeIncreaseCount;
            var isGenEligableToIncreaseBrainSize = m_GenerationNumber % m_Parameters.genPerBrainSizeIncrease == 0; 
            if (brainMaxSizeNotReached && isGenEligableToIncreaseBrainSize)
            {
                foreach (var entity in entities)
                {
                    entity.GetBrain().IncreaseSize(m_Parameters.brainBatchSize);
                }
                OnBrainSizeChanged?.Invoke(entities[0].GetBrain().GetSize());
                m_BrainSizeIncreaseCount += 1;
            }

            return entities;
        }
        
        private IGeneticAlgorithmBrain ReproduceBrain(IGeneticAlgorithmBrain a, IGeneticAlgorithmBrain b)
        {
            var brain = a.NewEmpty();
            var size = Mathf.Min(a.GetSize(), b.GetSize());
            
            for (var i = 0; i < size; i++)
            {
                brain.AddAction(Random.Range(0f, 1f) < 0.5f ? a.GetAction(i) : b.GetAction(i));
            }

            return brain;
        }
    }
}