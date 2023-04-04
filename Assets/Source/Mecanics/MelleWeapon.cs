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

        [Header("Components")]
        public Collider WeaponCollider;

        public override Character Owner
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

        protected override void Update()
        {
            base.Update();

            if (Owner && Owner.CharacterMoveDirection.magnitude > 0)
            {
                DisableAttack();
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

        private void DisableAttack()
        {
            IsAttacking = false;
            WeaponTarget = null;
        }

        private IEnumerator DisableAttackAfterTime()
        {
            yield return new WaitForSeconds(AttackLength);
            DisableAttack();
        }

        private Vector3 GetAttackForce(Character target)
        {
            var _direction = target.transform.position - Owner.transform.position;
            _direction /= _direction.magnitude;
            _direction *= CurrentAttackForce;

            return _direction;
        }

        private List<Character> GetNearInViewEnemies()
        {
            var _nearInView = new List<Character>();
            var _characters = FindObjectsOfType<Character>();

            foreach (var c in _characters)
            {
                // avoid own owner
                if (c == Owner) continue;

                // only near characters
                if (Vector3.Distance(c.transform.position, Owner.transform.position) > AttackRange) continue;

                // only in view characters
                if (Vector3.Dot(Owner.transform.forward, (c.transform.position - Owner.transform.position).normalized) < DotAttackAngle) continue;

                _nearInView.Add(c);
            }

            return _nearInView;
        }

        public override void Attack(Character target = null)
        {
            var _nearInViewEnemies = GetNearInViewEnemies();

            if (Owner.IsStoped && (_nearInViewEnemies.Count > 0 || target))
            {
                base.Attack(target);
            }

            _nearInViewEnemies = null;
        }

        // Called by owener character animation event
        public void ApplyDamageOnEnemies()
        {
            if (!IsAttacking)
            {
                return;
            }

            // attack only the target
            if (WeaponTarget)
            {
                WeaponTarget.AddDamage(CurrentAttackDamage, GetAttackForce(WeaponTarget));
                return;
            }

            // attack all near characters
            var _nearInViewEnemies = GetNearInViewEnemies();
            foreach (var c in _nearInViewEnemies) c.AddDamage(AttackDamageMultiplier, GetAttackForce(c));
            _nearInViewEnemies = null;
        }
    }
}
