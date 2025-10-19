namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmBrain
    {
        IGeneticAlgorithmBrain Copy();
        int GetSize();
        void IncreaseSize(int size);
        int GetAction(int index);
        void SetAction(int action, int index);
        void Mutate(float rate);
    }
}