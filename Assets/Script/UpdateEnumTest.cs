using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tangerine
{
    public enum MoveMethodTest
    {
        NoneMove = 0,
        MoveByCommand = 1,
        MoveByRigidbody = 2
    }
    public class UpdateEnumTest : MonoBehaviour
    {

       [SerializeField] private MoveMethod m_moveMethod;
        public bool m_isMoving = false;
        // Start is called before the first frame update

        public void ChangeMoveMethod(MoveMethod moveMethod)
        {
            m_moveMethod = moveMethod;
        }
        public void ChangeMoveState(bool isMoving)
        {
            m_isMoving = isMoving;
        }
        // Update is called once per frame
        void Update()
        {
            //Debug.Log("MoveMethod is :" + m_moveMethod);
            
        }
    }
}
