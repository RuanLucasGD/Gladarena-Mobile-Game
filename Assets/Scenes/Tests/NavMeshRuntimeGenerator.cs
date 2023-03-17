using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace Game.Mecanics
{
    // Build and update a localized navmesh from the sources marked by NavMeshSourceTag
    public class NavMeshRuntimeGenerator : MonoBehaviour
    {
        public Vector3 NavMeshSize;

        [Space]

        public UnityEvent OnGenerateNavMesh;

        private NavMeshData navMesh;
        private NavMeshDataInstance instance;
        private List<NavMeshBuildSource> sources;

        void OnEnable()
        {
            sources = new List<NavMeshBuildSource>();

            // Construct and add navmesh
            navMesh = new NavMeshData();
            instance = NavMesh.AddNavMeshData(navMesh);
        }

        void OnDisable()
        {
            // Unload navmesh and clear handle
            instance.Remove();
        }

        private void Update()
        {
            UpdateNavMesh();
        }

        public void UpdateNavMesh()
        {
            NavMeshSourceTag.Collect(ref sources);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);

            NavMeshBuilder.UpdateNavMeshData(navMesh, defaultBuildSettings, sources, new Bounds(transform.position, NavMeshSize));

            OnGenerateNavMesh.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (navMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(navMesh.sourceBounds.center, navMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, NavMeshSize);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, NavMeshSize);
            Gizmos.color = Color.white;
        }
    }
}