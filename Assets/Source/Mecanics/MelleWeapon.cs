using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class MelleWeapon : Weapon
    {
        public float AttackLength;
        public float Damage;
        public Collider WeaponCollider;

        private Character _weaponTarget;

        public bool IsAttacking
        {
            set
            {
                enabled = value;
            }
            get
            {
                return enabled;
            }
        }

        public float CurrentAttackDuration { get; private set; }

        public MelleWeapon()
        {
            AttackLength = 0.5f;
            Damage = 40;
        }

        protected virtual void OnEnable()
        {
            WeaponCollider.enabled = true;
        }

        protected virtual void OnDisable()
        {
            WeaponCollider.enabled = false;
        }

        protected override void Awake()
        {
            base.Awake();
            SetupWeapon();
            IsAttacking = false;
        }

        protected override void Update()
        {
            base.Update();
            UpdateAttackTimer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Owner)
            {
                return;
            }

            if (other.gameObject == Owner.gameObject)
            {
                return;
            }

            // TODO: optimize enemy attack avoid TryGetComponent on earch call of OnTriggerEnter
            if (other.gameObject.TryGetComponent<Character>(out var anotherCharacter))
            {
                // when the weapon has a target, check if the anotherCharacter is the target to apply damage
                if (_weaponTarget)
                {
                    if (anotherCharacter.gameObject == _weaponTarget.gameObject)
                    {
                        anotherCharacter.AddDamage(Damage);
                    }
                }
                // if doesn't has a target, the weapon can to apply damage in any characters
                else
                {
                    anotherCharacter.AddDamage(Damage);
                }
            }
        }

        private void SetupWeapon()
        {
            var _collider = GetComponent<Collider>();
            var _rigidBody = GetComponent<Rigidbody>();

            if (_collider) _collider.isTrigger = true;
            if (_rigidBody) _rigidBody.useGravity = false;
        }

        private void UpdateAttackTimer()
        {
            if (!IsAttacking)
            {
                return;
            }

            if (CurrentAttackDuration < AttackLength)
            {
                CurrentAttackDuration += Time.deltaTime;

                if (CurrentAttackDuration > AttackLength)
                {
                    CurrentAttackDuration = 0f;
                    IsAttacking = false;
                    _weaponTarget = null;
                }
            }
        }

        public override void Attack()
        {
            if (IsAttacking)
            {
                return;
            }

            IsAttacking = true;
        }

        public override void Attack(Character target)
        {
            base.Attack(target);
            
            Attack();
            _weaponTarget = target;
        }
    }
}
