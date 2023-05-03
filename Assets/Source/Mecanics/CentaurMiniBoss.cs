using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class CentaurMiniBoss : EnemyBase
    {
        [Header("Centaur")]
        public float TurnSpeed;
        public float AttackDistance;

        [Header("States")]
        public float LookToTargetTime;
        public float AttackTime;

        [Header("Animation Parameters")]
        public string IsStopedAnimParam;
        public string IsAttackingAnimParam;
        public string IsDeathAnimParam;

        public Vector3 MoveTo { get; private set; }

        private float _currentStateTime;
        private UnityAction _currentState;
        private UnityAction CurrentState { get => _currentState; set { _currentState = value; ResetStateTime(); } }

        protected override void Start()
        {
            base.Start();
            CurrentState = PrepareToRunState;
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimations();

            if (CurrentLife <= 0)
            {
                Death();
                return;
            }

            CurrentState();
            _currentStateTime += Time.deltaTime;
        }

        private void AttackState()
        {
            LookTo(Target.transform.position - transform.position, TurnSpeed);
            MoveDirectionVelocity = Vector3.zero;
            IsAttacking = true;

            var _isTargetNear = Vector3.Distance(Target.transform.position, transform.position) < AttackDistance;
            if (_currentStateTime > AttackTime && !_isTargetNear)
            {
                CurrentState = PrepareToRunState;
                IsAttacking = false;
            }
        }

        private void PrepareToRunState()
        {
            if (!IsOnScreen)
            {
                CurrentState = WalkToPlayerArea;
                return;
            }

            if (_currentStateTime > LookToTargetTime)
            {
                CurrentState = RunToDestinationState;
                return;
            }

            MoveDirectionVelocity = Vector3.zero;
            MoveTo = Target.transform.position;
            LookTo(MoveTo - transform.position, TurnSpeed);
        }

        private void RunToDestinationState()
        {
            MoveDirectionVelocity = (MoveTo - transform.position).normalized * MoveSpeed;

            if (Vector3.Distance(transform.position, MoveTo) < StopDistance)
            {
                _currentState = AttackState;
            }
        }

        private void WalkToPlayerArea()
        {
            var _directionToTarget = (Target.transform.position - transform.position).normalized;

            MoveDirectionVelocity = _directionToTarget * MoveSpeed;

            if (IsOnScreen)
            {
                CurrentState = PrepareToRunState;
                return;
            }
        }

        private void ResetStateTime()
        {
            _currentStateTime = 0f;
        }

        private void UpdateAnimations()
        {
            Animator.SetBool(IsStopedAnimParam, IsStoped);
            Animator.SetBool(IsAttackingAnimParam, IsAttacking);
            Animator.SetBool(IsDeathAnimParam, IsDeath);
        }

        public override void AttackAnimationEvent()
        {
            base.AttackAnimationEvent();

            if (!IsAttacking)
            {
                return;
            }

            Target.AddDamage(AttackDamage);
        }
    }
}
