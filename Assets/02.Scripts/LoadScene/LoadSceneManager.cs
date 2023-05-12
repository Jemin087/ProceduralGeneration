using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField]
    string sceneName;

    [SerializeField]
    Image  bar;

    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(LoadScene(sceneName));
    }

     IEnumerator LoadScene(string name)
     {
        AsyncOperation asyncOper=SceneManager.LoadSceneAsync(name);
        asyncOper.allowSceneActivation=false;
        float timer=0.0f;
        while(!asyncOper.isDone)
        {
            yield return null;
            timer+=Time.deltaTime;
            if(asyncOper.progress<=0.9f)
            {
                bar.fillAmount=Mathf.Lerp(bar.fillAmount,asyncOper.progress,timer);
                if(bar.fillAmount>=asyncOper.progress)
                {
                     timer=0f;
                }
               
            }
            else
            {
               bar.fillAmount=Mathf.Lerp(bar.fillAmount,1f,timer);
               if(bar.fillAmount==1.0f)
               {
                  asyncOper.allowSceneActivation=true;
                  yield break;
               }

            }
        }

     }

     
}
