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

        public override Character Owner
        {
            get => base.Owner;
            set
            {
                if (value)
                {
                    // when character attack, enable weapon collider to detect weapon hits
                    // the collider needs to set enable on animation events because the animation 
                    // needs a delay to start attack definitive
                    value.OnAttackAnimationEvent.AddListener(EnableWeaponCollider);
                }

                if (base.Owner)
                {
                    base.Owner.OnAttackAnimationEvent.RemoveListener(EnableWeaponCollider);
                }

                base.Owner = value;
            }
        }

        public SpearWeapon()
        {
            AttackForce = 15;
            MinDotAngleToAttack = 0.8f;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            if (Owner && !Owner.IsStoped)
            {
                DisableAttack();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking || other.gameObject == Owner.gameObject)
            {
                return;
            }

            // apply attack on detect character
            if (other.gameObject.TryGetComponent<Character>(out var character))
            {
                // when has a specific target, attack only this target, if not attack any character
                if (WeaponTarget && WeaponTarget != character)
                {
                    return;
                }

                character.AddDamage(CurrentAttackDamage, Owner.transform.forward * CurrentAttackForce);
            }
        }

        private Enemy FindNearEnemy()
        {
            var _enemies = FindObjectsOfType<Enemy>();

            if (_enemies.Length == 0)
            {
                return null;
            }

            var _near = _enemies[0];
            var _distance = Vector3.Distance(Owner.transform.position, _near.transform.position);

            foreach (var c in _enemies)
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
            yield return new WaitForSeconds(CurrentAttackLength);
            DisableAttack();
        }

        private void DisableAttack()
        {
            IsAttacking = false;
            WeaponTarget = null;
            DisableWeaponCollider();
        }

        public override void Attack(Enemy target = null)
        {
            if (IsAttacking || !Owner.IsStoped)
            {
                return;
            }

            var _targetToLookAt = target ? target : FindNearEnemy();

            if (!_targetToLookAt)
            {
                return;
            }

            var _directionToTarget = (_targetToLookAt.transform.position - Owner.transform.position).normalized;
            var _angleToTarget = Vector3.Dot(_directionToTarget, Owner.transform.forward);

            // wait character turn to target to after attack
            if (_angleToTarget <= MinDotAngleToAttack)
            {
                return;
            }

            base.Attack(target);

            StartCoroutine(DisableAttackDeleyed());
        }
    }
}