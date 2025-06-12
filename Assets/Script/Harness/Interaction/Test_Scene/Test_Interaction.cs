using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Voltage;

public class Test_Interaction : MonoBehaviour
{
    public bool m_enable = true;
    [ShowIf("m_enable")]
    public GameObject m_prefabs;
    [Space(6)]
    public Vector3 m_position = new Vector3(392f, 0, -26f);
    public Vector3 m_rotation = Vector3.zero;
    public float m_reachDistance = 0.8f;
    void Start()
    {
        PlayerManager.Instance.SetPosition(m_position);
        PlayerManager.Instance.SetRotation(m_rotation);
        #region 测试参数(仅此场景测试使用)
        //enable参数，设置为false时，不执行测试代码
        if (!m_enable)
        {
            //player状态调整
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetHandProjectorState(true);
            return;
        }
        //测试专用初始化，避免此管理器的寻找CanvasRoot的过程报错
        else 
        {
            GOManager.Instance.Init();
            AudioManager.Instance.Init();
            //SceneServiceManager.Instance.Init();
        }
        #endregion

        if (m_prefabs!= null)
        GOManager.Instance.ShowGO3D(m_prefabs.name);
    }
    [Button("JumpPosition")]
    private void JumpPosition()
    {
        PlayerManager.Instance.SetPosition(m_position);
        PlayerManager.Instance.SetRotation(m_rotation);
    }
}
