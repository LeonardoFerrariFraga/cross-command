using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Unit_01 : UnitEntity
{
    [SerializeField] Transform _visuals;
    [Space(15)]
    [SerializeField, InlineEditor] UnitAnimatedMoveBehaviour _moveBehaviour;

    protected override void Awake() {
        base.Awake();
        PathDrawer.Initialize(Invoker);
    }

    public override async Task Move(Vector3 direction) {
        CancellationToken cancellationToken = new();
        await _moveBehaviour.AnimatedMove(transform, _visuals, direction, cancellationToken);
    }
}