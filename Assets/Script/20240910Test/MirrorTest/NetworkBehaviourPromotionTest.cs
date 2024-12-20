using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tangerine;

/// <summary>
/// 测试网络行为发布器NetworkBehaviourPromotion的转发能力
/// </summary>
public class NetworkBehaviourPromotionTest : MonoBehaviour
{
    public GameObject m_player;
    [Space]
    public bool isServer;
    public bool isClient;
    public bool isCmd;
    public bool isExcludeLocalPlayer;
    public GameObject m_gameobjectPrefab;
    public Vector3 m_SpawnPosition=new Vector3(0,0.5f,0);
    private NetworkBehaviourPromotion _promotion;

    void Start()
    {
        if (_promotion == null) _promotion = this.transform.GetComponent<NetworkBehaviourPromotion>();
        if (_promotion == null) Debug.Log("No NetworkBehaviourPromotion found");
        m_player=MainManager_NoneVR.Instance.GetNoneVRPlayer();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isServer)
            {
                Debug.Log("is Server");
                _promotion.S_BehavioursPromotion(this.gameObject, this.GetType().Name, "CubeSpawner");
            }
            if (isClient)
            {
                Debug.Log("is Client");
            }
            if (isCmd)
            {
                Debug.Log("is Cmd");
                _promotion.Cmd_BehavioursPromotion(this.gameObject, this.GetType().Name, "CubeSpawner", null);
            }
            if (isExcludeLocalPlayer)
            {
                Debug.Log("is Exclude Local Player");
                _promotion.Cmd_BehavioursPromotionExcludeLocalPlayer(this.gameObject, this.GetType().Name, "CubeSpawner", null);
            }
        }
    }
    public void CubeSpawner()
    {
        GameObject gameObject = Instantiate(m_gameobjectPrefab,m_player.transform.position+ m_SpawnPosition,m_player.transform.rotation);
    }
}
