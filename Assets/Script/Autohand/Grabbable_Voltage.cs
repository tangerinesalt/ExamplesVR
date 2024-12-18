using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using Mirror;
using UnityEngine;
using Voltage;
using NaughtyAttributes;
using Tangerine;

namespace Voltage
{
    [RequireComponent(typeof(NetworkBehaviourPromotion))]
    public class Grabbable_Voltage : Grabbable
    {
        [AutoToggleHeader("Custom")]
        public bool custom = true;
        [Space,ShowIf("custom")]
        public bool m_disablePhysicsExceptGrabbing=false;      
        private NetworkBehaviourPromotion m_NetworkBehaviourPromotion;

        public event Action onDisable;
        public event Action onEnable;
        [HideInInspector]
        public bool isgrabbed = false;
        private CollisionDetectionMode RigidbodyDetectionMode;

        protected override void OnDisable()
        {
            base.OnDisable();
            onDisable?.Invoke();
        }
        protected void OnEnable()
        {
            onEnable?.Invoke();
            if (m_disablePhysicsExceptGrabbing) DisablePhysicsExceptGrabbing();
        }
        
        #region inherit Network behavior
        public override void HeldFixedUpdate()
        {
            base.HeldFixedUpdate();
            isgrabbed = true;
            if (BeingGrabbed())
            {
                if (m_NetworkBehaviourPromotion == null) GetNetworkBehaviourPromotion();
                m_NetworkBehaviourPromotion.Cmd_BehavioursPromotionExcludeLocalPlayer(this.gameObject, GetType().Name, "OnHeldFixedUpdate");
            }
        }
        protected override void OnGrab(Hand hand)
        {
            base.OnGrab(hand);
            if (m_disablePhysicsExceptGrabbing) EnablePhysicsInGrabbing();
        }
        protected override void OnRelease(Hand hand)
        {
            base.OnRelease(hand);

            isgrabbed = false;

            if (m_disablePhysicsExceptGrabbing) DisablePhysicsExceptGrabbing();
        }
        //当前玩家持有时，其他玩家的持有行为将被禁止
        public void OnHeldFixedUpdate()
        {
            ForceHandsRelease();
        }


        #endregion

        #region tools
        //网络行为工具
        private void GetNetworkBehaviourPromotion()
        {
            m_NetworkBehaviourPromotion = GetComponent<NetworkBehaviourPromotion>();
            if (m_NetworkBehaviourPromotion == null)
            {
                Debug.LogError("NetworkBehaviourPromotion component not found on " + gameObject.name, this);
            }
        }
        #endregion
        #region 自定义
        //禁用物理特性，可在Onrelease\onenable调用
        private void DisablePhysicsExceptGrabbing()
        {
            if (body != null)
            {
                RigidbodyDetectionMode = body.collisionDetectionMode;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                body.isKinematic = true;
            }
        }
        //启用物理特性,可在Ongrab调用
        private void EnablePhysicsInGrabbing()
        {
            if (body != null)
            {
                body.isKinematic = false;
                body.collisionDetectionMode = RigidbodyDetectionMode;
            }
        }
        #endregion
    }
}
