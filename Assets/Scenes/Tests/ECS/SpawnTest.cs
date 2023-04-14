using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnTest : MonoBehaviour
{
    public Text spawnText;
    public Transform follow;
    public NPC_Test Npc;

    public Transform[] Spawns;

    public int MaxSpawn;
    public float SpawnRate;

    [Space()]
    public int CurrentSpawnAmount;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), SpawnRate, SpawnRate);
    }

    void Update()
    {
        transform.position = follow.position;

        spawnText.text = "Spawned: " + CurrentSpawnAmount.ToString();
    }

    void Spawn()
    {
        if (CurrentSpawnAmount < MaxSpawn)
        {
            var pos = Spawns[Random.Range(0, Spawns.Length - 1)].position;
            var rot = Quaternion.LookRotation(transform.position - pos);
            var _npc = Instantiate(Npc, pos, rot);

            _npc.target = transform;

            _npc.OnDestroyEvent.AddListener(() =>
            {
                CurrentSpawnAmount -= 1;
            });

            CurrentSpawnAmount++;
        }
    }
}
