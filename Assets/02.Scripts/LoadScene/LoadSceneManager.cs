using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField]
    string sceneName;

 

    public void GameStart()
    {
        StartCoroutine(LoadScene(sceneName));
    }

     IEnumerator LoadScene(string name)
     {
        AsyncOperation asyncOper=SceneManager.LoadSceneAsync(name);

        yield return null;

     }

     
}
