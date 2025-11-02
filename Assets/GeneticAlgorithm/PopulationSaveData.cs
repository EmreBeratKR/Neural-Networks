using System;

namespace GeneticAlgorithm
{
    [Serializable]
    public struct PopulationSaveData
    {
        public float bestMeanFitness;
        public EntitySaveData[] entities;
    }

    [Serializable]
    public struct EntitySaveData
    {
        public float fitness;
        public int[] actions;
    }
}