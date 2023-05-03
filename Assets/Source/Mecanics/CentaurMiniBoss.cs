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
        public float PrepareAttackTime;

        [Header("Animation Parameters")]
        public string IsStopedAnimParam;
        public string IsAttackingAnimParam;
        public string IsPreparingAttackAnimParam;
        public string IsDeathAnimParam;
        public string UseSpecialAttackAnimParam;

        public Vector3 MoveTo { get; private set; }
        private UnityAction CurrentState { get => _currentState; set { _currentState = value; ResetStateTime(); } }

        private bool _isPreparingAttack;
        private bool _useSpecialAttack;
        private float _currentStateTime;

        private UnityAction _currentState;

        // attack state
        private bool _isTargetNear;

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

        private void PrepareAttackState()
        {
            MoveTo = Target.transform.position;
            var _direction = (MoveTo - transform.position).normalized;

            LookTo(_direction, TurnSpeed);
            MoveDirectionVelocity = -_direction * MoveSpeed;

            _useSpecialAttack = true;
            _isPreparingAttack = true;
            if (_currentStateTime > PrepareAttackTime)
            {
                _useSpecialAttack = false;
                _isPreparingAttack = false;

                CurrentState = RunToDestinationState;
            }
        }

        private void AttackState()
        {
            LookTo(Target.transform.position - transform.position, TurnSpeed);
            MoveDirectionVelocity = Vector3.zero;
            IsAttacking = true;

            var _isNear = Vector3.Distance(Target.transform.position, transform.position) < AttackDistance;

            // change to prepare attack state only if the Centaur is closed to target and target move to distant
            if (_isTargetNear && !_isNear)
            {
                CurrentState = PrepareAttackState;
                IsAttacking = false;
            }
            else if (!_isTargetNear && !_isNear)
            {
                CurrentState = PrepareToRunState;
                IsAttacking = false;
            }

            _isTargetNear = _isNear;
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

            var _isFinalTargetNear = Vector3.Distance(transform.position, MoveTo) < StopDistance;
            var _isTargetNear = Vector3.Distance(transform.position, Target.transform.position) < StopDistance;
            if (_isFinalTargetNear || _isTargetNear)
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
            Animator.SetBool(IsPreparingAttackAnimParam, _isPreparingAttack);
            Animator.SetBool(UseSpecialAttackAnimParam, _useSpecialAttack);
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
