using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    Renderer renderer;
    public bool highlighted; //testing

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //testing
        renderer.material.color = highlighted ? Color.green : Color.red;
        highlighted = false;
    }
}
