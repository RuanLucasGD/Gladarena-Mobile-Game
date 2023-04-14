using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public Rigidbody rb;
    public CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //characterController.Move(Vector3.down * Time.deltaTime * 10);
        rb.velocity = Vector3.forward * Time.fixedDeltaTime * 10;
    }
}
