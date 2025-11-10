using GeneticAlgorithm;
using TMPro;
using UnityEngine;

namespace Triple_Value_Match
{
    public class TripleValueMatchPlayer : MonoBehaviour, 
        IGeneticAlgorithmEntity
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TMP_Text _value0Field;
        [SerializeField] private TMP_Text _value1Field;
        [SerializeField] private TMP_Text _value2Field;
        
        
        private TripleValueMatchGame m_Game;
        private IGeneticAlgorithmBrain m_Brain;


        private void Update()
        {
            if (m_Brain is not null)
            {
                var values = GetValues();
                SetValue(values.Item1, values.Item2, values.Item3);
            }
        }


        public IGeneticAlgorithmBrain GetBrain()
        {
            return m_Brain;
        }

        public void ResetState()
        {
            var values = GetValues();
            SetValue(values.Item1, values.Item2, values.Item3);
        }

        public void SetBrain(IGeneticAlgorithmBrain brain)
        {
            m_Brain = brain;
        }

        public void SetValue(int value0, int value1, int value2)
        {
            var normalized0 = value0 / 255f;
            _value0Field.text = value0.ToString();
            
            var normalized1 = value1 / 255f;
            _value1Field.text = value1.ToString();
            
            var normalized2 = value2 / 255f;
            _value1Field.text = value2.ToString();
            
            _spriteRenderer.color = new Color(normalized0, normalized1, normalized2, 1f);
        }

        public (int, int, int) GetValues()
        {
            return (m_Brain.GetAction(0), m_Brain.GetAction(1), m_Brain.GetAction(2));
        }

        public float GetColorValue()
        {
            var values = GetValues();
            return (values.Item1 + values.Item2 + values.Item3) / 3f;
        }
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void SetGame(TripleValueMatchGame game)
        {
            m_Game = game;
        }

        public float GetFitness()
        {
            var values = GetValues();
            var targetValues = m_Game.GetValues();
            var absDif0 = Mathf.Abs(values.Item1 - targetValues.Item1);
            var absDif1 = Mathf.Abs(values.Item2 - targetValues.Item2);
            var absDif2 = Mathf.Abs(values.Item3 - targetValues.Item3);

            return (1f - (absDif0 / 255f)) / 3f + 
                   (1f - (absDif1 / 255f)) / 3f +
                   (1f - (absDif2 / 255f)) / 3f;
        }
    }
}