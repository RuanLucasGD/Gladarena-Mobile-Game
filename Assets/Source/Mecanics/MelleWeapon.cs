using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class MelleWeapon : Weapon
    {

        public float AttackLength;

        public Collider WeaponCollider;

        public MelleWeapon()
        {
            AttackLength = 0.5f;
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

            OnEnableAttack += EnableWeapon;
            OnDisableAttack += DisableWeapon;
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
                if (WeaponTarget)
                {
                    if (anotherCharacter.gameObject == WeaponTarget.gameObject)
                    {
                        anotherCharacter.AddDamage(Damage);
                    }
                }
                // if doesn't has a target, the weapon can to apply damage in any characters
                else
                {
                    anotherCharacter.AddDamage(Damage);
                }

                AddAttackForce(anotherCharacter);
            }
        }

        private void SetupWeapon()
        {
            var _collider = GetComponent<Collider>();
            var _rigidBody = GetComponent<Rigidbody>();

            if (_collider) _collider.isTrigger = true;
            if (_rigidBody) _rigidBody.useGravity = false;
        }

        private void DisableWeapon()
        {
            WeaponCollider.enabled = false;
        }

        private void EnableWeapon()
        {
            WeaponCollider.enabled = true;
            StartCoroutine(DisableAttackAfterTime());
        }

        private IEnumerator DisableAttackAfterTime()
        {
            yield return new WaitForSeconds(AttackLength);
            IsAttacking = false;
            WeaponTarget = null;
        }

        private void AddAttackForce(Character target)
        {
            var _direction = target.transform.position - Owner.transform.position;
            _direction /= _direction.magnitude;
            _direction *= AttackForce;

            target.AddExternalForce(_direction);
        }
    }
}
