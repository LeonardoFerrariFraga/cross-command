﻿using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Tools Data/Grid")]
public class GridData : ScriptableObject
{
    [field: SerializeField] public Vector2Int Size { get; private set; }
    public int TotalSize => Size.x * Size.y;
    
    [field: SerializeField] public float Scale { get; private set; }
    [field: SerializeField] public MinMaxFloat HeightVariation { get; private set; }
    [field: SerializeField] public MinMaxFloat EulerVariation { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
}

