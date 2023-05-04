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
        public float StartAttackDistance;

        [Header("States")]
        public float LookToTargetTime;
        public float AttackStopedTime;
        public float PrepareAttackTime;

        [Header("Move Random Direction State")]
        public float MaxDistance;
        public LayerMask ObstaclesLayer;

        [Header("Animation Parameters")]
        public string IsStopedAnimParam;
        public string IsAttackingAnimParam;
        public string IsPreparingAttackAnimParam;
        public string IsDeathAnimParam;
        public string UseSpecialAttackAnimParam;

        public Vector3 MoveTo { get; private set; }
        private UnityAction CurrentState { get => _currentState; set { _currentState = value; ResetStateTime(); } }

        private bool _useSpecialAttack;

        private float _currentStateTime;

        private UnityAction _currentState;

        // attack state
        private float _attackStopedTimer;

        // move random direction state
        private bool _onRandomMoviment;
        private bool _findedRandomPosition;

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
            var _directionToTarget = (MoveTo - transform.position).normalized;
            IsAttacking = true;

            if (_onRandomMoviment)
            {
                IsAttacking = true;
                MoveDirectionVelocity = Vector3.zero;
                LookTo(Target.transform.position - transform.position, TurnSpeed);

                if (Vector3.Distance(Target.transform.position, transform.position) > StopDistance)
                {
                    IsAttacking = false;
                    CurrentState = MoveRandomDirectionState;
                }

                return;
            }

            // stop only if the target is very near
            // the enemy can play attack animation in moviment
            if (Vector3.Distance(MoveTo, transform.position) < StopDistance)
            {
                MoveDirectionVelocity = Vector3.zero;
                _attackStopedTimer += Time.deltaTime;

                if (_attackStopedTimer > AttackStopedTime)
                {
                    _attackStopedTimer = 0f;
                    CurrentState = MoveRandomDirectionState;
                    IsAttacking = false;
                }
            }
            else
            {
                LookTo(_directionToTarget, TurnSpeed);
                MoveDirectionVelocity = _directionToTarget * MoveSpeed;
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

        private void MoveRandomDirectionState()
        {
            if (!_findedRandomPosition)
            {
                _findedRandomPosition = true;
                MoveTo = GetRandomPosition();
            }

            _onRandomMoviment = true;
            _useSpecialAttack = true;

            var _moveDirection = (MoveTo - transform.position).normalized;
            MoveDirectionVelocity = _moveDirection * MoveSpeed;
            LookTo(_moveDirection, TurnSpeed);

            if (Vector3.Distance(transform.position, Target.transform.position) < StopDistance)
            {
                CurrentState = AttackState;
                return;
            }

            if (Vector3.Distance(transform.position, MoveTo) < StopDistance)
            {
                _findedRandomPosition = false;
                _onRandomMoviment = false;
                _useSpecialAttack = false;
                CurrentState = PrepareToRunState;
            }
        }

        private void RunToDestinationState()
        {
            MoveDirectionVelocity = (MoveTo - transform.position).normalized * MoveSpeed;

            if (Vector3.Distance(transform.position, MoveTo) < StartAttackDistance)
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
            Animator.SetBool(UseSpecialAttackAnimParam, _useSpecialAttack);
        }

        private Vector3 GetRandomPosition()
        {
            var _randomOffset = new Vector3(1, 0, 1);

            _randomOffset /= _randomOffset.magnitude;
            _randomOffset *= MaxDistance;

            var _worldPos = transform.position + _randomOffset;

            if (Physics.Linecast(transform.position, _worldPos, out var hit, ObstaclesLayer))
            {
                var _directionToTarget = hit.point - transform.position;
                _worldPos -= _directionToTarget.normalized * StopDistance;
            }

            return _worldPos;
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
