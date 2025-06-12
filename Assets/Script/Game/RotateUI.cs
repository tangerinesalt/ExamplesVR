/****************************************************
    功能：UI动画
    作者：ZH
    创建日期：#2025/04/23#
    修改内容：
        1.UI选择功能实现    2025/04/23 ZH
*****************************************************/

using UnityEngine;
using UnityEngine.Events;

namespace Voltage
{
    public class RotateUI : MonoBehaviour
    {
        public float speed = 100f;
        private Vector3 rotateDelta = new Vector3(0, 0, 1);

        private void Update()
        {
            rotateDelta = Vector3.forward * speed * Time.deltaTime;
            transform.Rotate(rotateDelta);
        }
    }
}