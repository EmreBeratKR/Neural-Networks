using System;
using GeneticAlgorithm;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayer : MonoBehaviour,
        IGeneticAlgorithmEntity
    {
        [SerializeField] private Transform _visual;
        
        
        private CaptureTheFlagGame m_Game;
        private IGeneticAlgorithmBrain m_Brain;
        private ICaptureTheFlagPlayerInput m_Input;
        private bool m_IsCapturedFlag;
        private bool m_IsStopped;
        private bool m_IsDead;


        public static Action<CaptureTheFlagPlayer> OnAnyStop;
        

        private void Update()
        {
            if (!CanPlay()) return;
            
            m_Input.OnUpdate();

            if (CanCaptureFlag())
            {
                CaptureFlag();
            }
        }


        private void Move(Vector3 direction)
        {
            const float speed = 3f;
            transform.position += direction * (Time.deltaTime * speed);
        }

        private bool CanCaptureFlag()
        {
            var flagPosition = m_Game.GetState().flagPosition;
            var position = (Vector2) transform.position;
            var sqrDistance = Vector2.SqrMagnitude(flagPosition - position);
            var radius = GetRadius();
            var sqrRadius = radius * radius;

            return sqrDistance < sqrRadius;
        }

        private void CaptureFlag()
        {
            if (m_IsCapturedFlag) return;
            m_IsCapturedFlag = true;
            Stop();
        }

        private bool CanPlay()
        {
            if (m_IsCapturedFlag) return false;

            if (m_IsStopped) return false;
            
            return m_Game.GetState().isStarted;
        }


        public void SetGame(CaptureTheFlagGame game)
        {
            m_Game = game;
        }
        
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public float GetRadius()
        {
            return _visual.localScale.x;
        }

        public void SetInput(ICaptureTheFlagPlayerInput input)
        {
            m_Input = input;
        }

        public IGeneticAlgorithmBrain GetBrain()
        {
            return m_Brain;
        }
        
        public void SetBrain(IGeneticAlgorithmBrain brain)
        {
            m_Brain = brain;
        }

        public void MoveUp()
        {
            Move(Vector3.up);
        }

        public void MoveDown()
        {
            Move(Vector3.down);
        }

        public void MoveRight()
        {
            Move(Vector3.right);
        }
        
        public void MoveLeft()
        {
            Move(Vector3.left);
        }

        public void Die()
        {
            m_IsDead = true;
            Stop();
        }
        
        public void Stop()
        {
            m_IsStopped = true;
            OnAnyStop?.Invoke(this);
        }

        public bool IsDone()
        {
            return m_IsCapturedFlag || m_IsStopped;
        }

        public float CalculateFitness()
        {
            if (m_IsDead) return 0f;

            var position = (Vector2) transform.position;
            var gameState = m_Game.GetState();
            var flagPosition = gameState.flagPosition;
            var distanceToFlag = Vector2.Distance(flagPosition, position);

            return 1f / (1f + distanceToFlag);
        }
    }
}