using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCreator : MonoBehaviour
{
    [SerializeField] GameObject _grassPrefab;
    [Tooltip("The grid is created from left to right (x), bottom to top (z). Default value is Vector3.zero")]
    [SerializeField] Transform _startPosition;
    [SerializeField] bool _addCornerPoles;
    [SerializeField, ShowIf("_addCornerPoles")] Color _polesColor;
    [Space(15)]
    [SerializeField, InlineEditor] GridData _data;
    [Space(15)]
    [SerializeField, ReadOnly] List<GameObject> _instantiatedPrefabs;
    [FormerlySerializedAs("_cornerMarks")] [SerializeField, ReadOnly] List<GameObject> _poles;
    
    static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");

    [Button]
    void CreateGrid() {
        DeleteGrid();
        
        _instantiatedPrefabs = new List<GameObject>(_data.TotalSize);
        for (int col = 0; col < _data.Size.y; col++) {
            for (int row = 0; row < _data.Size.x; row++) {
                _instantiatedPrefabs.Add(InstantiateGrass(row, col, _data));
            }    
        }

        CreateCollider(_instantiatedPrefabs, _data, _startPosition.position);
        
        if (_addCornerPoles)
            AddPoles(_instantiatedPrefabs, _data, _startPosition.position, _polesColor);
    }

    [Button]
    void DeleteGrid() {
        while (_instantiatedPrefabs is { Count: > 0 }) {
#if UNITY_EDITOR
            DestroyImmediate(_instantiatedPrefabs[0]);
#else
            Destroy(_instantiatedPrefabs[0]);
#endif
            _instantiatedPrefabs.RemoveAt(0);
        }
        
        _instantiatedPrefabs.Clear();

        while (_poles is { Count: > 0 }) {
#if UNITY_EDITOR
            DestroyImmediate(_poles[0]);
#else
            Destroy(_poles[0]);
#endif
            _poles.RemoveAt(0);
        }
        
        _poles.Clear();

        BoxCollider col = GetComponent<BoxCollider>();
        if (col) {
#if UNITY_EDITOR
            DestroyImmediate(col);
#else
            Destroy(col);
#endif
        }
    }
    
    GameObject InstantiateGrass(int row, int col, GridData data) {
        Vector3 startPosition = _startPosition ? _startPosition.position : Vector3.zero;
        Vector3 position = startPosition + new Vector3() {
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

    void AddPoles(List<GameObject> cubes, GridData data, Vector3 startPosition, Color color) {
        _poles = new(4);

        // The naming is considering a topdown view (z is "up")
        int botLeftIndex = 0;
        int botRightIndex = data.Size.x - 1;
        int topLeftIndex = data.TotalSize - data.Size.x;
        int topRightIndex = data.TotalSize - 1;
        
        Bounds blBounds = cubes[botLeftIndex].GetComponent<MeshRenderer>().bounds;
        Bounds brBounds = cubes[botRightIndex].GetComponent<MeshRenderer>().bounds;
        Bounds tlBounds = cubes[topLeftIndex].GetComponent<MeshRenderer>().bounds;
        Bounds trBounds = cubes[topRightIndex].GetComponent<MeshRenderer>().bounds;
        
        Vector3 blCorner = blBounds.min;
        blCorner.y = startPosition.y;
        Vector3 brCorner = new (brBounds.max.x, startPosition.y, brBounds.min.z);
        Vector3 tlCorner = new (tlBounds.min.x, startPosition.y, tlBounds.max.z);
        Vector3 trCorner = trBounds.max;
        trCorner.y = startPosition.y;
        
        _poles.Add(
            new CubeBuilder()
                .WithPosition(blCorner)
                .WithName("Bot Left Corner")
                .WithScale(new Vector3(0.1f * data.Scale, data.Scale * 2f, 0.1f * data.Scale))
                .WithColor(color, ColorPropertyID)
                .Build()    
        );

        _poles.Add(
            new CubeBuilder()
                .WithPosition(brCorner)
                .WithName("Bot Right Corner")
                .WithScale(new Vector3(0.1f * data.Scale, data.Scale * 2f, 0.1f * data.Scale))
                .WithColor(color, ColorPropertyID)
                .Build()
        );

        _poles.Add(
            new CubeBuilder()
                .WithPosition(tlCorner)
                .WithName("Top Left Corner")
                .WithScale(new Vector3(0.1f * data.Scale, data.Scale * 2f, 0.1f * data.Scale))
                .WithColor(color, ColorPropertyID)
                .Build()
        );

        _poles.Add(
            new CubeBuilder()
                .WithPosition(trCorner)
                .WithName("Bot Right Corner")
                .WithScale(new Vector3(0.1f * data.Scale, data.Scale * 2f, 0.1f * data.Scale))
                .WithColor(color, ColorPropertyID)
                .Build()
        );
    }
    
    void CreateCollider(List<GameObject> cubes, GridData data, Vector3 startPosition) {
        
        
        BoxCollider col = GetComponent<BoxCollider>();
        if (!col)
            col = gameObject.AddComponent<BoxCollider>();
        
        Vector3 center = Vector3.Lerp(cubes[0].transform.position, cubes[^1].transform.position, 0.5f);
        center.y = startPosition.y - data.Scale / 2f;
        col.center = center;
        col.size = new Vector3(data.Size.x * data.Scale, data.Scale, data.Size.y * data.Scale);
    }
}