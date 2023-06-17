using UnityEngine;
using Game.Utils;

namespace Game.Mecanics
{
    public class KnightBoss : EnemyBase
    {
        [Header("Basic")]
        public float StopDistance;

        [Header("Attack")]
        public int AttacksAmount;
        public float AttackForce;
        public float AttackStopDistance;

        [Header("Walk Random")]
        public float MoveRandomDistance;
        public float TurnSpeed;
        public LayerMask ObstaclesLayer;

        [Header("States Control")]
        public float WalkTime;
        public float PrepareAttackTime;
        public float AttackTime;

        [Header("Animation")]
        public string IsDeathAnimParam;
        public string IsWalkingAnimParam;
        public string IsAttackinggAnimParam;
        public string IsPreparingAttackAnimParam;

        private int _currentAttacksAmount;
        private float _currentAttackProgression;

        private Vector3 _moveTo;
        private Vector3 _startAttackPos;

        public new bool IsOnScreen => CameraUtils.IsPointOnView(transform.position, Camera.main);

        public bool IsPreparingAttack { get; private set; }

        protected override void Start()
        {
            base.Start();
            CurrentState = WalkRantomState;
            _moveTo = GetRandomPosition();

            // pegue apenas o player original
            Target = GameManager.Instance.Player;
        }

        protected override void Update()
        {
            if (CurrentLife <= 0)
            {
                CurrentState = IdleState;
                CurrentState();
                Death();
            }

            base.Update();

            UpdateAnimations();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking)
            {
                return;
            }

            if (other.TryGetComponent<PlayerCharacter>(out var player))
            {
                player.AddExternalForces((player.transform.position - Rb.position).normalized * AttackForce);
                player.AddDamage(AttackDamage);
            }
        }

        private void IdleState()
        {
            MoveDirectionVelocity = Vector3.zero;
        }

        private void FollowTargetState()
        {
            MoveDirectionVelocity = (Target.transform.position - Rb.position).normalized * MoveSpeed;
            LookTo(MoveDirectionVelocity, TurnSpeed);

            if (IsOnScreen)
            {
                CurrentState = PrepareAttackState;
            }
        }

        private void WalkRantomState()
        {
            if (!IsOnScreen)
            {
                CurrentState = FollowTargetState;
                return;
            }

            var _distanceToTarget = Vector3.Distance(Rb.position, _moveTo);
            var _generateNewRandomPos = _distanceToTarget < StopDistance || MoveDirectionVelocity == Vector3.zero;

            if (_generateNewRandomPos)
            {
                _moveTo = GetRandomPosition();

                if (Physics.Linecast(Rb.position, _moveTo, out var hit, ObstaclesLayer))
                {
                    _moveTo = hit.point;
                }
            }

            MoveDirectionVelocity = (_moveTo - Rb.position).normalized * MoveSpeed;

            if (StateExecutionTime >= WalkTime)
            {
                CurrentState = PrepareAttackState;
            }

            LookTo(MoveDirectionVelocity, TurnSpeed);
        }

        private void PrepareAttackState()
        {
            if (!IsOnScreen)
            {
                CurrentState = FollowTargetState;
                IsPreparingAttack = false;
                return;
            }

            IsPreparingAttack = true;
            MoveDirectionVelocity = Vector3.zero;

            if (StateExecutionTime >= PrepareAttackTime)
            {
                IsPreparingAttack = false;
                CurrentState = AttackState;
                return;
            }

            _moveTo = Target.transform.position;
            LookTo(_moveTo - Rb.position, TurnSpeed);
        }

        private void AttackState()
        {
            // start attack
            if (!IsAttacking)
            {
                IsAttacking = true;
                var _directionToTarget = (Target.transform.position - Rb.position).normalized;
                var _stopOffset = _directionToTarget * AttackStopDistance;

                _startAttackPos = Rb.position;
                _moveTo = Target.transform.position + _stopOffset;
                _currentAttackProgression = 0f;

                if (Physics.Linecast(Rb.position, _moveTo, out var hit, ObstaclesLayer))
                {
                    _moveTo = hit.point;

                    // avoit to take the obstacle on finish attack
                    _moveTo -= _directionToTarget * StopDistance;
                }
            }

            _currentAttackProgression += Time.deltaTime * AttackTime;
            _currentAttackProgression = Mathf.Min(_currentAttackProgression, 1);
            Rb.MovePosition(Vector3.Lerp(_startAttackPos, _moveTo, _currentAttackProgression));

            // finish current attack
            if (StateExecutionTime >= AttackTime)
            {
                IsAttacking = false;
                _currentAttackProgression = 0f;

                _currentAttacksAmount++;

                // remake attack
                if (_currentAttacksAmount < AttacksAmount)
                {
                    CurrentState = PrepareAttackState;
                }
                // start walk
                else
                {
                    _currentAttacksAmount = 0;
                    CurrentState = WalkRantomState;
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            var _randomOffset = new Vector3(Random.Range(-MoveRandomDistance, MoveRandomDistance),
                                            0,
                                            Random.Range(-MoveRandomDistance, MoveRandomDistance));

            return Rb.position + _randomOffset;
        }

        private void UpdateAnimations()
        {
            Animator.SetBool(IsDeathAnimParam, IsDeath);
            Animator.SetBool(IsWalkingAnimParam, !IsStoped);
            Animator.SetBool(IsPreparingAttackAnimParam, IsPreparingAttack);
            Animator.SetBool(IsAttackinggAnimParam, IsAttacking);
        }
    }
}