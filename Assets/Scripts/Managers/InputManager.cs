using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public delegate void ClickAction(string TileName, GridTile.TileIndex index);
    public static event ClickAction OnClicked;

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

            if (Physics.Raycast(ray, out hit, 200f,layer_mask))
            {
                try
                {
                    GridTile.TileIndex index = hit.transform.gameObject.GetComponent<GridTile>().Index;
                    OnClicked?.Invoke(transform.name, index);
                }
                catch (NullReferenceException e) {
                    Debug.LogWarning("Missing element on clicked object");
                    Debug.Log(e.StackTrace);
                }
            }
        }
    }
    

}
