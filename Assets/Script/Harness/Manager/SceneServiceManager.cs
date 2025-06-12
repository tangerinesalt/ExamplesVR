/****************************************************
    功能：场景服务
    作者：ZH
    创建日期：#2025/01/08#
    修改内容：
        1.场景切换事件修改    2025/03/07 ZH
*****************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voltage                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
{
    public class SceneServiceManager : MonoSingleton<SceneServiceManager>
    {
        /// <summary>
        /// 要加载场景的名称
        /// </summary>
        private string sceneName;
        /// <summary>
        /// 加载进度
        /// </summary>
        [HideInInspector]
        public float loadValue;
        /// <summary>
        /// 加载进度文字
        /// </summary>
        [HideInInspector]
        public string str_LoadProcess;
        /// <summary>
        /// 进度条速度
        /// </summary>
        [HideInInspector]
        public float loadSpeed = 1;
        /// <summary>
        /// 进度条最终进度值
        /// </summary>
        private float loadTargetValue = 0;

        /// <summary>
        /// 当前的协程
        /// </summary>
        public Coroutine curt_Coroutine;

        public void Init()
        {
            AddSceneChangedEvent();
        }

        protected virtual void Destroy()
        {
            RemoveSceneChangedEvent();

            if (curt_Coroutine != null)
            {
                StopCoroutine(curt_Coroutine);
                curt_Coroutine = null;
            }
        }

        /// <summary>
        /// 添加场景切换事件
        /// </summary>
        private void AddSceneChangedEvent()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        /// <summary>
        /// 移除场景切换事件
        /// </summary>
        private void RemoveSceneChangedEvent()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        /// <summary>
        /// 场景切换业务
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Debug.Log($"********{arg0.name}切换到场景：{arg1.name}********");
            Utils.Unload_Collect();

            switch (arg1.name)
            {
                case "02_Menu":

                    GlobalMethodManager.Instance.LoadVRObj();
                    GlobalMethodManager.Instance.ProjectInit();
                    GOManager.Instance.ShowUI3D("StartUI");

                    break; 
                case "03_Teach":

                    GlobalMethodManager.Instance.TeachSceneInit();
                    GOManager.Instance.ShowUI3D("TeachStartUI");

                    break;
                case "04_Train":

                    GlobalMethodManager.Instance.TrainSceneInit();
                    GOManager.Instance.ShowUI3D("TrainStartUI");

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="_sceneName">场景名</param>
        /// <param name="_loadSpeed">加载速度</param>
        public void LoadScene(string _sceneName, float _loadSpeed = 10f)
        {
            sceneName = _sceneName;
            loadSpeed = _loadSpeed;

            loadValue = 0f;
            str_LoadProcess = string.Empty;
            loadTargetValue = 0f;

            if (curt_Coroutine != null)
            {
                StopCoroutine(curt_Coroutine);
                curt_Coroutine = null;
            }
            curt_Coroutine = StartCoroutine(LoadScene(sceneName));
        }

        /// <summary>
        /// 真实加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        private IEnumerator LoadScene(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                loadTargetValue = operation.progress;
                if (operation.progress >= 0.9f)
                {
                    loadTargetValue = 1;
                }

                if (!loadTargetValue.Equals(loadValue))
                {
                    loadValue = Mathf.Lerp(loadValue, loadTargetValue, Time.deltaTime * loadSpeed); //插值，让进度条流畅
                    yield return new WaitForEndOfFrame();
                    if (Mathf.Abs(loadValue - loadTargetValue) < 0.01f)
                    {
                        loadValue = loadTargetValue;
                        yield return new WaitForEndOfFrame();
                    }
                }

                str_LoadProcess = (int)(loadValue * 100) + "%";
                if ((int)(loadValue * 100) >= 99)
                {
                    operation.allowSceneActivation = true; //允许异步加载完毕后自动切换场景 
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}