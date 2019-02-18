using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject gameManager;

    //SOUND
    public GameObject audioManager;
    //

    // Use this for initialization
    void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);

        //SOUND
        if (AudioManager.instance == null)
            Instantiate(audioManager);
        //
    }
}
