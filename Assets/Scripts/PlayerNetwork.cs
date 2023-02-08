using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour {
    [SerializeField] private float movementSpeed = 3f;

    // network variable is synced across server/host and client
    // normally, clients do not have perms to modify NetworkVariable. passed perms that allow client to also make changes
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // here we pass custom struct into NetworkVariable
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 56,
            _bool = true
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    // NetworkVariables cannot hold reference variable types, only value types
    // here we make a custom data type using a struct that implments INetworkSerializable so that Unity knows how to serialize our custom struct
    public struct MyCustomData : INetworkSerializable {
        public int _int;
        public bool _bool;

        // implemented from INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
        }
    }
    
    // runs when an owned NetworkVariable gets updated
    public override void OnNetworkSpawn() {
        // onValueChange delegation gets run everytime the NetworkVariable is updated
        // delegation requires two parameters (previousValue and newValue parameters get passed into function)
        // here we are refering to the custom type created from struct and accessing the int from the struct
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log("previousValue (_int): " + previousValue._int + " | newValue: (_int)" + newValue._int);
            Debug.Log("previousValue (_bool): " + previousValue._bool + " | newValue: (_bool)" + newValue._bool);
            //Debug.Log("OwnerClientId: " + OwnerClientId + " | randomNumber: " + randomNumber.Value);
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

        // generates a rnadom number to be stored in struct, which in turn is stored as a NetworkVariable
        if (Input.GetKeyDown(KeyCode.T)) {
            randomNumber.Value = new MyCustomData {
                _int = Random.Range(0,100),
                _bool = false
            };
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
