using UnityEngine;

namespace Game.Utils
{
    public static class CameraUtils
    {
        public static bool IsPointOnView(Vector3 point, Camera camera)
        {
            var _direction = point - camera.transform.position;

            // the point is on back of the camera
            if (Vector3.Dot(_direction.normalized, camera.transform.forward) <= 0)
            {
                return false;
            }

            var _pointOnScreen = camera.WorldToScreenPoint(point);

            return (_pointOnScreen.x > 0) &&
                    (_pointOnScreen.y > 0) &&
                    (_pointOnScreen.x < Screen.width) &&
                    (_pointOnScreen.y < Screen.height);
        }
    }
}


