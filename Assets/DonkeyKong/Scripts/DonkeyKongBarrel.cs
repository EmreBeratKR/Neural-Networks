using System;
using System.Collections.Generic;
using Physics;
using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongBarrel : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _radius;
        
        
        public static Action<DonkeyKongBarrel> OnAnyDestroyed;
        
        
        private DonkeyKongGame m_Game;
        private float m_VerticalVelocity;
        private bool m_IsGoingLeft;
        private bool m_IsFallingFromLadder;


        private List<DonkeyKongLadder> m_IgnoredLadders;


        private void Awake()
        {
            m_IgnoredLadders = new List<DonkeyKongLadder>();
        }

        private void OnDrawGizmos()
        {
            var center = GetCenter();
            var radius = GetRadius();
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(center, radius);
        }


        private void Update()
        {
            var gravity = -9.81f * m_Game.GetConfig().barrelGravity;
            var speed = m_Game.GetConfig().barrelHorizontalSpeed;
            
            TryFallFromLadder();
            
            _animator.SetBool("isLadderRoll", m_IsFallingFromLadder);
            
            if (m_IsFallingFromLadder) return;
            
            m_VerticalVelocity += gravity * Time.deltaTime;
            
            CollideWithGround();
            CollideWithWall();

            var horizontalVelocity = speed * (m_IsGoingLeft ? -1f : 1f);
            _animator.SetFloat("roll", m_IsGoingLeft ? 1f : -1f);
            transform.position += new Vector3(horizontalVelocity, m_VerticalVelocity) * Time.deltaTime;
            
            DestroyIfFell();
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

        private void CollideWithWall()
        {
            var walls = m_Game.GetWalls();

            foreach (var wall in walls)
            {
                const float rayDistance = 0.15f;
                var rayStart = (Vector2) transform.position + Vector2.up * 0.1f;
                var rayDirection = m_IsGoingLeft ? Vector2.left : Vector2.right;
                var rayEnd = rayStart + rayDirection * rayDistance;
                var wallCenter = wall.GetCenter();
                var wallSize = wall.GetSize();
                
                if (Collisions2D.LineSegmentAndAxisAlignedRectIntersection(rayStart, rayEnd, wallCenter, wallSize, out var point))
                {
                    m_IsGoingLeft = !m_IsGoingLeft;
                }
            }
        }

        private void TryFallFromLadder()
        {
            if (m_IsFallingFromLadder)
            {
                var ladderFallSpeed = m_Game.GetConfig().barrelLadderFallSpeed;
                transform.position += Vector3.down * (Time.deltaTime * ladderFallSpeed);
                
                var grounds = m_Game.GetGrounds();

                foreach (var ground in grounds)
                {
                    if (ground.IsPartOfLadder()) continue;
                    
                    const float rayDistance = 0.06f;
                    var rayStart = (Vector2) transform.position + Vector2.up * 0.01f;
                    var rayEnd = rayStart + Vector2.down * rayDistance;
                    var groundCenter = ground.GetCenter();
                    var groundSize = ground.GetSize();
                    var groundTopLeft = new Vector2(groundCenter.x - groundSize.x * 0.5f, groundCenter.y + groundSize.y * 0.5f);
                    var groundTopRight = new Vector2(groundCenter.x + groundSize.x * 0.5f, groundCenter.y + groundSize.y * 0.5f);

                    if (Collisions2D.LineSegmentAndLineSegmentIntersection(rayStart, rayEnd, groundTopLeft, groundTopRight, out var point))
                    {
                        m_VerticalVelocity = 0f;
                        transform.position = point;
                        m_IsFallingFromLadder = false;
                        m_IsGoingLeft = !m_IsGoingLeft;
                    }
                }
                
                return;
            }
            
            var position = transform.position;
            var ladders = m_Game.GetLadders();

            foreach (var ladder in ladders)
            {
                if (m_IgnoredLadders.Contains(ladder)) continue;
                
                var enterRectCenter = ladder.GetBarrelEnterRectCenter();
                var enterRectSize = ladder.GetBarrelEnterRectSize();

                if (Collisions2D.CheckCircleAndAxisAlignedRect(position, 0f, enterRectCenter, enterRectSize))
                {
                    var barrelFallChance = m_Game.GetConfig().barrelLadderFallChance;
                    if (m_Game.RandomBool(barrelFallChance))
                    {
                        m_IsFallingFromLadder = true;
                    }
                    else
                    {
                        m_IgnoredLadders.Add(ladder);
                    }
                }
            }
        }

        private void DestroyIfFell()
        {
            if (transform.position.y < -10f)
            {
                OnAnyDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
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