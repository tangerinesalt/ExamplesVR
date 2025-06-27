using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using Voltage;

/// <summary>
/// 按钮在ObiParticleAttachmentToolsEditor类中实现
/// </summary>
public class ObiParticleAttachmentTools : MonoBehaviour
{
    public GameObject m_TargetObjectPrefab;
    
    public ObiActor m_ObiActor;
    public ObiParticleAttachment.AttachmentType m_AttachmentType = ObiParticleAttachment.AttachmentType.Static;
    public void SetparticleAttachment(ObiActor obiActor)
    {
        if (obiActor == null)
        {
            Debug.LogError("请确保该对象包含ObiActor组件(如ObiRope)。");
            return;
        }
        ObiActorBlueprint blueprint = obiActor.sourceBlueprint;
        if (blueprint == null)
        {
            Debug.LogError("未找到有效的蓝图, 请确保ObiActor已正确设置蓝图。");
            return;
        }
        if (blueprint.groups == null || blueprint.groups.Count == 0)
        {
            Debug.LogError("蓝图中没有有效的粒子组。请确保至少有一个粒子组存在。");
            return;
        }

        DestoryAttachmentComponent(obiActor);

        //添加并修改附着组件
        Utils.DebugLog(Color.yellow, $"{blueprint.name}中共有{blueprint.groups.Count}个粒子组。");
        for (int i = 0; i < blueprint.groups.Count; i++)
        {
            ObiParticleAttachment attachment = gameObject.AddComponent<ObiParticleAttachment>();
            //实例化预制体并为组件赋值
            //attachment.target = m_TargetObject != null ? m_TargetObject.transform : null;
            attachment.particleGroup = blueprint.groups[i];
            attachment.attachmentType = m_AttachmentType;
            Utils.DebugLog(Color.green, $"粒子组 {i} 名称: {blueprint.groups[i].name}");
        }
    }
    public void DestoryAttachmentComponent(ObiActor obiActor)
    {
        //销毁其余的附着组件——防止多个控制争抢同一粒子组
        ObiParticleAttachment[] attachments = obiActor.gameObject.GetComponents<ObiParticleAttachment>();
        if (attachments.Length == 0)
        {
            return;
        }

        Utils.DebugLog(Color.yellow, $"销毁:{name}_所有附着组件",this);
        foreach (var attachment in attachments)
        {
            DestroyImmediate(attachment);
        }
    }
}
