using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    [Serializable]
    public struct PopulationSaveData
    {
        public int generationNumber;
        public List<float> averageFitnessValues;
        public EntitySaveData[] entities;
    }

    [Serializable]
    public struct EntitySaveData
    {
        public float fitness;
        public int[] actions;
    }
}