using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointSlider : MonoBehaviour
{

    public Text selectedPoint;
    Slider slider;
    public float point;

    void Start()
    {
        slider = GetComponent<Slider>();   
    }

    void OnEnable() {
         if(GameManager.Instance.selectedPaper != null)
         {
            slider = GetComponent<Slider>(); 
            slider.value = 0;   
            point = GameManager.Instance.selectedPaper.article.assignedPoint;
            slider.maxValue = GameManager.Instance.point;
         }
    }

    
    void Update()
    {
       point = slider.value;
       GameManager.Instance.temp_point = (int)point;
       selectedPoint.text = point.ToString();
    }
}
