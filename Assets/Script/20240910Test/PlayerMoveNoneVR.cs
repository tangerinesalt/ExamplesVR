using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMoveNoneVR : NetworkBehaviour
{

    private Animator animator;
    [SerializeField] private GameObject m_gameobjectPrefab;
    float Horizontal;
    float Vertical;
    [SerializeField] float moveSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        //player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!isLocalPlayer) return;
        Horizontal = Input.GetAxisRaw("Horizontal");
        //Debug.Log("horizontal: " + Horizontal);
        Vertical = Input.GetAxisRaw("Vertical");
        Vector3 pos = this.transform.position;
        pos.x -= -(Horizontal * Time.deltaTime) * moveSpeed;
        pos.z -= -(Vertical * Time.deltaTime) * moveSpeed;
        this.transform.position = pos;

        if (Horizontal != 0 || Vertical != 0) animator.SetBool("is_walking", true);
        else animator.SetBool("is_walking", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cmd_Spawn();
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer) return;
       //Debug.Log("OnCollisionEnter: " + other.gameObject.name);
        CmdSpawnBytrigger(other.gameObject);
        //Cmd_Spawn();
        
    }
    // void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("OnTriggerEnter: "+other.gameObject.name);
    //     try
    //     {
    //         CmdSpawnBytrigger(other.gameObject);
    //     }
    //     catch (Exception e)
    //     {

    //         Debug.Log(e.Message);
    //     }
    // }
    [Command]//服务器执行
    void Cmd_Spawn()
    {
        Rpc_Spawn();
    }
    [Command]//客户端调用，服务端执行
    void CmdSpawnBytrigger(GameObject target)
    {
        if(target == null)return;
        //Debug.Log(target.name);
        try
        {
            NetworkIdentity targetIdentity =  target.GetComponent<NetworkIdentity>();
            if (targetIdentity == null) targetIdentity =  target.GetComponentInChildren<NetworkIdentity>();
            if (targetIdentity == null)
            {
                Debug.Log("targetIdentity is null");
                return;
            }
            if (targetIdentity.connectionToClient == connectionToClient) return;
            Target_RpcSpawn(targetIdentity.connectionToClient,  target.transform.position, target.transform.rotation);
        }
        catch (Exception e)
        {

            Debug.Log(e.Message);
        }
        
    }

    [ClientRpc]//服务端调用，客户端执行
    void Rpc_Spawn()
    {
        GameObject gameObject = Instantiate(m_gameobjectPrefab, this.transform.position, this.transform.rotation);
        //NetworkServer.Spawn(gameObject);
    }

    [TargetRpc]//服务端调用，特定客户端执行，其他客户端不执行
    void Target_RpcSpawn(NetworkConnection target, Vector3 position,Quaternion rotation)
    {
        //Debug.Log("Target_RpcSpawn: " + target.connectionId+" ; And form gameobject: "+this.gameObject.name);
        Debug.Log("Target_RpcSpawn from: " + this.gameObject.name );
        //GameObject targetGameObject = target.identity.gameObject;
        GameObject gameObject = Instantiate(m_gameobjectPrefab, position, rotation);
        //NetworkServer.Spawn(gameObject);

    }

}
