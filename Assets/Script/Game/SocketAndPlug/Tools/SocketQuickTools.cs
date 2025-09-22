/****************************************************
    功能：Socket快速修改工具--注册依次显示
    作者：ZZQ
    创建日期：#2025/09/05#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Voltage;

public class SocketQuickTools : MonoBehaviour
{
    [SerializeField] private SocketBase[] m_sockets;
    [Button("注册依次显示")]
    private void Start()
    {
        RegisterSocketShowInTurn();
    }
    void OnDestroy()
    {
        RemoveRegisterSocketShowInTurn();
    }
    private void RegisterSocketShowInTurn()
    {
        SocketBase[] sockets = this.transform.GetComponentsInChildren<SocketBase>();

        if (sockets != null && sockets.Length > 0)
        {
            m_sockets = sockets;
        }
        else
        {
            Debug.LogError("未找到Socket组件");
            return;
        }

        for (int i = 0; i < m_sockets.Length-1; i++)
        {
            m_sockets[i + 1].gameObject.SetActive(false);
            Debug.Log($"注册依次显示：{m_sockets[i].name}连接后触发显示{m_sockets[i + 1].name}");
            int index = i;
            m_sockets[i]?.m_afterConnection.AddListener(() =>
            {
                Debug.Log($"注册结果：--{m_sockets[index]?.name}--已连接，显示--{m_sockets[index + 1]?.name}--");
                m_sockets[index + 1]?.gameObject.SetActive(true);
            });
        }
    }
    [Button("移除注册依次显示")]
    private void RemoveRegisterSocketShowInTurn()
    {
        if (m_sockets.Length > 0)
        {
            for (int i = 0; i < m_sockets.Length; i++)
            {
                m_sockets[i].m_afterConnection.RemoveAllListeners();
            }
            m_sockets = null;
        }
    }
}
