using System;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayer : MonoBehaviour
    {
        private CaptureTheFlagGame m_Game;
        private CaptureTheFlagPlayerBrain m_Brain;
        private ICaptureTheFlagPlayerInput m_Input;
        private bool m_IsCapturedFlag;
        private bool m_IsStopped;
        private float m_StartTime;


        public static Action<CaptureTheFlagPlayer> OnAnyStop;


        private void Start()
        {
            m_StartTime = Time.time;
        }


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

            return sqrDistance < 0.01f;
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
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetInput(ICaptureTheFlagPlayerInput input)
        {
            m_Input = input;
        }

        public CaptureTheFlagPlayerBrain GetBrain()
        {
            return m_Brain;
        }
        
        public void SetBrain(CaptureTheFlagPlayerBrain brain)
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

        public void Stop()
        {
            m_IsStopped = true;
            OnAnyStop?.Invoke(this);
        }

        public bool IsDone()
        {
            return m_IsCapturedFlag || m_IsStopped;
        }

        public float GetElapsedTime()
        {
            return Time.time - m_StartTime;
        }

        public float CalculateFitness()
        {
            var flagPosition = m_Game.GetState().flagPosition;
            var position = (Vector2) transform.position;
            var distance = Vector2.Distance(flagPosition, position);
            var elapsedTime = GetElapsedTime();

            return 1f / (distance + (elapsedTime / 60f) + 1f);
        }
    }
}