using System;

[Serializable]
public class NeuralNetwork
{
    public NeuralLayer[] Layers;


    public int Predict(double[] inputs)
    {
        var currentInputs = inputs;
        for (var i = 0; i < Layers.Length; i++)
        {
            currentInputs = Layers[i].CalculateOutputs(currentInputs);
        }

        var best = 0;
        var bestValue = currentInputs[0];
        for (var i = 1; i < currentInputs.Length; i++)
        {
            if (currentInputs[i] > bestValue)
            {
                bestValue = currentInputs[i];
                best = i;
            }
        }

        return best;
    }
}