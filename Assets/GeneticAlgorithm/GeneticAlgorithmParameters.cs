using System;
using UnityEngine;

namespace GeneticAlgorithm
{
    [Serializable]
    public struct GeneticAlgorithmParameters
    {
        public int framesPerSeconds;
        public int populationCount;
        public int genPerBrainSizeIncrease;
        public int brainBatchSize;
        public int maxBrainSizeIncreaseCount;
        [Range(0f, 1f)]
        public float elitismRate;
        [Range(0f, 1f)]
        public float mutationRate;
        [Range(0f, 1f)]
        public float crossoverRate;
    }
}