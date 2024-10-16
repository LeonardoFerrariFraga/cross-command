using System.Collections.Generic;
using UnityEngine;

public class UnitCommandPathDrawer : MonoBehaviour
{
    public IUnitEntity PathEntity { get; private set; }

    Queue<ICommand> _commandQueue;
    ICommandInvoker _invoker;
    LineRenderer _lineRenderer;
    int _index;
    Transform _flag;
    
    public void Initialize(ICommandInvoker invoker) {
        _invoker = invoker;
        _invoker.OnStartExecutingAll += UnparentFlag;
        _invoker.OnStartExecutingNext += RemoveOldestPathPoint;
        _invoker.OnEndExecutingAll += ReparentFlag;
    }

    void OnDestroy() {
        if (_invoker != null) {
            _invoker.OnStartExecutingAll -= UnparentFlag;
            _invoker.OnStartExecutingNext -= RemoveOldestPathPoint;
            _invoker.OnEndExecutingAll -= ReparentFlag;
        }
    }

    void UnparentFlag() {
        _flag.SetParent(null);      
    }
    
    void RemoveOldestPathPoint() {
        if (_lineRenderer.positionCount > 0) {
            Vector3[] newPoints = new Vector3[_lineRenderer.positionCount - 1];
            for (int i = 1; i < _lineRenderer.positionCount; i++) {
                newPoints[i - 1] = _lineRenderer.GetPosition(i);
            }

            _lineRenderer.positionCount--;
            _lineRenderer.SetPositions(newPoints);
        }

        if (_lineRenderer.positionCount == 1) {
            _lineRenderer.positionCount--;
            _flag.gameObject.SetActive(false);
        }
    }
    
    void ReparentFlag() {
        _flag.SetParent(_lineRenderer.transform);
    }

    void Awake() {
        // InChildren also checks itself. In order to get the first children
        // use GetComponentS, and skip itself (0), leading to [1].
        PathEntity = GetComponentsInChildren<IUnitEntity>()[1];
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _flag = _lineRenderer.transform.GetChild(0);
    }
    
    public async void DrawCommand(ICommand command) {
        await command.Execute();
    
        if (_index == 0) {
            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(_index++, transform.position + Vector3.up * 0.01f);
            _flag.gameObject.SetActive(true);
        }
    
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_index++, _lineRenderer.transform.position + Vector3.up * 0.01f);
    }
}