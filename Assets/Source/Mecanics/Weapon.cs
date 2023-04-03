using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class Weapon : MonoBehaviour
    {
        public float AttackForce;
        public float AttackRange;
        public float Damage;
        public float AttackLength;

        [Header("Animation")]
        public int AnimationID;

        private bool _isAttacking;

        public virtual Character Owner { get; set; }

        public bool IsAttacking
        {
            get
            {
                return _isAttacking;
            }
            set
            {
                if (_isAttacking == value)
                {
                    return;
                }

                _isAttacking = value;
                if (_isAttacking) OnEnableAttack();
                else OnDisableAttack();
            }
        }

        private float _attackLengthMultiplier;

        public float AttackLengthMultiplier { get => _attackLengthMultiplier; set => _attackLengthMultiplier = Mathf.Max(value, 0.1f); }

        public float FinalAttackLength => AttackLength * AttackLengthMultiplier;

        /// <summary>
        /// Pass custom actions to weapon when enable
        /// </summary>
        protected UnityAction OnEnableAttack;

        /// <summary>
        /// Pass custom actions to weapon when disable
        /// </summary>
        protected UnityAction OnDisableAttack;

        /// <summary>
        /// Taget to weapon Damage
        /// </summary>
        /// <value></value>
        protected Character WeaponTarget { get; set; }

        public Weapon()
        {
            AttackRange = 3;
            Damage = 40;
            AttackLength = 1;
        }

        protected virtual void Awake()
        {
            AttackLengthMultiplier = 1;

            OnEnableAttack = () => { };
            OnDisableAttack = () => { };
        }

        protected virtual void Start() { }

        protected virtual void FixedUpdate() { }

        protected virtual void Update()
        {
        }

        public virtual void Attack(Character target = null)
        {
            if (IsAttacking)
            {
                return;
            }

            IsAttacking = true;
            WeaponTarget = target;
        }
    }
}
