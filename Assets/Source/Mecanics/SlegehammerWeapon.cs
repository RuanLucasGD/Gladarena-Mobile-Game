using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SlegehammerWeapon : Weapon
    {
        public float DeactiveAttackDelay;

        [Space]
        public Transform ApplyDamagePoint;
        public float AttackForceBack;
        public float AttackForceUp;

        private float _attackTimer;

        protected override void Update()
        {
            base.Update();
            UpdateAttackTimer();
        }

        private void UpdateAttackTimer()
        {
            if (!IsAttacking)
            {
                return;
            }

            _attackTimer += Time.deltaTime;

            if (_attackTimer > DeactiveAttackDelay)
            {
                _attackTimer = 0f;
                IsAttacking = false;
            }
        }

        /// <summary>
        /// Apply attack on enemies.
        /// Called on Unity Animation Event - On Weapon animation
        /// </summary>
        public void ApplySlegehammerAttack()
        {
            if (!IsAttacking)
            {
                return;
            }

            var _characters = GetNearCharacters();

            foreach (var character in _characters)
            {
                var _forceDirection = Vector3.ProjectOnPlane((character.transform.position - ApplyDamagePoint.position).normalized, Vector3.up);
                _forceDirection *= AttackForceBack;
                _forceDirection += Vector3.up * AttackForceUp;

                character.AddDamage(Damage);
                character.AddExternalForce(_forceDirection);
            }
        }

        private Character[] GetNearCharacters()
        {
            var _characters = new List<Character>(FindObjectsOfType<Character>());

            // remove the owner of this weapon. Apply damage only on enemies
            _characters.Remove(Owner);

            // remove all distant characters 
            for (int i = 0; i < _characters.Count; i++)
            {
                var _weaponPosition = ApplyDamagePoint.position;
                var _enemyPosition = _characters[i].transform.position;

                if (Vector3.Distance(_weaponPosition, _enemyPosition) > AttackRange)
                {
                    _characters.Remove(_characters[i]);
                }
            }

            return _characters.ToArray();
        }
    }
}