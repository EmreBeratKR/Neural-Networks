using TMPro;
using UnityEditor;
using UnityEngine;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithmRunner : MonoBehaviour
    {
        [SerializeField] private GameObject _environmentGameObject;
        [SerializeField] private TMP_Text _generationNumberText;
        [SerializeField] private TMP_Text _brainSizeText;
        [SerializeField] private GeneticAlgorithmParameters _parameters;
        
        [Header("Fitness Graph")]
        [SerializeField] private Vector2 _fitnessGraphScale;
        [SerializeField] private Vector2 _fitnessGraphOffset;
        [SerializeField] private float _fitnessGraphDotRadius;


        private GeneticAlgorithmModel m_Model;
        
        
        private void Start()
        {
            var environment = _environmentGameObject.GetComponent<IGeneticAlgorithmEnvironment>();
            m_Model = GeneticAlgorithmModel.New()
                .SetEnvironment(environment)
                .SetParameters(_parameters);

            m_Model.OnGenerationNumberChanged += OnModelGenerationNumberChanged;
            m_Model.OnBrainSizeChanged += OnModelBrainSizeChanged;
            m_Model.Run();
        }

        private void OnDestroy()
        {
            m_Model.OnGenerationNumberChanged -= OnModelGenerationNumberChanged;
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
    }
}