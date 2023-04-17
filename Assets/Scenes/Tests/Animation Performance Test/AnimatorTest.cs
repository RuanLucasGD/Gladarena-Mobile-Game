using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : MonoBehaviour
{
    public Animator Animator;

    private int currentId;

    void Start()
    {
        Animator.fireEvents = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentId++;

            if (currentId > 2)
            {
                currentId = 0;

            }

            
        }

        Animator.SetInteger("ID", currentId);
    }
}
