/****************************************************
    功能：处理子物体粒子渲染器
    作者：ZZQ
    创建日期：#2025/04/01#
    修改内容：#2025/04/01#
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Obi;

public class ObiParticleRendererProcessor
 : MonoBehaviour
{
    [Button("Disable ParticleRenderer")]
    public void DisableParticleRenderer()
    {
        ParticleRenderer(transform, false);
    }
    [Button("Enable ParticleRenderer")]
    public void EnableParticleRenderer()
    {
        ParticleRenderer(transform, true);
    }
    public void ParticleRenderer(Transform _transform,bool _enable)
    {
        if (_transform.childCount > 0)
        {
            foreach (Transform child in _transform)
            {
                ObiParticleRenderer[] particleRenderers = child.GetComponents<ObiParticleRenderer>();
                foreach (ObiParticleRenderer particleRenderer in particleRenderers)
                {
                    particleRenderer.enabled = _enable;
                }
                if (child.childCount > 0)
                {
                    ParticleRenderer(child, _enable);
                }
            }
        }
    }
}
