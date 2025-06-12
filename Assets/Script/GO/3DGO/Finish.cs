/****************************************************
    功能：结束训练物体
    作者：ZH
    创建日期：#2025/03/24#
    修改内容：
        1.增加训练结束界面和俯瞰平台    2025/03/24 ZH
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Voltage
{
    public class Finish : GO3DBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            Init();
            InitUI();
        }

        private void Init()
        {
            PlayerManager.Instance.SetPosition(new Vector3(-1f, 0, -15f));
            PlayerManager.Instance.SetRotation(Vector3.zero);

            PlayerManager.Instance.EnableMove = true;
            // PlayerManager.Instance.EnableTurn = true;
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetRayState(EHandType.Both, true);
            PlayerManager.Instance.SetReachDistance(0.8f);
            PlayerManager.Instance.SetHandProjectorState(true);
            PlayerManager.Instance.EyeFadeBright();

            PlayerManager.Instance.state = EInstallState.Install;
        }

        private void InitUI()
        {
            Transform block = GameObject.Find("50_Normal").transform;

            view["uiPanel/Step1/btn_yes"].GetComponent<Button>().onClick.AddListener(() =>
            {
                view["uiPanel/Step1"].SetActive(false);
                view["Platform"].SetActive(true);
                PlayerManager.Instance.EyeFadeDark();

                DelayFunc(0.5f, () =>
                {
                    Destroy(block.gameObject);
                    ResManager.Instance.LoadPrefab("GO/50_Simple");

                    view["Celebration Effect"].SetActive(false);
                    PlayerManager.Instance.moveMode = EMoveMode.MoveByKinematic;
                    PlayerManager.Instance.SetPosition(new Vector3(20f, 5f, 13.5f));
                    PlayerManager.Instance.SetRotation(new Vector3(0, 180f, 0));
                    PlayerManager.Instance.EyeFadeBright();

                    DelayFunc(10f, () =>
                    {
                        PlayerManager.Instance.EyeFadeDark();
                        PlayerManager.Instance.EnableMove = false;
                        PlayerManager.Instance.EnableTurn = false;
                        PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
                        PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
                        PlayerManager.Instance.SetRayState(EHandType.Both, false);

                        DelayFunc(0.5f, () =>
                        {
                            GOManager.Instance.RemoveAll(EGOType.GO3D);
                            GOManager.Instance.RemoveAll(EGOType.UI3D);
                            SceneServiceManager.Instance.LoadScene("02_Menu");
                        });
                    });
                });
            });
            view["uiPanel/Step1/btn_no"].GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerManager.Instance.EyeFadeDark();
                PlayerManager.Instance.moveMode = EMoveMode.MoveByKinematic;
                PlayerManager.Instance.EnableMove = false;
                PlayerManager.Instance.EnableTurn = false;
                PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
                PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
                PlayerManager.Instance.SetRayState(EHandType.Both, false);

                DelayFunc(0.5f, () =>
                {
                    GOManager.Instance.RemoveAll(EGOType.GO3D);
                    GOManager.Instance.RemoveAll(EGOType.UI3D);
                    SceneServiceManager.Instance.LoadScene("02_Menu");
                });
            });
        }
    }
}