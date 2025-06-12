using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;
using Tangerine;
using System.Reflection;

public class CubeSpwnedWithPlayer : NetworkBehaviour
{
    public bool isCmd = false;
    public bool isExcludeLocalPlayer = false;
    public GameObject m_gameobjectPrefab;
    public Vector3 m_SpawnPosition = new Vector3(0, 0.5f, 0);
    private NetworkBehaviourPromotion _promotion;
    void Start()
    {
        if (_promotion == null) _promotion = this.transform.GetComponent<NetworkBehaviourPromotion>();
        if (_promotion == null) Debug.Log("No NetworkBehaviourPromotion found");
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        Debug.Log("CubeSpawner", this);
        GameObject gameObject = Instantiate(m_gameobjectPrefab, transform.position, Quaternion.identity);
    }
}
