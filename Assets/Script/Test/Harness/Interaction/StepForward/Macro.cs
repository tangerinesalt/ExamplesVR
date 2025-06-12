/****************************************************
    功能：执行宏
    作者：Voltage
    创建日期：#2025/02/20#
    修改人：ZZQ
    修改日期：#2025/02/20#
    修改内容：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Voltage
{
    public class Macro : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_ToHideObjects;
        [SerializeField] private List<GameObject> m_ToShowObjects;
        [SerializeField] private List<Component> m_ToEnableComponents;
        [SerializeField] private UnityEvent m_TodoEvent;
        public void Execute()
        {
            foreach (GameObject obj in m_ToHideObjects)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in m_ToShowObjects)
            {
                obj.SetActive(true);
            }
            foreach (Component comp in m_ToEnableComponents)
            {
                SetComponent(comp, true);
            }
            m_TodoEvent.Invoke();

        }
        private void SetComponent(Component component, bool enable)
        {
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = enable;
            }
            else if (component is Renderer renderer)
            {
                renderer.enabled = enable;
            }
        }
    }
}