using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private void OnMouseDown()
    {
        LoadingSceneManager.LoadScene("Desk");
    }
}
