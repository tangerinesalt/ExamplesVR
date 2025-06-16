using System.Collections;
using System.Collections.Generic;
using Tangerine;
using UnityEngine;

public class AuxUpdateEnum : MonoBehaviour
{
    public bool m_ChangeMoveMethodTest;
    private UpdateEnumTest m_updateEnumTest;
    private VRMovement_Tangerinesalt m_vrMovementBase;
    private bool m_bool01=true;
    // public bool ChangeMoveMethodTest
    // {
    //     get => m_ChangeMoveMethodTest;
    //     set => m_ChangeMoveMethodTest=value;
    // }
    
    // Start is called before the first frame update
    void Start()
    {
        m_updateEnumTest =this.GetComponent<UpdateEnumTest>();
        m_vrMovementBase = this.GetComponent<VRMovement_Tangerinesalt>();
    }
    // Update is called once per frame
    void Update()
    {
        if (m_updateEnumTest != null)
        {
            if (m_ChangeMoveMethodTest&&m_bool01)
            {
                //m_updateEnumTest.ChangeMoveMethod(MoveMethod.MoveByCommand);
                changeMoveMethodTest(MoveMethod.MoveByCommand);
                m_updateEnumTest.ChangeMoveState(true);
                m_bool01 = false;
            }
            else if (!m_ChangeMoveMethodTest&&!m_bool01)
            {
                //m_updateEnumTest.ChangeMoveMethod(MoveMethod.MoveByRigidbody);
                changeMoveMethodTest(MoveMethod.MoveByRigidbody);
                m_updateEnumTest.ChangeMoveState(false);
                m_bool01 = true;
            }
        }
        if(m_vrMovementBase!= null)
        {
            if (m_ChangeMoveMethodTest && m_bool01)
            {
                m_vrMovementBase.ChangeMoveMethod(MoveMethod.MoveByCommand);
               m_vrMovementBase.ChangeMoveStatus(true);
                m_bool01 = false;
            }
            else if (!m_ChangeMoveMethodTest&&!m_bool01)
            {
                m_vrMovementBase.ChangeMoveMethod(MoveMethod.MoveByRigidbody);
                m_vrMovementBase.ChangeMoveStatus(false);
                m_bool01 = true;
            }
        }
    }
    private void changeMoveMethodTest(MoveMethod moveMethod)
    {
        m_updateEnumTest.ChangeMoveMethod(moveMethod);
    }
}
