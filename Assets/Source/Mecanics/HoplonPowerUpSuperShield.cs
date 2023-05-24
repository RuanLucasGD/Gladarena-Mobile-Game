using Assets.Source.Mecanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class HoplonPowerUpSuperShield : HoplonPowerUpShield
    {
        [Header("Power Up Collision")]
        public Collider Collider;

        [Header("Super Power Up")]
        public float Life;

        public float CurrentLife { get;private set; }   

        private void Start()
        {
            CurrentLife = Life;
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
            Collider.gameObject.SetActive(true);
            CurrentLife = Life;
        }

        private void DisablePowerUp()
        {
            Collider.gameObject.SetActive(false);
            CurrentLife = 0;
        }
    }
}