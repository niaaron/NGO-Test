using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        // only allows gameobject's client to control character
        // IsOwner extended from NetworkBehaviour
        if (!IsOwner) {
            return;
        }

        // basic player movement
        Vector3 move = new Vector3(0, 0 ,0);       

        if (Input.GetKey(KeyCode.W)) move.z = 1f;
        if (Input.GetKey(KeyCode.S)) move.z = -1f;
        if (Input.GetKey(KeyCode.A)) move.x = 1f;
        if (Input.GetKey(KeyCode.D)) move.x = -1f;

        transform.position += move * movementSpeed * Time.deltaTime;
    }
}
