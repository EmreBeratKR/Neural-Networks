using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithmRunner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _environmentGameObject;
        [SerializeField] private TMP_Text _generationNumberText;
        [SerializeField] private TMP_Text _brainSizeText;
        [SerializeField] private TMP_Text _bestFitnessText;

        [Header("Settings")] 
        [SerializeField] private GeneticAlgorithmRunnerMode _mode;
        [SerializeField] private TextAsset _saveFile;
        [SerializeField] private GeneticAlgorithmParameters _parameters;
        
        [Header("Fitness Graph")]
        [SerializeField] private Vector2 _fitnessGraphScale;
        [SerializeField] private Vector2 _fitnessGraphOffset;
        [SerializeField] private float _fitnessGraphDotRadius;

        [Header("Saves")] 
        [SerializeField] private string _savePath;
        [SerializeField] private bool _save;


        private string m_SaveFolderName;
        private GeneticAlgorithmModel m_Model;
        
        
        private void Start()
        {
            if (_mode is GeneticAlgorithmRunnerMode.Train)
            {
                var environment = _environmentGameObject.GetComponent<IGeneticAlgorithmEnvironment>();

                if (_save)
                {
                    m_SaveFolderName = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
                    Directory.CreateDirectory(Path.Join(_savePath, m_SaveFolderName));
                }
            
                m_Model = GeneticAlgorithmModel.New()
                    .SetEnvironment(environment)
                    .SetParameters(_parameters);

                m_Model.OnGenerationNumberChanged += OnModelGenerationNumberChanged;
                m_Model.OnGenerationEvaluated += OnGenerationEvaluated;
                m_Model.OnBrainSizeChanged += OnModelBrainSizeChanged;
                m_Model.Train();
            }
            else if (_mode is GeneticAlgorithmRunnerMode.PlayPopulation)
            {
                var environment = _environmentGameObject.GetComponent<IGeneticAlgorithmEnvironment>();
                
                m_Model = GeneticAlgorithmModel.New()
                    .SetEnvironment(environment)
                    .SetParameters(_parameters);
                m_Model.OnGenerationNumberChanged += OnModelGenerationNumberChanged;
                m_Model.OnGenerationEvaluated += OnGenerationEvaluated;
                m_Model.OnBrainSizeChanged += OnModelBrainSizeChanged;
                m_Model.LoadFromJson(_saveFile.text);
            }
            else if (_mode is GeneticAlgorithmRunnerMode.PlayPopulationBestPlayerOnly)
            {
                var environment = _environmentGameObject.GetComponent<IGeneticAlgorithmEnvironment>();
                
                m_Model = GeneticAlgorithmModel.New()
                    .SetEnvironment(environment)
                    .SetParameters(_parameters);
                m_Model.OnGenerationNumberChanged += OnModelGenerationNumberChanged;
                m_Model.OnGenerationEvaluated += OnGenerationEvaluated;
                m_Model.OnBrainSizeChanged += OnModelBrainSizeChanged;
                m_Model.LoadFromJsonBestEntity(_saveFile.text);
            }
        }

        private void OnDestroy()
        {
            m_Model.OnGenerationNumberChanged -= OnModelGenerationNumberChanged;
            m_Model.OnGenerationEvaluated -= OnGenerationEvaluated;
            m_Model.OnBrainSizeChanged -= OnModelBrainSizeChanged;
        }

        private void OnDrawGizmos()
        {
            if (m_Model is null) return;
            
            var fitnessValues = m_Model.GetFitnessValues();
            
            if (fitnessValues is null) return;
            
            Gizmos.color = Handles.xAxisColor;
            Gizmos.DrawLine(_fitnessGraphOffset, (Vector3) _fitnessGraphOffset + Vector3.right * _fitnessGraphScale.x);
                
            Gizmos.color = Handles.yAxisColor;
            Gizmos.DrawLine(_fitnessGraphOffset, (Vector3) _fitnessGraphOffset + Vector3.up * _fitnessGraphScale.y);
                
            for (var i = 0; i < fitnessValues.Count; i++)
            {
                Gizmos.color = Color.white;
                var x = i * _fitnessGraphScale.x * 0.01f + _fitnessGraphOffset.x;
                var y = fitnessValues[i] * _fitnessGraphScale.y + _fitnessGraphOffset.y;
                Gizmos.DrawSphere(new Vector3(x, y, 0f), _fitnessGraphDotRadius);
            }
        }


        private void OnModelGenerationNumberChanged(int generationNumber)
        {
            UpdateGenerationNumberText(generationNumber);
            UpdateBestFitnessText();
        }

        private void OnGenerationEvaluated(int generationNumber)
        {
            if (_save)
            {
                var save = m_Model.GetCurrentPopulationSaveData();
                var path = Path.Join(_savePath, m_SaveFolderName, $"gen_{generationNumber - 1}.json");
                var json = JsonUtility.ToJson(save, true);
            
                File.WriteAllText(path, json);
            }
        }

        private void OnModelBrainSizeChanged(int brainSize)
        {
            UpdateBrainSizeText(brainSize);
        }

        
        private void UpdateGenerationNumberText(int value)
        {
            _generationNumberText.text = $"Generation {value}";
        }

        private void UpdateBrainSizeText(int value)
        {
            _brainSizeText.text = $"Brain Size: {value}";
        }

        private void UpdateBestFitnessText()
        {
            var best = m_Model.GetFitnessValues()
                .OrderByDescending(e => e)
                .FirstOrDefault();
            _bestFitnessText.text = $"Best Fitness: {best:f3}";
        }
    }
}