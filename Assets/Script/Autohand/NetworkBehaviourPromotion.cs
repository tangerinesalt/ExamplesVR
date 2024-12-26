using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Reflection;

namespace Tangerine
{
    /// <summary>
    /// 作用：客户端控制服务器端执行在所有客户端上的行为
    /// 依赖networkIdentity组件,当前物体缺少Identity组件会报未知错误
    /// </summary>
    public class NetworkBehaviourPromotion : NetworkBehaviour
    {
        private static Dictionary<string, MethodInfo> cachedMethods = new Dictionary<string, MethodInfo>();

        //检查是否包含依赖组件
        private void Start()
        {
            if(gameObject.GetComponent<NetworkIdentity>()==null)
            {
                Debug.LogError("NetworkBehaviourPromotion must be attach a NetworkIdentity component but not found.",this);
            }
        }

        //调用示例：
        // if (_networkBehaviourPromotion.isServer) 
        //       _networkBehaviourPromotion.S_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
        // else  _networkBehaviourPromotion.Cmd_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");

        /// <summary>
        /// 排除本地玩家，其他玩家全部执行
        /// </summary>
        /// <param name="_gameObject">游戏对象</param>
        /// <param name="className">类名</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="sender">发布者</param>
        [Command(requiresAuthority = false)]
        public void Cmd_BehavioursPromotionExcludeLocalPlayer(GameObject _gameObject, string className, string MethodName, NetworkConnectionToClient sender = null)
        {
            if (sender == null || sender.identity == null) return;
            foreach (NetworkConnectionToClient conn2C in NetworkServer.connections.Values)
            {
                if (conn2C.identity == sender.identity)
                {
                    continue;
                }
                else
                {
                    Debug.Log("Cmd_BehavioursPromotionExcludeLocalPlayer");
                    Target_Behavioralpromotion(conn2C, _gameObject, className, MethodName);
                }
            }
        }

        //服务器端调用
        [Server]
        public void S_BehavioursPromotion(GameObject _gameObject, string className, string MethodName)
        {
            Debug.Log("Server:S_BehavioursPromotion");
            Rpc_BehavioralExecution(_gameObject, className, MethodName);
        }

        [Client]
        public void C_BehavioursPromotion(GameObject _gameObject, string className, string MethodName)
        {
            //Debug.Log("Client:C_BehavioursPromotion");
            Rpc_BehavioralExecution(_gameObject, className, MethodName);
        }

        //客户端调用,服务器端执行
        [Command(requiresAuthority = false)]
        public void Cmd_BehavioursPromotion(GameObject _gameObject, string className, string MethodName, NetworkConnectionToClient sender = null)
        {
            if (sender == null || sender.identity == null) return;
            //Debug.Log(_gameObject.GetComponent<NetworkIdentity>().netId);//网络身份测试，网络游戏对象
            //Debug.Log(sender.identity.netId);//网络身份测试，下发者

            //Debug.Log("Cmd_BehavioursPromotion");
            S_BehavioursPromotion(_gameObject, className, MethodName);
        }

        [TargetRpc]
        private void Target_Behavioralpromotion(NetworkConnection connection,GameObject _gameObject, string className, string MethodName)
        {
            ExecuteMethodOnClient(_gameObject, className, MethodName);
        }
        
        /// <summary>
        /// 所有客户端执行
        /// </summary>
        /// <param name="_gameObject">游戏对象</param>
        /// <param name="className">类名</param>
        /// <param name="MethodName">方法名</param>
        [ClientRpc]
        private void Rpc_BehavioralExecution(GameObject _gameObject, string className, string MethodName)
        {
            ExecuteMethodOnClient(_gameObject, className, MethodName);
        }

        //方法体，通过反射调用方法，调用对象需要有networkidentity组件
        private void ExecuteMethodOnClient(GameObject _gameObject, string className, string methodName)
        {
            // 获取类型
            Type classType = Type.GetType(className);
            if (classType == null)
            {
                Debug.LogError("Class not found: " + className);
                return;
            }
            Component component = _gameObject.GetComponent(classType);
            if (component == null)
            {
                Debug.LogError($"Component not found on {_gameObject.name}: {className}");
                return;
            }

            // Cache method info to avoid reflection overhead on repeated calls
            string methodKey = $"{className}.{methodName}";
            if (!cachedMethods.TryGetValue(methodKey, out MethodInfo methodInfo))
            {
                // If the method is not cached, fetch it and cache it
                methodInfo = classType.GetMethod(methodName);
                if (methodInfo == null)
                {
                    Debug.LogError($"Method '{methodName}' not found in class '{className}'");
                    return;
                }
                cachedMethods[methodKey] = methodInfo;
            }

            // Invoke the method
            try
            {
                methodInfo.Invoke(component, null); // Assuming no parameters for simplicity
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking method '{methodName}': {ex.Message}");
            }
        }
    }
}
