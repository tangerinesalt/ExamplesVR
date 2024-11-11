using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDraggerWithPosition : MonoBehaviour
{
    public Rigidbody rb;
    public float dragForceMagnitude = 10f;
    private Vector3 screenPoint;
	private Vector3 offset;
    public Camera _camera;    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(_camera == null) _camera = Camera.main;
        
    }
    void OnMouseDown()
	{
		screenPoint = _camera.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position -  _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	
	void OnMouseDrag()
	{
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
         Vector3 newPosition =  _camera.ScreenToWorldPoint(curScreenPoint) + offset;

         transform.position = newPosition;
	}
}
