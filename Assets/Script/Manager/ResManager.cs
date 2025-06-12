/****************************************************
	功能：资源管理器
    作者：ZH
    创建日期：#2025/01/09#
    修改人：ZH
    修改日期：#2025/01/14#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public class ResManager: MonoSingleton<ResManager>
    {
        private const string goPath = "Prefabs/";
        private const string adPath = "Audio/";
        private const string spPath = "Textures/";

        /// <summary>
        /// 预制体集合
        /// </summary>
        public Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
        /// <summary>
        /// 音效集合
        /// </summary>
        public Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
        /// <summary>
        /// 图片集合
        /// </summary>
        public Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();

        /// <summary>
        /// 当前协程
        /// </summary>
        private Coroutine curt_Coroutine;

        protected virtual void Destroy()
        {
            adDic.Clear();
            adDic = null;

            goDic.Clear();
            goDic = null;

            spDic.Clear();
            spDic = null;
        }

        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public GameObject LoadPrefab(string path, bool cache = false)
        {
            GameObject go;
            path = goPath + path;

            if (!goDic.TryGetValue(path, out go))
            {
                go = Resources.Load<GameObject>(path);
                if (go == null)
                {
                    Debug.LogError("Can not find the prefab: " + path);
                }
                if (!cache)
                {
                    goDic.Add(path, go);
                }
            }

            return Instantiate(go);
        }

        /// <summary>
        /// 加载音效
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="cache">是否在集合中</param>
        /// <returns>音效</returns>
        public AudioClip LoadAudio(string path, bool cache = false)
        {
            AudioClip au = null;
            path = adPath + path;

            if (!adDic.TryGetValue(path, out au))
            {
                au = Resources.Load<AudioClip>(path);
                if (!cache)
                {
                    adDic.Add(path, au);
                }
            }
            return au;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="cache">是否在集合中</param>
        /// <returns>图片</returns>
        public Sprite LoadSprite(string path, bool cache = false)
        {
            Sprite sp = null;
            path = spPath + path;

            if (!spDic.TryGetValue(path, out sp))
            {
                sp = Resources.Load<Sprite>(path);
                if (!cache)
                {
                    spDic.Add(path, sp);
                }
            }
            return sp;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">路径</param>
        /// <param name="callback">回调</param>
        public void LoadAsync<T>(string name, UnityAction<T> callback) 
            where T : Object
        {
            if (curt_Coroutine == null)
            {
                curt_Coroutine = StartCoroutine(ReallyLoadAsync(name, callback));
            }
        }

        /// <summary>
        /// 异步加载资源协程
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">路径</param>
        /// <param name="callback">回调</param>
        private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) 
            where T : Object
        {
            ResourceRequest r = Resources.LoadAsync<T>(name);
            yield return r;

            if (r.asset is GameObject)
            {
                callback(Instantiate(r.asset) as T);
            }
            else
            {
                callback(r.asset as T);
            }

            curt_Coroutine = null;
        }
    }
}