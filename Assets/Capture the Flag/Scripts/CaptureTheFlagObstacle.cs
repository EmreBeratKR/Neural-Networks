using Physics;
using UnityEngine;

namespace Capture_the_Flag
{
    public class CaptureTheFlagObstacle : MonoBehaviour
    {
        private CaptureTheFlagGame m_Game;


        private void Awake()
        {
            m_Game = GetComponentInParent<CaptureTheFlagGame>(true);
        }

        private void Update()
        {
            var players = m_Game.GetPlayers();

            foreach (var player in players)
            {
                var playerPosition = (Vector2) player.GetPosition();
                var playerRadius = player.GetRadius();
                var center = (Vector2) transform.position;
                var size = (Vector2) transform.localScale;
                var rot = transform.eulerAngles.z;
                var isColliding = Collisions2D.CheckCircleAndRotatedRect(playerPosition, playerRadius, center, size, rot);
                
                if (!isColliding) continue;
                
                player.Die();
            }
        }
    }
}