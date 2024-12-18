using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_UItest : MonoBehaviour
{
    public void TestUIClick()
    {
        Renderer renderer = GetComponent<Renderer>();
        Debug.Log("UIClick");
        if (renderer != null)
        {
            Color randomColor = new Color(
                  Random.value,   // Red component
                  Random.value,   // Green component
                  Random.value    // Blue component
              );
            renderer.material.color = randomColor;
        }
    }
}
