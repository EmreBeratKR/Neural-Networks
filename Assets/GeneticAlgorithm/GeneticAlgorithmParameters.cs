using System;
using UnityEngine;

namespace GeneticAlgorithm
{
    [Serializable]
    public struct GeneticAlgorithmParameters
    {
        public bool isFrameDependent;
        public int framesPerSeconds;
        
        public int populationCount;
        public int brainSize;
        
        public bool isFixedBrainSize;
        public int brainBatchSize;
        public BrainSizeIncreaseConditionType brainSizeIncreaseCondition;
        [Min(1)]
        public int generationPerBrainSizeIncrease;

        public ParentSelectionOperatorType parentSelectionOperator;
        public CrossoverOperatorType crossoverOperator;
        public MutationOperatorType mutationOperator;

        public TerminationConditionType terminationCondition;
        [Min(2)]
        public int lowVarianceSampleCount;
        [Min(0f)]
        public int lowVarianceThreshold;

        [Min(1)]
        public int generationThreshold;
        public float fitnessValueThreshold; 
        
        [Range(0f, 1f)]
        public float elitismRate;
        [Range(0f, 1f)]
        public float crossoverRate;
        [Range(0f, 1f)]
        public float mutationRate;
    }
}