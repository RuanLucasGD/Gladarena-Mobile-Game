using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    public class EnemyBase : MonoBehaviour
    {
        [System.Serializable]
        public enum EnemyType
        {
            Soldier,
            MiniBoss,
            Boss
        }

        public PlayerCharacter Target;

        [Header("Components")]
        public Rigidbody Rb;
        public Renderer ModelRenderer;
        public Animator Animator;

        [Header("Events")]
        public UnityEvent OnSpawned;
        public UnityEvent OnKilled;
        public UnityEvent OnAttack;

        public bool IsStoped => MoveDirectionVelocity.magnitude == 0;

        public float CurrentLife { get; protected set; }
        public bool IsDeath { get; private set; }
        public bool IsOnScreen { get; private set; }

        public Vector3 MoveDirectionVelocity { get; protected set; }

        protected virtual void Start()
        {

            Target = FindObjectOfType<PlayerCharacter>();

            if (Rb)
            {
                Rb.constraints |= RigidbodyConstraints.FreezePositionY;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationX;
                Rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }

            OnSpawned.Invoke();
        }

        protected virtual void Update()
        {
            // don't use OnBecameVisible or OnBecameInvisible becase it's not called when object is created
            UpdateVisibility();
        }
        protected virtual void FixedUpdate()
        {
            Rb.MovePosition(Rb.position + (MoveDirectionVelocity * Time.fixedDeltaTime));
        }

        private void UpdateVisibility()
        {
            if (!ModelRenderer)
            {
                return;
            }

            IsOnScreen = ModelRenderer.isVisible;
        }

        protected virtual void Attack()
        {
            OnAttack.Invoke();
        }

        protected void LookTo(Vector3 direction, float turnSpeed)
        {
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            direction.y = 0;
            direction.Normalize();
            var _turnSpeed = Mathf.Clamp01(turnSpeed * Time.deltaTime);

            Rb.rotation = Quaternion.Lerp(Rb.rotation, Quaternion.LookRotation(direction), _turnSpeed);
        }

        // called by character animator event
        public virtual void AttackAnimationEvent() { }

        public virtual void Death()
        {
            if (IsDeath)
            {
                return;
            }
            
            IsDeath = true;
            Rb.useGravity = false;

            if (TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }

            Rb.constraints |= RigidbodyConstraints.FreezePositionX;
            Rb.constraints |= RigidbodyConstraints.FreezePositionY;
            Rb.constraints |= RigidbodyConstraints.FreezePositionZ;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            Rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            Destroy(gameObject, 10);

            OnKilled.Invoke();
        }

        public virtual void AddDamage(float damage)
        {
            CurrentLife -= damage;
        }
    }
}


