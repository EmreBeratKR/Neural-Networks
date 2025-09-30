using System;
using UnityEngine;

[Serializable]
public struct SampleData
{
    public float x;
    public float y;
    public float r;
    public float g;
    public float b;
}

[Serializable]
public struct FruitData
{
    public float spikeLenght;
    public float spikeThickness;
    public bool isPoisonous;
}

public class Test : MonoBehaviour
{
    private static readonly int SAMPLE_COUNT = Shader.PropertyToID("_SampleCount");
    private static readonly int SAMPLES = Shader.PropertyToID("_Samples");


    [SerializeField] private TextAsset _trainDataset;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _safeColor;
    [SerializeField] private Color _notSafeColor;


    private ComputeBuffer m_Buffer;


    private void Awake()
    {
        var dataset = ParseDataset(_trainDataset);
        var samples = GetSampleDatas(dataset);
        m_Buffer = new ComputeBuffer(samples.Length, sizeof(float) * 5);
        m_Buffer.SetData(samples);
        _renderer.material.SetBuffer(SAMPLES, m_Buffer);
        _renderer.material.SetInt(SAMPLE_COUNT, samples.Length);
        
    }

    private void OnDestroy()
    {
        m_Buffer.Release();
    }

    private FruitData[] ParseDataset(TextAsset textAsset)
    {
        var lines = textAsset.text.Split('\n');
        var fruits = new FruitData[lines.Length - 2];
        for (var i = 1; i < lines.Length - 1; i++)
        {
            var values = lines[i].Split(',');
            fruits[i - 1] = new FruitData
            {
                spikeLenght = float.Parse(values[0]),
                spikeThickness = float.Parse(values[1]),
                isPoisonous = values[2] == "1"
            };
        }
        return fruits;
    }
    
    private SampleData[] GetSampleDatas(FruitData[] dataset)
    {
        var minSpikeLenght = float.NegativeInfinity;
        var maxSpikeLenght = float.PositiveInfinity;
        var minSpikeThickness = float.NegativeInfinity;
        var maxSpikeThickness = float.PositiveInfinity;

        for (var i = 0; i < dataset.Length; i++)
        {
            minSpikeLenght = Mathf.Max(minSpikeLenght, dataset[i].spikeLenght);
            maxSpikeLenght = Mathf.Min(maxSpikeLenght, dataset[i].spikeLenght);
            minSpikeThickness = Mathf.Max(minSpikeThickness, dataset[i].spikeThickness);
            maxSpikeThickness = Mathf.Min(maxSpikeThickness, dataset[i].spikeThickness);
        }
        
        var samples = new SampleData[dataset.Length];
        
        for (var i = 0; i < samples.Length; i++)
        {
            var fruit = dataset[i];
            var color = fruit.isPoisonous ? _notSafeColor : _safeColor;
            samples[i] = new SampleData
            {
                x = Mathf.InverseLerp(minSpikeLenght, maxSpikeLenght, dataset[i].spikeLenght),
                y = Mathf.InverseLerp(minSpikeThickness, maxSpikeThickness, dataset[i].spikeThickness),
                r = color.r,
                g = color.g,
                b = color.b
            };
        }
        return samples;
    }
}