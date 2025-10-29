using GeneticAlgorithm;
using TMPro;
using UnityEngine;

namespace Value_Match
{
    public class ValueMatchPlayer : MonoBehaviour, 
        IGeneticAlgorithmEntity
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TMP_Text _valueField;
        
        
        private ValueMatchGame m_Game;
        private IGeneticAlgorithmBrain m_Brain;


        private void Update()
        {
            if (m_Brain is not null)
            {
                var value = (byte) m_Brain.GetAction(0);
                SetValue(value);
            }
        }


        public IGeneticAlgorithmBrain GetBrain()
        {
            return m_Brain;
        }

        public void SetBrain(IGeneticAlgorithmBrain brain)
        {
            m_Brain = brain;
        }

        public void SetValue(int value)
        {
            var normalized = value / 255f;
            _spriteRenderer.color = new Color(normalized, normalized, normalized, 1f);
            _valueField.text = value.ToString();
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void SetGame(ValueMatchGame game)
        {
            m_Game = game;
        }

        public float GetFitness()
        {
            var value = m_Brain.GetAction(0);
            var targetValue = m_Game.GetValue();
            var absDif = Mathf.Abs(value - targetValue);

            return 1f - (absDif / 255f);
        }
    }
}