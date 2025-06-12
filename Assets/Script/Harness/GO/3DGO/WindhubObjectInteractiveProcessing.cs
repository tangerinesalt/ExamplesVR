/****************************************************
    功能：交互对象原模型的显隐处理
    作者：ZZQ
    创建日期：#2025/03/17#
    修改内容：#2025/03/17#
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage;

namespace Voltage
{
    public class WindhubObjectInteractiveProcessing : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }
        void Start()
        {
            HideAllChild();
            view["NotInstall"].SetActive(true);
        }
        public void UpdateState(EWindhubObjectState state)
        {
            HideAllChild();
            switch (state)
            {
                case EWindhubObjectState.NotInstall:
                    view["NotInstall"].SetActive(true);
                    break;
                case EWindhubObjectState.CableInstall:
                    view["NotInstall"].SetActive(true);
                    view["WindhubCable01"].SetActive(true);
                    view["WindhubCable01/TopCable"].SetActive(false);
                    view["WindhubCable01/Whip"].SetActive(false);
                    view["WindhubCable02"].SetActive(true);
                    view["WindhubCable02/TopCable"].SetActive(false);
                    view["WindhubCable02/Whip"].SetActive(false);
                    break;
                case EWindhubObjectState.PanelCableInstall:
                    view["PanelCableInstall"].SetActive(true);
                    view["WindhubCable01"].SetActive(true);
                    view["WindhubCable01/TopCable"].SetActive(false);
                    view["WindhubCable01/Whip"].SetActive(false);
                    view["WindhubCable02"].SetActive(true);
                    view["WindhubCable02/TopCable"].SetActive(false);
                    view["WindhubCable02/Whip"].SetActive(false);
                    break;
                case EWindhubObjectState.CableAndWhipInstall:
                    view["PanelCableInstall"].SetActive(true);
                    view["WindhubCable01"].SetActive(true);
                    view["WindhubCable01/WindhubCable01_Install"].SetActive(false);
                    view["WindhubCable01/TopCable"].SetActive(true);
                    view["WindhubCable01/Whip"].SetActive(true);
                    view["WindhubCable02"].SetActive(true);
                    view["WindhubCable02/WindhubCable02_Install"].SetActive(false);
                    view["WindhubCable02/TopCable"].SetActive(true);
                    view["WindhubCable02/Whip"].SetActive(true);
                    view["CBXCable"].SetActive(true);
                    foreach (Transform child in view["CBXCable"].transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    break;
            }
        }
        private void HideAllChild()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
    public enum EWindhubObjectState
    {
        NotInstall,
        CableInstall,
        PanelCableInstall,
        CableAndWhipInstall,
    }
}