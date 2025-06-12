/****************************************************
    功能：全局数据管理器
    作者：ZH
    创建日期：#2025/01/15#
    修改人：ZH
    修改日期：#2025/01/15#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Voltage
{
    public class GlobalDataManager : MonoSingleton<GlobalDataManager>
    {
        [HideInInspector]
        public GameObject VRObj;

        /// <summary>
        /// 菜单场景
        /// </summary>
        [Scene]
        public string menuScene;
        /// <summary>
        /// 教学场景
        /// </summary>
        [Scene]
        public string teachScene;
        /// <summary>
        /// 训练场景
        /// </summary>
        [Scene]
        public string trainScene;

        /// <summary>
        /// 是否初始化2D Canvas
        /// </summary>
        public bool isInit2DCanvas = false;
        /// <summary>
        /// 是否初始化3D Canvas
        /// </summary>
        public bool isInit3DCanvas = true;

        /// <summary>
        /// 当前训练类型
        /// </summary>
        public ETrainType eTrainType = ETrainType.None;
    }
}