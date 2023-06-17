using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace Game.Mecanics
{
    /// <summary>
    /// Manage level design, storage all level environments.
    /// Each level has a ground object.
    /// When change level, update navmesh.
    /// </summary>
    public class MapLevelGeneratorManager : MonoBehaviour
    {
        [System.Serializable]
        public class Level
        {
            [SerializeField] private string Name;
            public GameObject Ground;

            private bool _isEnabled;

            public bool IsEnabled
            {
                get => _isEnabled;
                set
                {
                    _isEnabled = value;

                    if (!Ground)
                    {
                        Debug.LogWarning("Add level ground on your level map generator");
                        return;
                    }


                    Ground.SetActive(value);
                }
            }
        }

        public Vector3 NavMeshSize;

        [Space]

        public UnityEvent OnChangeLevel;

        public Level[] Levels;

        private NavMeshData navMesh;
        private NavMeshDataInstance instance;
        private List<NavMeshBuildSource> sources;

        public int CurrentLevelIndex { get; private set; }

        private void Awake()
        {
            sources = new List<NavMeshBuildSource>();
            navMesh = new NavMeshData();
            instance = NavMesh.AddNavMeshData(navMesh);
        }

        private void Start()
        {
            SetLevel(0);
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

        private void UpdateNavMesh()
        {
            NavMeshSourceTag.Collect(ref sources);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);

            NavMeshBuilder.UpdateNavMeshData(navMesh, defaultBuildSettings, sources, new Bounds(transform.position, NavMeshSize));
        }

        public void SetLevel(int index)
        {
            if (index > Levels.Length - 1 || index < 0)
            {
                return;
            }

            foreach (var l in Levels)
            {
                l.IsEnabled = l == Levels[index];
            }

            UpdateNavMesh();

            CurrentLevelIndex = index;
            OnChangeLevel.Invoke();
        }

        public void SetNextLevel(int level)
        {
            if (level > 0) // dont set level if is first 
            {
                SetLevel(Mathf.Min(CurrentLevelIndex + 1, Levels.Length - 1));
            }
        }
    }
}