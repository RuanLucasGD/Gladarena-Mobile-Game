using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class MelleWeapon : Weapon
    {
        [Header("Melle Attack")]
        [Range(0.1f, 1)]
        public float DotAttackAngle;

        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                if (base.Owner)
                {
                    base.Owner.OnAttackAnimationEvent.RemoveListener(ApplyDamageOnEnemies);
                }

                if (value)
                {
                    value.OnAttackAnimationEvent.AddListener(ApplyDamageOnEnemies);
                }

                base.Owner = value;
            }
        }

        public MelleWeapon()
        {
            DotAttackAngle = 0.5f;
        }

        protected override void Awake()
        {
            base.Awake();

            SetupWeapon();
            IsAttacking = false;
        }

        private void SetupWeapon()
        {
            var _collider = GetComponent<Collider>();
            var _rigidBody = GetComponent<Rigidbody>();

            if (_collider) _collider.isTrigger = true;
            if (_rigidBody) _rigidBody.useGravity = false;
        }

        private void DisableAttack()
        {
            IsAttacking = false;
            WeaponTarget = null;
        }

        private IEnumerator DisableAttackAfterTime()
        {
            yield return new WaitForSeconds(CurrentAttackLength);
            DisableAttack();
        }

        private List<EnemyBase> GetNearInViewEnemies()
        {
            var _nearInView = new List<EnemyBase>();
            var _characters = FindObjectsOfType<EnemyBase>();

            foreach (var c in _characters)
            {
                // avoid own owner
                if (c == Owner) continue;

                // only near characters
                if (Vector3.Distance(c.transform.position, Owner.transform.position) > Owner.CurrentAttackDistance) continue;

                // only in view characters
                if (Vector3.Dot(Owner.transform.forward, (c.transform.position - Owner.transform.position).normalized) < DotAttackAngle) continue;

                _nearInView.Add(c);
            }

            return _nearInView;
        }

        public override void Attack(EnemyBase target = null)
        {
            if (IsAttacking)
            {
                return;
            }

            var _nearInViewEnemies = GetNearInViewEnemies();

            if (_nearInViewEnemies.Count > 0)
            {
                base.Attack(target);
            }

            _nearInViewEnemies = null;
        }

        // Called by owener character animation event
        public void ApplyDamageOnEnemies()
        {
            foreach (var c in GetNearInViewEnemies())
            {
                c.AddDamage(CurrentAttackDamage);
                if (DebugLog) Debug.Log($"Target damaged: {c.name}        damage: {CurrentAttackDamage}");
            }
        }
    }
}
