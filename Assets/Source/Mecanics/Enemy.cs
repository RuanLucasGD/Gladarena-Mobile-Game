using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class Enemy : EnemyBase
    {
        public EnemyType Type;

        [Header("Basic")]
        public float MoveSpeed;
        public float StopDistance;
        public float MaxLife;
        public float AttackDamage;
        public float AttackInterval;
        public float AttackDistance;

        [Header("Animation")]
        public int WeaponAnimID;
        public EnemyAnimationParameterSettings AnimationSettings;

        public bool SuperAttack { get; set; }

        public new bool IsStoped => Target && !Target.IsDeath ? Vector3.Distance(transform.position, Target.transform.position) < StopDistance : true;
        public bool IsTargetNearToAttack => Target ? Vector3.Distance(transform.position, Target.transform.position) < AttackDistance : false;
        public bool IsAttacking { get; private set; }

        private bool _attackDeleyedStarted;

        protected override void Start()
        {
            base.Start();

            CurrentLife = MaxLife;
            SetWeaponAnimation();
        }

        protected override void Update()
        {
            if (IsDeath)
            {
                return;
            }

            base.Update();

            UpdateRotation();
            UpdateAnimations();

            if (CurrentLife <= 0)
            {
                Death();
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateMovement(Time.fixedDeltaTime);
        }

        private void UpdateMovement(float delta)
        {
            if (!Rb || !Target || Target.IsDeath)
            {
                return;
            }

            if (IsStoped)
            {
                if (!_attackDeleyedStarted)
                {
                    StartCoroutine(StartAttackDeleyed());
                }

                MoveDirectionVelocity = Vector3.zero;
            }
            else
            {
                MoveDirectionVelocity = (Target.transform.position - transform.position).normalized * MoveSpeed;
            }
        }

        private void UpdateRotation()
        {
            if (IsDeath || !IsOnScreen)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(GetDirectionToTarget(Target.transform));
        }

        private Vector3 GetDirectionToTarget(Transform target)
        {
            return Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up).normalized;
        }

        private void UpdateAnimations()
        {
            if (!AnimationSettings)
            {
                return;
            }

            Animator.SetBool(AnimationSettings.IsWalkingParameter, !IsStoped);
            Animator.SetBool(AnimationSettings.IsAttackingParameter, IsAttacking);
        }

        private void SetDeathAnimation()
        {
            if (!AnimationSettings)
            {
                return;
            }

            Animator.SetBool(AnimationSettings.IsDeathParameter, IsDeath);
        }

        private void SetWeaponAnimation()
        {
            if (!AnimationSettings)
            {
                return;
            }

            Animator.SetInteger(AnimationSettings.WeaponIdParameter, WeaponAnimID);
        }

        protected override void Attack()
        {
            if (IsAttacking || !IsTargetNearToAttack || !Target || Target.IsDeath)
            {
                return;
            }

            IsAttacking = true;
            StartCoroutine(FinalizeAttackDeleyed());
        }

        private IEnumerator StartAttackDeleyed()
        {
            _attackDeleyedStarted = true;
            yield return new WaitForSeconds(AttackInterval);

            Attack();
            _attackDeleyedStarted = false;
        }

        private IEnumerator FinalizeAttackDeleyed()
        {
            var _animLenght = SuperAttack ? AnimationSettings.SuperAttackLenght : AnimationSettings.NormalAttackLenght;
            yield return new WaitForSeconds(_animLenght);

            IsAttacking = false;
        }

        // called by character animator event
        public override void AttackAnimationEvent()
        {
            if (!IsAttacking)
            {
                return;
            }

            base.AttackAnimationEvent();
            Target.AddDamage(AttackDamage);
        }

        public override void Death()
        {
            if (IsDeath)
            {
                return;
            }

            base.Death();
            SetDeathAnimation();
        }
    }
}