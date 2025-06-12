/****************************************************
    功能：帧数显示类
    作者：ZH
    创建日期：#2025/01/10#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    /// <summary>
    /// 固定的一个时间间隔
    /// </summary>
    private float time_delta = 0.5f;
    /// <summary>
    /// 上一次统计FPS的时间
    /// </summary>
    private float prev_time = 0.0f;
    /// <summary>
    /// 计算出来的FPS的值
    /// </summary>
    private float fps = 0.0f;
    /// <summary>
    /// 累计我们刷新的帧数
    /// </summary>
    private int i_frames = 0;

    /// <summary>
    /// GUI显示
    /// </summary>
    private GUIStyle style;

    void Awake()
    {
        // 假设CPU 100% 工作的状态下FPS 300，
        // 当你设置了这个以后，他就维持在90FPS左右，不会继续冲高;
        // -1, 游戏引擎就会不段的刷新我们的画面，有多高，刷多高; 
        Application.targetFrameRate = 300;
    }

    void Start()
    {
        prev_time = Time.realtimeSinceStartup;//指的是我们当前从启动开始到现在运行的时间，单位(s)
        style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = new Color(255, 255, 255);
    }

    void Update()
    {
        i_frames++;

        if (Time.realtimeSinceStartup >= prev_time + time_delta)
        {
            fps = ((float)i_frames) / (Time.realtimeSinceStartup - prev_time);
            prev_time = Time.realtimeSinceStartup;
            i_frames = 0; //重新累积我们的FPS
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 20, 200, 200), "FPS:" + fps.ToString("f2"), style);
    }
}