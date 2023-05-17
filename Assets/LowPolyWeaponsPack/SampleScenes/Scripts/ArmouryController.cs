using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmouryController : MonoBehaviour
{
    public Camera MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * 10 * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * 10 * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * 10 * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * 10 * Time.deltaTime);


        if (Input.GetAxis("Mouse X")<-0.2)
            transform.Rotate(Vector3.down * 250 * Time.deltaTime);
        if (Input.GetAxis("Mouse X") > 0.2)
            transform.Rotate(Vector3.up * 250 * Time.deltaTime);
        if (Input.GetAxis("Mouse Y") < -0.5)
            MainCamera.transform.Rotate(Vector3.left * 150 * Time.deltaTime);
        if (Input.GetAxis("Mouse Y") > 0.5)
            MainCamera.transform.Rotate(Vector3.right * 150 * Time.deltaTime);

    }
}
