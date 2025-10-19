namespace DonkeyKong
{
    public interface IDonkeyKongPlayerInput
    {
        bool IsJump();
        bool IsGoRight();
        bool IsGoLeft();
        bool IsLadderUp();
        bool IsLadderDown();
    }
}