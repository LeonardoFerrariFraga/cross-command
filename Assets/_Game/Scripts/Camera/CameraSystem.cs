using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    enum CameraMoveType { WASD, EdgeScrolling, PanScrolling }

    [SerializeField] CinemachineCamera _cinemachineCamera;
    [SerializeField] CameraMoveType _moveType;
    CameraMoveType _previousMoveType;
    [SerializeField] float _speed;
    float _previousSpeed;
    
    CameraMovement _cameraMovement;

#if UNITY_EDITOR
    void OnValidate() {
        if (_moveType != _previousMoveType || Math.Abs(_speed - _previousSpeed) > 0.001f) {
            CreateMovementClass(_moveType);    
        }
        
        _previousMoveType = _moveType;
        _previousSpeed = _speed;
    }
#endif

    void Start() => CreateMovementClass(_moveType);
    void CreateMovementClass(CameraMoveType type) {
        _cameraMovement = type switch {
            CameraMoveType.EdgeScrolling => new CameraEdgeScrolling(transform, _speed),
            CameraMoveType.WASD => new CameraWASD(transform, _speed),
            CameraMoveType.PanScrolling => new CameraPanScrolling(transform, _speed),
            _ => default
        };
    }
    
    void Update() {
        _cameraMovement.GetInputs();
    }

    void LateUpdate() {
        _cameraMovement.MoveCamera();
    }
}