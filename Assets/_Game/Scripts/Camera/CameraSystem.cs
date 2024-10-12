using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] CinemachineCamera _cinemachineCamera;
    
    [FoldoutGroup("Movement"), SerializeField] bool _usePanScrolling;
    bool _usePanPrevious;
    CameraMovement _panScrolling;
    
    [FoldoutGroup("Movement"), SerializeField] bool _useEdgeScrolling;
    bool _useEdgePrevious;
    CameraMovement _edgeScrolling;
    
    [FoldoutGroup("Movement"), SerializeField] bool _useWASDMovement;
    bool _useWASDPrevious;
    CameraMovement _wasdMovement;
    
    [FoldoutGroup("Movement"), SerializeField] float _speed;
    float _previousSpeed;
    
    [FoldoutGroup("Rotation"), SerializeField] float _rotationSpeed;
    
    float _rotationInput;
    bool _rotating;
    
    [FoldoutGroup("Zoom"), SerializeField] float _zoomStep;
    [FoldoutGroup("Zoom"), SerializeField] float _zoomDamp;
    [FoldoutGroup("Zoom/z", GroupName = "Z Axis"), SerializeField] AnimationCurve _zCurve;
    [FoldoutGroup("Zoom/z"), SerializeField] MinMaxFloat _minMaxDistanceZ;
    [FoldoutGroup("Zoom/y", GroupName = "Y Axis"), SerializeField] AnimationCurve _yCurve;
    [FoldoutGroup("Zoom/y"), SerializeField] MinMaxFloat _minMaxDistanceY;
    [FoldoutGroup("Zoom/fov", GroupName = "Field of View"), SerializeField] AnimationCurve _fovCurve;
    [FoldoutGroup("Zoom/fov"), SerializeField] MinMaxFloat _minMaxFieldOfView;
    
    float _zoomNormalized;
    float _zoomToAdd;
    float _scrollInput;
    bool _zoomingIn;
    
    CinemachineFollow _cinemachineFollow;

    const float COMPARISON_TOLERANCE = 0.0001f;
    
#if UNITY_EDITOR
    void OnValidate() {
        if (_usePanScrolling != _usePanPrevious) {
            _panScrolling = new CameraPanScrolling(transform, _speed);
            _usePanPrevious = _usePanScrolling;
        }

        if (_useEdgeScrolling != _useEdgePrevious) {
            _edgeScrolling = new CameraEdgeScrolling(transform, _speed);
            _useEdgePrevious = _useEdgeScrolling;
        }

        if (_useWASDMovement != _useWASDPrevious) {
            _wasdMovement = new CameraWASD(transform, _speed);
            _useWASDPrevious = _useWASDMovement;
        }
        
        _previousSpeed = _speed;
    }
#endif

    #region Initialization
    
    void Awake() => _cinemachineFollow = _cinemachineCamera.GetComponent<CinemachineFollow>();

    void Start() {
        CreateMovementClass();
        InitializeZoomValues();
    }

    void CreateMovementClass() {
        if (_usePanScrolling)
            _panScrolling = new CameraPanScrolling(transform, _speed);

        if (_useEdgeScrolling)
            _edgeScrolling = new CameraEdgeScrolling(transform, _speed);

        if (_useWASDMovement)
            _wasdMovement = new CameraWASD(transform, _speed);
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
    
    #endregion
    
    #region Get Inputs on Update
    
    void Update() {
        if (_usePanScrolling)
            _panScrolling.GetInputs();

        if (_useWASDMovement) 
            _wasdMovement.GetInputs();
        
        if (_useEdgeScrolling && OnlyMovingThroughEdge())
            _edgeScrolling.GetInputs();
        
        GetZoomInput();
        GetRotationInput();
    }

    bool OnlyMovingThroughEdge() => (!_useWASDMovement || !_wasdMovement.HasInput) && (!_usePanScrolling || !_panScrolling.HasInput);
    void GetZoomInput() => _scrollInput = Input.mouseScrollDelta.y;
    void GetRotationInput() {
        if (Input.GetMouseButtonDown(1)) 
            _rotating = true;
        else if (Input.GetMouseButtonUp(1)) {
            _rotating = false;
            _rotationInput = 0;
        }

        if (_rotating) {
            _rotationInput = Input.GetAxisRaw("Mouse X");
        }
    }
    
    #endregion
    
    #region Apply Movement, Rotation and Zoom on LateUpdate
    
    void LateUpdate() {
        if (_usePanScrolling)
            _panScrolling.MoveCamera();

        if (_useWASDMovement) 
            _wasdMovement.MoveCamera();

        if (_useEdgeScrolling && MouseOnGameView()) {
            _edgeScrolling.MoveCamera();
        }

        Zoom();
        Rotation();
    }
    
    bool MouseOnGameView() {
        Vector2 mousePos = Input.mousePosition;
        return (mousePos.x >= 0 && mousePos.x < Screen.width) && (mousePos.y >= 0f && mousePos.y < Screen.height);
    }
    
    #region Rotation
    
    void Rotation() {
        if (!_rotating && _rotationInput == 0f) return;
        transform.Rotate(Vector3.up, _rotationInput * _rotationSpeed * Time.deltaTime);
    }
    
    #endregion
    
    #region Zoom
    
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

        if (ZoomAtMinOrMax(y, z, fov))
            _zoomToAdd = 0f;
        else
            _zoomToAdd = Mathf.Clamp(_zoomToAdd - _zoomDamp, 0f, float.MaxValue);
    }
    bool ZoomAtMinOrMax(float y, float z, float fov) => _minMaxDistanceY.IsMinOrMax(y) || _minMaxDistanceZ.IsMinOrMax(z) || FieldOfViewIsMinOrMax(fov); 
    bool FieldOfViewIsMinOrMax(float fov) => Math.Abs(fov - _minMaxFieldOfView.min) < COMPARISON_TOLERANCE || Math.Abs(fov - _minMaxFieldOfView.max) < COMPARISON_TOLERANCE;
    
    #endregion
    
    #endregion
}