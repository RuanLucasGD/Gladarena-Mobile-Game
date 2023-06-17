using UnityEngine;
using Game.Utils;

namespace Game.Mecanics
{
    /// <summary>
    /// Boss que anda aleatoriamente pelo mapa e ataca o jogador usando dash jogando o player para longe
    /// </summary>
    public class KnightBoss : EnemyBase
    {
        [Header("Walk to Random Points")]
        public float StopDistance;
        public float WalkTime;

        [Header("Dash Attack")]
        public int DashsAmount;
        public float PrepareDashTime;
        public float DashTime;
        public float DashAttackForce;
        public float DashStopOffset;

        [Header("Walk Random")]
        public float MoveRandomDistance;
        public float TurnSpeed;
        public LayerMask ObstaclesLayer;

        [Header("Animation")]
        public string IsDeathAnimParam;
        public string IsWalkingAnimParam;
        public string IsAttackinggAnimParam;
        public string IsPreparingAttackAnimParam;

        private int _currentAttacksAmount;
        private float _currentAttackProgression;

        private Vector3 _moveTo;
        private Vector3 _startAttackPos;

        // O IsOnScreen padrão não funcionou muito bem para esse modelo em especifico
        // pois o tamanho dele gerava uma BoundBox grande demais que não representava corretamente
        // se o modelo do boss está realmente dentro do campo de visão.
        // Então verificar a posição central dele funcionou melhor nesse caso
        public new bool IsOnScreen => CameraUtils.IsPointOnView(transform.position, Camera.main);

        // True se o boss está preparando para fazer o dash
        public bool IsPreparingDash { get; private set; }

        protected override void Start()
        {
            base.Start();
            CurrentState = WalkRantomState;
            _moveTo = GetRandomPosition();

            // pegue apenas o player original
            Target = GameManager.Instance.Player;
        }

        protected override void Update()
        {
            if (Target.IsDeath)
            {
                CurrentState = IdleState;
                CurrentState();
                return;
            }

            if (CurrentLife <= 0)
            {
                Death();
            }

            base.Update();
            UpdateAnimations();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAttacking)
            {
                return;
            }

            // aplica dano no jogador e o joga pra longe se estiver fazendo dash
            if (other.TryGetComponent<PlayerCharacter>(out var player))
            {
                var _throwPlayerDirection = (player.transform.position - Rb.position).normalized;
                var _thorwPlayerForce = _throwPlayerDirection * DashAttackForce;
                player.AddExternalForces(_thorwPlayerForce * DashAttackForce);
                player.AddDamage(AttackDamage);
            }
        }

        private void IdleState()
        {
            MoveDirectionVelocity = Vector3.zero;
        }

        private void FollowTargetState()
        {
            var _targetDirection = (Target.transform.position - Rb.position).normalized;
            var _walkVelocity = _targetDirection * MoveSpeed;

            MoveDirectionVelocity = _walkVelocity;
            LookTo(MoveDirectionVelocity, TurnSpeed);

            // só ataque o jogador se o boss estiver na visão do player
            if (IsOnScreen)
            {
                CurrentState = PrepareDashAttackState;

                // Não caminhe aleatoriamente, vá direto para a preparação de ataque
                // se não deixaria a gameplay muito lenta
                //CurrentState = WalkRantomState;
            }
        }

        private void WalkRantomState()
        {
            if (!IsOnScreen)
            {
                CurrentState = FollowTargetState;
                return;
            }

            var _distanceToTarget = Vector3.Distance(Rb.position, _moveTo);
            var _isTargetNear = _distanceToTarget < StopDistance;

            // verifica se deve gerar alguma posição aleatoria na qual o boss deve ir
            var _generateNewRandomPos = _isTargetNear || MoveDirectionVelocity == Vector3.zero;

            if (_generateNewRandomPos)
            {
                // gera alguma posição aleatoria proximo ao boss
                _moveTo = GetRandomPosition();

                // evita que o boss caminhe em direção a algum obstaculo
                // casp tenha obstaculo faça com que ele ande até um ponto antes do obstaculo
                if (Physics.Linecast(Rb.position, _moveTo, out var hit, ObstaclesLayer))
                {
                    _moveTo = hit.point;
                }
            }

            // se mova até a posição aleatoria
            MoveDirectionVelocity = (_moveTo - Rb.position).normalized * MoveSpeed;
            LookTo(MoveDirectionVelocity, TurnSpeed);

            // depois de andar o suficiente se prepare para atacar
            if (StateExecutionTime >= WalkTime)
            {
                CurrentState = PrepareDashAttackState;
                return;
            }
        }

        private void PrepareDashAttackState()
        {
            // faça o dash apenas se estiver no campo de visão do jogador
            if (!IsOnScreen)
            {
                CurrentState = FollowTargetState;
                IsPreparingDash = false;
                return;
            }

            // Pare, olhe para o personagem alvo e depois começe o dash

            IsPreparingDash = true;
            MoveDirectionVelocity = Vector3.zero;

            if (StateExecutionTime >= PrepareDashTime)
            {
                IsPreparingDash = false;
                CurrentState = AttackState;
                return;
            }

            _moveTo = Target.transform.position;
            LookTo(_moveTo - Rb.position, TurnSpeed);
        }

        private void AttackState()
        {
            // comece o dash descobrindo para onde deve ir
            if (!IsAttacking)
            {
                IsAttacking = true;
                var _directionToTarget = (Target.transform.position - Rb.position).normalized;
                var _stopOffset = _directionToTarget * DashStopOffset;

                _startAttackPos = Rb.position;
                _moveTo = Target.transform.position + _stopOffset;
                _currentAttackProgression = 0f;

                // evite colidir em obstaculos durante o dash
                if (Physics.Linecast(Rb.position, _moveTo, out var hit, ObstaclesLayer))
                {
                    _moveTo = hit.point;

                    // evite o obstaculo parando antes de bater 
                    _moveTo -= _directionToTarget * StopDistance;
                }
            }

            // se movendo até a posição do fim do dash
            _currentAttackProgression += Time.deltaTime * DashTime;
            _currentAttackProgression = Mathf.Min(_currentAttackProgression, 1);
            Rb.MovePosition(Vector3.Lerp(_startAttackPos, _moveTo, _currentAttackProgression));

            // finaliza o dash
            if (StateExecutionTime >= DashTime)
            {
                IsAttacking = false;
                _currentAttackProgression = 0f;

                _currentAttacksAmount++;

                // refaça o dash se necessario
                if (_currentAttacksAmount < DashsAmount)
                {
                    CurrentState = PrepareDashAttackState;
                }
                // se não volte a andar
                else
                {
                    _currentAttacksAmount = 0;
                    CurrentState = WalkRantomState;
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            var x = Random.Range(-MoveRandomDistance, MoveRandomDistance);
            var z = Random.Range(-MoveRandomDistance, MoveRandomDistance);
            var _randomOffset = new Vector3(x, 0, z);
            return Rb.position + _randomOffset;
        }

        private void UpdateAnimations()
        {
            Animator.SetBool(IsDeathAnimParam, IsDeath);
            Animator.SetBool(IsWalkingAnimParam, !IsStoped);
            Animator.SetBool(IsPreparingAttackAnimParam, IsPreparingDash);
            Animator.SetBool(IsAttackinggAnimParam, IsAttacking);
        }
    }
}