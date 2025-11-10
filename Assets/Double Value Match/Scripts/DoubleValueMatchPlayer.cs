using GeneticAlgorithm;
using TMPro;
using UnityEngine;

namespace Double_Value_Match
{
    public class DoubleValueMatchPlayer : MonoBehaviour, 
        IGeneticAlgorithmEntity
    {
        [SerializeField] private DoubleChannelType _channelType;
        [SerializeField, Range(0f, 1f)] private float _otherChannelValue;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TMP_Text _value0Field;
        [SerializeField] private TMP_Text _value1Field;
        
        
        private DoubleValueMatchGame m_Game;
        private IGeneticAlgorithmBrain m_Brain;


        private void Update()
        {
            if (m_Brain is not null)
            {
                var values = GetValues();
                SetValue(values.Item1, values.Item2);
            }
        }


        public IGeneticAlgorithmBrain GetBrain()
        {
            return m_Brain;
        }

        public void ResetState()
        {
            var values = GetValues();
            SetValue(values.Item1, values.Item2);
        }

        public void SetBrain(IGeneticAlgorithmBrain brain)
        {
            m_Brain = brain;
        }

        public void SetValue(int value0, int value1)
        {
            var normalized0 = value0 / 255f;
            _value0Field.text = value0.ToString();
            
            var normalized1 = value1 / 255f;
            _value1Field.text = value1.ToString();

            if (_channelType is DoubleChannelType.RG)
            {
                _spriteRenderer.color = new Color(normalized0, normalized1, _otherChannelValue, 1f);
            }
            
            if (_channelType is DoubleChannelType.RB)
            {
                _spriteRenderer.color = new Color(normalized0, _otherChannelValue, normalized1, 1f);
            }
            
            if (_channelType is DoubleChannelType.GB)
            {
                _spriteRenderer.color = new Color(_otherChannelValue, normalized0, normalized1, 1f);
            }
        }

        public (int, int) GetValues()
        {
            return (m_Brain.GetAction(0), m_Brain.GetAction(1));
        }

        public float GetColorValue()
        {
            var values = GetValues();
            return (values.Item1 + values.Item2) / 2f;
        }
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void SetGame(DoubleValueMatchGame game)
        {
            m_Game = game;
        }

        public float GetFitness()
        {
            var values = GetValues();
            var targetValues = m_Game.GetValues();
            var absDif0 = Mathf.Abs(values.Item1 - targetValues.Item1);
            var absDif1 = Mathf.Abs(values.Item2 - targetValues.Item2);

            return (1f - (absDif0 / 255f)) * 0.5f + 
                   (1f - (absDif1 / 255f)) * 0.5f;
        }
    }
}