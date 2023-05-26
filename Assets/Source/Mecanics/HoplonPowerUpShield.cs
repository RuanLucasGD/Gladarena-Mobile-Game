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
        public float LifeTime;

        private EnemyBase _enemy;
        private bool _collisionChecked;

        private float _currentRotation;

        private float _cooldownTime;
        private bool _shieldEnabled;

        private void Start()
        {
            _shieldEnabled = true;
        }

        private void Update()
        {
            _currentRotation += TurnSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, _currentRotation, 0);

            if (_currentRotation > 360)
            {
                _currentRotation -= 360;
            }

            UpdateVisibleTime();
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

        protected virtual void UpdateVisibleTime()
        {
            if (LifeTime == 0 || LifeTime == Mathf.Infinity)
            {
                return;
            }

            _cooldownTime += Time.deltaTime;

            var _time = _shieldEnabled ? LifeTime : Cooldown;

            if (_cooldownTime > _time)
            {
                _cooldownTime = 0;
                _shieldEnabled = !_shieldEnabled;
            }

            var _targetScale = _shieldEnabled ? Vector3.one : Vector3.zero;
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * 5f);
        }
    }
}