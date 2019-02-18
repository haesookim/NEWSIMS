using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    //SOUND
    private void Awake()
    {
        AudioManager.instance.StartMusic("title");
    }
    //

    private void OnMouseDown()
    {
        LoadingSceneManager.LoadScene("Desk");
    }
}
