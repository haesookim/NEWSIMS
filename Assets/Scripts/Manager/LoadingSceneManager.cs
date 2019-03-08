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
        SceneManager.LoadScene("Loading");


        /////사운드
        if (sceneName == "02.Ending")
        {
            int ending_phase = EndingManager.instance.ending_phase;
            if (ending_phase == 0 || ending_phase == 1 || ending_phase == 13)
                AudioManager.instance.StartMusic("Ending_Loop_Sad_cut");
            else if (ending_phase == 8 || ending_phase == 9 || ending_phase == 10 || ending_phase == 11 || ending_phase == 12)
                AudioManager.instance.StartMusic("Ending_Loop_Happy_cut");
            else
                AudioManager.instance.StartMusic("Ending_Loop_Basic_cut");
        }
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
                    yield return new WaitForSeconds(0.68f);
                    op.allowSceneActivation = true;
                }
            }
        }
    }
}