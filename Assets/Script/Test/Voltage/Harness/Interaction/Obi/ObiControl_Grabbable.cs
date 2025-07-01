/****************************************************
    功能：obi线缆Attachment对象控制脚本，抓取该对象时将相关的obi线缆改为静态，放下时改为动态
    作者：ZZQ
    创建日期：#2025/02/20#
    修改人：ZZQ
    修改日期：#2025/02/20#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using Voltage;
using Obi;

namespace Voltage
{
    [RequireComponent(typeof(Grabbable))]
    public class ObiControl_Grabbable : ObiControlBase
    {
        [SerializeField] private Grabbable m_grabbable;
        public override void Awake()
        {
            base.Awake();

            if (m_grabbable == null) m_grabbable = GetGrabbable();
            if (m_grabbable != null)
            {
                m_grabbable.OnBeforeGrabEvent += OnBeforeGrab;
                m_grabbable.OnGrabEvent += OnGrab;
                m_grabbable.OnReleaseEvent += OnRelease;
            }
        }

        public void OnBeforeGrab(Hand hand, Grabbable grab)
        {
            if (m_particleAttachmentList.Count == 0)
                return;

            foreach (ObiParticleAttachment _particleAttachment in m_particleAttachmentList)
            {
                if (_particleAttachment.target == transform)
                    ChangeObiAttachmentType(_particleAttachment, AttachmentType.Static);
            }
            // if (m_particleAttachmentList.Count != 0)
            // {
            //     ChangeAllObiAttachmentType(AttachmentType.Static);
            // }
        }

        public void OnGrab(Hand hand, Grabbable grab)
        {

        }

        public void OnRelease(Hand hand, Grabbable grab)
        {
            if (m_particleAttachmentList.Count == 0)
                return;

            foreach (ObiParticleAttachment _particleAttachment in m_particleAttachmentList)
            {
                if (_particleAttachment.target == transform)
                    ChangeObiAttachmentType(_particleAttachment, AttachmentType.Dynamic);
            }
            // if (m_particleAttachmentList.Count != 0)
            // {
            //     ChangeAllObiAttachmentType(AttachmentType.Dynamic);
            // }
        }
        private Grabbable GetGrabbable()
        {
            Grabbable grabbable = GetComponent<Grabbable>();
            if (grabbable == null)
            {
                grabbable = GetComponentInParent<Grabbable>();
            }
            return grabbable;
        }
        [ContextMenu("Add Particle Attachment For Brother")]
        public void AddParticleAttachmentForBrother()
        {
            foreach (Transform brother in transform.parent)
            {
                if (brother.GetComponent<ObiControlBase>() != null)
                {
                    brother.GetComponent<ObiControlBase>().AddParticleAttachmentInInspector();
                }
            }
        }
    }
}