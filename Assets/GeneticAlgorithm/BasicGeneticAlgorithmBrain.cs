using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithm
{
    public class BasicGeneticAlgorithmBrain : IGeneticAlgorithmBrain
    {
        public static BasicGeneticAlgorithmBrain New(int size, int actionTypeCount)
        {
            var brain = new BasicGeneticAlgorithmBrain
            {
                m_ActionTypeCount = actionTypeCount,
                m_Actions = new List<int>(size)
            };
            brain.AddRandomActions(size);
            return brain;
        }


        private int m_ActionTypeCount;
        private List<int> m_Actions;


        private void AddRandomActions(int count)
        {
            for (var i = 0; i < count; i++)
            {
                AddRandomAction();
            }
        }
        
        private void AddRandomAction()
        {
            var action = GetRandomAction();
            AddAction(action);
        }

        private int GetRandomAction()
        {
            return Random.Range(0, m_ActionTypeCount);
        }
        
        private void AddAction(int action)
        {
            m_Actions.Add(action);
        }

        
        public IGeneticAlgorithmBrain Copy()
        {
            var size = GetSize();
            var brain = New(size, m_ActionTypeCount);

            for (var i = 0; i < size; i++)
            {
                brain.SetAction(GetAction(i), i);
            }

            return brain;
        }
        
        public int GetSize()
        {
            return m_Actions.Count;
        }
        
        public int GetAction(int index)
        {
            return m_Actions[index];
        }

        public void SetAction(int action, int index)
        {
            m_Actions[index] = action;
        }
        
        public void Mutate()
        {
            var index = Random.Range(0, GetSize());
            m_Actions[index] = GetRandomAction();
        }
    }
}