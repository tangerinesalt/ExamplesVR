using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAndTriggerTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
         if(other.transform.tag == "Respawn")
        Debug.Log($"{other.name} enter {gameObject.name} trigger");
    }
}
