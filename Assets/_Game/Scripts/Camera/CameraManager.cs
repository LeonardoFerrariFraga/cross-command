using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] CinemachineCamera _ccameraDefault;
    [SerializeField] CinemachineCamera _ccameraUnitClose;
    CinemachineBrain _brain;

    public float BlendDuration => _brain.DefaultBlend.Time;
    
    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        _brain = transform.GetComponentInChildren<CinemachineBrain>();
    }
    
    public void ZoomOnUnit(Transform unit) {
        _ccameraUnitClose.Follow = unit;
        _ccameraUnitClose.enabled = true;
        _ccameraDefault.enabled = false;
    }

    public void EnableDefault() {
        _ccameraDefault.enabled = true;
        _ccameraUnitClose.enabled = false;
    }
}