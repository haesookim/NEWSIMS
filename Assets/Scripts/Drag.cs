using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {

    bool inDesk = true;
    float distance = 10;

    Vector3 originPosition;
    Vector3 objPosition;

    private void Start()
    {
        GameManager.instance.AddPaperToList(this);
    }

    private void OnMouseDown()
    {
        originPosition = transform.position;
        for (int i = 0; i < GameManager.instance.papers.Count; i++)
        {
            if (GameManager.instance.papers[i] != this)
            {
                if (GameManager.instance.papers[i].transform.position.z < 989f)
                {
                    GameManager.instance.papers[i].transform.position += new Vector3(0f, 0f, 1f);
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
    }

    private void OnMouseUp()
    {
        if (inDesk)
        {
            originPosition = transform.position;
        }
        else
        {
            transform.position = originPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Desk")
        {
            inDesk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Desk")
        {
            inDesk = false;
        }
    }
}
