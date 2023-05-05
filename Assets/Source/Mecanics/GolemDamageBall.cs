using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class GolemDamageBall : MonoBehaviour
    {
        public float DestroyDistance;
        public float MoveSpeed;
        public Rigidbody Rb;
        public GameObject SpawnOnDestroy;
        public float SpawnLifeTime;

        public float Damage { get; set; }
        public Vector3 Target { get; set; }
        public GolemMiniBoss Owner { get; set; }

        void Start()
        {

        }

        void FixedUpdate()
        {
            Rb.velocity = (Target - transform.position).normalized * Time.fixedDeltaTime * MoveSpeed;

            if (Vector3.Distance(transform.position, Target) < DestroyDistance)
            {
                Destroy(gameObject);
                var _spawn = Instantiate(SpawnOnDestroy, transform.position, Quaternion.identity);
                Destroy(_spawn, SpawnLifeTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == Owner.gameObject)
            {
                return;
            }

            if (other.TryGetComponent<EnemyBase>(out var enemy))
            {
                enemy.AddDamage(Damage);
            }

            if (other.TryGetComponent<PlayerCharacter>(out var player))
            {
                player.AddDamage(Damage);
            }
        }
    }
}
