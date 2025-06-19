using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Obi;
using UnityEngine;

public class ObiParticleAttachmentFilling : MonoBehaviour
{
    private ObiRope rope;
    
    [Button("Self Fill Attachment")]
    public void SelfFillAttachment()
    {
        if (rope == null) rope = GetComponent<ObiRope>();
        ObiParticleAttachment[] particleAttachments = null;

        particleAttachments = rope.transform.GetComponents<ObiParticleAttachment>();
        foreach (ObiParticleAttachment particleAttachment in particleAttachments)
        {
            particleAttachment.target = transform;
        }
    }
}
