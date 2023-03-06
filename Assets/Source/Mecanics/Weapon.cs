using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class Weapon : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationControl
        {
            public Animator Animator;

            [Header("Parameters")]
            public string AttackAnimParameter;
        }

        public float AttackRange;

        [Space]
        public AnimationControl Animation;

        private bool _isAttacking;

        public Character Owner { get; set; }

        public bool IsAttacking
        {
            get
            {
                return _isAttacking;
            }
            set
            {
                _isAttacking = value;
                if (_isAttacking) OnEnableAttack();
                else OnDisableAttack();
            }
        }

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
        }

        protected virtual void Awake()
        {
            OnEnableAttack = () => { };
            OnDisableAttack = () => { };
        }

        protected virtual void Start() { }

        protected virtual void FixedUpdate() { }

        protected virtual void Update()
        {
            UpdateAnimations();
        }

        public virtual void Attack()
        {
            if (IsAttacking)
            {
                return;
            }

            IsAttacking = true;
        }

        public virtual void Attack(Character target)
        {
            Attack();
            WeaponTarget = target;
        }

        protected virtual void UpdateAnimations()
        {
            if (!Animation.Animator)
            {
                return;
            }
            
            Animation.Animator.SetBool(Animation.AttackAnimParameter, IsAttacking);
        }
    }
}
