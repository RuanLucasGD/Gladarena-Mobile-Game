using UnityEngine;

namespace Game.Mecanics
{
    public class PlayerCloneAI : MonoBehaviour
    {
        public AresArmyPowerUp PowerUpController { get; set; }
        public PlayerCharacter Clone { get; set; }

        public EnemyBase Target { get; private set; }

        private void Start()
        {
            Clone.EnablePlayerControl = false;
        }

        private void Update()
        {
            if (Target == null)
            {

                Target = FindEnemy();

                if (Target == null)
                {
                    return;
                }
            }

            if (Target.IsDeath)
            {
                Target = null;
            }

            ControlPlayerClone();
        }

        private void ControlPlayerClone()
        {
            if (!Target)
            {
                Clone.CharacterMoveDirection = Vector3.zero;
                return;
            }

            var _targetPosition = Target.transform.position;
            var _directionToTarget = (_targetPosition - transform.position).normalized;
            var _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            var _attackDistance = PowerUpController.CloneBehaviour.AttackDistance;

            if (_distanceToTarget < _attackDistance)
            {
                Clone.CharacterMoveDirection = Vector3.zero;
                Clone.LookAtDirection = _directionToTarget;
                return;
            }

            Clone.CharacterMoveDirection = _directionToTarget;
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
    }
}
