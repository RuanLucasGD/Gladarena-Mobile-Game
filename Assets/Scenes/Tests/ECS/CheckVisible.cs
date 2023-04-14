using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckVisible : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent OnVisible;
    public UnityEngine.Events.UnityEvent OnInvisible;

    public bool isVisible;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnBecameVisible()
    {
        OnVisible.Invoke();
        isVisible = true;
    }

    void OnBecameInvisible()
    {
        OnInvisible.Invoke();
        isVisible = false;
    }
}
