using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class ShieldPowerUp : PowerUp
    {
        [Header("Hit character")]
        public float Damage;
        public float Force;

        [Header("Shield resistence")]
        public int SupportedHits;
        public float RearmTime;

        [Header("Shield Model")]
        public GameObject ShieldMesh;

        public int CurrentHits { get; private set; }

        private Enemy _detectedCharacter;
        public override PlayerCharacter Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;

                Rearm();
                Owner.OnDamaged.AddListener(DetectHit);
            }
        }

        private void Start()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled)
            {
                return;
            }

            if (other.TryGetComponent<Enemy>(out var c))
            {
                if (c != Owner)
                {
                    // when the character collides with the field, the detection is called sometimes
                    // but needs to be called only one time
                    // this "gambiarra" avoid another calls to detect only once 
                    if (_detectedCharacter == c)
                    {
                        return;
                    }

                    _detectedCharacter = c;
                    StartCoroutine(RemoveDetectedCharacter());

                    DetectHit();

                    var _force = ((c.transform.position - transform.position).normalized * Force) + Owner.CharacterVelocity;
                    c.AddDamage(Damage, _force);
                }
            }
        }

        private IEnumerator RemoveDetectedCharacter()
        {
            yield return new WaitForSeconds(0.2f);
            _detectedCharacter = null;
        }

        private IEnumerator RearmDeleyed()
        {
            yield return new WaitForSeconds(RearmTime);
            Rearm();
        }

        private void DetectHit()
        {
            CurrentHits++;

            if (CurrentHits >= SupportedHits)
            {
                CurrentHits = 0;

                Disarm();
                StartCoroutine(RearmDeleyed());
            }
        }

        public void Disarm()
        {
            enabled = false;

            if (ShieldMesh)
            {
                ShieldMesh.SetActive(false);
            }

            Owner.IsInvencible = false;
        }

        public void Rearm()
        {
            enabled = true;

            if (ShieldMesh)
            {
                ShieldMesh.SetActive(true);
            }

            Owner.IsInvencible = true;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            Owner.OnDamaged.RemoveListener(DetectHit);
        }
    }
}