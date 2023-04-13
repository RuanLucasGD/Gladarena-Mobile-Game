using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SetRandomPos), 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetRandomPos()
    {
        transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }
}
