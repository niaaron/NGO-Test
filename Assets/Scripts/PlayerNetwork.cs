using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3f;

    // network variable is synced across server/host and client
    // passed perms that allow 
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // runs when an owned NetworkVariable gets updated
    public override void OnNetworkSpawn() {
        // onValueChange delegation gets run everytime the NetworkVariable is updated
        // delegation requires two parameters (previousValue and newValue parameters get passed into function)
        randomNumber.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log("previousValue: " + previousValue + " | newValue: " + newValue);
            Debug.Log("OwnerClientId: " + OwnerClientId + " | randomNumber: " + randomNumber.Value);
        };
        
    }

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

        // generates a rnadom number to be stored in NetworkVariable
        if (Input.GetKeyDown(KeyCode.T)) {
            randomNumber.Value = Random.Range(0,100);
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
