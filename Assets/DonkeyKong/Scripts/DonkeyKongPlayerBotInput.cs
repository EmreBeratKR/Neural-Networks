namespace DonkeyKong
{
    public class DonkeyKongPlayerBotInput : IDonkeyKongPlayerInput
    {
        public static DonkeyKongPlayerBotInput New(DonkeyKongPlayer player)
        {
            return new DonkeyKongPlayerBotInput
            {
                m_Player = player,
            };
        }
        
        
        private DonkeyKongPlayer m_Player;


        private bool IsAction(int action)
        {
            var frameIndex = m_Player.GetFrameIndex();
            var brain = m_Player.GetBrain();
            var brainSize = brain.GetSize();

            if (frameIndex >= brainSize)
            {
                m_Player.Stop();
                return false;
            }
            
            return brain.GetAction(frameIndex) == action;
        }
        
        
        public bool IsJump()
        {
            return IsAction(0);
        }

        public bool IsGoRight()
        {
            return IsAction(1);
        }

        public bool IsGoLeft()
        {
            return IsAction(2);
        }

        public bool IsLadderUp()
        {
            return IsAction(3);
        }

        public bool IsLadderDown()
        {
            return IsAction(4);
        }
    }
}