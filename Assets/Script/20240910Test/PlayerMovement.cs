using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Player player;
        private Animator animator;
        [SerializeField] float moveSpeed;
        // Start is called before the first frame update
        void Start()
        {
            player = GetComponent<Player>();
            animator = GetComponent<Animator>();
        }
    
        // Update is called once per frame
        void Update()
        {
            float horizontal = player.GetPlayerInput().GetHorizontal();
            float vertical = player.GetPlayerInput().GetVertical();

            Vector3 pos = this.transform.position;
            pos.x += -(horizontal * Time.deltaTime)*moveSpeed;
            pos.z += -(vertical * Time.deltaTime) * moveSpeed;
            this.transform.position = pos;

            if (horizontal != 0 || vertical != 0) animator.SetBool("is_walking", true);
            else animator.SetBool("is_walking", false);

        }
    }
}
