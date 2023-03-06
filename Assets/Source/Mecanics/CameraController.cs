using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 Offset;
        public Character Target;

        [Space]
        public float MoveSpeed;

        void Start()
        {
            if (!Target)
            {
                return;
            }
        }

        void Update()
        {
            if (!Target)
            {
                return;
            }

            UpdateCameraPosition(Time.deltaTime);
        }

        private void UpdateCameraPosition(float delta)
        {
            var _speed = Mathf.Max(MoveSpeed, (Target.CharacterVelocity.magnitude + MoveSpeed)) * delta;
            _speed = Mathf.Clamp01(_speed);

            transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, _speed);
        }

        private void UpdateCameraRotation()
        {
            transform.LookAt(Target.transform.position);
        }
    }
}
