using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectDragControl : MonoBehaviour
{
    public Camera _camera;
    private ObjectDragWithObiRigidbody SelectedDragObj;
    private Vector3 screenPoint;//物体的屏幕坐标
    private Vector3 offset;//输入点到物体空间坐标的向量
    private void Start()
    {
        if (_camera == null) _camera = Camera.main;
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        //记录相对位置
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent(out ObjectDragWithObiRigidbody DragObj))
                {
                    if (DragObj != null)
                    {
                        SelectedDragObj = DragObj;
                        DragObj.OnDragStart();
                        //初始位置计算
                        screenPoint = _camera.WorldToScreenPoint(hit.transform.position);
                        offset = hit.transform.position - _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                    }
                }
            }
        }
        //获取物体位移坐标
        if (SelectedDragObj != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 ObjMovingVector = _camera.ScreenToWorldPoint(curScreenPoint) + offset;
            SelectedDragObj.MoveTowards(ObjMovingVector);
        }
        if (SelectedDragObj != null && Input.GetMouseButtonUp(0)&&SelectedDragObj != null)
        {
            SelectedDragObj.OnDragEnd();
            SelectedDragObj = null;
            return;
        }

    }
}
