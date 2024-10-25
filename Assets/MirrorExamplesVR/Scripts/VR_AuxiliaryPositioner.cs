using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class VR_AuxiliaryPositioner : MonoBehaviour
{
    private Vector3 _TeleportationLocation = Vector3.zero;
    private bool OpenLocationRecord = true;
    //传送点生成测试
    public GameObject TestObject;
    public bool m_AutoGenerateTeleportationPos = false;

    void Start()
    {
        _TeleportationLocation = this.transform.position;
        //OnchangeSence();
        if (m_AutoGenerateTeleportationPos) StartCoroutine(AutoGenerateTeleportationPos());
        //EventManager.Instance.onChangeSence += OnchangeSence;
        //EventManager.Instance.OnChangeSence();

    }
    void OnDestroy()
    {
        //Debug.Log("Destroy VR_AuxiliaryPositioner");
        EventManager.Instance.onChangeSence -= OnchangeSence;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (OpenLocationRecord)
        {
            OpenLocationRecord = false;
            _TeleportationLocation = this.transform.position;
            _TeleportationLocation.y = 0;
            //Debug.Log("Teleportation Location: " + _TeleportationLocation.x + "," + _TeleportationLocation.y + "," + _TeleportationLocation.z);
            Debug.Log("Trigger Enter For VR_AuxiliaryPositioner");
            //Debug.Log($"{other.gameObject.name} entered VR_AuxiliaryPositioner");

            SignInTeleportPos();
        }
    }
    //传送点显示测试
    private void SignInTeleportPos()
    {
        if (TestObject != null)
        {
            GameObject go = Instantiate(TestObject, _TeleportationLocation, Quaternion.identity);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (OpenLocationRecord)
        {
            OpenLocationRecord = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!OpenLocationRecord)
        {
            OpenLocationRecord = true;
            Debug.Log("Trigger Exit For VR_AuxiliaryPositioner");
        }

    }
    public Vector3 GetTeleportationLocation()
    {
        return _TeleportationLocation;
    }
    public void SetTeleportationPointInPlace()
    {
        _TeleportationLocation = this.transform.position;
        _TeleportationLocation.y = 0;
    }
    //（测试用）隔一段时间自动生成传送点-通过协程实现
    private IEnumerator AutoGenerateTeleportationPos()
    {
        yield return new WaitForSeconds(0.5f);
        if (OpenLocationRecord)
        {
            _TeleportationLocation = this.transform.position;
            _TeleportationLocation.y = 0;
            SignInTeleportPos();
        }
        yield return new WaitForSeconds(9.5f);
        StartCoroutine(AutoGenerateTeleportationPos());
    }
    //（测试用）切换场景时更新传送点-使用事件系统（自建）实现
    private void OnchangeSence()
    {
        _TeleportationLocation = this.transform.position;
        _TeleportationLocation.y = 0;
        SignInTeleportPos();
        Debug.Log("OnChangeSence for VR_AuxiliaryPositioner");
    }
}
