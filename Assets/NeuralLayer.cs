using System;
using UnityEngine;

[Serializable]
public struct WeightGroup
{
    [Range(-1,1)]
    public double[] Weights;
}

[Serializable]
public class NeuralLayer
{
    public WeightGroup[] WeightGroups;
    [Range(-1,1)]
    public double[] Biases;


    public int GetOutputCount()
    {
        return Biases.Length;
    }
    
    public double[] CalculateOutputs(double[] inputs)
    {
        var outputCount = GetOutputCount();
        var outputs = new double[outputCount];
        for (var i = 0; i < outputCount; i++)
        {
            for (var j = 0; j < inputs.Length; j++)
            {
                outputs[i] += inputs[j] * WeightGroups[i].Weights[j];
            }
            outputs[i] += Biases[i];
            outputs[i] = ActivationFunction(outputs[i]);
        }

        return outputs;
    }


    private double ActivationFunction(double input)
    {
        return 1 / (1 + Math.Exp(-input));
    }
}