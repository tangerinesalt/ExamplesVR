using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

public class GrabbableAdsorptionTest : MonoBehaviour
{
    public Transform m_target;
    public float moveSpeed = 1f;
    public float stopDistance = 0.05f;
    private Rigidbody rb;
    private Coroutine goTargetCoroutine;
    //unity曲线
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0,0,0,0),new Keyframe(0.5f,0.5f,2,2), new Keyframe(1,1,0,0));

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("需要挂载Rigidbody组件");
        this.GetComponent<Grabbable>().onGrab.AddListener(OnGrab);
        this.GetComponent<Grabbable>().onRelease.AddListener(OnRelease);
    }

    void Update()
    {
        //测试——按空格键吸附至目标点
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveTarget(m_target);
        }
    }
    ///<summary> 吸附至默认目标点 </summary>
    public void MoveTarget(Transform target)
    {
        if (goTargetCoroutine == null)
            goTargetCoroutine = StartCoroutine(GoTargetTF(target));
    }

    void OnDisable()
    {
        if (goTargetCoroutine != null)
        {
            StopCoroutine(goTargetCoroutine);
            goTargetCoroutine = null;
        }
    }
    private void OnGrab(Hand hand, Grabbable grabbable)
    {
        Debug.Log("抓取");
        rb.isKinematic = false;
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {
        Debug.Log("放下");
        rb.isKinematic = false;
    }

    IEnumerator GoTargetTF(Transform target)
    {
        yield return new WaitForEndOfFrame();
        if (target == null)
        {
            Debug.LogError("目标为空");
            yield break;
        }
        Debug.Log("开始吸附");
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;
        float StartDistance = Vector3.Distance(transform.position, target.position);
        float Timer = 0;
        while (Vector3.Distance(transform.position, target.position) > stopDistance)
        {
            float CurDistance = Vector3.Distance(transform.position, target.position);
            float progress = (StartDistance - CurDistance) / StartDistance;
            Debug.Log($"吸附进度:{progress:F2}".FontColoring(Color.green));

            Timer += Time.deltaTime;
            float t = curve.Evaluate(moveSpeed * Timer);
            rb.isKinematic = true;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, t));
            yield return new WaitForEndOfFrame();
        }

        if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            rb.isKinematic = true;
            rb.Sleep();
            rb.MovePosition(target.position);
            rb.transform.position = target.position;
            rb.transform.rotation = target.rotation;
            Debug.Log($"到达目标点".FontColoring(Color.green));
        }
        goTargetCoroutine = null;

        yield return null;
    }
}