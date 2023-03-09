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

            var _targetPosition = new Vector3(Target.transform.position.x, 0, Target.transform.position.z);

            transform.position = Vector3.Lerp(transform.position, _targetPosition + Offset, _speed);
        }
    }
}
