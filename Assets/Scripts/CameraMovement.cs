using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject centre;
    private float sensitivity = 0.5f;
    private float horizontal = 1.5f;
    private float vertical = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
        {
            centre.transform.RotateAround(centre.transform.position, Vector3.forward, horizontal);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            centre.transform.RotateAround(centre.transform.position, Vector3.forward, -horizontal);
        }

    }

    private void OnGUI()
    {
        
        Vector3 position = transform.position;
        position.z += Input.mouseScrollDelta.y * sensitivity;
        transform.position = position;
        

    }
}
