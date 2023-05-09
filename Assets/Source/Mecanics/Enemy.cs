using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class Enemy : EnemyBase
    {
        [Header("Basic")]
        public float AttackInterval;
        public float AttackDistance;
        public float StopDistance;
        public bool SuperAttack;
        [Header("Animator Parameters")]
        public string IsWalkingParameter;
        public string IsAttackingParameter;
        public string IsSuperAttack;
        public string IsDeathParameter;
        public string WeaponIdParameter;

        [Header("Anim Settings")]
        public int WeaponID;
        public float NormalAttackLenght;
        public float SuperAttackLenght;

        public new bool IsStoped => Target && !Target.IsDeath ? Vector3.Distance(transform.position, Target.transform.position) < StopDistance : true;
        public bool IsTargetNearToAttack => Target ? Vector3.Distance(transform.position, Target.transform.position) < AttackDistance : false;

        private bool _attackDeleyedStarted;

        protected override void Start()
        {
            base.Start();
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

            if (!IsStoped && IsAttacking)
            {
                FinalizeAttack();
            }

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
            Animator.SetBool(IsWalkingParameter, !IsStoped);
            Animator.SetBool(IsAttackingParameter, IsAttacking);
            Animator.SetBool(IsSuperAttack, SuperAttack);
        }

        private void SetDeathAnimation()
        {
            Animator.SetBool(IsDeathParameter, IsDeath);
        }

        private void SetWeaponAnimation()
        {
            Animator.SetInteger(WeaponIdParameter, WeaponID);
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

        private void FinalizeAttack()
        {
            IsAttacking = false;
        }

        private IEnumerator FinalizeAttackDeleyed()
        {
            var _animLenght = SuperAttack ? SuperAttackLenght : NormalAttackLenght;
            yield return new WaitForSeconds(_animLenght);

            if (IsAttacking)
            {
                FinalizeAttack();
            }
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