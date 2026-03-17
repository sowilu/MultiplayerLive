using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 6;
    public float rotationSpeed = 150;
    public int maxHealth = 100;
    
    private CharacterController cc;
    private NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    
    
    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += OnHealthChanged;
        
        //random spawn
        if(IsServer) {
            transform.position = new Vector3(
            Random.Range(-10, 10), 1, Random.Range(-10, 10));
        }
    }

    public void OnDestroy()
    {
        health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int previousvalue, int newvalue)
    {
        //TODO: health bar
        print("Health: " + previousvalue + ", " + newvalue);
    }


    void Update()
    {
        if(!IsOwner) return;
        
        //movement
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        
       MoveServerRpc(h, v);
    }

    [ServerRpc]
    void MoveServerRpc(float h, float v)
    {
        var move = (transform.right * h + transform.forward * v).normalized;
        cc.SimpleMove(move * moveSpeed);
    }
}
