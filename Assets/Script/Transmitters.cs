using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Transmitters : MonoBehaviour
{
    public string LoadSceneName;
    
    private void OnTriggerEnter(Collider other)
    {
        if (LoadSceneName != "")
        {
            StartCoroutine(LoadSceneCoroutine(LoadSceneName, LoadSceneMode.Single));
            //SceneManager.LoadSceneAsync(LoadSceneName);
        }
        else
        {
            Debug.Log("No scene name provided");
        }
    }
    private void OnDestroy()
    {
        //Debug.Log("Destroy Transmitters");
    }
    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode)  
    {  
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);  
  
        // 你可以在这里添加逻辑来处理加载进度  
        // 例如，更新一个UI进度条：progressUI.value = asyncLoad.progress; 
         
  
        // 等待直到场景加载完成  
        while (!asyncLoad.isDone)  
        {  
            yield return null;
            Debug.Log("Loading Scene...");
            
        }
        // 场景加载完成后，调用OnChangeSence事件
        yield return null;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(1f);
        Debug.Log("OnChangeSence for Transmitters");  
        EventManager.Instance.OnChangeSence();
        this.gameObject.SetActive(false);
    }  
}
