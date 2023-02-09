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

        if (Input.GetKey(KeyCode.K)) {
            // sent from client to server
            TestServerRpc("Hello to server!");
        }

        if (Input.GetKey(KeyCode.T)) {
            // sent from server to client
            TestClientRpc("Hello to client!");

            // the specific client IDs the RPCs will be sent to
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new ulong[]{1} // <---- the client IDs are specified here
                }
            };

            // sends RPC to specific clients by their IDs
            TestSpecificClientRpc("Hello to specific client!", clientRpcParams);
        }

        // basic player movement
        Vector3 move = new Vector3(0, 0 ,0);       

        if (Input.GetKey(KeyCode.W)) move.z = 1f;
        if (Input.GetKey(KeyCode.S)) move.z = -1f;
        if (Input.GetKey(KeyCode.A)) move.x = 1f;
        if (Input.GetKey(KeyCode.D)) move.x = -1f;

        transform.position += move * movementSpeed * Time.deltaTime;
    }
    
    // RPCs are another way to sync data other than NetworkVariables
    // RPCs must be defined in a NetworkBehavior
    // you can only pass value type variables and strings through RPCs (strings are the only reference type variables that work with RPCs)

    // the ServerRPC is called from a client and is sent to the server (action is performed on server)
    // you can pass parameters to change things like invocation rights [ServerRpc(RequireOwnership = false)]
    // ServerRpcParams serverRpcParams parameter lets you reference things such as the client's ID
    [ServerRpc]
    private void TestServerRpc(string message, ServerRpcParams serverRpcParams = default) {
        Debug.Log("Message from client (client ID: " + serverRpcParams.Receive.SenderClientId + " ): " + message);
    }

    // the ClientRPC is called by the server and is sent to the client(s) (action is perfromed by the client(s))
    [ClientRpc]
    private void TestClientRpc(string message) {
        Debug.Log("Message from server: " + message);
    }

    // you can send a client RPC to a specific client by specifying the IDs through the ClientRpcParams
    [ClientRpc]
    private void TestSpecificClientRpc(string message, ClientRpcParams clientRpcParams = default) {
        Debug.Log("Message from server: " + message);
    }
}
