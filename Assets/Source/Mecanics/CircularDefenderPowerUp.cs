using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class CircularDefenderPowerUp : PowerUp
    {
        public float TurnSpeed;
        public float Force;

        private float _currentRot;

        void Update()
        {
            _currentRot += TurnSpeed * Time.deltaTime;
            if (_currentRot > 360) _currentRot -= 360;
            if (_currentRot < 0) _currentRot += 360;

            transform.rotation = Quaternion.Euler(Vector3.up * _currentRot);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                if (c != Owner)
                {
                    c.AddExternalForce((c.transform.position - transform.position).normalized * Force);
                }
            }
        }
    }
}