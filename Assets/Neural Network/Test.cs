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
    public double spikeLenght;
    public double spikeThickness;
    public bool isPoisonous;
}

public class Test : MonoBehaviour
{
    private const int WIDTH = 128;
    private const int HEIGHT = 128;
    
    private static readonly int SAMPLE_COUNT = Shader.PropertyToID("_SampleCount");
    private static readonly int SAMPLES = Shader.PropertyToID("_Samples");
    private static readonly int DECISION_TEX = Shader.PropertyToID("_DecisionTex");


    [SerializeField] private NeuralNetwork _neuralNetwork;
    [SerializeField] private TextAsset _trainDataset;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _safeColor;
    [SerializeField] private Color _notSafeColor;


    private ComputeBuffer m_Buffer;
    private Texture2D m_Texture;
    private FruitData[] m_Dataset;
    private double m_MinSpikeLenght;
    private double m_MaxSpikeLenght;
    private double m_MinSpikeThickness;
    private double m_MaxSpikeThickness;


    private void Awake()
    {
        m_Dataset = ParseDataset(_trainDataset);
        var samples = GetSampleDatas(m_Dataset);
        m_Buffer = new ComputeBuffer(samples.Length, sizeof(float) * 5);
        m_Buffer.SetData(samples);
        _renderer.material.SetBuffer(SAMPLES, m_Buffer);
        _renderer.material.SetInt(SAMPLE_COUNT, samples.Length);
        
        m_Texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        _renderer.material.SetTexture(DECISION_TEX, m_Texture);
    }

    private void OnDestroy()
    {
        m_Buffer.Release();
    }

    private void Update()
    {
        for (var y = 0; y < HEIGHT; y++)
        {
            for (var x = 0; x < WIDTH; x++)
            {
                var xT = Lerp(m_MinSpikeLenght, m_MaxSpikeLenght, x / (double) WIDTH);
                var yT = Lerp(m_MinSpikeThickness, m_MaxSpikeThickness, y / (double) HEIGHT);
                var value = Classify(xT, yT);
                var color = value == 0 ? _safeColor : _notSafeColor;
                color.a = 1;
                m_Texture.SetPixel(x, y, color);
            }
        }
        m_Texture.Apply();
        TestModel();
    }

    private void TestModel()
    {
        var correct = 0;
        foreach (var data in m_Dataset)
        {
            var predict = Classify(data.spikeLenght, data.spikeThickness);
            if (predict == 0 && data.isPoisonous == false)
            {
                correct += 1;
            }
            else if (predict == 1 && data.isPoisonous == true)
            {
                correct += 1;
            }
        }

        Debug.Log($"{correct}/{m_Dataset.Length} [%{(correct / (float) m_Dataset.Length) * 100}]");
    }

    private int Classify(double x, double y)
    {
        return _neuralNetwork.Predict(new []{x, y});
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
        var minSpikeLenght = double.NegativeInfinity;
        var maxSpikeLenght = double.PositiveInfinity;
        var minSpikeThickness = double.NegativeInfinity;
        var maxSpikeThickness = double.PositiveInfinity;

        for (var i = 0; i < dataset.Length; i++)
        {
            minSpikeLenght = Math.Max(minSpikeLenght, dataset[i].spikeLenght);
            maxSpikeLenght = Math.Min(maxSpikeLenght, dataset[i].spikeLenght);
            minSpikeThickness = Math.Max(minSpikeThickness, dataset[i].spikeThickness);
            maxSpikeThickness = Math.Min(maxSpikeThickness, dataset[i].spikeThickness);
        }
        
        var samples = new SampleData[dataset.Length];
        
        for (var i = 0; i < samples.Length; i++)
        {
            var fruit = dataset[i];
            var color = fruit.isPoisonous ? _notSafeColor : _safeColor;
            samples[i] = new SampleData
            {
                x = (float) InverseLerp(minSpikeLenght, maxSpikeLenght, dataset[i].spikeLenght),
                y = (float) InverseLerp(minSpikeThickness, maxSpikeThickness, dataset[i].spikeThickness),
                r = color.r,
                g = color.g,
                b = color.b
            };
        }

        m_MinSpikeLenght = minSpikeLenght;
        m_MaxSpikeLenght = maxSpikeLenght;
        m_MinSpikeThickness = minSpikeThickness;
        m_MaxSpikeThickness = maxSpikeThickness;
        
        return samples;
    }


    public static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * Clamp01(t);
    }
    
    private static double InverseLerp(double a, double b, double value)
    {
        return a != b ? Clamp01((value - a) / (b - a)) : 0.0;
    }
    
    private static double Clamp01(double value)
    {
        if (value < 0.0)
            return 0.0;
        return value > 1.0 ? 1.0 : value;
    }
}