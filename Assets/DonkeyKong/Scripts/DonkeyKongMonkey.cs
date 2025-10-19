using System;
using System.Collections.Generic;
using UnityEngine;

namespace DonkeyKong
{
    public class DonkeyKongMonkey : MonoBehaviour
    {
        [SerializeField] private DonkeyKongGame _game;
        [SerializeField] private DonkeyKongBarrel _barrelPrefab;
        [SerializeField] private Transform _barrelSpawnPoint;


        private List<DonkeyKongBarrel> m_Barrels;
        private float m_LastThrowTime;


        private void Awake()
        {
            m_Barrels = new List<DonkeyKongBarrel>();
            DonkeyKongBarrel.OnAnyDestroyed += OnBarrelDestroyed;
        }

        private void OnDestroy()
        {
            DonkeyKongBarrel.OnAnyDestroyed -= OnBarrelDestroyed;
        }

        private void Update()
        {
            const float barrelThrowInterval = 2.1f;
            var elapsedTime = Time.time - m_LastThrowTime;

            if (elapsedTime > barrelThrowInterval)
            {
                m_LastThrowTime = Time.time;
                ThrowBarrel();
            }
        }


        private void OnBarrelDestroyed(DonkeyKongBarrel barrel)
        {
            m_Barrels.Remove(barrel);
        }
        
        
        private void ThrowBarrel()
        {
            var position = _barrelSpawnPoint.position;
            var barrel = Instantiate(_barrelPrefab);
            barrel.SetGame(_game);
            barrel.SetPosition(position);
            m_Barrels.Add(barrel);
        }


        public List<DonkeyKongBarrel> GetBarrels()
        {
            return m_Barrels;
        }
    }
}