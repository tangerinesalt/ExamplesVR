using UnityEngine;

namespace Voltage
{
    public class ConnectTraining : GO3DBase
    {
        private void Start()
        {
            var socket = Utils.FindChildInTransform(transform, "socket")?.GetComponent<SocketBase>();
            if (socket != null)
            {
                socket.m_afterConnection.AddListener(CompleteTraining);
            }
            else
            {
                Debug.LogError("Socket not found in ConnectTraining");
            }
        }

        protected void CompleteTraining()
        {
            Utils.DebugLog(Color.green, "连接训练完成");
        }
    }
}