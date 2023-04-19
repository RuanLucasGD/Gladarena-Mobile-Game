using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        protected bool DebugLog;

        public float AttackRange;
        public float AttackDamage;
        public float AttackLength;

        [Header("Animation")]
        public int AnimationID;

        [Header("Sound")]
        public AudioSource AttackAudioSource;

        private bool _isAttacking;

        public virtual PlayerCharacter Owner { get; set; }

        public bool IsAttacking { get; set; }

        /// <summary>
        /// Taget to weapon Damage
        /// </summary>
        /// <value></value>
        protected Enemy WeaponTarget { get; set; }

        public bool IsSuperAttack { get; set; }

        public float CurrentAttackDamage => AttackDamage * Owner.Weapon.AttackDamageMultiplier;
        public float CurrentAttackLength => AttackLength * Owner.Weapon.AttackLengthMultiplier;

        public Weapon()
        {
            AttackRange = 3;
            AttackDamage = 40;
            AttackLength = 1;
        }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void FixedUpdate() { }

        protected virtual void Update()
        {
        }

        public virtual void Attack(Enemy target = null)
        {
            if (IsAttacking)
            {
                return;
            }

            IsAttacking = true;
            WeaponTarget = target;

            if (AttackAudioSource)
            {
                AttackAudioSource.Play();
            }
        }
    }
}
