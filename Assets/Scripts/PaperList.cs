using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperList : MonoBehaviour
{
    [HideInInspector] public string name; //기사 이름
    [HideInInspector] public int point; //기사 배정 포인트

    public void TextUpdate() //텍스트 업데이트하는 함수
    {
        GetComponent<Text>().text = name + " : " + point.ToString();
    }
}
