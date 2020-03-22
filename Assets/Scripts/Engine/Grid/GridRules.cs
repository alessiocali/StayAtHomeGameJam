using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRules : MonoBehaviour
{
    [SerializeField]
    List<GameObject> AroundNeedTiles = new List<GameObject>();

    [SerializeField]
    List<GameObject> AcceptableTiles = new List<GameObject>();

    [SerializeField]
    List<GameObject> BannedTiles = new List<GameObject>();
}