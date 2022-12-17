using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Generator generator;

    public float contPreference;
    public float perpPreference;

    public float lower_range;
    public float upper_range;

    public int iterations;
    public float roadColliderRadius;

    void Awake()
    {
        generator.RestartParameters(contPreference, perpPreference, lower_range, upper_range, iterations, roadColliderRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !generator.IsGenerating())
        {
            generator.RemoveModel();
        }
    }

}
