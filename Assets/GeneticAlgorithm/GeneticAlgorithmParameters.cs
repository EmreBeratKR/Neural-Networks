using System;
using UnityEngine;

namespace GeneticAlgorithm
{
    [Serializable]
    public struct GeneticAlgorithmParameters
    {
        public bool isFixedBrainSize;
        public int genPerBrainSizeIncrease;
        
        public bool isFrameDependent;
        public int framesPerSeconds;
        
        public int populationCount;
        public int brainSize;

        public ParentSelectionOperatorType parentSelectionOperator;
        public CrossoverOperatorType crossoverOperator;
        public MutationOperatorType mutationOperator;
        
        [Range(0f, 1f)]
        public float elitismRate;
        [Range(0f, 1f)]
        public float crossoverRate;
        [Range(0f, 1f)]
        public float mutationRate;
    }
}