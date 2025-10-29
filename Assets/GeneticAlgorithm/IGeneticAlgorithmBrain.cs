namespace GeneticAlgorithm
{
    public interface IGeneticAlgorithmBrain
    {
        IGeneticAlgorithmBrain Copy();
        int GetSize();
        int GetAction(int index);
        void SetAction(int action, int index);
        void Mutate();
    }
}