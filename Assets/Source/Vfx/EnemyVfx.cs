using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class EnemyVfx : MonoBehaviour
    {
        public EnemyBase Enemy;
        public float ParticlesLifeTime;
        public ParticleSystem DamageParticle;
        public Vector3 SpawnOffset;

        void Start()
        {
            Enemy.OnDamaged.AddListener(SpawnDamageParticle);
        }

        private void SpawnDamageParticle()
        {
            var _particle = Instantiate(DamageParticle, Enemy.transform.position + SpawnOffset, Quaternion.identity);
            Destroy(_particle.gameObject, ParticlesLifeTime);
        }
    }
}
