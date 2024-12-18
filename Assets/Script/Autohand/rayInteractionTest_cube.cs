using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Voltage;
using Tangerine;


public class rayInteractionTest_cube : MonoBehaviour
{
    public Button button;
    public bool usingServer = false;
    public bool usingClient = false;
    public bool usingCmd = false;
    public bool usingCmdExcludeLocalPlayer=false;
    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(UIClickTest);
        }
    }
    public void UIClickTest()
    {
        if (usingServer) 
        {
            Debug.Log("usingServer");
            NetworkBehaviourPromotion _networkBehaviourPromotion = GetComponent<NetworkBehaviourPromotion>();
            if (_networkBehaviourPromotion != null)
            {
                if (_networkBehaviourPromotion.isServer) _networkBehaviourPromotion.S_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
                else          _networkBehaviourPromotion.Cmd_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
                return;
            }
        }
        else if (usingClient)//客户端测试
        {
            Debug.Log("usingClient");
            NetworkBehaviourPromotion _networkBehaviourPromotion = GetComponent<NetworkBehaviourPromotion>();
            if (_networkBehaviourPromotion != null)
            {
                if (_networkBehaviourPromotion.isClient) _networkBehaviourPromotion.C_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
                else          _networkBehaviourPromotion.Cmd_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
                return;
            }
        }
        else if (usingCmd) //客户端测试
        {
            Debug.Log("usingCmd");
            NetworkBehaviourPromotion _networkBehaviourPromotion = GetComponent<NetworkBehaviourPromotion>();
            if (_networkBehaviourPromotion != null)
            {
                _networkBehaviourPromotion.Cmd_BehavioursPromotion(gameObject, this.GetType().Name, "ChangeColor");
                return;
            }
        }
        else if (usingCmdExcludeLocalPlayer) //客户端测试
        {
            Debug.Log("usingCmdExcludeLocalPlayer");
            NetworkBehaviourPromotion _networkBehaviourPromotion = GetComponent<NetworkBehaviourPromotion>();
            if (_networkBehaviourPromotion != null)
            {
                _networkBehaviourPromotion.Cmd_BehavioursPromotionExcludeLocalPlayer(gameObject, this.GetType().Name, "ChangeColor");
                return;
            }
        }
        else if (!(usingServer || usingClient|| usingCmd|| usingCmdExcludeLocalPlayer))
        {
            Debug.Log("using default");
            ChangeColor();
            return;
        }
    }
    public void ChangeColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color randomColor = new Color(
                  Random.value,   // Red component
                  Random.value,   // Green component
                  Random.value    // Blue component
              );
            renderer.material.color = randomColor;
        }
    }
}
