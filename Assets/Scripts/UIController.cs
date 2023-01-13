using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Generator generator;
    public BuildingGenerator bdGenerator;

    public float contPreference;
    public float perpPreference;

    public float lower_range;
    public float upper_range;

    public int iterations;
    public float roadColliderRadius;

    public Button regenerateButton;
    public Button buildingsButton;
    public Button removeButton;
    public Button exportButton;

    public Slider contSlider;
    public Slider perpSlider;

    public GameObject lowerRangeInput;
    public GameObject upperRangeInput;
    public GameObject iterationsInput;

    public GameObject startShapeInput;
    private TMP_Dropdown startShapeDropdown;

    public GameObject centre;
    public GameObject model;

    void Awake()
    {
        GetParameters();
        generator.RestartParameters(contPreference, perpPreference, lower_range, upper_range, iterations, roadColliderRadius);
        Button btn = regenerateButton.GetComponent<Button>();
        btn.onClick.AddListener(Regenerate);

        Button btn2 = buildingsButton.GetComponent<Button>();
        btn2.onClick.AddListener(GenerateBuildings);

        Button btn3 = removeButton.GetComponent<Button>();
        btn3.onClick.AddListener(RemoveModel);

        Button btn4 = exportButton.GetComponent<Button>();
        btn4.onClick.AddListener(ExportFbx);

        startShapeDropdown = startShapeInput.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if(bdGenerator.IsGenerating() || generator.IsGenerating())
        {
            buildingsButton.interactable = false;
            removeButton.interactable = false;
            regenerateButton.interactable = false;
        } else
        {
            buildingsButton.interactable = true;
            removeButton.interactable = true;
            regenerateButton.interactable = true;
        }

        //centre.transform.RotateAround(Vector3.zero, Vector3.forward, 30 * Time.deltaTime);
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
        if (generator.roads.Count == 0)
        {
            string shape = startShapeDropdown.options[startShapeDropdown.value].text;
            startShapeDropdown.interactable = false;

            switch(shape)
            {
                case "Square":
                    generator.shape = Generator.CentreShape.SQUARE;
                    break;
                case "Trapezoid":
                    generator.shape = Generator.CentreShape.TETRA;
                    break;
                case "Triangle":
                    generator.shape = Generator.CentreShape.TRIANGLE;
                    break;
                case "Line":
                    generator.shape = Generator.CentreShape.LINE;
                    break;
            }
        }

        

        //bdGenerator.modelGenerator.CreateBuilding(new Vector3(0, 0, 0), Quaternion.identity);

        
        GetParameters();
        generator.RestartParameters(contPreference, perpPreference, lower_range, upper_range, iterations, roadColliderRadius);
        if (generator.roads.Count == 0)
        {
            generator.CreateCentre(generator.shape);
        }

        generator.Continue();
        
        
    }

    void GenerateBuildings()
    {
        bdGenerator.Initialize(generator);
    }

    void RemoveModel()
    {
        startShapeDropdown.interactable = true;
        generator.RemoveModel();
        bdGenerator.RemoveBuildings();
    }

    void ExportFbx()
    {
        List<UnityEngine.Object> objectList = new List<UnityEngine.Object>();

        foreach (Transform tr in model.transform)
        {
            objectList.Add(tr.gameObject);
        }

        string filePath = Path.Combine(Application.dataPath, "City.fbx");
        ModelExporter.ExportObjects(filePath, objectList.ToArray());
    }
}
