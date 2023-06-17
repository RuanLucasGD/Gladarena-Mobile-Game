using System.Collections;
using UnityEngine;

namespace Game.Mecanics
{
    /// <summary>
    /// Centauro miniboss
    /// </summary>
    public class CentaurMiniBoss : EnemyBase
    {
        [Header("Centaur")]
        public float TurnSpeed;
        public float StopDistance;

        [Header("Prepare Attack State")]
        public float LookToTargetTime;

        [Header("Attack")]
        public float AttackLength;
        public float AttackDistance;

        [Header("Idle Attack State")]
        public float AttackInterval;

        [Header("Running Attack State")]
        public float StartAttackDistance;
        public float DelayToStartAttack;

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

        private bool _delayAttackStarted;

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
            // start attack animation even if's movement
            // this is just to make it look the character is scrolling :)

            _useSpecialAttack = false;

            IEnumerator DeleyedAttack()
            {
                yield return new WaitForSeconds(DelayToStartAttack);
                IsAttacking = true;
            }

            // wait sometime to start attack
            if (!_delayAttackStarted)
            {
                _delayAttackStarted = true;
                StartCoroutine(DeleyedAttack());
            }

            // run and look to target
            var _directionToTarget = (MoveTo - transform.position).normalized;
            MoveDirectionVelocity = _directionToTarget * MoveSpeed;
            LookTo(_directionToTarget, TurnSpeed);

            // when is near, stop
            if (Vector3.Distance(MoveTo, transform.position) < StopDistance)
            {
                MoveDirectionVelocity = Vector3.zero;
            }

            // finally attack animation and go to a random position to attack again
            var _attackCompleted = StateExecutionTime > AttackLength + DelayToStartAttack;
            var _isTargetNear = Vector3.Distance(Target.transform.position, transform.position) < AttackDistance;
            if (_attackCompleted && !_isTargetNear)
            {
                _delayAttackStarted = false;
                IsAttacking = false;
                CurrentState = MoveRandomDirectionState;
                return;
            }
        }

        private void IdleAttackState()
        {
            // stay stoped while target is near and attack each X seconds

            // each sometime, attack the target, end on finish animation wait the time to attack again
            IEnumerator IdleAttackCooldown()
            {
                _idleAttackCooldownUpdated = true;
                yield return new WaitForSeconds(IsAttacking ? AttackLength : AttackInterval);
                _idleAttackCooldownUpdated = false;
                IsAttacking = !IsAttacking;
            }

            // use another attack animation on this attack state. Why? idk....
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
            // stay stoped sometime and look to player,
            // on finish, run to designed target position

            if (!IsOnScreen)
            {
                CurrentState = WalkToPlayerState;
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

        private void WalkToPlayerState()
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

        // generate a random position around of this gameObject
        private Vector3 GetRandomPosition()
        {
            var _randomOffset = new Vector3(1, 0, 1);

            _randomOffset /= _randomOffset.magnitude;
            _randomOffset *= MaxDistance;

            var _worldPos = transform.position + _randomOffset;

            // check if has any obstacle on target position. If so, change target position to front of the obstacle
            if (Physics.Linecast(transform.position, _worldPos, out var hit, ObstaclesLayer))
            {
                _worldPos = hit.point - ((hit.point - transform.position).normalized * StopDistance);
            }

            return _worldPos;
        }

        public override void AttackAnimationEvent()
        {
            base.AttackAnimationEvent();

            var _isTargetNear = Vector3.Distance(transform.position, Target.transform.position) < AttackDistance;
            var _isTargetOnFront = Vector3.Dot(transform.forward, (Target.transform.position - transform.position).normalized) > 0.5f;

            if (!IsAttacking || !_isTargetNear || !_isTargetOnFront)
            {
                return;
            }

            Target.AddDamage(AttackDamage);
        }
    }
}
