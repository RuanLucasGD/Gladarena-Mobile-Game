using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class SpearWeapon : Weapon
    {
        public Transform AnimAttackBoby;
        public AnimationCurve AttackAnimationCurve;
        public Collider WeaponCollider;

        [Space]
        public float AttackSpeed;
        public float AttackForce;

        [Tooltip("Model size (z axis) of the weapon.")]
        public float SpearSize;

        private float _currentAttackAnimTime;
        private Character _currentTarget;

        public SpearWeapon()
        {
            AttackSpeed = 5;
            AttackForce = 15;
            SpearSize = 5;
        }

        protected override void Awake()
        {
            base.Awake();

            OnEnableAttack += EnableWeaponCollider;
            OnDisableAttack += DisableWeaponCollider;
            OnDisableAttack += ResetWeaponTarget;
        }

        protected override void Update()
        {
            base.Update();
            UpdateAttackAnimation();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking || !_currentTarget)
            {
                return;
            }

            // apply attack on detect target
            if (other.gameObject.TryGetComponent<Character>(out var character))
            {
                if (character == _currentTarget)
                {
                    _currentTarget.AddDamage(Damage);
                    AddAttackForce(_currentTarget);
                    IsAttacking = false;
                }
            }
        }

        private void UpdateAttackAnimation()
        {
            var _animIntensity = AttackAnimationCurve.Evaluate(_currentAttackAnimTime);
            var _attacking = IsAttacking && _currentTarget;

            // the animation needs to be completed even if not attaking
            var _updateAnim = _currentAttackAnimTime > 0 || _attacking;

            if (_updateAnim)
            {
                _currentAttackAnimTime += Time.deltaTime * AttackSpeed;

                if (_currentAttackAnimTime > 1)
                {
                    // restart animation and finish attack
                    ResetWeaponTarget();
                    IsAttacking = false;
                    _currentAttackAnimTime = 0f;
                }
            }

            // adjust the attack lenght to get distant targets
            if (_currentTarget)
            {
                var _targetDistance = Vector3.Distance(Owner.transform.position, _currentTarget.transform.position);
                _targetDistance = Mathf.Max(_targetDistance - SpearSize, 1);
                _animIntensity *= _targetDistance;
            }

            // update spear position only when necessary
            var _currenPosition = AnimAttackBoby.localPosition;
            if (_currenPosition.z != _currentAttackAnimTime)
            {
                AnimAttackBoby.localPosition = Vector3.forward * _animIntensity;
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

        private void ResetWeaponTarget()
        {
            _currentTarget = null;
        }

        private void AddAttackForce(Character target)
        {
            var _direction = (GetCharacterCenter(target) - AnimAttackBoby.position);

            _direction /= _direction.magnitude;
            _direction *= AttackForce;

            target.AddExternalForce(_direction);
        }

        public override void Attack(Character target = null)
        {
            if (IsAttacking)
            {
                return;
            }

            _currentTarget = target ? target : FindNearEnemy();

            if (_currentTarget)
            {
                base.Attack(target);
            }
        }
    }
}