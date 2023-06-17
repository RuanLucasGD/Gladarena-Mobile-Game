using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    /// <summary>
    /// A base de todos os inimigos, possui todo comportamento central que eles devem ter.
    /// </summary>
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>
        /// Usado pelo sistema de progressão de nivel
        /// </summary>
        [System.Serializable]
        public enum EnemyType
        {
            Soldier,
            MiniBoss,
            Boss
        }

        [Header("Basic")]
        public EnemyType Type;
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
        public UnityEvent OnDamaged;
        public UnityEvent OnAttack;

        public bool IsStoped => MoveDirectionVelocity.magnitude == 0;

        public PlayerCharacter Target { get; set; }

        public float CurrentLife { get; protected set; }
        public bool IsDeath { get; private set; }
        public bool IsOnScreen { get; private set; }
        public bool IsAttacking { get; protected set; }
        public Vector3 ExternalForce { get; private set; }

        /// <summary>
        /// Direção que o inimigo deve se mover
        /// </summary>
        public Vector3 MoveDirectionVelocity
        {
            get => _moveDirectionVelocity;
            protected set => _moveDirectionVelocity = value.normalized;
        }

        // tempo que o estado atual está sendo executado
        public float StateExecutionTime { get; private set; }

        // Todo comportamento do inimigo é feito por estados
        // parado, andando, atacando etc
        // cada inimigo tem seus proprios estados e só podem executar um estado por vez
        // Passe aqui qual comportamento ele deve ter, e mude a ação conforme sua necessidade
        protected UnityAction CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                StateExecutionTime = 0f;
            }
        }

        private Vector3 _moveDirectionVelocity;
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
            if (!Target || Target.IsDeath)
            {
                FindPlayer();
            }

            StateExecutionTime += Time.deltaTime;

            // atualiza a desaceleração das forças externas
            if (ExternalForce.magnitude > 0)
            {
                ExternalForce -= (ExternalForce / ExternalForce.magnitude) * ExternalForceDeceleration * Time.deltaTime;
            }

            // não use OnBecameVisible ou OnBecameInvisible porque eles não são chamados quando o objeto é criado
            UpdateVisibility();

            if (!IsDeath)
            {
                // atualiza o comportamento atual do personagem
                CurrentState();
            }
        }
        protected virtual void FixedUpdate()
        {
            // move o personagem para a direção designada
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

        // olhe para alguma direção 
        protected void LookTo(Vector3 direction, float turnSpeed)
        {
            // olhe sempre horizontalmente, mantenha o eixo Up para cima
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);

            var _turnSpeed = Mathf.Clamp01(turnSpeed * Time.deltaTime);
            var _lookRotation = Quaternion.LookRotation(direction);
            var _smoothRotation = Quaternion.Lerp(Rb.rotation, _lookRotation, _turnSpeed);

            transform.rotation = _smoothRotation;
        }

        protected void FindPlayer()
        {
            // encontra todos os jogadores da cena. A cena pode ter multiplos players por causa 
            // do power up AresArmy que gera clones do jogador
            var _playersOnScene = FindObjectsOfType<PlayerCharacter>();

            // tenta encontrar algum jogador que esteja vivo
            for (int i = 0; i < _playersOnScene.Length; i++)
            {
                if (!_playersOnScene[i].IsDeath)
                {
                    Target = _playersOnScene[i];
                    break;
                }
            }
        }

        /// <summary>
        /// executado pelo Animator do golem
        /// A cada vez que a animação de ataque for executada
        /// a animação deve chamar esse metodo e aplicar o ataque
        /// Se esse metodo não estiver sendo executado verifique se a animação
        /// do golem possui os eventos de animação
        /// https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html
        /// </summary>
        public virtual void AttackAnimationEvent() { }

        /// <summary>
        /// Desabilita todos os controles do personagem e fisica
        /// </summary>
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

        /// <summary>
        /// Adiciona dano neste personagem
        /// </summary>
        /// <param name="damage"></param>
        public virtual void AddDamage(float damage)
        {
            if (!enabled || IsDeath) return;
            CurrentLife -= damage;
            OnDamaged.Invoke();
        }

        /// <summary>
        /// Adicione um impulso a esse personagem, como a força de uma explosão por exemplo, só precisa ser chamado uma vez.
        /// </summary>
        /// <param name="force"></param>
        public virtual void AddExternalForce(Vector3 force)
        {
            ExternalForce += force;
        }
    }
}