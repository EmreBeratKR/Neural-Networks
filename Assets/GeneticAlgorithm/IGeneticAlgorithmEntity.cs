namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmEntity
    {
        void SetBrain(IGeneticAlgorithmBrain brain);
        IGeneticAlgorithmBrain GetBrain();
        void ResetState();
        float GetFitness();
    }
}