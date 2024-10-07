using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] GameObject _grassPrefab;
    [SerializeField, InlineEditor] GridData _data;
    [Space(15)]
    [SerializeField, ReadOnly] List<GameObject> _instantiatedPrefabs;
    
    static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");

    [Button]
    void CreateGrid() {
        DeleteGrid();
        
        for (int row = 0; row < _data.Size.x; row++) {
            for (int col = 0; col < _data.Size.y; col++) {
                _instantiatedPrefabs.Add(InstantiateGrass(row, col, _data));
            }    
        }
    }

    [Button]
    void DeleteGrid() {
        while (_instantiatedPrefabs is { Count: > 0 }) {
            DestroyImmediate(_instantiatedPrefabs[0]);
            _instantiatedPrefabs.RemoveAt(0);
        }

        _instantiatedPrefabs = new List<GameObject>(_data.Size.x * _data.Size.y);
    }
    
    GameObject InstantiateGrass(int row, int col, GridData data) {
        Vector3 position = new () {
            x = row * data.Scale, 
            y = -(data.Scale / 2f) + data.HeightVariation.RandomInBetween(), 
            z = col * data.Scale
        };
        
        Vector3 euler = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(euler.Add(data.EulerVariation.RandomInBetween()));
        
        Color color = data.Color;
        color.Add(data.ColorVariation.RandomInBetween());

        return new CubeBuilder()
            .WithPrefab(_grassPrefab)
            .WithName($"{_grassPrefab.name} : ({row},{col})")
            .WithPosition(position)
            .WithRotation(rotation)
            .WithScale(Vector3.one * data.Scale)
            .WithColor(color, ColorPropertyID)
            .WithParent(transform)
            .Build();
    }
}