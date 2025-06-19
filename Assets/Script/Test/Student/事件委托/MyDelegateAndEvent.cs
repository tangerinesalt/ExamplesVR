using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class MyDelegateAndEvent : MonoBehaviour
{
    private delegate void MyDelegate(string message);
    private MyDelegate myDelegate;
    private event MyDelegate myEvent;
    private UnityEvent<string> myUnityEvent;

    [ContextMenu("DelegateTest"),Button("DelegateTest")]
    public void DelegateTest()
    {
        myDelegate = null;
        myDelegate += MyMethod;
        myDelegate("delegate");
    }
    [ContextMenu("EventTest"),Button("EventTest")]
    public void EventTest()
    {
        myEvent = null;
        myEvent += MyMethod;
        myEvent?.Invoke("event");
    }

    private void MyMethod(string message = "")
    {
        Debug.Log(message+" execute");
    }
}
