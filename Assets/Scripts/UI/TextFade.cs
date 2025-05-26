using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFade : MonoBehaviour
{
    public TextMeshProUGUI textLevelText;
    public GameObject textLevel;

    public void TextDisapppearTutorial()
    {
        textLevelText = textLevel.GetComponentInChildren<TextMeshProUGUI>();
        textLevelText.text = "Stage - 1";
        textLevel.SetActive(false);
    }

    public void TextDisappearStageOne()
    {
        textLevelText = textLevel.GetComponentInChildren<TextMeshProUGUI>();
        textLevelText.text = "Stage - 2";
        textLevel.SetActive(false);
    }

    public void TextDisappearStageTwo()
    {
        textLevelText = textLevel.GetComponentInChildren<TextMeshProUGUI>();
        textLevelText.text = "Stage - 3";
        textLevel.SetActive(false);
    }

    public void TextDisappearStageThree()
    {
        textLevel.SetActive(false);
    }
}
