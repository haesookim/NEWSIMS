using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    //SOUND
    private void Awake()
    {
        AudioManager.Instance.StartMusic("title");
    }
    //

    private void OnMouseDown()
    {
        LoadingSceneManager.LoadScene("01.Desk");
    }
}
