using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUnitCommandOptions : MonoBehaviour
{
    static Transform _current;
    
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] Transform _buttonHolder;
    [SerializeField] LayerMask _unitLayer;
    
    List<CommandButtonChooser> _buttons = new();
    Camera _camera;
    
    void Awake() => _camera = FindAnyObjectByType<Camera>();

    void OnDestroy() => _current = null;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(OnMouseClick(Input.mousePosition));
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
            _current = null;
            CameraManager.Instance.EnableDefault();
            
            foreach (CommandButtonChooser button in _buttons) {
                button.Clear(true);
            }
        }
    }
    
    IEnumerator OnMouseClick(Vector3 mousePosition) {
        if (!Physics.Raycast(_camera.ScreenPointToRay(mousePosition), out RaycastHit hitInfo, 10000f, _unitLayer)) yield break;

        if (hitInfo.transform != _current) {
            _current = hitInfo.transform;
            
            foreach (CommandButtonChooser button in _buttons)
                button.Clear(true);
        }
        
        UnitEntity entity = hitInfo.transform.GetComponent<UnitEntity>();
        if (entity == null)
            yield break;
        
        CameraManager.Instance.ZoomOnUnit(hitInfo.transform);
        yield return new WaitForSeconds(CameraManager.Instance.BlendDuration);
        
        for (int i = 0; i < entity.Container.Commands.Length; i++) {
            int currentIndex = i;

            CommandButtonChooser button = GetOrInstantiate(i);
            ScriptableObjectCommand soCommand = entity.Container.Commands[currentIndex];
            button.InitializeButton(() => ScheduleCommandAndMovePathDrawer(entity, soCommand), soCommand.Label);
        }
    }

    void ScheduleCommandAndMovePathDrawer(UnitEntity entity, ScriptableObjectCommand soCommand) {
        entity.Scheduler.Schedule(soCommand.CreateCommand(entity));
        entity.PathDrawer.DrawCommand(soCommand.CreateCommand(entity.PathDrawer.PathEntity));
    }
    
    CommandButtonChooser GetOrInstantiate(int index) {
        if (index < _buttons.Count) {
            return _buttons[index];
        }
        
        CommandButtonChooser button = Instantiate(_buttonPrefab, _buttonHolder).GetComponent<CommandButtonChooser>();
        _buttons.Add(button);
        return button;
    }
}