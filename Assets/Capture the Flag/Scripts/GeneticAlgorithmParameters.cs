using System;
using UnityEngine;

namespace Capture_the_Flag
{
    [Serializable]
    public struct GeneticAlgorithmParameters
    {
        public int framesPerSeconds;
        public int populationCount;
        public int bestPopulationCount;
        public int genPerBrainSizeIncrease;
        public int brainBatchSize;
        public int maxBrainSizeIncreaseCount;
        [Range(0f, 1f)]
        public float mutationRate;
    }
}