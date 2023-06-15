using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    /// <summary>
    /// Arma branca do jogador, aplica ataques aos inimigos proximos que estiverem a frente.
    /// </summary>
    public class MelleWeapon : MonoBehaviour
    {
        [SerializeField]
        private bool DebugLog;

        [Header("Mesh")]
        // Modelo da arma, não interfere no ataque, porem codigos externos podem trocar a mesh da arma
        // Caso de uso: Power Ups de ataque que possuem modelos personalizados
        public GameObject Mesh;

        [Header("Animation")]

        // Cada animação de ataque do Animator do player tem seu ID, exemplo:
        // Espada  > ID 0
        // Lança   > ID 1
        // Machado > ID 2
        // Dessa forma sempre que estiver atacando essa arma irá chamar a animação 
        // correta no animator
        // verifique o parametro "Weapon ID" no animator do player
        public int AnimationID;

        /// <summary>
        /// Voce precisa definir um dono para que essa arma funcione, exemplo -> Player.SetWeapon
        /// </summary>
        [Header("Events")]
        public UnityEvent<PlayerCharacter> OnSetOwner;

        /// <summary>
        /// Chamado quando a arma consegue acertar algum inimigo no ataque
        /// </summary>
        public UnityEvent OnWeaponHitEnemy;

        public float AttackRange;   // distancia do ataque
        public float AttackDamage;  // Força do ataque
        public float AttackLength;  // Tamanho da animação do ataque, exemplo 1.1 segundos

        // O angulo maximo que a arma pode detectar um inimigo para o ataque. 
        // 0   > consegue atacar inimigos que estão ao lado e a frente do jogador
        // 0.5 > consegue atacar inimigos que estejam a +- 45° a frente do jogador
        // 0.9 > apenas consegue atacar inimigos que estejam na frente absoluta do jogador
        // veja: https://youtu.be/_61tlp2kOow?t=161 
        [Range(0.1f, 0.95f)]
        public float DotAttackAngle;

        private bool _enemyHit;
        private bool _isAttacking;
        private PlayerCharacter _owner;

        public PlayerCharacter Owner
        {
            get => _owner;
            set
            {
                // caso esteja trocando do dono, remova a ação de ataque do personagem antigo
                if (_owner) _owner.OnAttackAnimationEvent.RemoveListener(ApplyDamageOnEnemies);
                // adicione a ação de ataque ao novo personagem 
                if (value) value.OnAttackAnimationEvent.AddListener(ApplyDamageOnEnemies);

                _owner = value;
                OnSetOwner.Invoke(_owner);
            }
        }

        public bool IsAttacking { get; private set; }

        /// <summary>
        /// Não afeta em nada, porem pode ser usado pelo animator do player
        /// para reproduzir animações diferentes caso seja um super ataque
        /// </summary>
        public bool IsSuperAttack { get; set; }

        public MelleWeapon()
        {
            DotAttackAngle = 0.5f;
            AttackRange = 3;
            AttackDamage = 40;
            AttackLength = 1;
        }

        protected void Awake()
        {
            SetupWeapon();
            IsAttacking = false;
        }

        private void LateUpdate()
        {
            _enemyHit = false;
        }

        private void SetupWeapon()
        {
            var _collider = GetComponent<Collider>();
            var _rigidBody = GetComponent<Rigidbody>();

            if (_collider) _collider.isTrigger = true;
            if (_rigidBody) _rigidBody.useGravity = false;
        }

        public void DisableAttack()
        {
            IsAttacking = false;
        }

        private List<EnemyBase> GetNearInViewEnemies()
        {
            var _nearInView = new List<EnemyBase>();
            var _characters = FindObjectsOfType<EnemyBase>();

            foreach (var c in _characters)
            {
                // apenas inimigos proximos
                if (Vector3.Distance(c.transform.position, Owner.transform.position) > Owner.CurrentAttackDistance) continue;
                // apenas inimigos dentro do campo de visão do jogador
                if (Vector3.Dot(Owner.transform.forward, (c.transform.position - Owner.transform.position).normalized) < DotAttackAngle) continue;
                _nearInView.Add(c);
            }

            return _nearInView;
        }

        public void Attack()
        {
            // ataque apenas uma vez por reprodução de animação
            if (IsAttacking)
            {
                return;
            }

            // verifica se existe algum inimigo proximo no 
            // campo de visão para poder atacar
            var _nearInViewEnemies = GetNearInViewEnemies();

            if (_nearInViewEnemies.Count > 0)
            {
                IsAttacking = true;
            }

            // caso o ataque seja usado varias vezes ao mesmo tempo
            // apenas verifique se conseguiu atacar o inimigo uma vez
            if (!_enemyHit)
            {
                _enemyHit = true;
                OnWeaponHitEnemy.Invoke();
            }

            _nearInViewEnemies = null;
        }

        // Chamado pelo Animator do personagem
        private void ApplyDamageOnEnemies()
        {
            foreach (var c in GetNearInViewEnemies())
            {
                // pega os dados de dano do personagem
                // pois ele pode ter multiplicadores de 
                // dano obtidos pelos power ups
                c.AddDamage(Owner.CurrentAttackDamage);

#if UNITY_EDITOR
                // evita verificações desnecessarias na build do jogo
                if (DebugLog) Debug.Log($"Target damaged: {c.name}        damage: {Owner.CurrentAttackDamage}");
#endif
            }
        }
    }
}
