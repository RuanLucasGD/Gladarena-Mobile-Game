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
        public float StopDistance;

        [Header("Prepare Attack State")]
        public float LookToTargetTime;

        [Header("Attack")]
        public float AttackLength;

        [Header("Idle Attack State")]
        public float AttackInterval;

        [Header("Running Attack State")]
        public float StartAttackDistance;

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

        private bool _useSpecialAttack;

        // move random direction state
        private bool _findedRandomPosition;

        // idle attack state
        private bool _idleAttackCooldownUpdated;
        private float _attackExecutionTime;

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
        }

        private void RunningAttackState()
        {
            IsAttacking = true;
            _useSpecialAttack = false;

            var _directionToTarget = (MoveTo - transform.position).normalized;
            MoveDirectionVelocity = _directionToTarget * MoveSpeed;
            LookTo(_directionToTarget, TurnSpeed);

            if (Vector3.Distance(MoveTo, transform.position) < StopDistance)
            {
                MoveDirectionVelocity = Vector3.zero;
            }

            if (StateExecutionTime > AttackLength)
            {
                IsAttacking = false;
                CurrentState = MoveRandomDirectionState;
                return;
            }
        }

        private void IdleAttackState()
        {
            IEnumerator IdleAttackCooldown()
            {
                _idleAttackCooldownUpdated = true;
                yield return new WaitForSeconds(IsAttacking ? AttackLength : AttackInterval);
                _idleAttackCooldownUpdated = false;
                IsAttacking = !IsAttacking;
            }

            _useSpecialAttack = true;

            MoveDirectionVelocity = Vector3.zero;
            LookTo(Target.transform.position - transform.position, TurnSpeed);

            if (!_idleAttackCooldownUpdated)
            {
                StartCoroutine(IdleAttackCooldown());
            }

            if (Vector3.Distance(transform.position, Target.transform.position) > StopDistance)
            {
                CurrentState = MoveRandomDirectionState;
                StopCoroutine(IdleAttackCooldown());
                IsAttacking = false;
                _useSpecialAttack = false;
            }
        }

        private void PrepareToRunState()
        {
            if (!IsOnScreen)
            {
                CurrentState = WalkToPlayerArea;
                return;
            }

            if (StateExecutionTime > LookToTargetTime)
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

            var _moveDirection = (MoveTo - transform.position).normalized;
            MoveDirectionVelocity = _moveDirection * MoveSpeed;
            LookTo(_moveDirection, TurnSpeed);

            if (Vector3.Distance(transform.position, Target.transform.position) < StopDistance)
            {
                CurrentState = IdleAttackState;
                return;
            }

            if (Vector3.Distance(transform.position, MoveTo) < StopDistance)
            {
                _findedRandomPosition = false;
                _useSpecialAttack = false;
                CurrentState = PrepareToRunState;
            }
        }

        private void RunToDestinationState()
        {
            MoveDirectionVelocity = (MoveTo - transform.position).normalized * MoveSpeed;

            if (Vector3.Distance(transform.position, MoveTo) < StartAttackDistance)
            {
                CurrentState = RunningAttackState;
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
                _worldPos = hit.point - ((hit.point - transform.position).normalized * StopDistance);
                Debug.DrawLine(transform.position, _worldPos, Color.yellow, 10);
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
