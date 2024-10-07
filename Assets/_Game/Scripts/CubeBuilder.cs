using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CubeBuilder
{
    GameObject _prefab = null;
    Vector3 _position = Vector3.zero;
    Quaternion _rotation = Quaternion.identity;
    Vector3 _scale = Vector3.one;
    Color _color = Color.clear;
    int? _colorPropertyID = null;
    string _name = "Cube (Clone)";
    Transform _parent = null;

    public CubeBuilder WithPrefab([CanBeNull] GameObject prefab) {
        if (prefab)
            _prefab = prefab;
        
        return this;
    }

    public CubeBuilder WithPosition(Vector3 position) {
        _position = position;
        return this;
    }

    public CubeBuilder WithRotation(Quaternion rotation) {
        _rotation = rotation;
        return this;
    }

    public CubeBuilder WithScale(Vector3 scale) {
        _scale = scale;
        return this;
    }

    public CubeBuilder WithColor(Color color, int propertyID) {
        _color = color;
        _colorPropertyID = propertyID;
        return this;
    }

    public CubeBuilder WithName(string name) {
        _name = name;
        return this;
    }

    public CubeBuilder WithParent(Transform parent) {
        _parent = parent;
        return this;
    }
    
    public GameObject Build() {
        #if UNITY_EDITOR
        GameObject cube = _prefab ? (GameObject)PrefabUtility.InstantiatePrefab(_prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
        #else
        GameObject cube = _prefab ? Object.Instantiate(_prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
        #endif
        
        if (!_color.Equals(Color.clear) && _colorPropertyID.HasValue) {
            SetColor(cube, _color, _colorPropertyID.Value);
        }

        cube.transform.position = _position;
        cube.transform.rotation = _rotation;
        cube.transform.localScale = _scale;
        cube.name = _name;
        
        if (_parent)
            cube.transform.SetParent(_parent);

        return cube;
    }

    void SetColor(GameObject cube, Color color, int propertyID) {
        Renderer render = cube.GetComponent<Renderer>();
        MaterialPropertyBlock matProperty = new();
        matProperty.SetColor(propertyID, color);
        render.SetPropertyBlock(matProperty);
    }
}