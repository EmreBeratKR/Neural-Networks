using Physics;
using UnityEngine;

namespace DonkeyKong
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        ClimbLadder,
        Win,
        Dead
    }
    
    public class DonkeyKongPlayer : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _radius;
        
        
        private DonkeyKongGame m_Game;
        private PlayerState m_State;
        private float m_VerticalVelocity;
        private bool m_IsGrounded;


        private void OnDrawGizmos()
        {
            var center = GetCenter();
            var radius = GetRadius();
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(center, radius);
        }

        private void Update()
        {
            _animator.SetInteger("state", (int) m_State);
            
            if (!IsPlaying()) return;

            if (m_State is PlayerState.Jump && m_IsGrounded && m_VerticalVelocity <= 0f)
            {
                m_State = PlayerState.Idle;
            }
            
            DieIfCollidesWithBarrel();
            DieIfCollidesWithMonkey();
            WinIfCollidesWithPrincess();
            
            if (!IsPlaying()) return;
            
            UseLadder();
            
            if (m_State is PlayerState.ClimbLadder) return;
            
            HorizontalMovement();
            ApplyGravity();
            
            CollideWithGround();
            
            Jump();
            
            transform.position += Vector3.up * (m_VerticalVelocity * Time.deltaTime);
        }


        private void UpdateFacingDirection(bool isRight)
        {
            var scale = _animator.transform.localScale;
            var scaleX = Mathf.Abs(scale.x);
            scale.x = scaleX * (isRight ? 1f : -1f);
            _animator.transform.localScale = scale;
        }
        
        private void Jump()
        {
            if (Input.GetKey(KeyCode.Space) && m_IsGrounded)
            {
                var gravity = Mathf.Abs(GetGravity());
                var maxHeight = m_Game.GetConfig().playerJumpHeight;
                var jumpSpeed = Mathf.Sqrt(2 * maxHeight * gravity);
                m_VerticalVelocity = jumpSpeed;
                m_State = PlayerState.Jump;
            }
        }
        
        private void HorizontalMovement()
        {
            var speed = m_Game.GetConfig().playerHorizontalSpeed;

            if (m_State is PlayerState.Walk)
            {
                m_State = PlayerState.Idle;
            }
            
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                var position = transform.position;
                var x = position.x + Time.deltaTime * speed;
                position.x = Mathf.Min(x, 4.14f);
                transform.position = position;
                UpdateFacingDirection(true);

                if (m_State is not PlayerState.Jump)
                {
                    m_State = PlayerState.Walk;
                }
            }
            
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                var position = transform.position;
                var x = position.x - Time.deltaTime * speed;
                position.x = Mathf.Max(x, -4.14f);
                transform.position = position;
                UpdateFacingDirection(false);
                
                if (m_State is not PlayerState.Jump)
                {
                    m_State = PlayerState.Walk;
                }
            }
        }

        private float GetGravity()
        {
            return -9.81f * m_Game.GetConfig().playerGravity;
        }
        
        private void ApplyGravity()
        {
            var gravity = GetGravity();
            m_VerticalVelocity += gravity * Time.deltaTime;
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
        
        private void DieIfCollidesWithMonkey()
        {
            var monkey = m_Game.GetMonkey();
            var center = GetCenter();
            var radius = GetRadius();
            var monkeyCenter = monkey.GetCenter();
            var monkeySize = monkey.GetSize();

            if (Collisions2D.CheckCircleAndAxisAlignedRect(center, radius, monkeyCenter, monkeySize))
            {
                Die();
            }
        }
        
        private void WinIfCollidesWithPrincess()
        {
            var princess = m_Game.GetPrincess();
            var center = GetCenter();
            var radius = GetRadius();
            var princessCenter = princess.GetCenter();
            var princessSize = princess.GetSize();

            if (Collisions2D.CheckCircleAndAxisAlignedRect(center, radius, princessCenter, princessSize))
            {
                Win();
            }
        }

        private void Win()
        {
            Debug.Log("win");
            m_State = PlayerState.Win;
        }
        
        private void Die()
        {
            Debug.Log("die");
            m_State = PlayerState.Dead;
        }

        private void CollideWithGround()
        {
            if (m_VerticalVelocity > 0f)
            {
                m_IsGrounded = false;
                return;
            }

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
                    m_IsGrounded = true;
                    m_VerticalVelocity = 0f;
                    transform.position = point;
                }
            }
        }

        private void UseLadder()
        {
            if (m_State is PlayerState.Jump) return;
            
            var climbSpeed = m_Game.GetConfig().playerLadderClimbSpeed;
            var climbUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            var climbDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            var playerBottom = transform.position + Vector3.down * 0.03f;
            
            _animator.SetFloat("ladderClimbSpeed", (climbUp || climbDown) ? 1f : 0f);
            
            if (m_State is not PlayerState.ClimbLadder && climbUp)
            {
                var ladders = m_Game.GetLadders();

                foreach (var ladder in ladders)
                {
                    if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetBottomRectCenter(), ladder.GetBottomRectSize()))
                    {
                        m_State = PlayerState.ClimbLadder;
                    }
                }
            }
            
            if (m_State is not PlayerState.ClimbLadder && climbDown)
            {
                var ladders = m_Game.GetLadders();

                foreach (var ladder in ladders)
                {
                    if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetTopRectCenter(), ladder.GetTopRectSize()))
                    {
                        m_State = PlayerState.ClimbLadder;
                    }
                }
            }

            if (m_State is PlayerState.ClimbLadder)
            {
                if (!climbDown)
                {
                    var ladders = m_Game.GetLadders();

                    foreach (var ladder in ladders)
                    {
                        if (Collisions2D.CheckCircleAndAxisAlignedRect(playerBottom, 0f, ladder.GetTopRectCenter(), ladder.GetTopRectSize()))
                        {
                            m_State = PlayerState.Idle;
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
                            m_State = PlayerState.Idle;
                        }
                    }

                    if (m_State is PlayerState.ClimbLadder)
                    {
                        transform.position += Vector3.down * (Time.deltaTime * climbSpeed);
                    }
                    
                }
            }
        }

        private bool IsPlaying()
        {
            return m_State is not PlayerState.Win && m_State is not PlayerState.Dead;
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