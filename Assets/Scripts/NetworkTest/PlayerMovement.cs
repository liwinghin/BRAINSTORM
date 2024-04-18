using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3 (h, 0, v);
        movementDirection.Normalize();

        transform.Translate(movementDirection * 7f * Time.deltaTime, Space.World);

        if(movementDirection != Vector3.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, 500f * Time.deltaTime);
        }
    }
}
