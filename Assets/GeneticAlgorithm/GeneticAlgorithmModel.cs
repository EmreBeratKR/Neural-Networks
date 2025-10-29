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
        public event Action<int> OnBrainSizeChanged;


        private IGeneticAlgorithmEntity[] m_Population;
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
            m_Environment.OnSimulationDone += OnSimulationDone;
            m_Environment.Initialize(m_Parameters);
            
            m_Population = CreateInitialPopulation();

            m_FitnessValues = new List<float>();
            SetGenerationNumber(1);
            m_BrainSizeIncreaseCount = 0;
            OnBrainSizeChanged?.Invoke(m_Population[0].GetBrain().GetSize());
            
            m_Environment.Simulate();
        }

        public List<float> GetFitnessValues()
        {
            return m_FitnessValues;
        }


        private void OnSimulationDone()
        {
            var averageFitness = m_Population.Average(e => e.GetFitness());
            
            m_Population = CreateNextPopulation(m_Population);
            m_FitnessValues.Add(averageFitness);
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
        
        private IGeneticAlgorithmEntity[] CreateNextPopulation(IGeneticAlgorithmEntity[] population)
        {
            var newPopulation = new IGeneticAlgorithmEntity[population.Length];
            var eliteCount = Mathf.RoundToInt(population.Length * m_Parameters.elitismRate);
            var elites = population.OrderByDescending(e => e.GetFitness()).Take(eliteCount).ToArray();

            for (var i = 0; i < eliteCount; i++)
            {
                var brain = elites[i].GetBrain().Copy();
                var eliteCopy = m_Environment.CreateEntityWithBrain(brain);
                newPopulation[i] = eliteCopy;
            }
            
            for (var i = eliteCount; i < newPopulation.Length; i++)
            {
                var brain = ReproduceNewBrainFromPopulation(population, m_Parameters);
                var child = m_Environment.CreateEntityWithBrain(brain);
                newPopulation[i] = child;
            }

            return newPopulation;
        }

        
        private static IGeneticAlgorithmEntity RouletteWheelSelection(IGeneticAlgorithmEntity[] population)
        {
            var fitnessSum = population.Sum(e => e.GetFitness());
            var value = Random.Range(0f, fitnessSum);
            var cumulative = 0f;

            foreach (var entity in population)
            {
                cumulative += entity.GetFitness();

                if (value <= cumulative)
                {
                    return entity;
                }
            }
            
            return population[^1];
        }

        private static IGeneticAlgorithmBrain ReproduceNewBrainFromPopulation(IGeneticAlgorithmEntity[] population, GeneticAlgorithmParameters parameters)
        {
            IGeneticAlgorithmBrain newBrain;
            var a = RouletteWheelSelection(population).GetBrain();
            var b = RouletteWheelSelection(population).GetBrain();

            if (Random.Range(0f, 1f) <= parameters.crossoverRate)
            {
                newBrain = SinglePointCrossover(a, b);
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

        private static IGeneticAlgorithmBrain SinglePointCrossover(IGeneticAlgorithmBrain a, IGeneticAlgorithmBrain b)
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
    }
}