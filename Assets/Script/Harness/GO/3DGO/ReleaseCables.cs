/****************************************************
    功能：释放线缆
    作者：ZZQ
    创建日期：#2025/04/18#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using System;

namespace Voltage
{
    public class ReleaseCables : GO3DBase
    {
        private ReleaseCableAnimation animationController;
        private Tween moveTween01;
        private Tween moveTween02;
        private float InitCarPosition;
        protected override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            InitPlayer();
            InitElement();
            InitInteractionUI();
        }
        private void InitPlayer()
        {
            PlayerManager.Instance.SetPosition(new Vector3(395f, 0, -39f));
            PlayerManager.Instance.SetRotation(new Vector3(0, -90f, 0));
            
            PlayerManager.Instance.moveMode = EMoveMode.MovebyPhysics;
            PlayerManager.Instance.EnableMove = true;
            // PlayerManager.Instance.EnableTurn = true;
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);
            PlayerManager.Instance.SetReachDistance(0.8f);
            PlayerManager.Instance.SetHandProjectorState(true);
            PlayerManager.Instance.EyeFadeBright();
        }
        private void InitElement()
        {
            //游戏对象
            view["MoveMark"].SetActive(false);
            SetChilderActive("Cable/Cable_Onground", false);
            view["Cable/Cable_Onground/show"].SetActive(true);
            InitCarPosition = view["Vehicle"].transform.position.x;
            ChangeCableNumberInCar(1);
            //动画组件
            animationController = this.gameObject.GetOrAddComponent<ReleaseCableAnimation>();
            animationController.target = view["Vehicle"].transform;
            //UI
            SetChilderActive("uiPanel/InteractiveUI", false);
            SetChilderActive("uiPanel/OperationTips", false);
            view["uiPanel/InteractiveUI/ReleaseCableGuide"].SetActive(true);
            //任务
            PlayerManager.Instance.UpdateCurrentTask("release1");
        }
        void InitInteractionUI()
        {

            view["uiPanel/InteractiveUI/ReleaseCableGuide"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    view["uiPanel/InteractiveUI/ReleaseCableGuide"].SetActive(false);
                    view["uiPanel/InteractiveUI/ReleaseCable01"].SetActive(true);
                    PlayerManager.Instance.SetRayState(EHandType.Both, true);
                });
            view["uiPanel/InteractiveUI/ReleaseCable01/btn"].GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    view["uiPanel/InteractiveUI/ReleaseCable01"].SetActive(false);
                    moveTween01 = animationController.Move(MoveDirection.X, 341.5f, 10f, Ease.InQuad);
                    PlayerManager.Instance.EnableMove = false;
                    PlayerManager.Instance.SetRayState(EHandType.Both, false);
                    
                    DelayFunc(1f, () =>
                    {
                        PlayerManager.Instance.EnableMove = true;
                        view["uiPanel/InteractiveUI/GoReleaseCable02"].SetActive(true);
                    });
                });
            view["uiPanel/InteractiveUI/GoReleaseCable02"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.EyeFadeDark();

                    DelayFunc(0.5f, () =>
                    {
                        PlayerManager.Instance.SetPosition(new Vector3(351f, 0f, -41f));
                        PlayerManager.Instance.SetRotation(new Vector3(0f, 75f, 0f));
                        view["uiPanel/InteractiveUI/GoReleaseCable02"].SetActive(false);

                        //重新赋值动画
                        if(moveTween01!= null) moveTween01.Kill();
                        float currentCarPoint = view["Vehicle"].transform.position.x;
                        float duration = math.abs((341.5f - currentCarPoint) / (341.5f - InitCarPosition)) * 10f;
                        moveTween01 = animationController.Move(MoveDirection.X, 341.5f, duration, Ease.Linear);
                        ChangeCableNumberInCar(2);
                        moveTween01.onComplete += () =>
                        {
                            view["uiPanel/InteractiveUI/ReleaseCable02"].SetActive(true);
                            view["uiPanel/InteractiveUI/PopText01"].SetActive(true);
                            PlayerManager.Instance.SetRayState(EHandType.Both, true);
                            view["uiPanel/InteractiveUI/PopText01/GoReleaseCable02Guide"]?.SetActive(false);
                            DelayFunc(5f, () => view["uiPanel/InteractiveUI/PopText01/GoReleaseCable02Guide"]?.SetActive(true));
                        };
                        PlayerManager.Instance.EyeFadeBright();
                    });
                });
            view["uiPanel/InteractiveUI/ReleaseCable02/btn"].GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    view["uiPanel/InteractiveUI/ReleaseCable02"].SetActive(false);
                    moveTween02 = animationController.Move(MoveDirection.X, -8.29f, 80f, Ease.OutQuad);
                    PlayerManager.Instance.SetRayState(EHandType.Both, false);
                    PlayerManager.Instance.EnableMove = false;

                    DelayFunc(1f, () =>
                    {
                        PlayerManager.Instance.EnableMove = true;
                        //UI
                        view["uiPanel/InteractiveUI/GoEnd"].SetActive(true);
                        view["uiPanel/InteractiveUI/PopText01"].SetActive(false);
                    });
                });
            view["uiPanel/InteractiveUI/GoEnd"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.EyeFadeDark();

                    DelayFunc(0.5f, () =>
                    {
                        PlayerManager.Instance.SetPosition(new Vector3(-2f, 0f, -37f));
                        PlayerManager.Instance.SetRotation(new Vector3(0f, 90f, 0f));
                        view["uiPanel/InteractiveUI/GoEnd"].SetActive(false);

                        //动画时长太长，重新赋值动画
                        if(moveTween02!= null) moveTween02.Kill();
                        Vector3 currentCarPosition = view["Vehicle"].transform.position;
                        view["Vehicle"].transform.position = new Vector3(10f, currentCarPosition.y, currentCarPosition.z);
                        ChangeCableNumberInCar(8);
                        float currentCarPoint = view["Vehicle"].transform.position.x;
                        float duration = math.abs((-8.29f - currentCarPoint) / (341.5f + 8.29f)) * 80f;
                        moveTween02 = animationController.Move(MoveDirection.X, -8.29f, duration, Ease.OutQuad);
                        moveTween02.onComplete += () =>
                        {
                            PlayerManager.Instance.UpdateCurrentTask("release2");

                            ChangeCableNumberInCar(0);
                            view["uiPanel/InteractiveUI/GoEndGuide"].SetActive(true);
                            view["MoveMark"]?.SetActive(true);
                            DelayFunc(5f, () =>
                            {
                                view["uiPanel/InteractiveUI/GoEndGuide"]?.SetActive(false);
                            });
                        };
                        
                        PlayerManager.Instance.EyeFadeBright();
                    });
                });
            view["MoveMark"]?.GetComponent<TriggerEvent>().enterTriggerEvent.AddListener(
                ()=>
                {
                    view["uiPanel/InteractiveUI/End"].SetActive(true);
                    view["uiPanel/InteractiveUI/PopText02"].SetActive(true);
                    view["uiPanel/InteractiveUI/GoEndGuide"]?.SetActive(false);
                    view["MoveMark"]?.SetActive(false);
                });
            view["uiPanel/InteractiveUI/End"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    view["uiPanel/InteractiveUI/End"].SetActive(false);
                    view["uiPanel/InteractiveUI"].SetActive(false);
                    view["uiPanel/OperationTips"].SetActive(false);
                    ReleaseCableComplete();
                });
        }
        /// <summary>
        /// 放轴完成
        /// </summary>
        void ReleaseCableComplete()
        {
            PlayerManager.Instance.EyeFadeDark();
            DelayFunc(0.5f, () =>
            {
                GOManager.Instance.RemoveGO(name);

                GOManager.Instance.ShowGO3D("HarnessObiOperate");
            });
        }
        /// <summary>
        /// 设置子物体激活状态
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="state"></param>
        void SetChilderActive(String objName, bool state)
        {
            foreach (Transform child in view[objName].transform)
            {
                child.gameObject.SetActive(state);
            }
        }
        /// <summary>
        /// 改变车上线缆数量
        /// </summary>
        /// <param name="number"></param>
        void ChangeCableNumberInCar(int number)
        {
            SetChilderActive("Vehicle/spool/down", false);
            SetChilderActive("Vehicle/spool", false);
            view["Vehicle/spool/roller"].SetActive(true);
            switch (number)
            {
                case 0:
                    SetChilderActive("Vehicle/spool", false);
                    view["Vehicle/spool/roller"].SetActive(true);
                    break;
                case 1:
                    view["Vehicle/spool/down"].SetActive(true);
                    view["Vehicle/spool/down/01"].SetActive(true);
                    break;
                case 2:
                    view["Vehicle/spool/down"].SetActive(true);
                    view["Vehicle/spool/down/01"].SetActive(true);
                    view["Vehicle/spool/down/02"].SetActive(true);
                    break;
                case 8:
                    view["Vehicle/spool/up"].SetActive(true);
                    view["Vehicle/spool/other"].SetActive(true);
                    view["Vehicle/spool/down"].SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
