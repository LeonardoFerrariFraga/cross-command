using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    enum CameraMoveType { WASD, EdgeScrolling, PanScrolling }

    [SerializeField] CinemachineCamera _cinemachineCamera;
    [FoldoutGroup("Movement"), SerializeField] CameraMoveType _moveType;
    CameraMoveType _previousMoveType;
    [FoldoutGroup("Movement"), SerializeField] float _speed;
    float _previousSpeed;

    [FoldoutGroup("Zoom"), SerializeField, ReadOnly, Range(0f, 1f)] float _zoomNormalized;
    [FoldoutGroup("Zoom"), SerializeField] float _zoomStep;
    [FoldoutGroup("Zoom"), SerializeField] float _zoomDamp;
    [FoldoutGroup("Zoom/z", GroupName = "Z Axis"), SerializeField] AnimationCurve _zCurve;
    [FoldoutGroup("Zoom/z"), SerializeField] MinMaxFloat _minMaxDistanceZ;
    [FoldoutGroup("Zoom/y", GroupName = "Y Axis"), SerializeField] AnimationCurve _yCurve;
    [FoldoutGroup("Zoom/y"), SerializeField] MinMaxFloat _minMaxDistanceY;
    [FoldoutGroup("Zoom/fov", GroupName = "Field of View"), SerializeField] AnimationCurve _fovCurve;
    [FoldoutGroup("Zoom/fov"), SerializeField] MinMaxFloat _minMaxFieldOfView;
    
    float _zoomToAdd;
    float _scrollInput;
    bool _zoomingIn;
    
    CameraMovement _cameraMovement;
    CinemachineFollow _cinemachineFollow;

    const float COMPARISON_TOLERANCE = 0.0001f;
    
#if UNITY_EDITOR
    void OnValidate() {
        if (_moveType != _previousMoveType || Math.Abs(_speed - _previousSpeed) > 0.001f) {
            CreateMovementClass(_moveType);    
        }
        
        _previousMoveType = _moveType;
        _previousSpeed = _speed;
    }
#endif

    void Awake() => _cinemachineFollow = _cinemachineCamera.GetComponent<CinemachineFollow>();

    void Start() {
        CreateMovementClass(_moveType);
        InitializeZoomValues();
    }

    void CreateMovementClass(CameraMoveType type) {
        _cameraMovement = type switch {
            CameraMoveType.EdgeScrolling => new CameraEdgeScrolling(transform, _speed),
            CameraMoveType.WASD => new CameraWASD(transform, _speed),
            CameraMoveType.PanScrolling => new CameraPanScrolling(transform, _speed),
            _ => default
        };
    }

    /// <summary>
    /// To prevent the camera to instantly zoom in at the start (_zoomNormalized at 0),
    /// gets the normalized average of all zoom fields (y, z and field of view).
    /// </summary>
    void InitializeZoomValues() {
        float y = Mathf.InverseLerp(_minMaxDistanceY.min, _minMaxDistanceY.max, _cinemachineFollow.FollowOffset.y);
        float z = Mathf.InverseLerp(_minMaxDistanceZ.min, _minMaxDistanceZ.max, _cinemachineFollow.FollowOffset.z);
        float fov = Mathf.InverseLerp(_minMaxFieldOfView.min, _minMaxFieldOfView.max, _cinemachineCamera.Lens.FieldOfView);
        _zoomNormalized = (y + z + fov) / 3f;
    }
    
    void Update() {
        _cameraMovement.GetInputs();
        _scrollInput = Input.mouseScrollDelta.y;
    }
    
    void LateUpdate() {
        _cameraMovement.MoveCamera();
        Zoom();
    }

    void Zoom() {
        if (_scrollInput == 0f && _zoomToAdd == 0) return;
        
        if (_scrollInput > 0)
            _zoomingIn = true;
        else if (_scrollInput < 0)
            _zoomingIn = false;
        
        _zoomToAdd += Mathf.Abs(_scrollInput) * _zoomStep;
        _zoomNormalized = Mathf.Lerp(_zoomNormalized, _zoomNormalized + (_zoomingIn ? -_zoomToAdd : _zoomToAdd), Time.deltaTime);
        _zoomNormalized = Mathf.Clamp01(_zoomNormalized);

        Vector3 followOffset = _cinemachineFollow.FollowOffset;
        float y = Mathf.Lerp(_minMaxDistanceY.min, _minMaxDistanceY.max, _yCurve.Evaluate(_zoomNormalized));
        float z = Mathf.Lerp(_minMaxDistanceZ.min, _minMaxDistanceZ.max, _zCurve.Evaluate(_zoomNormalized));
        _cinemachineFollow.FollowOffset = new Vector3(followOffset.x, y, z);

        float fov = Mathf.Lerp(_minMaxFieldOfView.min, _minMaxFieldOfView.max, _fovCurve.Evaluate(_zoomNormalized));
        _cinemachineCamera.Lens.FieldOfView = fov;

        if (_minMaxDistanceY.IsMinOrMax(y) || _minMaxDistanceZ.IsMinOrMax(z) || FieldOfViewIsMinOrMax(fov))
            _zoomToAdd = 0f;
        else
            _zoomToAdd = Mathf.Clamp(_zoomToAdd - _zoomDamp, 0f, float.MaxValue);
    }

    bool FieldOfViewIsMinOrMax(float fov) => Math.Abs(fov - _minMaxFieldOfView.min) < COMPARISON_TOLERANCE || Math.Abs(fov - _minMaxFieldOfView.max) < COMPARISON_TOLERANCE;
}