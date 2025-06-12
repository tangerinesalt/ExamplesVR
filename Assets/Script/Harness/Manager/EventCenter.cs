/****************************************************
    功能：消息中心
    作者：ZH
    创建日期：#2025/02/07#
    修改人：ZH
    修改日期：#2025/02/07#
    修改内容：
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public interface IEventInfo { }

    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }

    public class EventInfo : IEventInfo
    {
        public UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }

    public class EventCenter : Singleton<EventCenter>
    {
        /// <summary>
        /// key对应着事件的名字，
        /// value对应的是监听这个事件
        /// </summary>
        private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

        /// <summary>
        /// 添加事件监听--注意避免用匿名函数
        /// </summary>
        public void AddEventListener<T>(string name, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).actions += action;
            }
            else
            {
                eventDic.Add(name, new EventInfo<T>(action));
            }
        }
        /// <summary>
        /// 添加事件监听--注意避免用匿名函数
        /// </summary>
        public void AddEventListener(string name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).actions += action;
            }
            else
            {
                eventDic.Add(name, new EventInfo(action));
            }
        }

        /// <summary>
        /// 通过事件名字进行事件触发
        /// </summary>
        public void EventTrigger<T>(string name, T info)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).actions?.Invoke(info);
            }
        }
        /// <summary>
        /// 事件触发
        /// </summary>
        public void EventTrigger(string name)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).actions?.Invoke();
            }
        }

        /// <summary>
        /// 移除对应的事件监听
        /// </summary>
        public void RemoveEventListener<T>(string name, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo<T>).actions -= action;
            }
        }
        /// <summary>
        /// 移除对应的事件监听
        /// </summary>
        public void RemoveEventListener(string name, UnityAction action)
        {
            if (eventDic.ContainsKey(name))
            {
                (eventDic[name] as EventInfo).actions -= action;
            }
        }

        /// <summary>
        /// 清空所有事件监听(主要用在切换场景时)
        /// </summary>
        public void Clear()
        {
            eventDic.Clear();
        }
    }
}