using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private int layer_mask;

    private void Awake()
    {
        layer_mask = LayerMask.GetMask("Tile");
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 200f, Color.red,5);

            if (Physics.Raycast(ray, out hit, 200f,layer_mask))
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
    

}
