using UnityEditor;
using UnityEngine;

namespace GeneticAlgorithm
{
    [CustomPropertyDrawer(typeof(GeneticAlgorithmParameters))]
    public class GeneticAlgorithmParametersPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty m_PopulationCountProp;
        private SerializedProperty m_BrainSizeProp;
        
        private SerializedProperty m_IsFixedBrainSizeProp;
        private SerializedProperty m_GenPerBrainSizeIncreaseProp;
        
        private SerializedProperty m_IsFrameDependentProp;
        private SerializedProperty m_FramesPerSecondProp;
        
        private SerializedProperty m_ParentSelectionOperationTypeProp;
        private SerializedProperty m_CrossoverOperationTypeProp;
        private SerializedProperty m_MutationOperationTypeProp;
        
        private SerializedProperty m_ElitismRateProp;
        private SerializedProperty m_CrossoverRateProp;
        private SerializedProperty m_MutationRateProp;
        
        private SerializedProperty m_TerminationConditionProp;
        
        private SerializedProperty m_NotEnoughProgressionSampleCountProp;
        private SerializedProperty m_NotEnoughProgressionThresholdProp;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));
            
            m_PopulationCountProp = property.FindPropertyRelative("populationCount");
            m_BrainSizeProp = property.FindPropertyRelative("brainSize");
            
            m_IsFixedBrainSizeProp = property.FindPropertyRelative("isFixedBrainSize");
            m_GenPerBrainSizeIncreaseProp = property.FindPropertyRelative("genPerBrainSizeIncrease");
            
            m_IsFrameDependentProp = property.FindPropertyRelative("isFrameDependent");
            m_FramesPerSecondProp = property.FindPropertyRelative("framesPerSeconds");
            
            m_ParentSelectionOperationTypeProp = property.FindPropertyRelative("parentSelectionOperator");
            m_CrossoverOperationTypeProp = property.FindPropertyRelative("crossoverOperator");
            m_MutationOperationTypeProp = property.FindPropertyRelative("mutationOperator");
            
            m_ElitismRateProp = property.FindPropertyRelative("elitismRate");
            m_CrossoverRateProp = property.FindPropertyRelative("crossoverRate");
            m_MutationRateProp = property.FindPropertyRelative("mutationRate");
            
            m_TerminationConditionProp = property.FindPropertyRelative("terminationCondition");
            
            m_NotEnoughProgressionSampleCountProp = property.FindPropertyRelative("lowVarianceSampleCount");
            m_NotEnoughProgressionThresholdProp = property.FindPropertyRelative("lowVarianceThreshold");

            var boldStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20
            };
            EditorGUI.LabelField(rect, "Hyper Parameters", boldStyle);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(rect, m_PopulationCountProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(rect, m_BrainSizeProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
            rect = BrainSizeIncreaseGUI(rect, property, label);
            rect = FrameRateGUI(rect, property, label);
            
            EditorGUI.PropertyField(rect, m_ParentSelectionOperationTypeProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
            EditorGUI.PropertyField(rect, m_ElitismRateProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            
            
            EditorGUI.PropertyField(rect, m_CrossoverOperationTypeProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            if ((CrossoverOperatorType) m_CrossoverOperationTypeProp.intValue is not CrossoverOperatorType.None)
            {
                EditorGUI.PropertyField(rect, m_CrossoverRateProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            
            EditorGUI.PropertyField(rect, m_MutationOperationTypeProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            if ((MutationOperatorType) m_MutationOperationTypeProp.intValue is not MutationOperatorType.None)
            {
                EditorGUI.PropertyField(rect, m_MutationRateProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            
            EditorGUI.PropertyField(rect, m_TerminationConditionProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            if ((TerminationConditionType) m_TerminationConditionProp.intValue is TerminationConditionType.LowFitnessValueVariance)
            {
                EditorGUI.PropertyField(rect, m_NotEnoughProgressionSampleCountProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, m_NotEnoughProgressionThresholdProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private Rect BrainSizeIncreaseGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(rect, m_IsFixedBrainSizeProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            if (!m_IsFixedBrainSizeProp.boolValue)
            {
                EditorGUI.PropertyField(rect, m_GenPerBrainSizeIncreaseProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            return rect;
        }

        private Rect FrameRateGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(rect, m_IsFrameDependentProp);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            if (m_IsFrameDependentProp.boolValue)
            {
                EditorGUI.PropertyField(rect, m_FramesPerSecondProp);
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            
            return rect;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 300f;
        }
    }
}