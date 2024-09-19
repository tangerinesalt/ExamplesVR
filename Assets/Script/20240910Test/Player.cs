using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerMovement))]
    public class Player : MonoBehaviour
    {
        PlayerInput playerInput;
        // Start is called before the first frame update
        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }

        public PlayerInput GetPlayerInput()
        {
            return playerInput;
        }

    }
}
