using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    //控制变量
    [Header("动画属性")]
    public Animator animator = null;
    public float m_speed = 0.1f;
    public String m_animationName = "CubeTestAni";

    [Header("动画控制")]
    [Range(0, 1)] public float StopPoint = 0.21f; // 动画停止点

    //运行变量
    private bool playState = false;
    private float AniProgress = 0;
    private bool isStop = false;
    private Coroutine playEndCheck =null;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) Debug.LogError("没有找到Animator组件！");
        }
        animator.speed = m_speed;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!playState)
                ChangeAnimationState(m_animationName, true);
            else
                ChangeAnimationState(m_animationName, false);
        }
    }
    private IEnumerator CheckPlayEnd()
    {
        yield return null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (animator != null && stateInfo.IsName(m_animationName) && AniProgress < 1)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1;
            if (normalizedTime < 0.99f)
                AniProgress = normalizedTime;
            else
                AniProgress = 1;

            if (AniProgress >= StopPoint && !isStop)
            {
                ChangeAnimationState(m_animationName, false);
                isStop = true;
            }

            yield return null;
        }
    }
    /// <summary>
    /// 设置动画播放状态
    /// </summary>
    /// <param name="isPlay"></param>
    private void ChangeAnimationState(string animName, bool isPlay)
    {
        if (isPlay)
        {
            playState = true;
            animator.enabled = true;
            animator.Play(animName);
            if (playEndCheck == null)
                playEndCheck = StartCoroutine(CheckPlayEnd());
        }
        else
        {
            playState = false;
            animator.enabled = false;
        }
    }
    /// <summary>
    /// 设置动画播放速度
    /// </summary>
    /// <param name="speed"></param>
    public void SetSpeed(float speed)
    {
        animator.speed = speed;
        m_speed = speed;
    }
}
