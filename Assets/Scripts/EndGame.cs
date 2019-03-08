using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnMouseDown()
    {
        EndingManager.instance.ending_phase = 0;
        LoadingSceneManager.LoadScene("00.Title");
    }
}
