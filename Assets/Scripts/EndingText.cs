using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingText : MonoBehaviour
{
    public Text[] text;

    // Start is called before the first frame update
    void Start()
    {
        text[0].text = EndingManager.instance.endingText[0][EndingManager.instance.ending_phase.ToString()].ToString();
        text[1].text = EndingManager.instance.endingText[1][EndingManager.instance.ending_phase.ToString()].ToString();
        text[2].text = EndingManager.instance.endingText[2][EndingManager.instance.ending_phase.ToString()].ToString();
        text[3].text = EndingManager.instance.endingText[3][EndingManager.instance.ending_phase.ToString()].ToString();

    }
}
