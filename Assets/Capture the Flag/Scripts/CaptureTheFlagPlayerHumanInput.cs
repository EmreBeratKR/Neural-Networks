using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayerHumanInput : ICaptureTheFlagPlayerInput
    {
        public static CaptureTheFlagPlayerHumanInput New(CaptureTheFlagPlayer player)
        {
            return new CaptureTheFlagPlayerHumanInput
            {
                m_Player = player
            };
        }

        private CaptureTheFlagPlayer m_Player;
        
        
        public void OnUpdate()
        {
            if (Input.GetKey(KeyCode.W))
            {
                m_Player.MoveUp();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                m_Player.MoveDown();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                m_Player.MoveRight();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                m_Player.MoveLeft();
            }
        }
    }
}