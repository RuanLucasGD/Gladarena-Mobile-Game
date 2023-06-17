using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Mecanics
{
    /// <summary>
    /// A character with player controls.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerInputs
        {
            /// <summary>
            /// The input action asset with all gameplay actions
            /// </summary>
            public InputActionAsset InputAsset;

            [Header("Movimentation")]

            [Tooltip("Axis Positive / Negative to move to forward or backward")]
            public string VerticalAction;

            [Tooltip("Axis Positive / Negative to move to right or back")]
            public string HorizontalAction;

            [Tooltip("Vector2 action value with direction to player move. \n'X' to right/left and 'Y' to forward/backward")]
            public string MobileJoystickAction;

            public PlayerInputs()
            {
                VerticalAction = "Vertical";
                HorizontalAction = "Horizontal";
                MobileJoystickAction = "Mobile Joystick";
            }
        }

        [System.Serializable]
        public class Moviment
        {
            [Min(0)] public float MoveSpeed;
            [Min(0)] public float Gravity;
            [Min(0)] public float TurnSpeed;
            [Min(0)] public float StopDistance;
            [Min(0)] public float ExternalForceDeceleration;

            [Space]
            public float MoveSpeedMultiplier;

            public Moviment()
            {
                MoveSpeed = 10;
                Gravity = 10;
                TurnSpeed = 10;
                StopDistance = 1;
            }
        }

        [System.Serializable]
        public class WeaponSlot
        {
            public PlayerMelleWeapon WeaponObject;
            public Transform Hand;

            public int SequencialAttacks;
            public float AttackRate;
            public float AttackLengthMultiplier;
            public float AttackRateMultiplier;
            public float AttackDamageMultiplier;
            public float AttackDistanceMultiplier;

            public UnityEvent OnStartAttack;

            public WeaponSlot()
            {
                AttackRate = 0.2f;
                AttackLengthMultiplier = 1;
                AttackRateMultiplier = 1;
                AttackDamageMultiplier = 1;
                AttackDistanceMultiplier = 1;
            }
        }

        [System.Serializable]
        public class LifeStorage
        {
            [Min(1)] public float InicialLife;
            [Min(0.1f)] public float AutoDestroyOnDeathDelay;

            [Space]
            public float LifeMultiplier;

            public UnityEvent OnResetLife;

            public LifeStorage()
            {
                InicialLife = 100;
            }
        }

        [System.Serializable]
        public class AnimationControl
        {
            public Animator Animator;

            [Tooltip("A interger value, from 0 to 1 representing if the player is moving")]
            public string MovimentParam;

            [Tooltip("Bool parameter, check if the character is death")]
            public string IsDeath;

            [Tooltip("Bool parameter")]
            public string IsAttacking;

            [Tooltip("Bool parameter, true if the character is using special attack animation")]
            public string IsSuperAttack;

            [Tooltip("Interger parameter, what weapon animation ID is using on AnimatorController?")]
            public string AttackAnimationID;

            [Tooltip("Float parameter, speed multiplier of the attack animation")]
            public string AttackAnimSpeed;

            public AnimationControl()
            {
                MovimentParam = "Vertical";
                IsDeath = "Is Death";
                IsAttacking = "Is Attacking";
                IsSuperAttack = "Is Super Attack";
                AttackAnimationID = "Attack Animation ID";
                AttackAnimSpeed = "Attack Speed";
            }
        }

        public PlayerInputs InputMaps;
        public Moviment Movimentation;
        public WeaponSlot Weapon;
        public LifeStorage Life;
        public AnimationControl Animation;

        [Space]

        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnSetWeapon;
        public UnityEvent OnRevive;

        [Header("Animation Events")]
        public UnityEvent OnAttackAnimationEvent;

        private Vector3 _moveDirection;
        private Vector3 _lookDirection;

        public CharacterController CharacterController { get; set; }

        public float CurrentMaxLife => Life.InicialLife * Life.LifeMultiplier;
        public float CurrentAttackRate => Weapon.AttackRate * Weapon.AttackRateMultiplier;
        public float CurrentAttackDistance => Weapon.WeaponObject.AttackRange * Weapon.AttackDistanceMultiplier;
        public float CurrentAttackDamage => Weapon.WeaponObject.AttackDamage * Weapon.AttackDamageMultiplier;
        public float CurrentMoveSpeed => Movimentation.MoveSpeed * Movimentation.MoveSpeedMultiplier;
        public float CurrentAttackLength => Weapon.WeaponObject.AttackLength * Weapon.AttackLengthMultiplier;


        public bool HasWeapon => Weapon.WeaponObject;
        public bool IsGrounded => CharacterController.isGrounded;
        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;
        public bool IsInvencible { get; set; }
        public bool IsDeath { get; private set; }
        public bool CanAttack { get; private set; }
        public bool IsAttacking { get; private set; }

        public float CurrentLife { get; private set; }

        public InputAction VerticalAction { get; private set; }
        public InputAction HorizontalAction { get; private set; }
        public InputAction MobileJoystickAction { get; private set; }
        public Vector3 ExternalForces { get; private set; }

        public Vector3 Forward { get; set; }
        public bool CanMove { get; set; }
        public bool EnablePlayerControl { get; set; }

        /// <summary>
        /// Current player moviment velocity with gravity
        /// </summary>
        /// <value></value>
        public Vector3 CharacterVelocity { get; private set; }

        /// <summary>
        /// Set direction to character move. To stop set as Vector3(0, 0, 0)
        /// </summary>
        /// <returns></returns>
        public Vector3 CharacterMoveDirection { get => _moveDirection; set => _moveDirection = new Vector3(value.x, 0, value.z).normalized; }
        public Vector3 LookAtDirection { get => _lookDirection; set => _lookDirection = Vector3.ProjectOnPlane(value, Vector3.up); }

        private void Awake()
        {
            InputMaps.InputAsset.Enable();
            CharacterController = GetComponent<CharacterController>();

            CanMove = true;
            EnablePlayerControl = true;
            CurrentLife = CurrentMaxLife;
            LookAtDirection = transform.forward;

            if (!CharacterController)
            {
                Debug.LogError($"Character Controller not added on gameObject {CharacterController}");
                return;
            }

            Forward = Camera.main ? Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) : transform.forward;
        }

        private void Start()
        {
            if (Weapon.WeaponObject)
            {
                SetWeapon(Weapon.WeaponObject);
            }

            if (!InputMaps.InputAsset)
            {
                Debug.LogError("Input action map not added on player!");
                return;
            }

            CanAttack = true;

            VerticalAction = InputMaps.InputAsset.FindAction(InputMaps.VerticalAction, throwIfNotFound: true);
            HorizontalAction = InputMaps.InputAsset.FindAction(InputMaps.HorizontalAction, throwIfNotFound: true);
            MobileJoystickAction = InputMaps.InputAsset.FindAction(InputMaps.MobileJoystickAction, throwIfNotFound: true);
        }

        private void Update()
        {
            UpdateRotation(Time.deltaTime);
            UpdateMoviment(Time.deltaTime);
            UpdateExternalForces(Time.deltaTime);

            UpdatePlayerControls();
        }

        private void LateUpdate()
        {
            UpdateAnimations();
        }

        private void UpdatePlayerControls()
        {
            if (!InputMaps.InputAsset || !EnablePlayerControl)
            {
                return;
            }

            if (Forward.magnitude == 0)
            {
                Debug.LogError($"Player {nameof(Forward)} can not be have magnetude as 0. Set player move direction to another value, example {Vector3.forward}");
                return;
            }

            // the movement inputs is the sum of gamepad/keyboard inputs with mobile joystick
            var _mobileJoystick = MobileJoystickAction.ReadValue<Vector2>();
            var _vertical = VerticalAction.ReadValue<float>();
            var _horizontal = HorizontalAction.ReadValue<float>();
            _vertical += _mobileJoystick.y;
            _horizontal += _mobileJoystick.x;

            var _forward = Forward;
            var _right = -Vector3.Cross(_forward, Vector3.up);
            var _moveDirection = ((_forward * _vertical) + (_right * _horizontal)).normalized;

            if (!IsStoped)
            {
                LookAtDirection = CharacterMoveDirection / CharacterMoveDirection.magnitude;
            }

            CharacterMoveDirection = _moveDirection;
        }

        private void UpdateMoviment(float delta)
        {
            if (!CharacterController)
            {
                return;
            }

            if (!CanMove)
            {
                CharacterMoveDirection = Vector3.zero;
            }

            CharacterVelocity = CharacterMoveDirection;
            CharacterVelocity *= CurrentMoveSpeed;
            CharacterVelocity = new Vector3(CharacterVelocity.x, -Movimentation.Gravity, CharacterVelocity.z);
            CharacterVelocity += ExternalForces;
            CharacterVelocity *= delta;

            CharacterController.Move(CharacterVelocity);
        }

        private void UpdateExternalForces(float delta)
        {
            if (ExternalForces.magnitude <= 0.1f)
            {
                ExternalForces = Vector3.zero;
                return;
            }

            ExternalForces -= (ExternalForces / ExternalForces.magnitude) * Movimentation.ExternalForceDeceleration * delta;
        }

        private void UpdateRotation(float delta)
        {
            if (!CanMove)
            {
                return;
            }

            var _turnSpeed = Mathf.Clamp01(delta * Movimentation.TurnSpeed);
            var _currentRot = transform.rotation;



            var _targetRot = Quaternion.LookRotation(LookAtDirection);

            transform.rotation = Quaternion.Lerp(_currentRot, _targetRot, _turnSpeed);
        }

        private void UpdateAnimations()
        {
            if (!Animation.Animator)
            {
                return;
            }

            Animation.Animator.SetFloat(Animation.MovimentParam, CharacterMoveDirection.magnitude);
            Animation.Animator.SetBool(Animation.IsAttacking, IsAttacking);
            Animation.Animator.SetBool(Animation.IsDeath, IsDeath);

            if (HasWeapon)
            {
                Animation.Animator.SetBool(Animation.IsSuperAttack, Weapon.WeaponObject.IsSuperAttack);
                Animation.Animator.SetInteger(Animation.AttackAnimationID, Weapon.WeaponObject.AnimationID);
                Animation.Animator.SetFloat(Animation.AttackAnimSpeed, CurrentAttackLength);
            }
        }

        public void SetWeapon(Game.Mecanics.PlayerMelleWeapon newWeapon)
        {
            if (IsDeath)
            {
                return;
            }

            if (!Weapon.Hand)
            {
                Debug.LogError($"Character '{gameObject.name}' does not have '{nameof(Weapon.Hand)}' of '{nameof(Weapon)}' assigned. Is not possible set the weapon.");
                return;
            }

            if (!newWeapon)
            {
                if (Weapon.WeaponObject)
                {
                    Destroy(Weapon.WeaponObject.gameObject);
                    return;
                }
            }

            newWeapon.transform.parent = Weapon.Hand;
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;
            newWeapon.Owner = this;
            Weapon.WeaponObject = newWeapon;

            OnSetWeapon.Invoke();
        }

        public void AddDamage(float damage)
        {
            if (IsDeath || !enabled)
            {
                return;
            }

            if (!IsInvencible)
            {
                CurrentLife -= damage;
            }

            OnDamaged.Invoke();

            if (CurrentLife <= 0)
            {
                CurrentLife = 0;
                IsDeath = true;
                StartCoroutine(DisableCharacter());
                StartCoroutine(DestroyCharacterDeleyed());

                OnDeath.Invoke();

                // wait to finalize all character logic to desactive
                IEnumerator DisableCharacter()
                {
                    yield return new WaitForEndOfFrame();

                    if (IsDeath) // the player can be revived after is death (in same frame)
                    {
                        enabled = false;
                    }
                }

                IEnumerator DestroyCharacterDeleyed()
                {
                    yield return new WaitForSeconds(Life.AutoDestroyOnDeathDelay);

                    if (IsDeath)
                    {
                        enabled = false;
                        CharacterController.enabled = false;
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void StartAttack()
        {
            if (!CanAttack || !Weapon.WeaponObject || !enabled)
            {
                return;
            }

            IsAttacking = true;
            Weapon.WeaponObject.Attack();

            var _attackLength = CurrentAttackLength * Weapon.SequencialAttacks;
            Invoke(nameof(FinalizeAttack), _attackLength);
            Weapon.OnStartAttack.Invoke();
        }

        private void FinalizeAttack()
        {
            IsAttacking = false;
            Weapon.WeaponObject.DisableAttack();

            // reinicie o ataque novamente
            Invoke(nameof(StartAttack), CurrentAttackRate);
        }

        public void KillCharacter()
        {
            if (IsDeath)
            {
                return;
            }

            AddDamage(Mathf.Infinity);
        }

        public void ResetLife()
        {
            CurrentLife = CurrentMaxLife;
            Life.OnResetLife.Invoke();

            if (IsDeath)
            {
                IsDeath = false;
                enabled = true;
                CharacterController.enabled = true;
                OnRevive.Invoke();
            }
        }

        public void AddExternalForces(Vector3 force)
        {
            ExternalForces += force;
        }

        // called by character animation event
        public void AttackAnimationEvent()
        {
            OnAttackAnimationEvent.Invoke();
        }
    }
}