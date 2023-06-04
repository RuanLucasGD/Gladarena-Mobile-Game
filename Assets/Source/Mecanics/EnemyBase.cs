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

        public EnemyType Type;

        [Header("Basic")]
        public float MoveSpeed;
        public float MaxLife;
        public float AttackDamage;
        public float ExternalForceDeceleration;

        [Header("Components")]
        public Rigidbody Rb;
        public Renderer ModelRenderer;
        public Animator Animator;

        [Header("Events")]
        public UnityEvent OnSpawned;
        public UnityEvent OnKilled;
        public UnityEvent OnAttack;

        public bool IsStoped => MoveDirectionVelocity.magnitude == 0;

        public PlayerCharacter Target { get; set; }

        public float CurrentLife { get; protected set; }
        public bool IsDeath { get; private set; }
        public bool IsOnScreen { get; private set; }
        public bool IsAttacking { get; protected set; }

        public float StateExecutionTime => _currentStateTime;

        protected UnityAction CurrentState { get => _currentState; set { _currentState = value; _currentStateTime = 0f; } }

        public Vector3 MoveDirectionVelocity { get; protected set; }
        public Vector3 ExternalForce;

        private float _currentStateTime;
        private UnityAction _currentState;

        protected virtual void Awake()
        {
            Target = FindObjectOfType<PlayerCharacter>();
            CurrentLife = MaxLife;
            CurrentState = () => { };
        }

        protected virtual void Start()
        {
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
            if (Target && Target.IsDeath)
            {
                FindPlayer();
            }

            _currentStateTime += Time.deltaTime;

            if (ExternalForce.magnitude > 0)
            {
                ExternalForce -= (ExternalForce / ExternalForce.magnitude) * ExternalForceDeceleration * Time.deltaTime;
            }

            // don't use OnBecameVisible or OnBecameInvisible becase it's not called when object is created
            UpdateVisibility();

            if (!IsDeath)
            {
                CurrentState();
            }
        }
        protected virtual void FixedUpdate()
        {
            Rb.velocity = (MoveDirectionVelocity + ExternalForce) * Time.fixedDeltaTime;
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

            transform.rotation = Quaternion.Lerp(Rb.rotation, Quaternion.LookRotation(direction), _turnSpeed);
        }

        protected void FindPlayer()
        {
            // can have multiple player (clones of the default player)
            var _playersOnScene = FindObjectsOfType<PlayerCharacter>();

            for (int i = 0; i < _playersOnScene.Length; i++)
            {
                if (!_playersOnScene[i].IsDeath)
                {
                    Target = _playersOnScene[i];
                    break;
                }
            }
        }

        // called by character animator event
        public virtual void AttackAnimationEvent() { }

        public virtual void Death()
        {
            if (IsDeath || !enabled)
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
            if (!enabled) return;
            CurrentLife -= damage;
        }

        public virtual void AddExternalForce(Vector3 force)
        {
            ExternalForce += force;
        }
    }
}


