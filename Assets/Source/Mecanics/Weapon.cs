using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class Weapon : MonoBehaviour
    {
        private PlayerCharacter _owner;

        [SerializeField]
        protected bool DebugLog;

        public float AttackRange;
        public float AttackDamage;
        public float AttackLength;
        [Range(0.1f, 1)]
        public float DotAttackAngle;

        [Header("Mesh")]
        public GameObject Mesh;

        [Header("Animation")]
        public int AnimationID;

        [Header("Events")]
        public UnityEvent<PlayerCharacter> OnSetOwner;

        private bool _isAttacking;

        public virtual PlayerCharacter Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                OnSetOwner.Invoke(_owner);
            }
        }

        public bool IsAttacking { get; set; }

        /// <summary>
        /// Taget to weapon Damage
        /// </summary>
        /// <value></value>
        protected EnemyBase WeaponTarget { get; set; }

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

        public virtual void Attack(EnemyBase target = null)
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
