using System;
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
            OnMouseClick(Input.mousePosition);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
            _current = null;
            foreach (CommandButtonChooser button in _buttons) {
                button.Clear(true);
            }
        }
    }
    
    void OnMouseClick(Vector3 mousePosition) {
        if (!Physics.Raycast(_camera.ScreenPointToRay(mousePosition), out RaycastHit hitInfo, 1000f, _unitLayer)) return;

        if (hitInfo.transform != _current) {
            _current = hitInfo.transform;
            
            foreach (CommandButtonChooser button in _buttons)
                button.Clear(true);
        }
        
        UnitCommandContainer container = hitInfo.transform.GetComponent<UnitCommandContainer>();
        if (!container)
            return;

        UnitCommandInvoker commandInvoker = container.gameObject.GetComponent<UnitCommandInvoker>();
        
        for (int i = 0; i < container.Commands.Length; i++) {
            int currentIndex = i;

            CommandButtonChooser button;
            if (i < _buttons.Count) {
                button = _buttons[i];
            }
            else {
                button = Instantiate(_buttonPrefab, _buttonHolder).GetComponent<CommandButtonChooser>();
                _buttons.Add(button);
            }
            
            Func<ICommand> createCommand = container.Commands[currentIndex].command;
            string label = container.Commands[currentIndex].name;
            button.InitializeButton(() => commandInvoker.Schedule(createCommand()), label);
        }
    }
}