using System.Collections.Generic;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayerBrain
    {
        public static CaptureTheFlagPlayerBrain New(int size)
        {
            var brain = new CaptureTheFlagPlayerBrain
            {
                m_Actions = new List<int>(size)
            };
            brain.AddRandomActions(size);
            return brain;
        }


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
            return Random.Range(0, 4);
        }


        public int GetSize()
        {
            return m_Actions.Count;
        }

        public int GetAction(int index)
        {
            return m_Actions[index];
        }

        public void IncreaseSize(int size)
        {
            AddRandomActions(size);
        }
        
        public void AddAction(int action)
        {
            m_Actions.Add(action);
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