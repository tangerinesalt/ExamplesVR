using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XR_GripInteractiveObj : MonoBehaviour
{
    private XR_Grip m_Grip;
    private bool isGrip;
    protected Transform _followedTrans = null;
    protected Vector3 _selfLocalPos = Vector3.zero;
    protected Vector3 _selfLocalForward = Vector3.zero;
    protected Vector3 _selfLocalUpwards = Vector3.zero;
     [SerializeField]protected Transform m_RootTrans = null;
    // Start is called before the first frame update
    void Start()
    {
        if (m_RootTrans == null)
        {
            m_RootTrans = transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isGrip)
        {
            FollowsGripPoint();
        }
    }
   
    void InitFollow()
    {
        _followedTrans = m_Grip.m_GripPoint;//获取握持点位置
        _selfLocalPos = _followedTrans.InverseTransformPoint(m_RootTrans.position);//计算当前对象相对于握持点的局部位置
        _selfLocalForward = _followedTrans.InverseTransformDirection(m_RootTrans.forward);//计算当前对象相对于握持点的局部前向量
        _selfLocalUpwards = _followedTrans.InverseTransformDirection(m_RootTrans.up);//计算当前对象相对于握持点的局部上向量
    }
    void StopFollow()
    {
        _followedTrans = null;
        _selfLocalPos = Vector3.zero;
        _selfLocalForward = Vector3.zero;
        _selfLocalUpwards = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        // if(m_Grip == null)
        // {
        //     m_Grip = other.GetComponent<XR_Grip>();
        // }
    }
    private void OnTriggerExit(Collider other)
    {
         if(m_Grip == null)
         {
            isGrip = false;
         }
    }
    public void GripStart(XR_Grip XrGripper)
    {
        //换手
        XR_Grip oldXrGripper = m_Grip;
        m_Grip = XrGripper;
        InitFollow();//初始化相对握持点的局部位置、方向

        if (oldXrGripper!=null&&oldXrGripper!=XrGripper)
        {
            oldXrGripper.GiveUpGrabbing(this);
        }

        isGrip = true;

    }

    public void GripEnd(XR_Grip XrGripper)
    {
        if (m_Grip == XrGripper)
        {
            isGrip = false;
            StopFollow();
            Debug.Log("GripEnd");
        }
    }
    public void FollowsGripPoint()
    {
        m_RootTrans.position = _followedTrans.TransformPoint(_selfLocalPos);// 更新当前对象的位置，使其保持相对于握持点的局部位置
        m_RootTrans.rotation = Quaternion.LookRotation
        (
            _followedTrans.TransformDirection(_selfLocalForward),
            _followedTrans.TransformDirection(_selfLocalUpwards)
        ); // 更新当前对象的旋转，使其朝向相对于握持点的局部方向
    }
}
