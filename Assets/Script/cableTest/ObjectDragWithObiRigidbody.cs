using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class ObjectDragWithObiRigidbody : MonoBehaviour
{
    public float stiffness = 200;
    public float damping = 20;
    public float maxAccel = 50;
    public float minDistance = 0.05f;
    public Rigidbody rb { get; private set; }
    public ObiRigidbody orb { get; private set; }
    private Vector3 followPosition;
    private Vector3 relativeposition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        orb = GetComponent<ObiRigidbody>();
    }
    public float MoveTowards(Vector3 _destination)
    {
        //Debug.Log($"{this.name} MoveTowards",this);
        Vector3 vector = _destination - transform.position;
        float distance = Vector3.Magnitude(vector);

        // simple damped spring: F = -kx - vu
        Vector3 accel = stiffness * vector - damping * rb.velocity;

        // clamp spring acceleration:
        accel = Vector3.ClampMagnitude(accel, maxAccel);

        rb.AddForce(accel, ForceMode.Acceleration);
        
        //rb.Move(_destination, rb.transform.rotation);//运动学移动

        return distance;
    }
   public  void OnDragStart()
    {
        rb.isKinematic = false;
    }

    public  void OnDragEnd()
    {
        rb.isKinematic = true;
        if (orb.kinematicForParticles) orb.kinematicForParticles = false;
    }
    void InitFollowPosition(Vector3 interactablePosition)
    {
        followPosition = interactablePosition;
        relativeposition  = Subtract(interactablePosition, transform.position);
    }
    private  Vector3 Subtract(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

}
