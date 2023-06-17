using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class GolemMiniBoss : EnemyBase
    {
        [Header("Golem")]
        public float FollowDistance;
        public float TurnSpeed;
        public float ThrowRockInterval;

        [Header("Attack State")]
        public Transform Hand;
        public GolemDamageBall Ball;
        public float UseMelleAttackDistance;

        public UnityEvent OnThrowBall;
        public UnityEvent OnUseMelleAttack;

        [Header("Animation")]
        public string IsStopedAnimParam;
        public string IsAttackingAnimParam;
        public string IsDeathAnimParam;
        public string UseThrowAttackAnimParam;
        public float MelleAttackAnimLength;
        public float ThrowRockAttackAnimLength;

        private float _throwRockInterval;

        // throw rock only if target is distant
        private bool UseMelleAttackAnim
        {
            get
            {
                return Vector3.Distance(transform.position, Target.transform.position) < UseMelleAttackDistance;
            }
        }

        private float CurrentAnimLength
        {
            get
            {
                return !UseMelleAttackAnim ? ThrowRockAttackAnimLength : MelleAttackAnimLength;
            }
        }

        protected override void Start()
        {
            base.Start();
            CurrentState = FollowTargetState;
        }

        protected override void Update()
        {
            base.Update();

            if (Target && Target.IsDeath)
            {
                FindPlayer();
            }

            if (!Target)
            {
                if (CurrentState != IdleState) CurrentState = IdleState;
            }

            UpdateAnimations();

            if (CurrentState != AttackState && !UseMelleAttackAnim)
            {
                _throwRockInterval += Time.deltaTime;
            }

            if (_throwRockInterval > ThrowRockInterval || UseMelleAttackAnim)
            {
                _throwRockInterval = 0f;
                CurrentState = AttackState;
            }

            if (CurrentLife < 0)
            {
                Death();
            }
        }

        private void FollowTargetState()
        {
            var _directionToTarget = (Target.transform.position - transform.position).normalized;
            MoveDirectionVelocity = _directionToTarget * MoveSpeed;
            LookTo(_directionToTarget, TurnSpeed);

            if (Vector3.Distance(transform.position, Target.transform.position) < FollowDistance)
            {
                CurrentState = IdleState;
            }
        }

        private void IdleState()
        {
            MoveDirectionVelocity = Vector3.zero;
            LookTo(Target.transform.position - transform.position, TurnSpeed);

            if (Vector3.Distance(transform.position, Target.transform.position) > FollowDistance || !IsOnScreen)
            {
                CurrentState = FollowTargetState;
            }
        }

        private void AttackState()
        {
            if (Target.IsDeath)
            {
                CurrentState = IdleState;
                IsAttacking = false;
            }

            MoveDirectionVelocity = Vector3.zero;
            IsAttacking = true;
            LookTo(Target.transform.position - transform.position, TurnSpeed);

            if (StateExecutionTime > CurrentAnimLength)
            {
                IsAttacking = false;
                CurrentState = FollowTargetState;
            }
        }

        private void UpdateAnimations()
        {
            Animator.SetBool(IsStopedAnimParam, IsStoped);
            Animator.SetBool(IsAttackingAnimParam, IsAttacking);
            Animator.SetBool(UseThrowAttackAnimParam, !UseMelleAttackAnim);
            Animator.SetBool(IsDeathAnimParam, IsDeath);
        }

        public override void AttackAnimationEvent()
        {
            base.AttackAnimationEvent();

            // throw rock
            if (!UseMelleAttackAnim)
            {
                SpawnBall();
                OnThrowBall.Invoke();
            }
            // normal attack
            else
            {
                Target.AddDamage(AttackDamage);
                OnUseMelleAttack.Invoke();
            }
        }

        private void SpawnBall()
        {
            var _ball = Instantiate(Ball, Hand.position, Quaternion.LookRotation(Target.transform.position - transform.position));
            _ball.Damage = AttackDamage;
            _ball.Target = Target.transform.position;
            _ball.Owner = this;
        }
    }
}