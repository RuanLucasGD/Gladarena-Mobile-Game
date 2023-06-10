using System.Collections;
using UnityEngine;

namespace Game.Mecanics
{
    public class PlayerCloneAI : MonoBehaviour
    {
        public bool ExplodeOnDeath { get; set; }
        public bool FollowPlayer { get; set; }
        public Vector3 FollowPlayerOffset { get; set; }

        public EnemyBase Target { get; private set; }
        public AresArmyPowerUp PowerUpController { get; set; }
        public PlayerCharacter Clone { get; set; }

        public bool UseDashModeOnDeath { get; set; }
        public float DashTimeOnDeath { get; set; }

        public bool IsStoped { get; private set; }
        public bool DashModeEnabled { get; private set; }

        private PlayerCharacter _originalPlayer;
        private EnemyBase[] _enemies;

        protected virtual void Start()
        {
            Clone.EnablePlayerControl = false;
            IsStoped = false;
            _originalPlayer = GameManager.Instance.Player;

            if (UseDashModeOnDeath) Clone.OnDeath.AddListener(StartDashWithDeath);
            if (ExplodeOnDeath) Clone.OnDeath.AddListener(AddExplosionOnDeath);
        }

        protected virtual void Update()
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

            Clone.LookAtDirection = _directionToTarget;
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

        protected EnemyBase FindEnemy()
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

        protected void EnableDashMode()
        {
            if (DashModeEnabled)
            {
                return;
            }

            _enemies = FindObjectsOfType<EnemyBase>();
            DashModeEnabled = true;
            Clone.IsInvencible = true;
            Clone.ResetLife();
        }

        protected void DisableDashMode()
        {
            DashModeEnabled = false;
            Clone.IsInvencible = false;
        }

        protected void DashMode()
        {
            // move with dash
            var _direction = Clone.transform.forward;
            Clone.CharacterMoveDirection = _direction;
            Clone.LookAtDirection = _direction;

            //apply damage on enemies in front
            for (int i = 0; i < _enemies.Length; i++)
            {
                var _enemy = _enemies[i];
                var _dashAttackDamage = Clone.Weapon.WeaponObject.AttackDamage;
                var _dashAttackDistance = Clone.CurrentAttackDistance;
                var _dashAttackDotAngle = Clone.Weapon.WeaponObject.DotAttackAngle;

                if (_enemy.IsDeath) continue;

                // is enemy on back of the clone
                if (Vector3.Dot((_enemy.transform.position - transform.position).normalized, transform.forward) < _dashAttackDotAngle) continue;

                // is enemy distant
                if (Vector3.Distance(transform.position, _enemy.transform.position) > _dashAttackDistance) continue;

                _enemy.AddDamage(_dashAttackDamage);
            }
        }

        protected void IdleMode()
        {
            Clone.CharacterMoveDirection = Vector3.zero;
        }

        private void StartDashWithDeath()
        {
            EnableDashMode();
            Invoke(nameof(FinalizeDashWithDeath), DashTimeOnDeath);
        }

        private void FinalizeDashWithDeath()
        {
            DisableDashMode();
            Clone.OnDeath.RemoveListener(StartDashWithDeath);
            Clone.KillCharacter();
        }

        private void AddExplosionOnDeath()
        {
            if (!DashModeEnabled)
            {
                PowerUpController.AddExplosion(transform.position);
            }
        }
    }
}
