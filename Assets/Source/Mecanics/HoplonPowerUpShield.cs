using Game.Mecanics;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Mecanics
{
    public class HoplonPowerUpShield : MonoBehaviour
    {
        public float TurnSpeed;
        public float Force;
        public float Cooldown;

        private EnemyBase _enemy;
        private bool _collisionChecked;

        private float currentRotation;

        void Update()
        {
            currentRotation += TurnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            if (currentRotation > 360)
            {
                currentRotation -= 360;
            }
        }

        private void LateUpdate()
        {
            _collisionChecked = false;
            _enemy = null;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            IsEnemy(other, out var enemy);

            if (enemy)
            {
                var _directionToEnemy = other.transform.position - transform.position;
                var _force = _directionToEnemy.normalized * Force;
                enemy.AddExternalForce(_force);
            }
        }

        protected bool IsEnemy(Collider collider, out EnemyBase enemy)
        {
            if (_collisionChecked)
            {
                enemy = _enemy;
                return _enemy;
            }

            _enemy = collider.gameObject.GetComponent<EnemyBase>();
            _collisionChecked = true;
            enemy = _enemy;

            return enemy;
        }
    }
}