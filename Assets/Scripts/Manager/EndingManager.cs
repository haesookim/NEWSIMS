using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : Singleton<EndingManager>
{
    public static EndingManager instance = null;

    [HideInInspector] public List<Dictionary<string, object>> endingText;

    public int ending_phase;
    

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); //파괴되지않아!

        endingText = CSVReader.Read("EndingText"); //CSV를 불러옴



    }
}
