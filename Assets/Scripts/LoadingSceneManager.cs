using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{

    public static string nextScene; //어떤 씬으로 이동할지 저장

    [SerializeField]
    Image progressBar; //로딩바 이미지

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName) //다른 씬에서 로딩씬을 불러오고 여기서 로딩할 씬을 불러오게 하는 함수
    {
        nextScene = sceneName;
        SceneManager.LoadScene("loading");
    }

    IEnumerator LoadScene() //로딩씬에서 로딩할 씬을 불러오게 하는 함수 및 로딩바가 차오르는 함수
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                }
            }
        }
    }
}