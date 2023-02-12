using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3f;

    // the spawnable prefab
    [SerializeField] private Transform spawnableObjectPrefab;
    // reference to the instantiated object of spawnableObjectPrefab
    private Transform spawnedObjectTransform;
    
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

        // keys for spawning and deleting
        // spawning object
        if (Input.GetKey(KeyCode.P)) {
            // objects can only be spawned directly on the server. if a client wants to spawn something, the client will
            // have to use a ServerRPC to tell the server to spawn the object

            // spawns/instantiates a prefab locally (just like how you would normally) and stores the reference to the spawned object
            // only shows up locally on host/server
            spawnedObjectTransform = Instantiate(spawnableObjectPrefab);

            // this spawns the object on the network and lets others see the object
            // the Spawn method takes a boolean parameter of whether or not the object should be destroyed if the scene is closed or changed
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

        }

        // destroying object
        if (Input.GetKey(KeyCode.L)) {
            // an alternative to destroying is despawning
            // despawning will remove the object from the network, but it will still be active
            // the Despawn method takes a boolean parameter of whether or not the object should be destroyed also if despawned (pretty much same as Destroy())
            //spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(false);

            // destroying a network object is just like normal, just call Destroy()
            Destroy(spawnedObjectTransform.gameObject);

            // additionally, the NetworkObject component has the attribute "Dont Destroy With Owner" which doesn't delete the object even if the owner is deleted/quits
        }
    }
}
