using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Generator generator;

    public float contPreference;
    public float perpPreference;

    public float lower_range;
    public float upper_range;

    public int iterations;
    public float roadColliderRadius;

    public Button regenerateButton;

    public Slider contSlider;
    public Slider perpSlider;

    public GameObject lowerRangeInput;
    public GameObject upperRangeInput;
    public GameObject iterationsInput;

    void Awake()
    {
        GetParameters();
        generator.RestartParameters(contPreference, perpPreference, lower_range, upper_range, iterations, roadColliderRadius);
        Button btn = regenerateButton.GetComponent<Button>();
        btn.onClick.AddListener(Regenerate);
    }

    // Update is called once per frame
    void Update()
    {
        if(generator.IsGenerating())
        {
            regenerateButton.interactable = false;
        } else
        {
            regenerateButton.interactable = true;
        }
    }

    void GetParameters()
    {
        contPreference = contSlider.value;
        perpPreference = perpSlider.value;
        lower_range = float.Parse(lowerRangeInput.GetComponent<TMP_InputField>().text);
        upper_range = float.Parse(upperRangeInput.GetComponent<TMP_InputField>().text);
        iterations = int.Parse(iterationsInput.GetComponent<TMP_InputField>().text);
    }

    void Regenerate()
    {
        GetParameters();
        generator.RemoveModel();
        generator.RestartParameters(contPreference, perpPreference, lower_range, upper_range, iterations, roadColliderRadius);
        generator.StartOver();
    }



}
