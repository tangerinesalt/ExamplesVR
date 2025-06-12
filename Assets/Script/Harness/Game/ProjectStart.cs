/****************************************************
    功能：项目启动入口
    作者：ZH
    创建日期：#2025/01/08#
    修改内容：
        1.增加震动音效测试    2025/03/06 ZH
*****************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voltage
{
    public class ProjectStart : GameEntry<ProjectStart>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            Utils.DebugLog(Color.green, "项目启动");
            
            SceneServiceManager.Instance.LoadScene(GlobalDataManager.Instance.menuScene);
        }

        protected override void Update()
        {
            base.Update();

            if (SceneManager.GetActiveScene().name == "02_Menu")
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    GlobalDataManager.Instance.eTrainType = ETrainType.Teach;
                    GOManager.Instance.RemoveAll(EGOType.UI3D);
                    SceneServiceManager.Instance.LoadScene(GlobalDataManager.Instance.teachScene);
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    GlobalDataManager.Instance.eTrainType = ETrainType.Train;
                    GOManager.Instance.RemoveAll(EGOType.UI3D);
                    SceneServiceManager.Instance.LoadScene(GlobalDataManager.Instance.trainScene);
                }
            }
        }
    }
}