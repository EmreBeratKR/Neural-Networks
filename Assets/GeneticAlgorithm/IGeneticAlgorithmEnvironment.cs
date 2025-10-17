using System;

namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmEnvironment
    {
        void Initialize(GeneticAlgorithmParameters parameters);
        void ResetState();
        void SetPopulation(IGeneticAlgorithmEntity[] population);
        IGeneticAlgorithmEntity CreateEntityWithBrainSize(int brainSize);
        IGeneticAlgorithmEntity CreateEntityWithBrain(IGeneticAlgorithmBrain brain);
        IGeneticAlgorithmEntity[] GetPopulationOfBestEntities();
        float GetAverageFitnessOfCurrentPopulation();
        void Simulate();
        event Action OnSimulationDone;
    }
}