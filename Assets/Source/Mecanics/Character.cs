using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    /// <summary>
    /// The base to make characters.
    /// Have all base behaviours of a NPC.
    /// Can move, or attack.
    /// Dont have self-control / AI
    /// </summary>
    public class Character : MonoBehaviour
    {
        [System.Serializable]
        public class Moviment
        {
            [Min(0)] public float MoveSpeed;
            [Min(0)] public float Gravity;
            [Min(0)] public float TurnSpeed;

            public Moviment()
            {
                MoveSpeed = 10;
                Gravity = 10;
                TurnSpeed = 10;
            }
        }

        public Moviment Movimentation;

        private Vector3 _moveDirection;
        private CharacterController _characterController;

        public CharacterController CharacterController => _characterController;

        public bool IsStoped => CharacterMoveDirection.magnitude < 0.1f;

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



        protected virtual void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (!CharacterController)
            {
                Debug.LogError($"Character Controller not added on gameObject {CharacterController}");
                return;
            }
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            UpdateRotation(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            UpdateMoviment(Time.fixedDeltaTime);
        }

        private void UpdateMoviment(float delta)
        {
            if (!CharacterController)
            {
                return;
            }

            CharacterVelocity = new Vector3(CharacterMoveDirection.x, -Movimentation.Gravity, CharacterMoveDirection.z);
            CharacterVelocity *= Movimentation.MoveSpeed;
            CharacterVelocity *= delta;

            CharacterController.Move(CharacterVelocity);
        }

        private void UpdateRotation(float delta)
        {
            if (IsStoped)
            {
                // don't need to rotate if is stoped
                return;
            }

            var _turnSpeed = Mathf.Clamp01(delta * Movimentation.TurnSpeed);
            var _currentRot = transform.rotation;
            var _targetRot = Quaternion.LookRotation(CharacterMoveDirection / CharacterMoveDirection.magnitude);

            transform.rotation = Quaternion.Lerp(_currentRot, _targetRot, _turnSpeed);
        }
    }
}

