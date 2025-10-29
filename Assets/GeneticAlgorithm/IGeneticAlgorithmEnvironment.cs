using System;

namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmEnvironment
    {
        void Initialize(GeneticAlgorithmParameters parameters);
        void ResetState();
        IGeneticAlgorithmEntity CreateEntityWithBrainSize(int brainSize);
        IGeneticAlgorithmEntity CreateEntityWithBrain(IGeneticAlgorithmBrain brain);
        void Simulate();
        event Action OnSimulationDone;
    }
}