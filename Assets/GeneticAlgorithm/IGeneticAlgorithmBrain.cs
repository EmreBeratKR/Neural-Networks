namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmBrain
    {
        IGeneticAlgorithmBrain NewEmpty();
        int GetSize();
        void IncreaseSize(int size);
        void AddAction(int action);
        int GetAction(int index);
        void Mutate(float rate);
    }
}