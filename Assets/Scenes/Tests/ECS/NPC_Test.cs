using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC_Test : MonoBehaviour
{
    public Transform target;
    public Rigidbody character;
    public Animator Animator;
    public float moveSpeed;
    public float rotSpeed;

    public UnityEngine.Events.UnityEvent OnDestroyEvent;

    public CheckVisible checkVisible;

    public bool usePhysics;

    void Awake()
    {
        checkVisible.OnInvisible.AddListener(OnInvisible);
        checkVisible.OnVisible.AddListener(OnVisible);

        if (!checkVisible.isVisible)
            OnInvisible();
    }

    void OnDestroy()
    {
        OnDestroyEvent.Invoke();

    }

    void Update()
    {
        var _moveDir = (target.position - transform.position).normalized;

        if (usePhysics)
        {
            transform.rotation = Quaternion.LookRotation(_moveDir.normalized);

            // if (Vector3.Distance(transform.position, target.position) < 2)
            // {
            //     Destroy(gameObject);
            // }
        }
    }

    void FixedUpdate()
    {
        var _moveDir = (target.position - transform.position).normalized * Time.fixedDeltaTime * moveSpeed;

        transform.rotation = Quaternion.LookRotation(_moveDir.normalized);

        if (Vector3.Distance(transform.position, target.position) < 5)
        {
            _moveDir = Vector3.zero;
        }

        character.velocity = (_moveDir);
    }

    void OnVisible()
    {
        usePhysics = true;
        Animator.enabled = true;
    }

    void OnInvisible()
    {
        usePhysics = false;
        Animator.enabled = false;
    }
}
