using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMoveNoneVR : MonoBehaviour
{

    private Animator animator;
    [SerializeField] private GameObject m_gameobjectPrefab;
    [SerializeField] private bool m_allowSpawning = false;
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
        Horizontal = Input.GetAxisRaw("Horizontal");
        //Debug.Log("horizontal: " + Horizontal);
        Vertical = Input.GetAxisRaw("Vertical");
        Vector3 pos = this.transform.position;
        pos.x -= -(Horizontal * Time.deltaTime) * moveSpeed;
        pos.z -= -(Vertical * Time.deltaTime) * moveSpeed;
        this.transform.position = pos;

        if (Horizontal != 0 || Vertical != 0) 
        {
            //Debug.Log("H: " + Horizontal + " V: " + Vertical);
        }
    }

}
