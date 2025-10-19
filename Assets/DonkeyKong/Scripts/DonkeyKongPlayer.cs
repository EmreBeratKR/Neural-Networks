using Physics;
using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongPlayer : MonoBehaviour
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _radius;
        
        
        private DonkeyKongGame m_Game;
        private float m_VerticalVelocity;
        private bool m_IsClimbingLadder;
        private bool m_IsDead;


        private void OnDrawGizmos()
        {
            var center = GetCenter();
            var radius = GetRadius();
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(center, radius);
        }

        private void Update()
        {
            const float speed = 1.5f;
            const float gravity = -9.81f * 1.5f;
            const float jumpSpeed = 4.45f;
            
            if (!IsPlaying()) return;
            
            DieIfCollidesWithBarrel();
            
            if (m_IsDead) return;
            
            UseLadder();
            
            if (m_IsClimbingLadder) return;
            
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * (Time.deltaTime * speed);
            }
            
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * (Time.deltaTime * speed);
            }
            
            m_VerticalVelocity += gravity * Time.deltaTime;
            
            CollideWithGround();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_VerticalVelocity = jumpSpeed;
            }
            
            transform.position += Vector3.up * (m_VerticalVelocity * Time.deltaTime);
        }


        private void DieIfCollidesWithBarrel()
        {
            var barrels = m_Game.GetMonkey().GetBarrels();

            foreach (var barrel in barrels)
            {
                var center = GetCenter();
                var radius = GetRadius();
                var barrelCenter = barrel.GetCenter();
                var barrelRadius = barrel.GetRadius();

                if (Collisions2D.CircleAndCircleIntersection(center, radius, barrelCenter, barrelRadius))
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            m_IsDead = true;
        }

        private void CollideWithGround()
        {
            if (m_VerticalVelocity > 0f) return;

            var grounds = m_Game.GetGrounds();

            foreach (var ground in grounds)
            {
                const float rayDistance = 0.15f;
                var rayStart = (Vector2) transform.position + Vector2.up * 0.1f;
                var rayEnd = rayStart + Vector2.down * rayDistance;
                var groundCenter = ground.GetCenter();
                var groundSize = ground.GetSize();
                var groundTopLeft = new Vector2(groundCenter.x - groundSize.x * 0.5f, groundCenter.y + groundSize.y * 0.5f);
                var groundTopRight = new Vector2(groundCenter.x + groundSize.x * 0.5f, groundCenter.y + groundSize.y * 0.5f);

                if (Collisions2D.LineSegmentAndLineSegmentIntersection(rayStart, rayEnd, groundTopLeft, groundTopRight, out var point))
                {
                    m_VerticalVelocity = 0f;
                    transform.position = point;
                }
            }
        }

        private void UseLadder()
        {
            const float climbSpeed = 1f;
            var climbUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            var climbDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            var playerBottom = transform.position + Vector3.down * 0.03f;
            
            if (!m_IsClimbingLadder && climbUp)
            {
                var ladders = m_Game.GetLadders();

                foreach (var ladder in ladders)
                {
                    if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetBottomRectCenter(), ladder.GetBottomRectSize()))
                    {
                        m_IsClimbingLadder = true;
                    }
                }
            }
            
            if (!m_IsClimbingLadder && climbDown)
            {
                var ladders = m_Game.GetLadders();

                foreach (var ladder in ladders)
                {
                    if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetTopRectCenter(), ladder.GetTopRectSize()))
                    {
                        m_IsClimbingLadder = true;
                    }
                }
            }

            if (m_IsClimbingLadder)
            {
                if (!climbDown)
                {
                    var ladders = m_Game.GetLadders();

                    foreach (var ladder in ladders)
                    {
                        if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetTopRectCenter(), ladder.GetTopRectSize()))
                        {
                            m_IsClimbingLadder = false;
                        }
                    }
                }
                
                if (climbUp)
                {
                    transform.position += Vector3.up * (Time.deltaTime * climbSpeed);
                } 
                
                else if (climbDown)
                {
                    var ladders = m_Game.GetLadders();

                    foreach (var ladder in ladders)
                    {
                        if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetBottomRectCenter(), ladder.GetBottomRectSize()))
                        {
                            m_IsClimbingLadder = false;
                        }
                    }

                    if (m_IsClimbingLadder)
                    {
                        transform.position += Vector3.down * (Time.deltaTime * climbSpeed);
                    }
                    
                }
            }
        }

        private bool IsPlaying()
        {
            return !m_IsDead;
        }
        

        public void SetGame(DonkeyKongGame game)
        {
            m_Game = game;
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public Vector2 GetCenter()
        {
            return (Vector2) transform.position + _offset;
        }
        
        public float GetRadius()
        {
            return _radius;
        }
    }
}