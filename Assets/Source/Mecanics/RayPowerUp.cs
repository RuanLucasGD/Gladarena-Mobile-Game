using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

namespace Game.Mecanics
{
    public class RayPowerUp : PowerUp
    {
        public GameObject RayPrefab;
        [Min(0.001f)] public float RayObjectLifeTime;

        [Space]
        [Min(0)] public float Damage;
        [Min(1)] public float AttackTime;

        private void Start()
        {
            InvokeRepeating(nameof(Attack), AttackTime, AttackTime);
        }

        private void Attack()
        {
            if (!Owner || !Owner.enabled)
            {
                return;
            }

            var _target = FindRandomTarget();

            if (!_target)
            {
                return;
            }

            _target.AddDamage(Damage);
            SpawnRayEffect(_target.transform.position);
        }

        private Enemy FindRandomTarget()
        {
            var _allCharacters = FindObjectsOfType<Enemy>();

            var _characters = new List<Enemy>();

            foreach (var c in _allCharacters)
            {
                if (c != Owner)
                {
                    if (CameraUtils.IsPointOnView(c.transform.position, Camera.main))
                    {
                        _characters.Add(c);
                    }
                }
            }

            if (_characters.Count == 0)
            {
                return null;
            }

            var _target = _characters[Random.Range(0, _characters.Count - 1)];

            return _target;
        }

        private void SpawnRayEffect(Vector3 position)
        {
            if (!RayPrefab)
            {
                return;
            }

            var _randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            var _ray = Instantiate(RayPrefab, position, _randomRotation);
            Destroy(_ray, RayObjectLifeTime);
        }
    }
}