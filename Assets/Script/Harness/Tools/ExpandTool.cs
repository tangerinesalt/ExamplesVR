/****************************************************
    功能：拓展工具类
    作者：ZH
    创建日期：#2025/01/08#
    修改人：ZH
    修改日期：#2025/01/16#
    修改内容：
*****************************************************/

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;

public static class ExpandTool
{
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

}