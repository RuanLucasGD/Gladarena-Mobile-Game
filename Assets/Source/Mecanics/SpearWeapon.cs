using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SpearWeapon : Weapon
    {
        [Header("Spear Attack")]
        [Range(0, 1)] public float MinDotAngleToAttack;

        [Header("Components")]
        public Collider WeaponCollider;

        private List<Character> _attackedEnemies; // list ta be contain all attacked enemies of earch attack

        public SpearWeapon()
        {
            AttackForce = 15;
            MinDotAngleToAttack = 0.8f;
        }

        protected override void Awake()
        {
            _attackedEnemies = new List<Character>();

            base.Awake();

            OnEnableAttack += EnableWeaponCollider;
            OnDisableAttack += DisableWeaponCollider;

            DisableWeaponCollider();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking || !WeaponTarget)
            {
                return;
            }

            // apply attack on detect target
            if (other.gameObject.TryGetComponent<Character>(out var character))
            {
                if (character == WeaponTarget)  // attack only one enemy
                {
                    foreach (var attackedEnemy in _attackedEnemies)
                    {
                        if (character == attackedEnemy) // don't attack enemy if is already attacked
                        {
                            return;
                        }
                    }

                    WeaponTarget.AddDamage(Damage, Owner.transform.forward * AttackForce);
                    _attackedEnemies.Add(WeaponTarget);
                }
            }
        }

        private Character FindNearEnemy()
        {
            var _allCharacters = FindObjectsOfType<Character>();
            var _characters = new List<Character>();

            foreach (var c in _allCharacters)
            {
                if (c == Owner)
                {
                    continue;
                }

                _characters.Add(c);
            }

            if (_characters.Count == 0)
            {
                return null;
            }

            var _near = _characters[0];
            var _distance = Vector3.Distance(Owner.transform.position, _near.transform.position);

            foreach (var c in _characters)
            {
                var _characterDistance = Vector3.Distance(Owner.transform.position, c.transform.position);

                if (_characterDistance < _distance)
                {
                    _distance = _characterDistance;
                    _near = c;
                }
            }

            if (_distance > AttackRange)
            {
                _near = null;
            }

            return _near;
        }

        private Vector3 GetCharacterCenter(Character character)
        {
            return character.transform.position + character.CharacterController.center;
        }

        private void EnableWeaponCollider()
        {
            WeaponCollider.enabled = true;
        }

        private void DisableWeaponCollider()
        {
            WeaponCollider.enabled = false;
        }

        private IEnumerator DisableAttackDeleyed()
        {
            yield return new WaitForSeconds(FinalAttackLength);

            IsAttacking = false;
            WeaponTarget = null;
            _attackedEnemies.Clear();
        }

        public override void Attack(Character target = null)
        {
            if (IsAttacking || !Owner.IsStoped)
            {
                return;
            }

            WeaponTarget = target != null ? target : FindNearEnemy();

            if (!WeaponTarget)
            {
                return;
            }

            var _angleToTarget = Vector3.Dot((WeaponTarget.transform.position - Owner.transform.position).normalized, Owner.transform.forward);

            if (_angleToTarget <= MinDotAngleToAttack)
            {
                return;
            }

            base.Attack(WeaponTarget);

            StartCoroutine(DisableAttackDeleyed());
        }
    }
}