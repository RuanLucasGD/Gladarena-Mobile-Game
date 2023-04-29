using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Utils;

namespace Game.Mecanics
{
    public class DryadEnemy : EnemyBase
    {
        public enum State
        {
            Idle,
            Attack,
            RandomWalk,
            WalkToTarget
        }

        [Header("State")]
        public State CurrentState;
        public float ChangeStateTime;

        [Header("Animation")]
        public string IdleAnimParam;
        public string AttackAnimParam;

        [Header("Movimentation")]
        public float WalkSpeed;
        public float TurnSpeed;
        public float MoveRandomDistance;

        [Header("Attack")]
        public float AttackSpeed;
        public float AttackForce;

        private UnityAction _updateState;

        public bool IsAttacking { get; private set; }
        public Vector3 MovePosition { get; private set; }

        public float DistanceToMovePosition => Vector3.Distance(MovePosition, Rb.position);
        public bool IsNearMoveTo => DistanceToMovePosition < StopDistance;

        protected override void Start()
        {
            base.Start();
            SetRandomWalkState();
            InvokeRepeating(nameof(SetNextState), ChangeStateTime, ChangeStateTime);
        }

        protected override void Update()
        {
            base.Update();

            _updateState();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking)
            {
                return;
            }
            if (other.TryGetComponent<PlayerCharacter>(out var player))
            {

                var _attackForce = (player.transform.position - Rb.position).normalized * AttackForce;
                player.AddExternalForces(_attackForce);
                player.AddDamage(AttackDamage);
            }
        }

        private void SetNextState()
        {
            if (CurrentState == State.Idle)
            {
                if (IsOnScreen) SetAttackState();
                else SetWalkToTargetState();
                return;
            }

            if (CurrentState == State.WalkToTarget)
            {
                if (IsOnScreen) SetRandomWalkState();
                return;
            }

            if (CurrentState == State.RandomWalk)
            {
                if (IsOnScreen) SetIdleState();
                else SetWalkToTargetState();
                return;
            }

            if (CurrentState == State.Attack)
            {
                // wait to finalize attack
                return;
            }
        }

        private void IdleState()
        {
            MoveDirectionVelocity = Vector3.zero;
            SetAnimParam(IdleAnimParam, true);
            SetAnimParam(AttackAnimParam, false);
        }

        private void AttackState()
        {
            SetAnimParam(IdleAnimParam, false);
            SetAnimParam(AttackAnimParam, true);

            if (!IsAttacking)
            {
                var _directionToTarget = (Target.transform.position - Rb.position).normalized * 10;
                MovePosition = Target.transform.position + _directionToTarget;
                IsAttacking = true;
                return;
            }

            if (IsNearMoveTo && IsAttacking)
            {
                IsAttacking = false;
                if (IsOnScreen) SetRandomWalkState();
                else SetWalkToTargetState();
            }

            if (IsAttacking && !IsNearMoveTo)
            {
                MoveDirectionVelocity = (MovePosition - transform.position).normalized * AttackSpeed;
                LookTo(MovePosition - transform.position, TurnSpeed);
            }
        }

        private void RandomWalkState()
        {
            SetAnimParam(IdleAnimParam, false);
            SetAnimParam(AttackAnimParam, false);

            var _isStartPosition = MovePosition == Vector3.zero;
            var _isTargetOnScreen = CameraUtils.IsPointOnView(MovePosition, Camera.main);

            if (_isStartPosition || IsNearMoveTo || !_isTargetOnScreen)
            {
                var _randomOffset = new Vector3(Random.Range(-MoveRandomDistance, MoveRandomDistance),
                                                0,
                                                Random.Range(-MoveRandomDistance, MoveRandomDistance));

                MovePosition = Rb.position + _randomOffset;
            }

            if (!IsNearMoveTo && !_isStartPosition)
            {
                MoveDirectionVelocity = (MovePosition - transform.position).normalized * WalkSpeed;
                LookTo(MovePosition - transform.position, TurnSpeed);
            }
        }

        private void WalkToTargetState()
        {
            SetAnimParam(IdleAnimParam, false);
            SetAnimParam(AttackAnimParam, false);

            MoveDirectionVelocity = (MovePosition - transform.position).normalized * WalkSpeed;
            LookTo(Target.transform.position - Rb.position, TurnSpeed);
        }

        private void SetAnimParam(string name, object value)
        {
            return;
            if (value is bool) Animator.SetBool(name, (bool)value);
            if (value is int) Animator.SetInteger(name, (int)value);
            if (value is float) Animator.SetFloat(name, (float)value);
        }

        private void SetIdleState()
        {
            CurrentState = State.Idle;
            IdleState();
        }

        private void SetRandomWalkState()
        {
            CurrentState = State.RandomWalk;
            _updateState = RandomWalkState;
        }

        private void SetWalkToTargetState()
        {
            CurrentState = State.WalkToTarget;
            _updateState = WalkToTargetState;
        }

        private void SetAttackState()
        {
            CurrentState = State.Attack;
            _updateState = AttackState;
        }
    }
}

