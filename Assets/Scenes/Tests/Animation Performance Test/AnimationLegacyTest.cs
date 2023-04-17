using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLegacyTest : MonoBehaviour
{
    public Animation Animation;

    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip Attack;

    private int currentId;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentId++;

            if (currentId > 2)
            {
                currentId = 0;
            }

            var _anim = Idle;
            if (currentId == 1) _anim = Run;
            if (currentId == 2) _anim = Attack;
            Animation.clip = _anim;
        }
    }
}
