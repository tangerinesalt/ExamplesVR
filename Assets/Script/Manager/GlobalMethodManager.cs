/****************************************************
    功能：全局方法管理器
    作者：ZH
    创建日期：#2025/01/15#
    修改内容：
        1.VR物体加载封装    2025/03/07 ZH
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public class GlobalMethodManager : MonoSingleton<GlobalMethodManager>
    {
        private Coroutine coroutine;

        /// <summary>
        /// 延迟执行方法
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="unityAction"></param>
        public void DelayFunc(float delayTime, UnityAction unityAction)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = StartCoroutine(DelayAction(delayTime, unityAction));
        }

        private IEnumerator DelayAction(float delayTime, UnityAction unityAction)
        {
            yield return new WaitForSeconds(delayTime);
            unityAction?.Invoke();
            coroutine = null;
        }

        /// <summary>
        /// 加载VR对象
        /// </summary>
        public void LoadVRObj()
        {
            if (!GlobalDataManager.Instance.VRObj)
            {
                GameObject vrobj = ResManager.Instance.LoadPrefab("XR/VRObjAutoHand");

                if (vrobj != null)
                {
                    vrobj.name = "VRObj";
                    DontDestroyOnLoad(vrobj);
                    GlobalDataManager.Instance.VRObj = vrobj;
                }
            }
        }

        /// <summary>
        /// 项目初始化
        /// </summary>
        public void ProjectInit()
        {
            GlobalDataManager.Instance.eTrainType = ETrainType.None;
            PlayerManager.Instance.state = EInstallState.Uninstall;

            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;

            GlobalDataManager.Instance.VRObj.transform.Find("OpenXRAutoHandPlayer/TrackerOffsets").localEulerAngles = Vector3.zero;
            GlobalDataManager.Instance.VRObj.transform.Find("OpenXRAutoHandPlayer/TrackerOffsets/Camera (head)").localEulerAngles = Vector3.zero;

            PlayerManager.Instance.SetPosition(Vector3.zero);
            PlayerManager.Instance.SetRotation(Vector3.zero);
            PlayerManager.Instance.EnableMove = false;
            PlayerManager.Instance.EnableTurn = false;
            // PlayerManager.Instance.moveMode = EMoveMode.MovebyPhysics;
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetRayState(EHandType.Both, true);
            PlayerManager.Instance.EyeFadeBright();

            PlayerManager.Instance.isCanOpenMainMenu = false;
        }

        /// <summary>
        /// 关闭交互
        /// </summary>
        public void CloseInteration()
        {
            PlayerManager.Instance.EnableMove = false;
            PlayerManager.Instance.EnableTurn = false;
            PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);
            PlayerManager.Instance.EyeFadeDark();
        }

        /// <summary>
        /// 教学场景初始化
        /// </summary>
        public void TeachSceneInit()
        {
            PlayerManager.Instance.SetPosition(Vector3.zero);
            PlayerManager.Instance.SetRotation(new Vector3(0, -180, 0));

            PlayerManager.Instance.SetHandModelState(EHandType.Both, false);
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, true);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);

            Camera.main.clearFlags = CameraClearFlags.Skybox;
            PlayerManager.Instance.EyeFadeBright();

            //测试临时打开
            // PlayerManager.Instance.EnableMove = true;
        }

        /// <summary>
        /// 训练场景初始化
        /// </summary>
        public void TrainSceneInit()
        {
            PlayerManager.Instance.SetPosition(Vector3.zero);
            PlayerManager.Instance.SetRotation(Vector3.zero);

            PlayerManager.Instance.SetHandModelState(EHandType.Both, true);
            PlayerManager.Instance.SetControllerModelState(EHandType.Both, false);
            PlayerManager.Instance.SetRayState(EHandType.Both, false);

            Camera.main.clearFlags = CameraClearFlags.Skybox;
            PlayerManager.Instance.EyeFadeBright();

            PlayerManager.Instance.EnableMove = true;
            PlayerManager.Instance.EnableTurn = false;
            PlayerManager.Instance.isCanOpenMainMenu = true;
            PlayerManager.Instance.isCanOpenMainMenuLeft = true;
            PlayerManager.Instance.isCanOpenMainMenuRight = true;
        }
    }
}