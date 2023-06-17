using UnityEngine;

namespace Game.Mecanics
{
    /// <summary>
    /// Pedra arremessada pelo Golem, causa dano em qualquer personagem que colidir
    /// </summary>
    public class GolemDamageBall : MonoBehaviour
    {
        public float DestroyDistance;
        public float MoveSpeed;

        public Rigidbody Rb;
        public GameObject SpawnOnDestroy;
        public float SpawnLifeTime;

        // dados fornecidos pelo golem
        public float Damage { get; set; }
        public Vector3 Target { get; set; }
        public GolemMiniBoss Owner { get; set; }

        void FixedUpdate()
        {
            var _distanceToTarget = Vector3.Distance(transform.position, Target);
            var _moveDirection = (Target - transform.position).normalized;
            var _moveVelocity = _moveDirection * Time.fixedDeltaTime * MoveSpeed;
            Rb.velocity = _moveVelocity;

            if (_distanceToTarget < DestroyDistance)
            {
                var _spawn = Instantiate(SpawnOnDestroy, transform.position, Quaternion.identity);
                Destroy(_spawn, SpawnLifeTime);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Evita colidir com o proprio golem que jogou a pedra
            if (other.gameObject == Owner.gameObject)
            {
                return;
            }

            // aplica dano em qualquer personagem que colidir
            if (other.TryGetComponent<EnemyBase>(out var enemy)) enemy.AddDamage(Damage);
            if (other.TryGetComponent<PlayerCharacter>(out var player)) player.AddDamage(Damage);
        }
    }
}
