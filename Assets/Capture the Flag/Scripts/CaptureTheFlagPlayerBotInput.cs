namespace Capture_the_Flag
{
    public class CaptureTheFlagPlayerBotInput : ICaptureTheFlagPlayerInput
    {
        public static CaptureTheFlagPlayerBotInput New(CaptureTheFlagPlayer player)
        {
            return new CaptureTheFlagPlayerBotInput
            {
                m_Player = player,
            };
        }

        private CaptureTheFlagPlayer m_Player;
        private int m_BrainIndex;
        
        
        public void OnUpdate()
        {
            var brain = m_Player.GetBrain();
            if (m_BrainIndex >= brain.GetSize())
            {
                m_Player.Stop();
                return;
            }
            var action = brain.GetAction(m_BrainIndex);
            m_BrainIndex += 1;

            if (action == 0)
            {
                m_Player.MoveUp();
            }
            else if (action == 1)
            {
                m_Player.MoveDown();
            }
            else if (action == 2)
            {
                m_Player.MoveRight();
            }
            else if (action == 3)
            {
                m_Player.MoveLeft();
            }
        }

        public void ResetState()
        {
            m_BrainIndex = 0;
        }
    }
}