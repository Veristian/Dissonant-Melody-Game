using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI textLevelText;
    private Animator _animator;
    public GameObject textLevel;
    // Start is called before the first frame update
    void Start()
    {
        _animator = textLevel.GetComponent<Animator>();
        textLevel.SetActive(false);
    }
    public void TextAppear()
    {
        textLevelText = textLevel.GetComponentInChildren<TextMeshProUGUI>();
        textLevelText.text = "Tutorial";
        textLevel.SetActive(true);
        _animator.SetTrigger("Appear");
    }

    public void TextAppearStageOne()
    {
        textLevel.SetActive(true);
        textLevelText.text = "Stage - 1";
        _animator.SetTrigger("AppearStageOne");
    }

    public void TextAppearStageTwo()
    {
        textLevel.SetActive(true);
        textLevelText.text = "Stage - 2";
        _animator.SetTrigger("AppearStageTwo");
    }

    public void TextAppearStageThree()
    {
        textLevel.SetActive(true);
        textLevelText.text = "Stage - 3";
        _animator.SetTrigger("AppearStageThree");
    }
}
