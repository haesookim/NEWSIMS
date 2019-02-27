using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//실제 게임에 등장하는 기사 오브젝트 정보
public class Paper : MonoBehaviour
{
    public Sprite reporterImage;
    public Sprite paperImage;
    public Reporter reporter;
    public Article article;
    private void Start() {
        
        SetImage();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = paperImage;

    }

    void SetImage() //랜덤으로 기자 이미지와 기사 이미지 설정
    {
        int randomFace = Random.Range(0,GameManager.Instance.ReporterImages.Length);
        int randomPaper = Random.Range(0,GameManager.Instance.PaperImages.Length);
        
        reporterImage = GameManager.Instance.ReporterImages[randomFace];
        paperImage = GameManager.Instance.PaperImages[randomPaper];

    }

}
