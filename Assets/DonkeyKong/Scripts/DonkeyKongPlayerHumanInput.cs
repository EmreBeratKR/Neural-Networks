using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongPlayerHumanInput : IDonkeyKongPlayerInput
    {
        public bool IsJump()
        {
            return Input.GetKey(KeyCode.Space);
        }

        public bool IsGoRight()
        {
            return Input.GetKey(KeyCode.RightArrow);
        }

        public bool IsGoLeft()
        {
            return Input.GetKey(KeyCode.LeftArrow);
        }

        public bool IsLadderUp()
        {
            return Input.GetKey(KeyCode.UpArrow);
        }

        public bool IsLadderDown()
        {
            return Input.GetKey(KeyCode.DownArrow);
        }
    }
}