using System.Threading.Tasks;
using UnityEngine;

public class MoveObjectCommand : ICommand
{
    public bool IsExecuting { get; private set; }

    Transform transform;
    float _duration;
    float _jumpHeight;
    float _squashEffector;
    AnimationCurve _scaleCurve;
    AnimationCurve _walkCurve;
    AnimationCurve _jumpCurve;
    AnimationCurve _rotationCurve;
    
    readonly Vector3 _direction;
    
    const float DURATION_JUMP_MULTIPLIER = 1.5f;

    MoveObjectCommand(Transform transform, Vector3 direction) {
        this.transform = transform;
        _direction = direction;  
    } 
    
    public async Task Execute() {
        IsExecuting = true;
        
        MoveOneUnit(_direction);
        await JumpSquashAnimation();

        IsExecuting = false;
    }

    public async Task Undo() {
        IsExecuting = true;
        
        MoveOneUnit(-_direction);
        await JumpSquashAnimation();
    
        IsExecuting = false;
    }
    
    async void MoveOneUnit(Vector3 offset){ 
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + offset;
        
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(endPos - startPos, Vector3.up);
        
        float elapsedTime = 0f;
        while (elapsedTime < _duration) {

            float t = elapsedTime / _duration;
            Vector3 position = Vector3.Lerp(startPos, endPos, _walkCurve.Evaluate(t));
            position.y = Mathf.Lerp(startPos.y, startPos.y + _jumpHeight, _jumpCurve.Evaluate(t));
            transform.position = position;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, _rotationCurve.Evaluate(t));
            
            elapsedTime += Time.deltaTime;
            await Awaitable.FixedUpdateAsync();
        }

        transform.position = endPos;
        transform.rotation = endRotation;
    }

    async Task JumpSquashAnimation() {
        Vector3 startScale = transform.localScale;
        
        float elapsedTime = 0f;
        float dur = _duration * DURATION_JUMP_MULTIPLIER;
        while (elapsedTime < dur) {

            float t = elapsedTime / dur;
            float multiplier = 1f + _squashEffector * _scaleCurve.Evaluate(t);
            transform.localScale = new Vector3(startScale.x / multiplier, startScale.y * multiplier, startScale.z / multiplier);
            
            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        transform.localScale = startScale;
    }

    public class Builder
    {
        float _duration = 0.25f;
        float _jumpHeight = 0f;
        float _squashEffector = 0f;
        
        AnimationCurve _walkCurve = new(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));
        AnimationCurve _rotationCurve = new(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

        AnimationCurve _jumpCurve = new(
            new Keyframe(0f, 0f, 0f, 5f),
            new Keyframe(0.5f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, -5f, 1f));
        
        AnimationCurve _squashCurve = new (
            new Keyframe(0f, 0f, 0f, 7f), 
            new Keyframe(0.4f, 1f, 0f, 0f), 
            new Keyframe(0.7f, 0f, -4f, -4f), 
            new Keyframe(0.8f, -0.3f, 0f, 0f), 
            new Keyframe(0.95f, 0.15f, 0f, 0f), 
            new Keyframe(1f, 0f, -5f, 1f));

        public Builder WithDuration(float duration) {
            _duration = duration;
            return this;
        }

        public Builder WithJumpHeight(float height) {
            _jumpHeight = height;
            return this;
        }

        public Builder WithSquashEffector(float squashEffect) {
            _squashEffector = squashEffect;
            return this;
        }

        public Builder WithSquashCurve(AnimationCurve curve) {
            _squashCurve = curve;
            return this;
        }
        
        public Builder WithWalkCurve(AnimationCurve curve) {
            _walkCurve = curve;
            return this;
        }
        
        public Builder WithJumpCurve(AnimationCurve curve) {
            _jumpCurve = curve;
            return this;
        }
        
        public Builder WithRotationCurve(AnimationCurve curve) {
            _rotationCurve = curve;
            return this;
        }
        
        public ICommand Build(Transform transform, Vector3 direction) {
            MoveObjectCommand command = new (transform, direction) {
                _duration = _duration,
                _jumpHeight = _jumpHeight,
                _squashEffector = _squashEffector,
                _scaleCurve = _squashCurve,
                _walkCurve = _walkCurve,
                _jumpCurve = _jumpCurve,
                _rotationCurve = _rotationCurve
            };

            return command;
        }
        
        public ICommand Build(Transform transform, Vector3 direction, MoveObjectData data) {
            MoveObjectCommand command = new (transform, direction) {
                _duration = data.Duration,
                _jumpHeight = data.JumpHeight,
                _squashEffector = data.SquashEffector,
                _scaleCurve = data.SquashCurve,
                _walkCurve = data.WalkCurve,
                _jumpCurve = data.JumpCurve,
                _rotationCurve = data.RotationCurve
            };

            return command;
        }
    }
}