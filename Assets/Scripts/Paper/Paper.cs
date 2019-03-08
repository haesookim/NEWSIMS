using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//실제 게임에 등장하는 기사 오브젝트 정보
public class Paper : MonoBehaviour
{
    //public Sprite reporterImage;
    public Sprite paperImage;
    public string reporter_name;
    public Article article;
    public Text viewText;
    private void Start() {

    }

    public void SetImage() //랜덤으로 기자 이미지와 기사 이미지 설정
    {
        //int randomFace = Random.Range(0,GameManager.Instance.ReporterImages.Length);
        int randomPaper = Random.Range(0,GameManager.Instance.PaperImages.Length);
        
        //reporterImage = GameManager.Instance.ReporterImages[randomFace];
        paperImage = GameManager.Instance.PaperImages[randomPaper];
       SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = paperImage;

    }

    //발표를 위해 급조된 함수이므로 나중에 고칠것.
    public void UpdateViewText(string paperRatio)
    {
        Text nameText = viewText.transform.GetChild(0).GetComponent<Text>();
        float ratio = (float.Parse(paperRatio) / 30f) * 100f;
        nameText.text =   article.article_name ;
        viewText.text = ratio.ToString("0.0") + " %";
        if(ratio > 5f)
        {
            int nameSize =  Mathf.CeilToInt((0.2f * ratio) + 10);
            viewText.fontSize = 15; 
            nameText.fontSize = nameSize;
        }
        else
        {
            viewText.fontSize = 10; 
            nameText.fontSize = 10;
        }
    }

}
