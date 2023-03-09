using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class SlegehammerWeapon : Weapon
    {
        public float ApplyAttackDelay;
        public float DeactiveAttackDelay;

        [Space]
        public Transform ApplyDamagePoint;
        public float AttackForceUp;

        protected override void Start()
        {
            base.Start();

            OnEnableAttack += () => StartCoroutine(Delay(DeactiveAttackDelay, DeactiveAttack));
        }

        protected override void Update()
        {
            base.Update();
            if (!Owner.IsStoped)
            {
                IsAttacking = false;
            }
        }

        // Called on Animation ovent of this weapon
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
                _forceDirection *= AttackForce;
                _forceDirection += Vector3.up * AttackForceUp;

                character.AddDamage(Damage);
                character.AddExternalForce(_forceDirection);
            }
        }

        private Character[] GetNearCharacters()
        {
            var _allCharacters = FindObjectsOfType<Character>();

            var _nearCharacters = new List<Character>();

            // remove all distant characters 
            for (int i = 0; i < _allCharacters.Length; i++)
            {
                var _weaponPosition = ApplyDamagePoint.position;
                var _enemyPosition = _allCharacters[i].transform.position;
                var _enemyDistance = Vector3.Distance(_weaponPosition, _enemyPosition);

                if (_enemyDistance < AttackRange && _allCharacters[i] != Owner)
                {
                    _nearCharacters.Add(_allCharacters[i]);
                }
            }

            return _nearCharacters.ToArray();
        }

        private void DeactiveAttack()
        {
            IsAttacking = false;
        }

        private IEnumerator Delay(float delay, UnityAction onCompleted, UnityAction onProgress = null)
        {
            if (onProgress != null)
            {
                onProgress();
            }

            yield return new WaitForSeconds(delay);
            onCompleted();
        }

        public override void Attack(Character target = null)
        {
            if (Owner.IsStoped)
            {
                base.Attack(target);
            }
        }
    }
}