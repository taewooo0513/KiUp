using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public Slider LoadingBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene");

        while (!op.isDone)
        {
            yield return null;
            
            if(LoadingBar.value < 0.9f)
            {
                LoadingBar.value = Mathf.MoveTowards(LoadingBar.value, 0.9f, Time.deltaTime);
            }
            else if(LoadingBar.value >= 0.9f)
            {
                LoadingBar.value = Mathf.MoveTowards(LoadingBar.value, 1f, Time.deltaTime);
            }

            if(LoadingBar.value > 1f && op.progress > 0.9f)
            {
                op.allowSceneActivation = true;
            }
        }
    }
}
