/****************************************************
    功能：初始面板
    作者：ZH
    创建日期：#2025/01/22#
    修改内容：
        1.UI动画修改    2025/03/07 ZH
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Voltage
{
    public class StartUI : UI3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            PlayUIAnim();
        }

        /// <summary>
        /// 播放UI动画
        /// </summary>
        private void PlayUIAnim()
        {
            view["uiPanel/LogoAnim"].SetActive(true);
            view["uiPanel/LogoAnim/Image"].GetOrAddComponent<CanvasGroup>().alpha = 0;
            view["uiPanel/LogoAnim/Image"].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            view["uiPanel/LogoAnim/btn_start"].GetOrAddComponent<CanvasGroup>().alpha = 0;

            view["uiPanel/LogoAnim/Image"].GetOrAddComponent<CanvasGroup>().DOFade(1, 1.8f);
            view["uiPanel/LogoAnim/Image"].transform.DOScale(1, 2f).OnComplete(() =>
            {
                view["uiPanel/LogoAnim/btn_start"].GetOrAddComponent<CanvasGroup>().DOFade(1, 1f);
                view["uiPanel/LogoAnim/Image"].transform.DOLocalMove(new Vector3(0, 300, 0), 1f);
                view["uiPanel/LogoAnim/btn_start"].transform.DOLocalMove(new Vector3(0, -680, 0), 1f).OnComplete(() =>
                {
                    GOManager.Instance.AddButtonListener(this, "uiPanel/LogoAnim/btn_start", () =>
                    {
                        PlayerManager.Instance.Haptic(EHandType.Both);
                        GOManager.Instance.RemoveGO(name);
                        GOManager.Instance.ShowUI3D(typeof(AgreementUI).Name);
                        DOTween.KillAll();
                    });
                });
            });
        }
    }
}