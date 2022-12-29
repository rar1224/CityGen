using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderText;


    // Start is called before the first frame update
    void Start()
    {
        sliderText = GetComponent<TextMeshProUGUI>();
        ShowSliderValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSliderValue()
    {
        sliderText.text = slider.value.ToString();
    }
}
