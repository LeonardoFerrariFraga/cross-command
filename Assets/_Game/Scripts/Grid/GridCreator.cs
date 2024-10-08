using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider), typeof(MeshFilter), typeof(MeshRenderer))]
public class GridCreator : MonoBehaviour
{
    [SerializeField] GameObject _cubePrefab;
    
    [Tooltip("The grid is created from left to right (x), bottom to top (z). Default value is Vector3.zero")]
    [SerializeField] Transform _startPositionTransform;
    Vector3 _startPosition => _startPositionTransform ? _startPositionTransform.position : Vector3.zero;
    
    [SerializeField] bool _addCornerPoles;
    [SerializeField, ShowIf("_addCornerPoles")] Material _polesMaterial;
    
    [Space(15)]
    
    [SerializeField, Required, InlineEditor] GridData _data;
    
    [Space(15)]
    
    [SerializeField, ReadOnly] List<GameObject> _instantiatedPrefabs;
    [SerializeField, ReadOnly] GameObject _poles;

    BoxCollider _boxCollider;
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;

    bool _meetRequirements => _data;

    const int VERTICES_PER_CUBE = 24; // Cube has 6 faces, each face has 4 vertices, 6 * 4 = 24.
    static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");
    
    void Reset() {
        _boxCollider = GetComponent<BoxCollider>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    
    [Button, ShowIf("_meetRequirements")]
    void CreateGrid() {
        DeleteGrid();
        
        _instantiatedPrefabs = new List<GameObject>(_data.TotalSize);
        for (int col = 0; col < _data.Size.y; col++) {
            for (int row = 0; row < _data.Size.x; row++) {
                _instantiatedPrefabs.Add(InstantiateCubes(row, col));
            }    
        }

        CreateCollider(_instantiatedPrefabs, _data, _startPosition);
        
        if (_addCornerPoles)
            AddPoles(_instantiatedPrefabs, _data, _startPosition, _polesMaterial);
        
        CombineMeshes(_instantiatedPrefabs);
    }

    [Button, ShowIf("_meetRequirements")]
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

        if (!_poles) {
            Transform edgePoles = transform.Find("Edge Poles");
            if (edgePoles) {
                _poles = edgePoles.gameObject;
            }
        }

        if (_poles) {
#if UNITY_EDITOR
            DestroyImmediate(_poles);
#else
            Destroy(_poles);
#endif
        }

        if (!_boxCollider)
            _boxCollider = GetComponent<BoxCollider>();
        
        _boxCollider.center = transform.position;
        _boxCollider.size = Vector3.one;

        if (!_meshFilter)
            _meshFilter = GetComponent<MeshFilter>();
        
        Mesh mesh = _meshFilter.sharedMesh;
#if UNITY_EDITOR
        DestroyImmediate(mesh);
#else
        Destroy(mesh);
#endif

        if (!_meshRenderer)
            _meshRenderer = GetComponent<MeshRenderer>();
        
        _meshRenderer.sharedMaterial = null;
    }
    
    GameObject InstantiateCubes(int row, int col) {
        Vector3 position = _startPosition + new Vector3() {
            x = row * _data.Scale, 
            y = -(_data.Scale / 2f) + _data.HeightVariation.RandomInBetween(), 
            z = col * _data.Scale
        };
        
        Vector3 euler = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(euler.Add(_data.EulerVariation.RandomInBetween()));
        
        return new CubeBuilder()
            .WithPrefab(_cubePrefab)
            .WithPosition(position)
            .WithRotation(rotation)
            .WithScale(Vector3.one * _data.Scale)
            .WithParent(transform)
            .Build();    
    }

    void CombineMeshes(List<GameObject> cubes) {
        CombineInstance[] meshesToCombine = new CombineInstance[cubes.Count];
        for (int index = 0; index < cubes.Count; index++) {
            meshesToCombine[index].mesh = cubes[index].GetComponent<MeshFilter>().sharedMesh;
            meshesToCombine[index].transform = cubes[index].transform.localToWorldMatrix;
        }
        
        _meshFilter.sharedMesh = new Mesh {
            indexFormat = _data.TotalSize * VERTICES_PER_CUBE > UInt16.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16
        };
        
        _meshFilter.sharedMesh.CombineMeshes(meshesToCombine);
        _meshRenderer.sharedMaterial = _data.Material;

        foreach (GameObject cube in cubes) {
            #if UNITY_EDITOR
            DestroyImmediate(cube);
            #else
            Destroy(cube);
            #endif
        }
    }
    
    void AddPoles(List<GameObject> cubes, GridData data, Vector3 startPosition, Material material) {
        _poles = new GameObject("Edge Poles");;
        _poles.transform.SetParent(transform);
        _poles.transform.SetSiblingIndex(_startPositionTransform ? 1 : 0);
        
        // Naming is considering a top-down view (z is "up")
        Bounds botLeftBounds = cubes[0].GetComponent<MeshRenderer>().bounds;
        Bounds botRightBounds = cubes[data.Size.x - 1].GetComponent<MeshRenderer>().bounds;
        Bounds topLeftBounds = cubes[data.TotalSize - data.Size.x].GetComponent<MeshRenderer>().bounds;
        Bounds topRightBounds = cubes[data.TotalSize - 1].GetComponent<MeshRenderer>().bounds;

        Vector3 position = transform.position;
        Vector3[] positions = {
            position + new Vector3(botLeftBounds.min.x, startPosition.y, botLeftBounds.min.z),
            position + new Vector3(botRightBounds.max.x, startPosition.y, botRightBounds.min.z),
            position + new Vector3(topLeftBounds.min.x, startPosition.y, topLeftBounds.max.z),
            position + new Vector3(topRightBounds.max.x, startPosition.y, topRightBounds.max.z)
        };
        
        string[] names = { "Bot Left Corner", "Bot Right Corner", "Top Left Corner", "Top Right Corner" };
        
        for (int i = 0; i < 4; i++) {
            new CubeBuilder()
                .WithPosition(positions[i])
                .WithName(names[i])
                .WithScale(new Vector3(0.1f * data.Scale, data.Scale * 2f, 0.1f * data.Scale))
                .WithMaterial(material)
                .Build().transform.SetParent(_poles.transform);    
        }
    }
    
    void CreateCollider(List<GameObject> cubes, GridData data, Vector3 startPosition) {
        Vector3 center = Vector3.Lerp(cubes[0].transform.position, cubes[^1].transform.position, 0.5f);
        center.y = startPosition.y - data.Scale / 2f;
        _boxCollider.center = center;
        _boxCollider.size = new Vector3(data.Size.x * data.Scale, data.Scale, data.Size.y * data.Scale);
    }
}