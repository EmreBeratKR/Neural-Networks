using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagGame : MonoBehaviour
    {
        [SerializeField] private CaptureTheFlagPlayer _playerPrefab;
        [SerializeField] private Flag _flagPrefab;


        private CaptureTheFlagPlayer m_Player;
        private Flag m_Flag;
        private bool m_IsStarted;
        
        
        private void Start()
        {
            var startPosition = GetRandomStartPosition();
            var flagPosition = GetRandomFlagPosition();
            var player = Instantiate(_playerPrefab);
            var input = CaptureTheFlagPlayerHumanInput.New(player);
            player.SetGame(this);
            player.SetInput(input);
            player.SetPosition(startPosition);
            m_Player = player;

            var flag = Instantiate(_flagPrefab);
            flag.SetPosition(flagPosition);
            m_Flag = flag;

            m_IsStarted = true;
        }


        private Vector3 GetRandomStartPosition()
        {
            return new Vector3(Random.Range(-5f, 5f), -4.5f, 0f);
        }
        
        private Vector3 GetRandomFlagPosition()
        {
            return new Vector3(Random.Range(-5f, 5f), 4.5f, 0f);
        }


        public CaptureTheFlagGameState GetState()
        {
            return new CaptureTheFlagGameState
            {
                flagPosition = m_Flag.GetPosition(),
                isStarted = m_IsStarted
            };
        }
    }
}