using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mecanics
{
    public class GolemAttackMarker : MonoBehaviour
    {
        public float MarkerLifeTime;
        public GameObject MarkerPrefab;
        public GolemMiniBoss Golem;

        private PlayerCharacter Player;

        void Start()
        {
            Player = GameObject.FindObjectOfType<PlayerCharacter>();
            Golem.OnThrowBall.AddListener(SpawnMarker);
        }
        private void SpawnMarker()
        {
            if (!Player)
            {
                return;
            }

            var _marker = Instantiate(MarkerPrefab, Player.transform.position, Quaternion.identity);
            Destroy(_marker, MarkerLifeTime);
        }
    }
}