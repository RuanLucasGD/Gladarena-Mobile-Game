using System.Runtime.Serialization.Json;
using UnityEngine;

namespace Game.Mecanics
{
    public class PlayerCloneAI : MonoBehaviour
    {
        public bool FollowPlayer;
        public Vector3 FollowPlayerOffset;

        public EnemyBase Target { get; private set; }
        public AresArmyPowerUp PowerUpController { get; set; }
        public PlayerCharacter Clone { get; set; }

        public bool UseDashMode { get; set; }

        public bool IsStoped { get; private set; }
        public bool DashModeEnabled { get; private set; }

        private PlayerCharacter _originalPlayer;

        private void Start()
        {
            Clone.EnablePlayerControl = false;
            IsStoped = false;
            _originalPlayer = GameManager.Instance.Player;

            Clone.OnDeath.AddListener(EnableDashMode);
        }

        private void Update()
        {
            if (Target == null || Target.IsDeath)
            {

                Target = FindEnemy();

                if (Target == null)
                {
                    return;
                }
            }

            if (FollowPlayer && !DashModeEnabled) FollowPlayerPosition();
            else if (DashModeEnabled) DashMode();
            else WalkToRandomEnemy();
        }

        private void WalkToRandomEnemy()
        {
            if (!Target)
            {
                Clone.CharacterMoveDirection = Vector3.zero;
                return;
            }

            var _targetPosition = Target.transform.position;
            var _directionToTarget = (_targetPosition - transform.position).normalized;
            var _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            var _attackDistance = Clone.CurrentAttackDistance;
            var _stopDistance = Clone.Movimentation.StopDistance;

            if (IsStoped && _distanceToTarget > _attackDistance) IsStoped = false;
            if (!IsStoped && _distanceToTarget <= _stopDistance) IsStoped = true;

            if (IsStoped)
            {
                Clone.CharacterMoveDirection = Vector3.zero;
                Clone.LookAtDirection = _directionToTarget;
                return;
            }

            Clone.CharacterMoveDirection = _directionToTarget;
        }

        private void FollowPlayerPosition()
        {
            var _playerPosition = _originalPlayer.transform.position;
            var _movePosition = _playerPosition + FollowPlayerOffset;
            var _direction = (_movePosition - transform.position).normalized;

            if (Vector3.Distance(_movePosition, transform.position) < 0.3f)
            {
                _direction = Vector3.zero;
            }

            Clone.LookAtDirection = transform.position - _playerPosition;
            Clone.CharacterMoveDirection = _direction;
        }

        private EnemyBase FindEnemy()
        {
            var _enemies = FindObjectsOfType<EnemyBase>();

            for (int i = 0; i < _enemies.Length; i++)
            {
                var _enemy = _enemies[i];

                if (_enemy.IsDeath)
                {
                    continue;
                }

                var _enemyDistance = Vector3.Distance(transform.position, _enemy.transform.position);
                var _enemyIsNear = _enemyDistance < PowerUpController.CloneBehaviour.FindEnemiesDistance;

                if (_enemyIsNear)
                {
                    return _enemy;
                }
            }

            //if not have any near enemy follow any enemy (if exist...)
            return FindObjectOfType<EnemyBase>();
        } 

        private void EnableDashMode()
        {
            if (DashModeEnabled || !UseDashMode)
            {
                return;
            }

            DashModeEnabled = true;
            Clone.IsInvencible = true;
            Clone.ResetLife();

            Invoke(nameof(FinalizeDashMode), PowerUpController.CloneBehaviour.DashTime);
        }

        private void FinalizeDashMode()
        {
            Clone.IsInvencible = false;
            Clone.KillCharacter();
        }

        private void DashMode()
        {
            var _direction = Clone.transform.forward;
            Clone.CharacterMoveDirection = _direction;
            Clone.LookAtDirection = _direction;
        }
    }
}
