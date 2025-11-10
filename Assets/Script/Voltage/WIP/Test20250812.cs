// /*
// 功能：注册事件使得插座按顺序连接
// */
// using System.Collections;
// using System.Collections.Generic;
// using NaughtyAttributes;
// // using Org.BouncyCastle.Asn1.X509;
// using Unity.VisualScripting;
// using Unity.XR.CoreUtils;
// using UnityEngine;
// using Voltage;

// public class Test20250812 : MonoBehaviour
// {
//     /// <summary> 目标插座，按照顺序连接 </summary>
//     [Header("目标插座"), Tooltip("按照顺序连接的插座"), SerializeField]
//     public SocketBase[] TargetObject;
//     private List<SocketBase> m_SocketList = new List<SocketBase>();

//     /// <summary> 一键从子对象获取插座 </summary>
//     [Button("一键获取插座")]
//     private void GetSocketList()
//     {
//         TargetObject = GetComponentsInChildren<SocketBase>();
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         // // 获取所有插座给列表
//         // m_SocketList.Clear();
//         // if (TargetObject != null)
//         // {
//         //     m_SocketList.AddRange(TargetObject);
//         // }
//         // // 隐藏所有插座的模型渲染
//         // foreach (var socket in m_SocketList)
//         // {
//         //     socket.transform.GetChild(0).gameObject.SetActive(false);
//         // }
//         for (int i = 0; i < TargetObject.Length; i++)
//         {
//             SetSocketStateBeforeConnection(TargetObject[i]);
//             RegisterEvents(TargetObject[i], i + 1 < TargetObject.Length ? TargetObject[i + 1] : null);
//         }
//         SetSocketStateOnConnection(TargetObject[0]);
//         // int i = 0;
//         // RegisterEvents(i);
//     }
//     /// <summary>
//     /// 按照顺序注册事件--当前一个插座连接完成后，注册下一个插座事件
//     /// </summary>
//     /// <param name="i"></param>
//     private void RegisterEvents(int i)
//     {
//         TargetObject[i].m_enable = true;
//         TargetObject[i].transform.GetChild(0).gameObject.SetActive(true);
//         Debug.Log("注册事件给插座：" + TargetObject[i].gameObject.name, TargetObject[i].gameObject);
//         TargetObject[i].m_afterConnection.RemoveAllListeners();
//         TargetObject[i].m_afterConnection.AddListener(() =>
//         {
//             Debug.Log("连接完成", TargetObject[i].gameObject);
//             TargetObject[i].m_enable = false;
//             TargetObject[i].transform.GetChild(0).gameObject.SetActive(false);
//             m_SocketList.ForEach(socket => { socket.m_enable = false; });
//             if (TargetObject[i]._connectedPlug == null)
//             {
//                 Debug.LogWarning("未找到连接的插头");
//             }
//             else
//             {
//                 Debug.Log("连接的插头是：" + TargetObject[i]._connectedPlug.gameObject.name, TargetObject[i]._connectedPlug.gameObject);
//                 TargetObject[i]._connectedPlug.m_enable = false;
//             }

//             i++;
//             if (i < TargetObject.Length)
//             {
//                 RegisterEvents(i);
//             }
//             else
//             {
//                 Debug.Log($"全部注册完成，一共{i}个插座");
//             }
//         });

//     }
//     private void RegisterEvents(SocketBase currentSocket,SocketBase nextSocket = null)
//     {
//         currentSocket.m_afterConnection.RemoveAllListeners();
//         currentSocket.m_afterConnection.AddListener(() =>
//         {
//             Debug.Log("连接完成", currentSocket.gameObject);
//             SetSocketStateAfterConnection(currentSocket);
//             if (nextSocket!= null)
//             {
//                 SetSocketStateOnConnection(nextSocket);
//             }
//         });
//     }
//     /// <summary>
//     /// 设置插座连接前的状态
//     /// </summary>
//     /// <param name="socket"></param>
//     private void SetSocketStateBeforeConnection(SocketBase socket)
//     {
//         socket.m_enable = false;
//         socket.transform.GetChild(0).gameObject.SetActive(false);
//     }
//     /// <summary>
//     /// 设置插座将要连接时的状态
//     /// </summary>
//     private void SetSocketStateOnConnection(SocketBase socket)
//     {
//         socket.m_enable = true;
//         socket.transform.GetChild(0).gameObject.SetActive(true);
//     }
//     /// <summary>
//     /// 设置插座连接后的状态
//     /// </summary>
//     private void SetSocketStateAfterConnection(SocketBase socket)
//     {
//         socket.m_enable = false;
//         socket.transform.GetChild(0).gameObject.SetActive(false);
//     }



// }
