using UnityEngine;
using UnityEngine.Events;

namespace Game.Mecanics
{
    /// <summary>
    /// Golem MiniBoss, Segue o jogador e joga pedras que causam danos no que colidir
    /// </summary>
    public class GolemMiniBoss : EnemyBase
    {
        [Header("Golem")]
        public float FollowDistance;
        public float TurnSpeed;
        public float ThrowRockInterval;             // Tempo que leva para arremessar cada pedra

        [Header("Attack State")]
        public Transform Hand;                      // Local que será criada a pedra
        public GolemDamageBall BallPrefab;          // Prefab da pedra que será arremessada
        public float UseMelleAttackDistance;        // Distancia proxima do jogador para usar ataque corpo a corpo

        public UnityEvent OnThrowBall;
        public UnityEvent OnUseMelleAttack;

        [Header("Animation Parameters")]
        public string IsStopedAnimParam;
        public string IsAttackingAnimParam;
        public string IsDeathAnimParam;
        public string UseThrowAttackAnimParam;
        public float MelleAttackAnimLength;
        public float ThrowRockAttackAnimLength;

        private float _throwRockInterval;

        /// <summary>
        /// use o ataque corpo a corpo apenas se o alvo estiver proximo
        /// </summary>
        public bool UseMelleAttackAnim
        {
            get
            {
                return Vector3.Distance(transform.position, Target.transform.position) < UseMelleAttackDistance;
            }
        }

        /// <summary>
        /// Comprimento atual da animação de ataque
        /// </summary>
        public float CurrentAnimLength
        {
            get
            {
                return !UseMelleAttackAnim ? ThrowRockAttackAnimLength : MelleAttackAnimLength;
            }
        }

        protected override void Start()
        {
            base.Start();
            CurrentState = FollowTargetState;

            // pegue apenas o player original
            Target = GameManager.Instance.Player;
        }

        protected override void Update()
        {
            base.Update();

            // procura o alvo caso o alvo atual morreu
            // podem ter multiplos players por causa do power-up AresArmy
            if (Target && Target.IsDeath)
            {
                FindPlayer();
            }

            // se não tiver mais um alvo, fique parado
            if (!Target)
            {
                if (CurrentState != IdleState) CurrentState = IdleState;
            }

            UpdateAnimations();

            // Caso ele não esteja atacando e não esteja proximo do jogador
            // estere o tempo certo para jogar a pedra
            if (CurrentState != AttackState && !UseMelleAttackAnim)
            {
                _throwRockInterval += Time.deltaTime;
            }

            // caso chegue o tempo certo de jogar a pedra ou o alvo esteja proximo, ataque
            if (_throwRockInterval > ThrowRockInterval || UseMelleAttackAnim)
            {
                _throwRockInterval = 0f;
                CurrentState = AttackState;
            }

            if (CurrentLife < 0)
            {
                Death();
            }
        }

        private void FollowTargetState()
        {
            var _directionToTarget = (Target.transform.position - transform.position).normalized;
            var _distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

            MoveDirectionVelocity = _directionToTarget * MoveSpeed;
            LookTo(_directionToTarget, TurnSpeed);

            if (_distanceToTarget < FollowDistance)
            {
                CurrentState = IdleState;
            }
        }

        private void IdleState()
        {
            MoveDirectionVelocity = Vector3.zero;
            LookTo(Target.transform.position - transform.position, TurnSpeed);

            var _targetDistance = Vector3.Distance(transform.position, Target.transform.position);

            // se o alvo estiver distante ou o golem estiver fora do campo de visão do jogador, caminhe até o alvo
            if (_targetDistance > FollowDistance || !IsOnScreen)
            {
                CurrentState = FollowTargetState;
            }
        }

        private void AttackState()
        {
            if (Target.IsDeath)
            {
                CurrentState = IdleState;
                IsAttacking = false;
                return;
            }

            MoveDirectionVelocity = Vector3.zero;
            IsAttacking = true;
            LookTo(Target.transform.position - transform.position, TurnSpeed);

            // desative o ataque depois que acabou o tempo
            // de reprodução da animação de ataque
            // e volte a seguir o jogador
            if (StateExecutionTime > CurrentAnimLength)
            {
                IsAttacking = false;
                CurrentState = FollowTargetState;
            }
        }

        private void UpdateAnimations()
        {
            Animator.SetBool(IsStopedAnimParam, IsStoped);
            Animator.SetBool(IsAttackingAnimParam, IsAttacking);
            Animator.SetBool(UseThrowAttackAnimParam, !UseMelleAttackAnim);
            Animator.SetBool(IsDeathAnimParam, IsDeath);
        }

        /// <inheritdoc/>
        public override void AttackAnimationEvent()
        {
            base.AttackAnimationEvent();

            // jogue a bola se o alvo estiver distante, se não use ataque corpo a corpo
            if (!UseMelleAttackAnim)
            {
                var _ball = Instantiate(BallPrefab, Hand.position, Quaternion.LookRotation(Target.transform.position - transform.position));
                _ball.Damage = AttackDamage;
                _ball.Target = Target.transform.position;
                _ball.Owner = this;

                OnThrowBall.Invoke();
            }
            else
            {
                Target.AddDamage(AttackDamage);
                OnUseMelleAttack.Invoke();
            }
        }
    }
}