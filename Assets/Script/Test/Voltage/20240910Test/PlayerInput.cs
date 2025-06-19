using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        float Horizontal;
        float Vertical;
        // Start is called before the first frame update
        void Start()
        {
            Horizontal = 0;
            Vertical = 0;
        }

        // Update is called once per frame
        void Update()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
            Vertical = Input.GetAxisRaw("Vertical");
        }

        public float GetHorizontal()
        {
            return Horizontal;
        }
        public float GetVertical()
        {
            return Vertical;
        }
    }
}