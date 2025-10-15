using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayer : MonoBehaviour
    {
        private CaptureTheFlagGame m_Game;
        private ICaptureTheFlagPlayerInput m_Input;
        private bool m_IsCapturedFlag;
        
        
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
            Debug.Log("flag captured");
        }

        private bool CanPlay()
        {
            if (m_IsCapturedFlag) return false;
            
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
    }
}