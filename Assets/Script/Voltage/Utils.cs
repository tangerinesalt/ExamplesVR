using kcp2k;
using Mirror;
using UnityEngine;
using Voltage;

namespace Voltage
{
    public class Utils
    {
        /// <returns>
        /// 方向 <paramref name="direction"/> 的水平角
        /// <para>水平角：以世界+Z为北，在俯视角下某向量与北的夹角，偏东为正值，偏西为负值。</para>
        /// </returns>
        public static float GetHAngle(Vector3 direction){
            return Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        }
        
        #region Network
        
        public static bool TrySetTransportPort(ushort port)
        {
            if (Transport.active == null) return false;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = port;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // public static NetworkDiscoveryV GetNetworkDiscoveryV()
        // {
        //     NetworkManager networkManager = NetworkManager.singleton;
        //     if (networkManager == null) return null;
        //     return networkManager.GetComponent<NetworkDiscoveryV>();
        // }
        
        public static uint GetPlayerId()
        {
            if (NetworkClient.localPlayer == null)
            {
                return uint.MaxValue;
            }
            else
            {
                return NetworkClient.localPlayer.netId;
            }
        }
        
        // Server only
        public static NetworkConnectionToClient GetHostConn2C()
        {
            if (NetworkServer.connections.ContainsKey(0)) return NetworkServer.connections[0];
            else return null;
        }

        public static NetworkConnectionToClient GetNetConn2C(uint netId)
        {
            if (NetworkServer.spawned.ContainsKey(netId))
            {
                return NetworkServer.spawned[netId].connectionToClient;
            }
            else
            {
                Debug.Log("Network connection not found for netId " + netId);
                return null;
            }
        }

        #endregion
    }
}
