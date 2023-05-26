using Assets.Source.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class HoplonPowerUpSuperShield : HoplonPowerUpShield
    {
        [Header("Super Shield")]

        [Range(0.02f, 1f)]
        public float ExpelRefreshRate;
        public float ShieldSize;
        public GameObject ShieldModel;

        [Header("Super Power Up")]
        public float Life;

        public float CurrentLife { get; private set; }

        private float _expelRefreshTimer;

        private void Start()
        {
            CurrentLife = Life;
        }

        private void FixedUpdate()
        {
            if (CurrentLife <= 0)
            {
                return;
            }

            _expelRefreshTimer += Time.deltaTime;
            if (_expelRefreshTimer > ExpelRefreshRate)
            {
                ExpelAllNearEnemies();
                _expelRefreshTimer = 0;
            }
        }

        private void ExpelAllNearEnemies()
        {
            var _colliders = Physics.OverlapSphere(transform.position, ShieldSize, -1);

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].TryGetComponent<Enemy>(out var enemy))
                {
                    var _additionalForce = Force - enemy.ExternalForce.magnitude;
                    ExpelEnemy(enemy, _additionalForce);
                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;

            if (IsEnemy(other, out var enemy))
            {
                CurrentLife -= enemy.AttackDamage;

                if (CurrentLife <= 0)
                {
                    DisablePowerUp();
                    Invoke(nameof(EnablePowerUp), Cooldown);
                    return;
                }
            }

            base.OnTriggerEnter(other);
        }

        private void EnablePowerUp()
        {
            CurrentLife = Life;
            ShieldModel.SetActive(true);
        }

        private void DisablePowerUp()
        {
            CurrentLife = 0;
            ShieldModel.SetActive(false);
        }
    }
}