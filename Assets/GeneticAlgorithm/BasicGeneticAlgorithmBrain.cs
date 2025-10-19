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
            for (int i = 0; i < count; i++)
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

        
        public IGeneticAlgorithmBrain NewEmpty()
        {
            return New(0, m_ActionTypeCount);
        }
        
        public int GetSize()
        {
            return m_Actions.Count;
        }
        
        public void IncreaseSize(int size)
        {
            AddRandomActions(size);
        }

        public void AddAction(int action)
        {
            for (var i = 0; i < 10; i++)
            {
                m_Actions.Add(action);
            }
        }
        
        public int GetAction(int index)
        {
            return m_Actions[index];
        }
        
        public void Mutate(float rate)
        {
            var size = GetSize();
            for (var i = 0; i < size; i++)
            {
                if (rate < Random.Range(0f, 1f)) continue;

                m_Actions[i] = GetRandomAction();
            }
        }
    }
}