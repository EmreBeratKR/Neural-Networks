namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmEntity
    {
        IGeneticAlgorithmBrain GetBrain();
        float GetFitness();
    }
}