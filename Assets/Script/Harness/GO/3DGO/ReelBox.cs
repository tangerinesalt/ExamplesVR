/****************************************************
    功能：选轴、箱子相关
    作者：ZH
    创建日期：#2025/02/24#
    修改内容：
        1.合并轴箱相关    2025/03/20 ZH
        2.增加上车相关    2025/03/21 ZH
        3.解决放动画时移动bug    2025/03/22 ZZQ
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Autohand;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.InputSystem;

namespace Voltage
{
    public class ReelBox : GO3DBase
    {
        [Header("Input")]
        [SerializeField] private InputActionReference m_OpenLeftPanel;

        private string stepRef = string.Empty;

        private Grabbable correctReel;
        private Grabbable correctBox;
        private VideoPlayer driveCarVideo;
        private VideoPlayer releaseReelVideo;

        private bool isReelFirst = false;
        private bool isBoxFirst = false;

        protected override void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            InitUIElements();
            RegisterEvents();
            InitSelectButton();
            InitTriggerEvent();
            InitOtherButton();
            InitOther();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void InitUIElements()
        {
            correctReel = view["Reel/Reel3"].GetComponentInChildren<Grabbable>();
            correctBox = view["Box/Box3"].GetComponentInChildren<Grabbable>();

            driveCarVideo = view["PlayVideoSpace/Canvas/RawImageCar"].GetComponent<VideoPlayer>();
            releaseReelVideo = view["PlayVideoSpace/Canvas/RawImageReel"].GetComponent<VideoPlayer>();

            PlayerManager.Instance.UpdateCurrentTask("reel1");
        }

        private void RegisterEvents()
        {
            correctReel.onRelease.AddListener(HandleReelRelease);
            correctBox.onRelease.AddListener(HandleBoxRelease);

            driveCarVideo.loopPointReached += DriveCarVideoEnd;
            releaseReelVideo.loopPointReached += ReleaseReelVideoEnd;

            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.Enable();
                m_OpenLeftPanel.action.performed += ChangeLeftPanelStatus;
            }
        }

        private void UnregisterEvents()
        {
            correctReel.onRelease.RemoveListener(HandleReelRelease);
            correctBox.onRelease.RemoveListener(HandleBoxRelease);

            driveCarVideo.loopPointReached -= DriveCarVideoEnd;
            releaseReelVideo.loopPointReached -= ReleaseReelVideoEnd;

            if (m_OpenLeftPanel != null)
            {
                m_OpenLeftPanel.action.performed -= ChangeLeftPanelStatus;
            }
        }

        private void HandleReelRelease(Hand hand, Grabbable grabbable)
        {
            if (isCanReleaseReel&&!isReelFirst)
            {
                isReelFirst = true;
                view["uiPanel/Step2"].SetActive(false);
                view["uiPanel/Step3"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("reel3");

                PlayerManager.Instance.SetRayState(EHandType.Both, true);
            }
        }

        private bool isCanReleaseReel = false;
        private bool isCanReleaseBox = false;

        private void HandleBoxRelease(Hand hand, Grabbable grabbable)
        {
            if (isCanReleaseBox && !isBoxFirst)
            {
                isBoxFirst = true;
                view["uiPanel/Step7"].SetActive(false);
                view["uiPanel/Step8"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("box4");

                PlayerManager.Instance.SetRayState(EHandType.Both, true);
            }
        }

        private void InitSelectButton()
        {
            SetupButton("uiPanel/ReelSelectButton/btn01", false);
            SetupButton("uiPanel/ReelSelectButton/btn02", false);
            SetupButton("uiPanel/ReelSelectButton/btn03", true);
            SetupButton("uiPanel/ReelSelectButton/btn04", false);

            SetupButton("uiPanel/BoxSelectButton/btn01", false);
            SetupButton("uiPanel/BoxSelectButton/btn02", false);
            SetupButton("uiPanel/BoxSelectButton/btn03", true);
            SetupButton("uiPanel/BoxSelectButton/btn04", false);

            SetupButton("uiPanel/CBXSelectButton/btn01/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn02/btn", true);
            SetupButton("uiPanel/CBXSelectButton/btn03/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn04/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn05/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn06/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn07/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn08/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn09/btn", false);
            SetupButton("uiPanel/CBXSelectButton/btn10/btn", false);

            view["uiPanel/ReelSelectButton/btn03"].GetComponent<Button>().onClick.AddListener(() =>
            {
                DelayFunc(0.5f, () =>
                {
                    view["uiPanel/Step2"].SetActive(false);
                    isReelFirst = true;
                    view["uiPanel/Step3"].SetActive(false);
                    view["uiPanel/Step4"].SetActive(true);
                    view["uiPanel/ReelSelectButton"].SetActive(false);

                    PlayerManager.Instance.UpdateCurrentTask("reel4");
                });
            });

            view["uiPanel/BoxSelectButton/btn03"].GetComponent<Button>().onClick.AddListener(() =>
            {
                DelayFunc(0.5f, () =>
                {
                    view["uiPanel/Step7"].SetActive(false);
                    isBoxFirst = true;
                    view["uiPanel/Step8"].SetActive(false);
                    view["uiPanel/Step9"].SetActive(true);
                    view["uiPanel/BoxSelectButton"].SetActive(false);

                    PlayerManager.Instance.UpdateCurrentTask("box5");
                });
            });

            view["uiPanel/CBXSelectButton/btn02/btn"].GetComponent<Button>().onClick.AddListener(() =>
            {
                DelayFunc(0.5f, () =>
                {
                    view["uiPanel/CBXSelectButton"].SetActive(false);

                    PlayerStateDark();
                    DelayFunc(0.5f, () =>
                    {
                        PlayerManager.Instance.SetPosition(new Vector3(0, -20, 0));
                        PlayerManager.Instance.SetRotation(Vector3.zero);

                        PlayerManager.Instance.EyeFadeBright();
                        DelayFunc(0.5f, () =>
                        {
                            releaseReelVideo.gameObject.SetActive(true);
                            view["PlayVideoSpace/Canvas/Text (TMP)"].SetActive(true);
                        });
                    });
                });
            });
        }

        private void SetupButton(string buttonName, bool isRight)
        {
            GameObject feedbackObject = isRight ? view[$"{buttonName}/Right"] : view[$"{buttonName}/Wrong"];
            CanvasGroup canvasGroup = feedbackObject.GetOrAddComponent<CanvasGroup>();

            view[$"{buttonName}"].GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerManager.Instance.PlaySound(isRight);
                canvasGroup.alpha = 0;
                feedbackObject.SetActive(true);
                canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
                {
                    feedbackObject.SetActive(false);
                });
            });
        }

        private void InitTriggerEvent()
        {
            view["Trigger/ReelHideTrigger"].GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
            {
                view["Trigger/ReelHideTrigger"].SetActive(false);
                view["uiPanel/Step1"].SetActive(false);
                view["uiPanel/Step2"].SetActive(true);
                view["uiPanel/ReelSelectButton"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("reel2");
                isCanReleaseReel = true;
            });

            view["Trigger/BoxHideTrigger"].GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
            {
                view["Trigger/BoxHideTrigger"].SetActive(false);
                view["uiPanel/Step5"].SetActive(false);
                view["uiPanel/Step6"].SetActive(false);
                view["uiPanel/Step7"].SetActive(true);
                view["uiPanel/BoxSelectButton"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("box3");
                isCanReleaseBox = true;
            });
            view["Trigger/CarHideTrigger"].GetOrAddComponent<TriggerEvent>().enterTriggerEvent.AddListener(() =>
            {
                view["Trigger/CarHideTrigger"].SetActive(false);
                view["uiPanel/Step10"].SetActive(false);
                view["uiPanel/Step11"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("car2");

                PlayerManager.Instance.SetRayState(EHandType.Both, true);
            });
        }

        private void InitOtherButton()
        {
            view["uiPanel/Step4/btnPut"].GetComponent<Button>().onClick.AddListener(() =>
            {
                view["uiPanel/Step4"].SetActive(false);
                PlayerManager.Instance.SetRayState(EHandType.Both, false);
                PlayerManager.Instance.SetReachDistance(0.8f);

                DelayFunc(0.5f, () =>
                {
                    view["Reel/Reel3"].SetActive(false);
                    view["Vehicle/ReelPos/Reel"].SetActive(true);
                    view["uiPanel/Step5"].SetActive(true);

                    PlayerManager.Instance.UpdateCurrentTask("box1");
                });
            });

            view["uiPanel/Step5"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step5"].SetActive(false);
                view["uiPanel/Step6"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("box2");

                view["Trigger/BoxHideTrigger"].SetActive(true);

                view["Nav/BoxStart"].SetActive(true);
                PathFinding.Instance.SetNavigationTarget(view["Nav/BoxStart"].transform);
            });

            view["uiPanel/Step9/btnPut"].GetComponent<Button>().onClick.AddListener(() =>
            {
                view["uiPanel/Step9"].SetActive(false);
                PlayerManager.Instance.SetRayState(EHandType.Both, false);
                PlayerManager.Instance.SetReachDistance(0.8f);

                DelayFunc(0.5f, () =>
                {
                    view["Box/Box3"].SetActive(false);
                    view["Vehicle/BoxPos/Box"].SetActive(true);
                    view["uiPanel/Step10"].SetActive(true);

                    PlayerManager.Instance.UpdateCurrentTask("car1");

                    view["Trigger/CarHideTrigger"].SetActive(true);

                    view["Nav/CarStart"].SetActive(true);
                    PathFinding.Instance.SetNavigationTarget(view["Nav/CarStart"].transform);
                });
            });

            view["uiPanel/Step11/btnPut"].GetComponent<Button>().onClick.AddListener(() =>
            {
                view["uiPanel/Step11"].SetActive(false);
                PlayerManager.Instance.SetRayState(EHandType.Both, false);
                PlayerManager.Instance.SetReachDistance(0.8f);

                PlayerStateDark();
                DelayFunc(0.5f, () =>
                {
                    view["Reel"].SetActive(false);
                    view["Box"].SetActive(false);
                    PathFinding.Instance.ToggleNavigation(false);
                    view["Nav"].SetActive(false);

                    PlayerManager.Instance.SetPosition(new Vector3(0, -20, 0));
                    PlayerManager.Instance.SetRotation(Vector3.zero);

                    PlayerManager.Instance.EyeFadeBright();
                    DelayFunc(0.5f, () =>
                    {
                        driveCarVideo.gameObject.SetActive(true);
                        view["PlayVideoSpace/Canvas/Text (TMP)"].SetActive(true);
                    });
                });
            });

            view["uiPanel/Step12"].GetComponentInChildren<PressAnyKeyToContinue>().anyKeyPressedEvent.AddListener(() =>
            {
                view["uiPanel/Step12"].SetActive(false);
                view["uiPanel/Step13"].SetActive(true);
                stepRef = "Step13";
                PlayerManager.Instance.isCanOpenMainMenuRight = false;
                PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
                PlayerManager.Instance.SetControllerModelState(EHandType.Both, true);
                // view["uiPanel/CBXSelectButton"].SetActive(true);

                PlayerManager.Instance.UpdateCurrentTask("cbx2");
            });
        }

        private void DriveCarVideoEnd(VideoPlayer vp)
        {
            PlayerManager.Instance.EyeFadeDark();

            view["Vehicle"].transform.position = view["Pos/VehicleEndPos"].transform.position;
            view["Vehicle"].transform.eulerAngles = view["Pos/VehicleEndPos"].transform.eulerAngles;
            view["uiPanel/Step12"].SetActive(true);

            PlayerManager.Instance.UpdateCurrentTask("cbx1");

            DelayFunc(0.5f, () =>
            {
                view["PlayVideoSpace/Canvas/RawImageCar"].SetActive(false);
                view["PlayVideoSpace/Canvas/Text (TMP)"].SetActive(false);

                PlayerManager.Instance.SetPosition(new Vector3(-1f, 0, -15f));
                PlayerManager.Instance.SetRotation(new Vector3(0f, 0f, 0f));

                PlayerManager.Instance.moveMode = EMoveMode.MovebyPhysics;
                PlayerManager.Instance.EnableMove = true;
                // PlayerManager.Instance.EnableTurn = true;
                PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
                PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
                PlayerManager.Instance.SetReachDistance(1f);
                PlayerManager.Instance.SetHandProjectorState(true);
                PlayerManager.Instance.EyeFadeBright();
            });
        }

        private void ReleaseReelVideoEnd(VideoPlayer vp)
        {
            PlayerManager.Instance.EyeFadeDark();

            DelayFunc(0.5f, () =>
            {
                view["PlayVideoSpace/Canvas/RawImageReel"].SetActive(false);
                view["PlayVideoSpace/Canvas/Text (TMP)"].SetActive(false);

                GOManager.Instance.RemoveGO(name);
                GOManager.Instance.ShowGO3D("ReleaseCables");
                // GOManager.Instance.ShowGO3D("Finish");
            });
        }

        private void PlayerStateDark()
        {
            PlayerManager.Instance.moveMode = EMoveMode.MoveByKinematic;
            PlayerManager.Instance.EnableMove = false;
            PlayerManager.Instance.EnableTurn = false;
            PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);
            PlayerManager.Instance.EyeFadeDark();
        }

        private void InitOther()
        {
            view["Nav/ReelStart"].SetActive(true);
            PathFinding.Instance.SetNavigationTarget(view["Nav/ReelStart"].transform);
        }

        private void ChangeLeftPanelStatus(InputAction.CallbackContext context)
        {
            switch (stepRef)
            {
                case "Step13":
                    view["uiPanel/Step13"].SetActive(false);
                    stepRef = "Step13_1";
                    break;
                case "Step13_1":
                    view["uiPanel/CBXSelectButton"].SetActive(true);
                    stepRef = "Step14";
                    PlayerManager.Instance.isCanOpenMainMenuRight = true;
                    PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
                    PlayerManager.Instance.SetRayState(EHandType.Both, true);
                    PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
                    break;
            }
        }
    }
}