using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC_Test : MonoBehaviour
{
    public Transform target;
    public CharacterController character;
    public float moveSpeed;
    public float rotSpeed;

    public UnityEngine.Events.UnityEvent OnDestroyEvent;

    public CheckVisible checkVisible;

    public bool usePhysics;

    void Start()
    {
        checkVisible.OnInvisible.AddListener(OnInvisible);
        checkVisible.OnVisible.AddListener(OnVisible);
    }

    void OnDestroy()
    {
        OnDestroyEvent.Invoke();
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(target.position - transform.position);
        var _moveDir = transform.forward * Time.deltaTime * moveSpeed;
        
        if (usePhysics) character.Move(_moveDir);
        else transform.Translate(_moveDir, Space.World);
        
        if (Vector3.Distance(transform.position, target.position) < 1)
        {
            Destroy(gameObject);
        }
    }

    void OnVisible()
    {
        usePhysics = true;
        character.enabled = true;
    }

    void OnInvisible()
    {
        usePhysics = false;
        character.enabled = false;
    }
}
