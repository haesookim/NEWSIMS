using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListSort : MonoBehaviour
{
    public GameObject content;

public void Sorting()
    {
        GameManager.instance.papers.Sort(delegate (Drag x, Drag y) //정렬
        {
            return x.eachPoint.CompareTo(y.eachPoint);
        });
        GameManager.instance.papers.Reverse(); //역순으로 정렬

        for (int i = 0; i < transform.childCount; i++) //페이퍼들에 대해 텍스트를 순서대로 보여주게 정렬
        {
            transform.GetChild(i).GetComponent<Drag>().ChangeListContent();
        }
    }
}
