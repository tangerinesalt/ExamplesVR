using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Text;

public static class ExpandTool
{
    #region GameObject
    /// <summary>
    /// 增加或获取组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go)
        where T : Component
    {
        T temp_T = null;
        if (go.GetComponent<T>() == null)
        {
            temp_T = go.AddComponent<T>();
        }
        else
        {
            temp_T = go.GetComponent<T>();
        }

        return temp_T;
    }

    public static Component GetOrAddComponent(this GameObject go, Type type)
    {
        Component temp_T = null;
        if (go.GetComponent(type) == null)
        {
            temp_T = go.AddComponent(type);
        }
        else
        {
            temp_T = go.GetComponent(type);
        }

        return temp_T;
    }

    /// <summary>
    /// 根据名字和挂在的组件查找子物体，同名的返回第一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T SearchComponent<T>(this GameObject gameObject, string name) where T : Component
    {
        List<T> ts = gameObject.GetComponentsInChildren<T>().ToList();
        if (ts != null && ts.Count > 0)
        {
            for (int i = 0; i < ts.Count; i++)
            {
                if (ts[i].name.Equals(name))
                {
                    return ts[i];
                }
            }
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>{gameObject.name}物体下没有找到对应名字{name}拥有{typeof(T)}的物体</color>");
        }

        return null;
    }
    /// <summary>
    /// 获取这个物体在Hierarchy中的路径
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static string GameObjectPath(this GameObject gameObject)
    {
        var path = "/" + gameObject.name;
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
            path = "/" + gameObject.name + path;
        }

        Debug.Log($"<color=#eb6ea5>{gameObject.name}物体的路径：{path}</color>");

        return path;
    }
    /// <summary>
    /// EventTrigger，对应拖拽、点击、鼠标在物体上等回调，用起来比较方便，
    /// 注意：场景中要有EventSystem，如果是3D物体，则还需要给Camera加上Physics Raycaster组件
    /// </summary>
    public static void AddEventTrigger(this GameObject obj, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(callback);
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        trigger.triggers.Add(entry);
    }
    #endregion

    #region Transform
    /// <summary>
    /// 根据名字查找子物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChildByName(this Transform transform, string name)
    {
        List<Transform> list = transform.GetComponentsInChildren<Transform>(true).ToList();
        if (list != null && list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name.Equals(name))
                {
                    return list[i];
                }
            }
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>{transform.name}没有子物体</color>");
        }

        return null;
    }

    public static List<Transform> FindChildrenByName(this Transform transform, string name)
    {
        List<Transform> temp = new List<Transform>();
        List<Transform> list = transform.GetComponentsInChildren<Transform>(true).ToList();
        if (list != null && list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name.Contains(name))
                {
                    temp.Add(list[i]);
                }
            }

            return temp;
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>{transform.name}没有子物体</color>");
        }

        return null;
    }
    public static Transform FindByPath(this Transform target, string path)
    {
        return target.Find(path);
    }

    public static List<T> FindChilds<T>(this Transform origin, bool includeInactive = true) where T : Component
    {
        List<T> list = new List<T>();
        origin.GetComponentsInChildren<T>(includeInactive, list);
        return list;
    }
    /// <summary>
    /// 清除子物体
    /// </summary>
    /// <param name="transform"></param>
    public static void ClearChild(this Transform transform)
    {
        int count = transform.childCount;
        if (count > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>{transform.name}没有子物体</color>");
        }
    }
    public static void ActiveMeshAndChilds(this Transform target, bool enable)
    {
        List<MeshRenderer> list = new List<MeshRenderer>();
        target.GetComponentsInChildren<MeshRenderer>(true, list);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].enabled = enable;
        }
    }
    /// <summary>
    /// v3转四元数
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public static Quaternion V3ToQua(this Vector3 v3)
    {
        if (v3 == Vector3.zero)
        {
            return Quaternion.identity;
        }
        return Quaternion.LookRotation(v3);
    }

    /// <summary>
    /// Vector3转Vector2
    /// </summary>
    public static Vector2 V3ToV2(this Vector3 v3)
    {
        int x = Mathf.RoundToInt(v3.x);
        int y = Mathf.RoundToInt(v3.y);

        return new Vector2(x, y);
    }

    /// <summary>
    /// 获取到面板上对应的Rotation数值
    /// </summary>
    /// <param name="transform">物体变换</param>
    /// <returns></returns>
    public static Vector3 GetInspectorRotationValueMethod(this Transform transform)
    {
        // 获取原生值
        Type transformType = transform.GetType();
        PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
        MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
        string temp = value.ToString();
        //将字符串第一个和最后一个去掉
        temp = temp.Remove(0, 1);
        temp = temp.Remove(temp.Length - 1, 1);
        //用‘，’号分割
        string[] tempVector3;
        tempVector3 = temp.Split(',');
        //将分割好的数据传给Vector3
        Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
        return vector3;
    }

    /// <summary>
    /// 根据物体朝向，返回这个物体与水平面夹角（-90~90°）
    /// </summary>
    /// <param name="toward">物体朝向</param>
    /// <returns></returns>
    public static float ReturnAngleBaseToward(this Vector3 toward)
    {
        return (Mathf.Atan(toward.y / Mathf.Pow((toward.x * toward.x + toward.z * toward.z), 0.5f)) * 180 / Mathf.PI);
    }

    /// <summary>
    /// 返回物体朝向水平面分量与世界坐标X轴正方向夹角（-180~180）
    /// </summary>
    /// <param name="toward">朝向任一向量</param>
    /// <returns></returns>
    public static float ReturnLevelRadian(this Vector3 toward)
    {
        float tempR = 0;

        if (toward.x < 0 && toward.z < 0)
        {
            tempR = Mathf.Atan(toward.z / toward.x) - Mathf.PI;
        }
        else if (toward.x == 0 && toward.z < 0)
        {
            tempR = -0.5f * Mathf.PI;
        }
        else if (toward.x > 0 && toward.z < 0)
        {
            tempR = Mathf.Atan(toward.z / toward.x);
        }
        else if (toward.x > 0 && toward.z == 0)
        {
            tempR = 0;
        }
        else if (toward.x > 0 && toward.z > 0)
        {
            tempR = Mathf.Atan(toward.z / toward.x);
        }
        else if (toward.x == 0 && toward.z > 0)
        {
            tempR = 0.5f * Mathf.PI;
        }
        else if (toward.x < 0 && toward.z > 0)
        {
            tempR = Mathf.Atan(toward.z / toward.x) + Mathf.PI;
        }
        else if (toward.x < 0 && toward.z == 0)
        {
            tempR = Mathf.PI;
        }

        return tempR / Mathf.PI * 180;
    }

    /// <summary>
    /// 读取xml
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="fileFullPath">xml路径</param>
    /// <returns></returns>
    public static T DeserilizeXML<T>(string fileFullPath)
    {
        using (StreamReader sr = File.OpenText(fileFullPath))
        {
            string dataStr = sr.ReadToEnd();

            XmlSerializer xs = new XmlSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(dataStr)))
            {
                return (T)xs.Deserialize(ms);
            }
        }
    }
    #endregion

    #region string
    /// <summary>
    /// 添加首行缩进
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string TextIndent(this string info)
    {
        return "\u3000\u3000" + info;
    }

    /// <summary>
    /// 字体上色
    /// </summary>
    /// <param name="info"></param>
    /// <param name="colorNumber"></param>
    /// <returns></returns>
    public static string FontColoring(this string info, Color color)
    {
        string colorNumber = "#" + ColorUtility.ToHtmlStringRGB(color);
        return $"<color={colorNumber}>{info}</color>";
    }
    /// <summary> 字体上色 </summary>
    public static string FontColoring(this string info, string colorNumber = "")
    {
        return $"<color={colorNumber}>{info}</color>";
    }

    /// <summary>
    /// 字符串组合，多用于类的ToString
    /// </summary>
    /// <param name="info"></param>
    /// <param name="textContent"></param>
    /// <returns></returns>
    public static string TextMerge(this string info,params string[] textContent)
    {
        if (textContent.Length > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < textContent.Length; i++)
            {
                sb.AppendLine(textContent[i]);
            }

            info = sb.ToString();
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>需要添加输出的字符串为空</color>");
        }

        return info;
    }

    public static string ListTextShow(this List<string> list, string separator = "")
    {
        if (list != null && list.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (!string.IsNullOrEmpty(separator))
                {
                    sb.AppendLine(list[i] + separator);
                }
                else
                {
                    sb.AppendLine(list[i]);
                }
            }
            Debug.Log($"<color=#eb6ea5>字符串列表最终生成的是{sb.ToString()}</color>");
            return sb.ToString();
        }
        else
        {
            Debug.Log($"<color=#eb6ea5>需要输出的字符串列表为空</color>");
        }

        return string.Empty;
    }

    /// <summary>
    /// 服务端日志
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string ServerLog(this string info)
    {
        return $"服务端：{info}";
    }

    /// <summary>
    /// 客户端日志
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string ClientLog(this string info)
    {
        return $"客户端：{info}";
    }

    #endregion

    #region Coroutine

    /// <summary>
    /// 延时回调
    /// </summary>
    /// <param name="behaviour"></param>
    /// <param name="time"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static Coroutine Wait(this MonoBehaviour behaviour,float time,Action callBack)
    {
        return behaviour.StartCoroutine(Wait(time, callBack));
    }

    /// <summary>
    /// 循环调用Func<bool>这个回调，知道Func<bool>返回值为true时停止
    /// </summary>
    /// <param name="behaviour"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static Coroutine Until(this MonoBehaviour behaviour,Func<bool> callBack)
    {
        return behaviour.StartCoroutine(Until(callBack));
    }

    public static Coroutine WaitAndUntil(this MonoBehaviour behaviour, bool isShow, float time, Action callBack)
    {
        return behaviour.StartCoroutine(WaitAndUntil(isShow, time, callBack));
    }

    private static IEnumerator Wait(float time,Action callBack)
    {
        yield return new WaitForSeconds(time);

        if (callBack != null)
            callBack.Invoke();
    }

    private static IEnumerator Until(Func<bool> callBack)
    {
        if(callBack!=null)
        {
            yield return new WaitUntil(callBack);
        }
    }

    private static IEnumerator WaitAndUntil(bool isShow, float time,Action callBack)
    {
        while (isShow)
        {
            yield return new WaitForSeconds(time);

            if (callBack != null)
                callBack.Invoke();
        }
    }
    #endregion
}