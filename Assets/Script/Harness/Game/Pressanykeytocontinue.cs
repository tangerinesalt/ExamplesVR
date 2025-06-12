/****************************************************
    功能：UI任意键事件
    作者：ZH
    创建日期：#2025/02/25#
    修改内容：
        1.增加主菜单面板显隐参数控制    2025/03/05 ZH
        2.取消KillAll动画(Tween)，改为Kill当前动画(Tween)  2025/04/21 ZZQ
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

namespace Voltage
{
    public class PressAnyKeyToContinue : MonoBehaviour
    {
        /// <summary>
        /// 闪烁物体
        /// </summary>
        private GameObject flashGO;
        /// <summary>
        /// 闪烁动画
        /// </summary>
        private Tween flashTween;
        /// <summary>
        /// 输入系统的任意键事件
        /// </summary>
        [SerializeField]
        private InputActionReference anyKeyInputAction;
        /// <summary>
        /// 当任意键按下时触发的事件
        /// </summary>
        public UnityEvent anyKeyPressedEvent;
        /// <summary>
        /// 延时时间
        /// </summary>
        private float delayTime = 0.5f;

        private void Awake()
        {
            flashGO = transform.Find("content").gameObject;
        }

        private void OnEnable()
        {
            PlayerManager.Instance.isCanOpenMainMenu = false;
            PlayerManager.Instance.UpdatePanelState();

            StartCoroutine(DelayFunc());
        }

        private void OnDisable()
        {
            PlayerManager.Instance.isCanOpenMainMenu = true;

            if (anyKeyInputAction != null)
            {
                anyKeyInputAction.action.performed -= OnAnyKeyPerformed;
            }

            if(flashTween!= null)flashTween.Kill();
            StopAllCoroutines();
        }

        private IEnumerator DelayFunc()
        {
            flashGO.GetOrAddComponent<CanvasGroup>().alpha = 1;
            flashTween = flashGO.GetOrAddComponent<CanvasGroup>().DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);

            yield return new WaitForSeconds(delayTime);

            if (anyKeyInputAction != null)
            {
                anyKeyInputAction.action.performed += OnAnyKeyPerformed;
            }
        }

        private void OnAnyKeyPerformed(InputAction.CallbackContext context)
        {
            anyKeyPressedEvent?.Invoke();
        }
    }
}