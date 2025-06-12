/****************************************************
    功能：线缆(Obi)交互操作
    作者：ZZQ
    创建日期：#2025/03/28#
    修改内容：
                1.代码优化 ZZQ #2025/04/09#
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Voltage
{

    public class HarnessObiOperate : GO3DBase
    {
        /// <summary>
        /// 线缆的显隐交互部分
        /// </summary>
        private WindhubObjectInteractiveProcessing ObjectProcessing;
        /// <summary>
        /// 放置线圈的交互部分
        /// </summary>
        private CableRingPlace cableRingPlace;
        protected override void Awake()
        {
            base.Awake();
        }
        void Start()
        {
            InitPlayer();
            InitElement();
            InitObiInteraction();
            InitInteractiveUI();

            StartHangCable01();//默认步骤-->挂轴01N
        }
        private void InitPlayer()
        {
            PlayerManager.Instance.SetPosition(new Vector3(392f, 0, -39f));
            PlayerManager.Instance.SetRotation(Vector3.zero);
            PlayerManager.Instance.EnableMove = true;
            // PlayerManager.Instance.EnableTurn = true;
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);
            PlayerManager.Instance.SetReachDistance(0.8f);
            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.3f, 0.3f));
            PlayerManager.Instance.SetHandProjectorState(true);
            PlayerManager.Instance.EyeFadeBright();
            PathFinding.Instance.UpdateTransform();
        }
        private void InitElement()
        {
            ObjectProcessing = (WindhubObjectInteractiveProcessing)GOManager.Instance.ShowGO3D("WindhubObjectInteractiveProcessing");
            //obi元素
            SetChildActive(this.gameObject, true);
            view["ExternalSocket"].SetActive(false);
            SetChildActive(view["Obi Solver"], false, true);
            SetChildActive(view["Attachment"], false, true);
            //UI元素
            view["uiPanel/OperationTips"].SetActive(true);
            SetChildActive(view["uiPanel/OperationTips"], false);
            view["uiPanel/InteractiveUI"].SetActive(true);
            SetChildActive(view["uiPanel/InteractiveUI"], false);
        }
        void InitObiInteraction()
        {
            #region 线缆挂轴(01N)、连接
            //线缆挂轴
            view["ExternalSocket/CableControlPoint_HalfN1/ControlPoint 1"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 1", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 1", false);
                    //UI
                    view["uiPanel/OperationTips/HangCable01"].SetActive(false);
                    view["uiPanel/OperationTips/HangCable02"].SetActive(true);
                    view["uiPanel/OperationTips/GoHangCable02Tip"].SetActive(true);
                    DelayFunc(3f, () => { view["uiPanel/OperationTips/GoHangCable02Tip"].SetActive(false); });
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 2", true);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 2", true);
                }
            );
            view["ExternalSocket/CableControlPoint_HalfN1/ControlPoint 2"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 2", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 2", false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 3", true);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 3", true);
                }
            );
            view["ExternalSocket/CableControlPoint_HalfN1/ControlPoint 3"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 3", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 3", false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 4", true);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 4", true);
                }
            );
            view["ExternalSocket/CableControlPoint_HalfN1/ControlPoint 4"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 4", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 4", false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 5", true);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 5", true);
                }
            );
            view["ExternalSocket/CableControlPoint_HalfN1/ControlPoint 5"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("hang2");
                    
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 5", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 5", false);
                    //自动连接
                    view["Attachment/Cable/CableControlPoint_HalfN1"].GetComponent<AutoConnection_Parallel>().AutoConnect();
                    //UI
                    view["uiPanel/OperationTips/HangCable02"].SetActive(false);
                    view["uiPanel/OperationTips/ConnectCable01"].SetActive(true);
                    //寻路
                    view["Nav/EndPoint"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["Nav/EndPoint"].transform);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //拓展线缆(行尾)连接_引导
                    SetObiActive("PanelCable_CableEnd", true);
                    StepGrabAndFlickerState("Attachment/PanelCable_CableEnd/CableEnd/Panel Cable N1", true);
                    StepGrabAndFlickerState("Attachment/Cable/B1S1_HalfN/HomerunExtender", true);
                }
            );
            view["Attachment/PanelCable_CableEnd/CableEnd/Panel Cable N1"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/PanelCable_CableEnd/CableEnd/Panel Cable N1", false);
                    StepGrabAndFlickerState("Attachment/Cable/B1S1_HalfN/HomerunExtender", false);
                    //（半）N极挂轴完成->P极挂轴（电机位置）
                    Vector3 position = new Vector3(0.05424119f, 1.850178f, 38.56562f);
                    Vector3 rotation = new Vector3(97.5f, 90f, 0f);
                    MovePoint("Attachment/PanelCable_CableEnd/CableEnd/Panel Cable N1", position, rotation);
                    MovePoint("Attachment/Cable/B1S1_HalfN/HomerunExtender", "Attachment/PanelCable_CableEnd/CableEnd/Panel Cable N1");
                    //UI
                    view["uiPanel/OperationTips/ConnectCable01"].SetActive(false);
                    //关闭寻路
                    view["Nav/EndPoint"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    DelayFunc(0.5f, StartHangCable02);//延迟0.5秒后开始挂轴02P
                    view["uiPanel/InteractiveUI/GoToHangCable02P"].SetActive(true);//快速跳转到挂轴02P
                }
            );
            #endregion

            #region 挂轴(02P)
            view["ExternalSocket/CableControlPoint_HalfN2/ControlPoint 40"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN2/ControlPoint 40", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN2/ControlPoint 40", false);
                    //UI
                    view["uiPanel/OperationTips/HangCable03"].SetActive(false);
                    view["uiPanel/OperationTips/HangCable04"].SetActive(true);
                    view["uiPanel/OperationTips/GoHangCable04Tip"].SetActive(true);
                    DelayFunc(3f, () => { view["uiPanel/OperationTips/GoHangCable04Tip"].SetActive(false); });
                    //寻路
                    view["Nav/NXPoint"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["Nav/NXPoint"].transform);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN2/ControlPoint 41", true);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN2/ControlPoint 41", true);
                }
            );
            view["ExternalSocket/CableControlPoint_HalfN2/ControlPoint 41"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN2/ControlPoint 41", false);
                    StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN2/ControlPoint 41", false);

                    //线缆挂轴结束
                    view["Attachment/Cable/CableControlPoint_HalfN2"].GetComponent<AutoConnection_Parallel>().AutoConnect();
                    Debug.Log("线缆挂轴完成");
                    //UI
                    view["uiPanel/OperationTips/HangCable04"].SetActive(false);
                    //关闭寻路
                    view["Nav/NXPoint"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    DelayFunc(5f, StartPanelCable);//板间线连接
                    view["uiPanel/InteractiveUI/GoToPanelCable"].SetActive(true);//快速跳转到板间线连接位置
                }
            );
            #endregion

            #region 板间线连接
            view["Attachment/BranchCable/BranchCable"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("panelcable2");
                    
                    SetObiActive("PanelCable/ExtenderConnection_B3S2", false);
                    SetObiActive("BranchCable", false);
                    ObjectProcessing.view["NotInstall/PanelCable_Install/Top"].gameObject.SetActive(true);
                    //UI
                    view["uiPanel/OperationTips/ConnectCable02"].SetActive(false);
                    view["uiPanel/OperationTips/ConnectPanelCable01"].SetActive(true);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //板间线01连接_引导
                    StepGrabAndFlickerState("Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_01/Panel Cable N", true);
                    StepGrabAndFlickerState("Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_01/Panel Cable P", true);
                }
            );
            view["Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_01/Panel Cable N"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    SetObiActive("PanelCable/PanelCable_B3S2/Panel_Cable_01", false);
                    ObjectProcessing.view["NotInstall/PanelCable_Install/PanelCable01"].gameObject.SetActive(true);
                    //UI
                    view["uiPanel/OperationTips/ConnectPanelCable01"].SetActive(false);
                    view["uiPanel/OperationTips/ConnectPanelCable02"].SetActive(true);
                    //寻路
                    view["Nav/PanelCable"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["Nav/PanelCable"].transform);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //板间线02引导
                    StepGrabAndFlickerState("Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_02/Panel Cable N", true);
                    StepGrabAndFlickerState("Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_02/Panel Cable P", true);
                }
            );
            view["Attachment/PanelCable/PanelCable_B3S2/Panel_Cable_02/Panel Cable N"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("placewhip0");

                    SetObiActive("PanelCable/PanelCable_B3S2/Panel_Cable_02", false);
                    SetObiActive("PanelCable", false);
                    ObjectProcessing.view["NotInstall/PanelCable_Install/PanelCable02"].gameObject.SetActive(true);

                    //板间线结束
                    ObjectProcessing.UpdateState(EWindhubObjectState.PanelCableInstall);
                    Debug.Log("板间线连接完成");
                    //UI
                    view["uiPanel/OperationTips/ConnectPanelCable02"].SetActive(false);
                    view["uiPanel/OperationTips/GoBoxTip01"].SetActive(true);
                    DelayFunc(3f, () => { view["uiPanel/OperationTips/GoBoxTip01"].SetActive(false); });
                    //关闭寻路
                    view["Nav/PanelCable"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步_第一排的线圈交互
                    cableRingPlace = (CableRingPlace)GOManager.Instance.ShowGO3D("CableRingPlace");//线圈放置预制体-->引导whip_string01放置
                    cableRingPlace.StartCableRingPlace01();
                }
            );
            #endregion

            #region whip_string01放置
            view["Attachment/WhipPlace/String01/Frame_Socket"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndRendererActive("Attachment/WhipPlace/String01/Frame_Socket", false);
                    StepGrabAndRendererActive("Attachment/WhipPlace/String01/Luminous_Obi", false);

                    //放置完成
                    SetObiActive("WhipPlace/String01", false);
                    //UI
                    view["uiPanel/OperationTips/PlaceString01Whip"].SetActive(false);
                    view["uiPanel/OperationTips/PlaceWhipExplain"].SetActive(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    DelayFunc(0.01f, StartWhip01);//whip_string01连接
                }
            );
            #endregion

            #region 连接Whip01
            view["Attachment/WhipConnect_String01/Whip01/Whip_Obi 7"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip01/Whip_Obi 7", false);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Cable/CableTop_Obi 7", false);
                    Vector3 position = new Vector3(0.14f, 1.85f, -0.7f);
                    Vector3 rotation = new Vector3(0f, 270f, 0f);
                    MovePoint("Attachment/WhipConnect_String01/Cable/CableTop_Obi 7", position, rotation);
                    MovePoint("Attachment/WhipConnect_String01/Whip01/Whip_Obi 7", "Attachment/WhipConnect_String01/Cable/CableTop_Obi 7");
                    //UI_下一步关闭
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Cable/CableTop_Obi 4", true);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip01/Whip_Obi 4", true);
                }
            );
            view["Attachment/WhipConnect_String01/Cable/CableTop_Obi 4"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Cable/CableTop_Obi 4", false);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip01/Whip_Obi 4", false);
                    Vector3 position = new Vector3(0.1232f, 1.85f, -0.85f);
                    Vector3 rotation = new Vector3(0f, 270f, 0f);
                    MovePoint("Attachment/WhipConnect_String01/Cable/CableTop_Obi 4", position, rotation);
                    MovePoint("Attachment/WhipConnect_String01/Whip01/Whip_Obi 4", "Attachment/WhipConnect_String01/Cable/CableTop_Obi 4");

                    //Whip01连接完成
                    SetObiActive("WhipConnect_String01/Whip01", false);
                    SetObiActive("WhipConnect_String01/Cable", false);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install/Whip01"].gameObject.SetActive(true);
                    ObjectProcessing.view["WindhubCable01/TopCable"].gameObject.SetActive(true);
                    Debug.Log("Whip01连接完成");
                    //UI
                    view["uiPanel/OperationTips/ConnectString01Whip"].SetActive(false);
                    view["uiPanel/OperationTips/GoCBXTip01"].SetActive(true);
                    DelayFunc(3f, () => { view["uiPanel/OperationTips/GoCBXTip01"].SetActive(false); });
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    DelayFunc(0.1f, StartCBX01);//CBX_String01连接
                }
            );
            #endregion

            #region 连接CBX01
            //测试_CBX连接
            view["Attachment/CBXConnect/CBXCable/CBXCable01/CBXCableP02"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip02/WhipCable01/CBXCableP02", false);
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable01/CBXCableP02", false);
                    SetObiActive("CBXConnect/CBXCable/CBXCable01", false);
                    SetObiActive("WhipConnect_String01/Whip02/WhipCable01", false);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install/Whip02"].gameObject.SetActive(true);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install/Whip02/WhipCable01"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable/bundle01"].gameObject.SetActive(true);
                    //UI_下一步关闭
                    //关闭寻路
                    view["Nav/CBX01"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip02/WhipCable02/CBXCableN02", true);
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable02/CBXCableN02", true);
                }
            );
            view["Attachment/WhipConnect_String01/Whip02/WhipCable02/CBXCableN02"].GetComponent<SocketBase>().m_afterConnection.AddListener(
               () =>
                {
                    PlayerManager.Instance.UpdateCurrentTask("placewhip0");
                    StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip02/WhipCable02/CBXCableN02", false);
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable02/CBXCableN02", false);
                    SetObiActive("CBXConnect/CBXCable/CBXCable02", false);
                    SetObiActive("WhipConnect_String01/Whip02/WhipCable02", false);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install/Whip02"].gameObject.SetActive(true);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install/Whip02/WhipCable02"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable/bundle02"].gameObject.SetActive(true);

                    //CBX01连接完成
                    SetObiActive("WhipConnect_String01/Whip02", false);
                    SetObiActive("WhipConnect_String01", false);
                    ObjectProcessing.view["WindhubCable01/WindhubCable01_Install"].gameObject.SetActive(false);
                    ObjectProcessing.view["WindhubCable01/Whip"].gameObject.SetActive(true);
                    Debug.Log("CBX01连接完成");
                    //UI
                    view["uiPanel/OperationTips/ConnectString01CBX"].SetActive(false);
                    view["uiPanel/OperationTips/GoBoxTip02"].SetActive(true);
                    DelayFunc(3f, () => { view["uiPanel/OperationTips/GoBoxTip02"].SetActive(false); });
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步_第二排的线圈交互
                    if (cableRingPlace == null) {cableRingPlace = (CableRingPlace)GOManager.Instance.ShowGO3D("CableRingPlace");}
                    DelayFunc(0.1f, () =>
                    { cableRingPlace.StartCableRingPlace02(); });//线圈放置预制体-->引导whip_string02放置
                }
            );
            #endregion

            #region whip_string02放置
            view["Attachment/WhipPlace/String02/Frame_Socket"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipPlace/String02/WhipPlaceP_female_Obi01", false);
                    StepGrabAndFlickerState("Attachment/WhipPlace/String02/WhipPlaceN_male_Obi01", false);
                    StepGrabAndFlickerState("Attachment/WhipPlace/String02/Frame_Socket", false);

                    //放置完成
                    SetObiActive("WhipPlace/String02", false);
                    //UI
                    view["uiPanel/OperationTips/PlaceString02Whip"].SetActive(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    DelayFunc(0.01f, StartWhip02);//whip_string02连接
                }
            );
            #endregion

            #region 连接Whip02
            view["Attachment/WhipConnect_String02/Whip01/Whip_Obi 7"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip01/Whip_Obi 7", false);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Cable/CableTop_Obi 7", false);
                    Vector3 position = new Vector3(0.1462f, 1.85f, -0.74f);
                    Vector3 rotation = new Vector3(0f, 270f, 0f);
                    MovePoint("Attachment/WhipConnect_String02/Cable/CableTop_Obi 7", position, rotation);
                    MovePoint("Attachment/WhipConnect_String02/Whip01/Whip_Obi 7", "Attachment/WhipConnect_String02/Cable/CableTop_Obi 7");
                    //UI_下一步关闭
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Cable/CableTop_Obi 4", true);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip01/Whip_Obi 4", true);
                }
            );
            view["Attachment/WhipConnect_String02/Cable/CableTop_Obi 4"].GetComponent<SocketBase>().m_afterConnection.AddListener(
               () =>
               {
                   StepGrabAndFlickerState("Attachment/WhipConnect_String02/Cable/CableTop_Obi 4", false);
                   StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip01/Whip_Obi 4", false);
                   Vector3 position = new Vector3(0.1232f, 1.85f, -0.896f);
                   Vector3 rotation = new Vector3(0f, 270f, 0f);
                   MovePoint("Attachment/WhipConnect_String02/Cable/CableTop_Obi 4", position, rotation);
                   MovePoint("Attachment/WhipConnect_String02/Whip01/Whip_Obi 4", "Attachment/WhipConnect_String02/Cable/CableTop_Obi 4");

                   //Whip02连接完成
                   SetObiActive("WhipConnect_String02/Whip01", false);
                   SetObiActive("WhipConnect_String02/Cable", false);
                   ObjectProcessing.view["WindhubCable02/WindhubCable02_Install/Whip01"].gameObject.SetActive(true);
                   ObjectProcessing.view["WindhubCable02/TopCable"].gameObject.SetActive(true);
                   Debug.Log("Whip02连接完成");
                   //UI
                   view["uiPanel/OperationTips/ConnectString02Whip"].SetActive(false);
                   view["uiPanel/OperationTips/GoCBXTip02"].SetActive(true);
                   DelayFunc(3f, () => { view["uiPanel/OperationTips/GoCBXTip02"].SetActive(false); });
                   //音效
                   PlayerManager.Instance.PlaySound(true);
                   //下一步
                   DelayFunc(0.5f, StartCBX02);//CBX_String02连接
               }
            );
            #endregion
            #region 连接CBX02
            view["Attachment/CBXConnect/CBXCable/CBXCable03/CBXCableP01"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable03/CBXCableP01", false);
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip02/WhipCable03/CBXCableP01", false);
                    SetObiActive("CBXConnect/CBXCable/CBXCable03", false);
                    SetObiActive("WhipConnect_String02/Whip02/WhipCable03", false);
                    ObjectProcessing.view["WindhubCable02/WindhubCable02_Install/Whip02"].gameObject.SetActive(true);
                    ObjectProcessing.view["WindhubCable02/WindhubCable02_Install/Whip02/WhipCable03"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable/bundle03"].gameObject.SetActive(true);
                    //关闭寻路
                    view["Nav/CBX02"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    //音效
                    PlayerManager.Instance.PlaySound(true);
                    //引导
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip02/WhipCable04/CBXCableN01", true);
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable04/CBXCableN01", true);
                }
            );
            view["Attachment/WhipConnect_String02/Whip02/WhipCable04/CBXCableN01"].GetComponent<SocketBase>().m_afterConnection.AddListener(
                () =>
                {
                    StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip02/WhipCable04/CBXCableN01", false);
                    StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable04/CBXCableN01", false);
                    SetObiActive("CBXConnect/CBXCable/CBXCable04", false);
                    SetObiActive("WhipConnect_String02/Whip02/WhipCable04", false);
                    ObjectProcessing.view["WindhubCable02/WindhubCable02_Install"].gameObject.SetActive(false);
                    ObjectProcessing.view["WindhubCable02/Whip"].gameObject.SetActive(true);
                    ObjectProcessing.view["CBXCable/bundle04"].gameObject.SetActive(true);

                    //CBX02连接完成
                    SetObiActive("WhipConnect_String02/Whip02", false);
                    SetObiActive("WhipConnect_String02", false);
                    Debug.Log("CBX02连接完成");
                    //UI
                    view["uiPanel/OperationTips/ConnectString02CBX"].SetActive(false);
                    PlayerManager.Instance.PlaySound(true);
                    //下一步
                    OnCompleate();
                }
            );
            #endregion
        }
        private void InitInteractiveUI()
        {
            view["uiPanel/InteractiveUI/GoToHangCable02P"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.EyeFadeDark();

                    DelayFunc(0.5f, () =>
                    {
                        view["uiPanel/InteractiveUI/GoToHangCable02P"].SetActive(false);
                        if (!isStartHandCable02) StartHangCable02();
                        PlayerManager.Instance.SetPosition(new Vector3(356.2f, 0, -39f));
                        PlayerManager.Instance.SetRotation(new Vector3(0, 0, 0));
                        PlayerManager.Instance.EyeFadeBright();
                    });
                }
            );
            view["uiPanel/InteractiveUI/GoToPanelCable"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(
                () =>
                {
                    PlayerManager.Instance.EyeFadeDark();

                    DelayFunc(0.5f, () =>
                    {
                        view["uiPanel/InteractiveUI/GoToPanelCable"].SetActive(false);
                        if (!isStartPanelCable) StartPanelCable();//板间线连接
                        PlayerManager.Instance.SetPosition(new Vector3(4.3f, 0, -38.5f));
                        PlayerManager.Instance.SetRotation(new Vector3(0, 0, 0));
                        PlayerManager.Instance.EyeFadeBright();
                    });
                }
            );
        }

        /// <summary>
        /// 主缆挂轴(01N)引导
        /// </summary>
        public void StartHangCable01()
        {
            PlayerManager.Instance.UpdateCurrentTask("hang1");

            SetObiActive("Cable", true);
            view["ExternalSocket"].SetActive(true);
            //引导
            StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN1/ControlPoint 1", true);
            StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN1/ControlPoint 1", true);
            //UI
            view["uiPanel/OperationTips/HangCable01"].SetActive(true);
            //任务
            PlayerManager.Instance.UpdateCurrentTask("hang1");
        }
        private bool isStartHandCable02 = false;
        /// <summary>
        /// 主缆挂轴(02P)引导
        /// </summary>
        public void StartHangCable02()
        {
            PlayerManager.Instance.UpdateCurrentTask("hang1");
            
            if (isStartHandCable02) return;//避免重复执行
            //电机处挂轴
            StepGrabAndRendererActive("Attachment/Cable/CableControlPoint_HalfN2/ControlPoint 40", true);
            StepGrabAndRendererActive("ExternalSocket/CableControlPoint_HalfN2/ControlPoint 40", true);
            //UI
            view["uiPanel/OperationTips/HangCable03"].SetActive(true);
            isStartHandCable02 = true;
        }
        private bool isStartPanelCable = false;
        /// <summary>
        /// 板间线连接引导
        /// </summary>
        public void StartPanelCable()
        {
            PlayerManager.Instance.UpdateCurrentTask("panelcable1");

            if (isStartPanelCable) return;//避免重复执行
            SetObiActive("Cable", false);
            SetObiActive("PanelCable_CableEnd", false);
            ObjectProcessing.UpdateState(EWindhubObjectState.CableInstall);

            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.2f, 0.2f));
            SetObiActive("PanelCable", true);
            SetObiActive("BranchCable", true);
            SetObiActive("WhipConnect_String01", true);
            SetObiActive("WhipConnect_String01/Cable", true);
            //板间线顶端连接引导
            StepGrabAndFlickerState("Attachment/BranchCable/BranchCable", true);
            StepGrabAndFlickerState("Attachment/PanelCable/ExtenderConnection_B3S2/Panel_Cable_P2", true);
            //UI
            view["uiPanel/OperationTips/ConnectCable02"].SetActive(true);
            isStartPanelCable = true;
        }
        /// <summary>
        /// Whip(String01)放置引导
        /// </summary>
        public void StartWhipPlace01()
        {
            PlayerManager.Instance.UpdateCurrentTask("placewhip2");

            ObjectProcessing.UpdateState(EWindhubObjectState.PanelCableInstall);

            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.3f, 0.3f));
            SetObiActive("WhipPlace", true);
            SetObiActive("WhipPlace/String01", true);
            //拾取引导
            StepGrabAndRendererActive("Attachment/WhipPlace/String01/Frame_Socket", true);
            StepGrabAndRendererActive("Attachment/WhipPlace/String01/Luminous_Obi", true);
            //UI
            view["uiPanel/OperationTips/PlaceString01Whip"].SetActive(true);
            view["uiPanel/OperationTips/PlaceWhipExplain"].SetActive(true);
        }
        /// <summary>
        /// Whip(String01)连接引导
        /// </summary>
        public void StartWhip01()
        {
            PlayerManager.Instance.UpdateCurrentTask("whip");

            ObjectProcessing.UpdateState(EWindhubObjectState.PanelCableInstall);
            
            SetObiActive("WhipConnect_String01", true);
            SetObiActive("WhipConnect_String01/Cable", true);
            SetObiActive("WhipConnect_String01/Whip01", true);
            SetObiActive("WhipConnect_String01/Whip02", true);
            view["Attachment/WhipConnect_String01/Ties"].SetActive(true);
            //某一whip连接cable引导（07）
            StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip01/Whip_Obi 7", true);
            StepGrabAndFlickerState("Attachment/WhipConnect_String01/Cable/CableTop_Obi 7", true);
            //UI
            view["uiPanel/OperationTips/ConnectString01Whip"].SetActive(true);
        }
        /// <summary>
        /// CBX(String01)连接引导
        /// </summary>
        public void StartCBX01()
        {
            PlayerManager.Instance.UpdateCurrentTask("cbx");

            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.2f, 0.2f));
            SetObiActive("CBXConnect", true);
            //某一whip连接CBX引导（02）
            StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable01/CBXCableP02", true);
            StepGrabAndFlickerState("Attachment/WhipConnect_String01/Whip02/WhipCable01/CBXCableP02", true);
            //UI
            view["uiPanel/OperationTips/ConnectString01CBX"].SetActive(true);
            //寻路
            view["Nav/CBX01"].SetActive(true);
            PathFinding.Instance.SetNavigationTarget(view["Nav/CBX01"].transform);
        }
        /// <summary>
        /// Whip(String02)放置引导
        /// </summary>
        public void StartWhipPlace02()
        {
            PlayerManager.Instance.UpdateCurrentTask("placewhip3");

            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.3f, 0.3f));
            SetObiActive("WhipPlace", true);
            SetObiActive("WhipPlace/String02", true);
            //引导
            StepGrabAndRendererActive("Attachment/WhipPlace/String02/Frame_Socket", true);
            StepGrabAndFlickerState("Attachment/WhipPlace/String02/WhipPlaceP_female_Obi01", true);
            StepGrabAndFlickerState("Attachment/WhipPlace/String02/WhipPlaceN_male_Obi01", true);
            //UI
            view["uiPanel/OperationTips/PlaceString02Whip"].SetActive(true);
        }
        /// <summary>
        /// Whip(String02)连接引导
        /// </summary>
        public void StartWhip02()
        {
            PlayerManager.Instance.UpdateCurrentTask("whip");

            SetObiActive("WhipConnect_String02", true);
            SetObiActive("WhipConnect_String02/Cable", true);
            SetObiActive("WhipConnect_String02/Whip01", true);
            //某一whip连接cable引导（07）
            StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip01/Whip_Obi 7", true);
            StepGrabAndFlickerState("Attachment/WhipConnect_String02/Cable/CableTop_Obi 7", true);
            //UI
            view["uiPanel/OperationTips/ConnectString02Whip"].SetActive(true);
        }
        /// <summary>
        /// CBX(String02)连接引导
        /// </summary>
        public void StartCBX02()
        {
            PlayerManager.Instance.UpdateCurrentTask("cbx");

            PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.2f, 0.2f));
            SetObiActive("WhipConnect_String02/Whip02", true);
            //某一whip连接CBX引导（03）
            StepGrabAndFlickerState("Attachment/CBXConnect/CBXCable/CBXCable03/CBXCableP01", true);
            StepGrabAndFlickerState("Attachment/WhipConnect_String02/Whip02/WhipCable03/CBXCableP01", true);
            //UI
            view["uiPanel/OperationTips/ConnectString02CBX"].SetActive(true);
            //寻路
            view["Nav/CBX02"].SetActive(true);
            PathFinding.Instance.SetNavigationTarget(view["Nav/CBX02"].transform);
        }
        /// <summary>
        /// 所有连接完成
        /// </summary>
        private void OnCompleate()
        {
            PlayerManager.Instance.EyeFadeDark();

            DelayFunc(0.5f, () =>
            {
                PlayerManager.Instance.SetCollisionPrecision(new Vector2(0.1f, 0.1f));

                GOManager.Instance.RemoveGO("WindhubObjectInteractiveProcessing");
                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("Finish");
            });
        }
        #region Tools
        /// <summary>
        /// 设置线缆的激活状态
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="state"></param>
        private void SetObiActive(string objectName, bool state)
        {
            view[$"Obi Solver/{objectName}"].SetActive(state);
            view[$"Attachment/{objectName}"].SetActive(state);
        }
        /// <summary>
        /// 设置物体的子物体的激活状态
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="active">子对象活跃状态</param>
        /// <param name="openPrompt">是否打印错误提示</param>
        private void SetChildActive(GameObject obj, bool active, bool openPrompt = false)
        {
            foreach (Transform child in obj.transform)
            {
                if (!active && openPrompt && child.gameObject.activeInHierarchy)
                {
                    Debug.LogError($"{obj.name} child is active , May cause errors", obj);
                }
                child.gameObject.SetActive(active);
            }
        }
        /// <summary>
        /// 设置物体的抓取和发光状态
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="state"></param>
        private void StepGrabAndFlickerState(string objectName, bool state)
        {
            GameObject obj = view[objectName];
            if (obj == null) return;

            if (obj.GetComponent<SocketBase>() != null) obj.GetComponent<SocketBase>().m_enable = state;
            obj.GetComponent<Grabbable_Voltage>()?.SetGrabbableState(state);
            obj.GetOrAddComponent<MaterialFlicker>().SetFlickerState(state);
        }
        /// <summary>
        /// 设置物体的抓取和渲染状态
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="state"></param>
        private void StepGrabAndRendererActive(string objectName, bool state)
        {
            GameObject obj = view[objectName];
            if (obj == null) return;

            if (obj.GetComponent<SocketBase>() != null) obj.GetComponent<SocketBase>().m_enable = state;
            obj.GetComponent<Grabbable_Voltage>()?.SetGrabbableState(state);
            obj.GetOrAddComponent<MaterialFlicker>().SetRendererActive(state);
        }
        /// <summary>
        /// 使对象移动到指定位置
        /// </summary>
        /// <param name="objectName">移动对象名称</param>
        /// <param name="position">相对于父对象位置偏移量</param>
        /// <param name="rotation">相对于父对象旋转角度</param>
        private void MovePoint(string objectName, Vector3 position, Vector3 rotation)
        {
            GameObject obj = view[objectName];
            if (obj == null) return;
            SocketPhysicsSetting(obj);

            obj.transform.localPosition = position;
            obj.transform.localEulerAngles = rotation;
        }
        /// <summary>
        /// 使对象移动到指定位置
        /// </summary>
        /// <param name="objectName">移动对象名称</param>
        /// <param name="targetName">目标对象名称</param>
        private void MovePoint(string objectName, string targetName)
        {
            GameObject obj = view[objectName];
            GameObject target = view[targetName];
            if (obj == null || target == null) return;
            SocketPhysicsSetting(obj);

            obj.transform.position = target.transform.position;
            obj.transform.rotation = target.transform.rotation;
        }
        private void SocketPhysicsSetting(GameObject obj)
        {
            SocketBase objsocket = obj.GetComponent<SocketBase>();
            if (objsocket != null)
            {
                objsocket.m_enable = false;
                objsocket.m_fixedAfterConnection = true;
            }
            obj.GetComponent<Grabbable_Voltage>()?.DisablePhysics();
        }
        #endregion
    }
}
