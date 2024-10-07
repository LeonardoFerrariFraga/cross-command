using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Editor tool used to create a grass grid in edit mode.
/// </summary>
public class GridCreator : MonoBehaviour
{
#if UNITY_EDITOR
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
                _instantiatedPrefabs.Add(InstantiateGrass(row, col));
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
    
    GameObject InstantiateGrass(int row, int col) {
        Vector3 scale = Vector3.one * _data.Scale;
        Vector3 position = new (row * scale.x, scale.y, col * scale.z);
        position.y += _data.HeightVariation.RandomInBetween();
        Vector3 euler = Vector3.zero.Add(_data.EulerVariation.RandomInBetween());
        Color color = _data.Color;
        color.AddVariation(_data.ColorVariation);

        Transform grass = ((GameObject)PrefabUtility.InstantiatePrefab(_grassPrefab)).transform;
        grass.localScale = scale;
        grass.position = position;
        grass.rotation = Quaternion.Euler(euler);

        Renderer render = grass.GetComponent<Renderer>();
        MaterialPropertyBlock propertyBlock = new();
        render.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorPropertyID, color);
        render.SetPropertyBlock(propertyBlock);

        grass.name = $"{grass.name} ({row},{col})";
        return grass.gameObject;
    }
#endif
}