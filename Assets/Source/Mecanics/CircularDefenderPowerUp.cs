using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class CircularDefenderPowerUp : PowerUp
    {
        [Header("Behaviour")]
        public float TurnSpeed;
        public float Force;

        [Header("Timers")]
        public float DisarmAfterTime;
        public float RearmAfterTime;

        public GameObject Model;

        private float _currentRot;

        public bool PowerUpEnabled { get; private set; }

        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;
                EnablePowerUp();
            }
        }

        void Update()
        {
            if (!PowerUpEnabled)
            {
                return;
            }

            _currentRot += TurnSpeed * Time.deltaTime;
            if (_currentRot > 360) _currentRot -= 360;
            if (_currentRot < 0) _currentRot += 360;

            transform.rotation = Quaternion.Euler(Vector3.up * _currentRot);
        }

        private void EnablePowerUp()
        {
            PowerUpEnabled = true;
            Model.SetActive(true);
            Invoke(nameof(DisablePowerUp), DisarmAfterTime);
        }

        private void DisablePowerUp()
        {
            PowerUpEnabled = false;
            Model.SetActive(false);
            Invoke(nameof(EnablePowerUp), RearmAfterTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PowerUpEnabled)
            {
                return;
            }
            if (other.TryGetComponent<Enemy>(out var c))
            {
                if (c != Owner)
                {
                    //c.AddExternalForce((c.transform.position - transform.position).normalized * Force);
                }
            }
        }
    }
}