using UnityEngine;

namespace Game.Mecanics
{
    /// <summary>
    /// Marcador da posição que a pedra do golem vai cair
    /// </summary>
    public class GolemAttackMarker : MonoBehaviour
    {
        public float MarkerLifeTime;
        public GameObject MarkerPrefab;
        public GolemMiniBoss Golem;

        void Start()
        {
            Golem.OnThrowBall.AddListener(SpawnMarker);
        }

        private void SpawnMarker()
        {
            if (!Golem.Target)
            {
                return;
            }

            var _marker = Instantiate(MarkerPrefab, Golem.Target.transform.position, Quaternion.identity);
            Destroy(_marker, MarkerLifeTime);
        }
    }
}