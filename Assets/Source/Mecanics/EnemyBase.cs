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

        public float CurrentLife { get; protected set; }
        public bool IsDeath { get; private set; }
        public bool IsOnScreen { get; private set; }

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

        protected void MoveTo(Vector3 position, float moveSpeed)
        {
            Rb.MovePosition(Rb.position + ((position - Rb.position).normalized * moveSpeed));
        }

        // called by character animator event
        public virtual void AttackAnimationEvent()
        {
        }

        public virtual void Death()
        {
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

        
    }
}


