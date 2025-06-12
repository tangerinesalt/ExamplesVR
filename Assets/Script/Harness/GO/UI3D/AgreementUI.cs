/****************************************************
    功能：风险说明面板
    作者：ZH
    创建日期：#2025/02/14#
    修改人：ZH
    修改日期：#2025/02/14#
    修改内容：
        1.删除模式选择，增加震动触发    2025/03/18 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    public class AgreementUI : UI3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            GOManager.Instance.AddButtonListener(this, "uiPanel/Agreement/btn_no", () =>
            {
                PlayerManager.Instance.Haptic(EHandType.Both);
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowUI3D(typeof(StartUI).Name);
            });

            GOManager.Instance.AddButtonListener(this, "uiPanel/Agreement/btn_yes", () =>
            {
                PlayerManager.Instance.Haptic(EHandType.Both);
                view["uiPanel/Agreement"].gameObject.SetActive(false);
                view["uiPanel/Risk"].gameObject.SetActive(true);
            });

            GOManager.Instance.AddButtonListener(this, "uiPanel/Risk/btn_no", () =>
            {
                PlayerManager.Instance.Haptic(EHandType.Both);
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowUI3D(typeof(StartUI).Name);
            });

            GOManager.Instance.AddButtonListener(this, "uiPanel/Risk/btn_yes", () =>
            {
                PlayerManager.Instance.Haptic(EHandType.Both);
                GlobalDataManager.Instance.eTrainType = ETrainType.Teach;
                GlobalMethodManager.Instance.CloseInteration();

                DelayFunc(0.5f, () =>
                {
                    GOManager.Instance.RemoveGO(name);
                    SceneServiceManager.Instance.LoadScene(GlobalDataManager.Instance.teachScene);
                });
            });
        }
    }
}