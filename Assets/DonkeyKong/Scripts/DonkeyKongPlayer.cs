using Physics;
using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongPlayer : MonoBehaviour
    {
        private DonkeyKongGame m_Game;
        private float m_VerticalVelocity;
        
        
        private void Update()
        {
            const float speed = 3f;
            const float gravity = -9.81f * 1f;
            const float jumpSpeed = 5f;
            
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * (Time.deltaTime * speed);
            }
            
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * (Time.deltaTime * speed);
            }

            if (m_VerticalVelocity < 0f)
            {
                m_VerticalVelocity += gravity * Time.deltaTime * 2f;
            }

            else
            {
                m_VerticalVelocity += gravity * Time.deltaTime;
            }
            
            
            CollideWithGround();
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_VerticalVelocity = jumpSpeed;
            }
            
            transform.position += Vector3.up * (m_VerticalVelocity * Time.deltaTime);
        }


        private void CollideWithGround()
        {
            if (m_VerticalVelocity > 0f) return;
            /*if (transform.position.y <= -3f)
            {
                m_VerticalVelocity = 0f;
                var temp = transform.position;
                temp.y = -3f;
                transform.position = temp;
            }*/

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

                /*if (Collisions2D.LineSegmentAndAxisAlignedRectIntersection(rayStart, rayEnd, groundCenter, groundSize, out var point))
                {
                    m_VerticalVelocity = 0f;
                    transform.position = point;
                }*/

                if (Collisions2D.LineSegmentAndLineSegmentIntersection(rayStart, rayEnd, groundTopLeft, groundTopRight, out var point))
                {
                    m_VerticalVelocity = 0f;
                    transform.position = point;
                }
            }
        }
        

        public void SetGame(DonkeyKongGame game)
        {
            m_Game = game;
        }
    }
}