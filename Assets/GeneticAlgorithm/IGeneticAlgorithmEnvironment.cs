using System;

namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmEnvironment
    {
        void Initialize(GeneticAlgorithmParameters parameters);
        void ResetState();
        IGeneticAlgorithmEntity[] GetPopulationPool();
        IGeneticAlgorithmBrain CreateBrainWithSize(int brainSize);
        void Simulate();
        event Action OnSimulationDone;
    }
}