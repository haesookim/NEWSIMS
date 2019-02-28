using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeReporter : MonoBehaviour
{
    [Header("기자 정보 받는 칸")]
    public SpriteRenderer reporterImage;
    public Text name;
    public Text mainField;
    public Text level;
    public Text writing;
    public Text logic;
    public Text survey;
    public Reporter reporter;

    public string mf = "";

    // Start is called before the first frame update
    void Start()
    {
        reporterImage.sprite = reporter.reporterImage;

        name.text = "이름 : " + reporter.name;
        float temp_max = 0;
        for (int i = 0; i < System.Enum.GetValues(typeof(Setting.Fields)).Length; i++)
        {
            if (i == 0)
            {
                mf = System.Enum.GetName(typeof(Setting.Fields),i);
                temp_max = reporter.interests[(Setting.Fields)i];
            }
            else
            {
                if (reporter.interests[(Setting.Fields)i] > temp_max)
                {
                    mf = System.Enum.GetName(typeof(Setting.Fields), i);
                    temp_max = reporter.interests[(Setting.Fields)i];
                }
            }
        }
        mainField.text = "주 분야 : " + mf;
        level.text = "레벨 : " + reporter.level.ToString();
        writing.text = "필력 : " + reporter.writing.ToString();
        logic.text = "논리력 : " + reporter.logic.ToString();
        survey.text = "조사력 : " + reporter.survey.ToString();
    }

    //하루가 지나갈 때마다의 이벤트에 추가해야 함
    public void UpdateStatus()
    {
        for (int i = 0; i < GameManager.Instance.company.reporters.Count; i++)
        {
            if (reporter.reporter_index == GameManager.Instance.company.reporters[i].reporter_index)
            {
                reporter = GameManager.Instance.company.reporters[i];
                level.text = "레벨 : " + reporter.level.ToString();
                writing.text = "필력 : " + reporter.writing.ToString();
                logic.text = "논리력 : " + reporter.logic.ToString();
                survey.text = "조사력 : " + reporter.survey.ToString();
                break;
            }
        }
    }
}
