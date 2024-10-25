using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRetentionTool : MonoBehaviour
{
    void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
}
