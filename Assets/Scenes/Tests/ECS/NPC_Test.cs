using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NPC_ECS_TestData
{
    public Transform target;
    public float moveSpeed;
    public float rotSpeed;

}

public class NPC_Test : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public float rotSpeed;

    void Start()
    {

    }

    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) > 2)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * rotSpeed);
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
    }
}
