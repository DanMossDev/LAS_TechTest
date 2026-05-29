using Unity.VisualScripting;
using UnityEngine;

namespace LAS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private float _startSpeed = 10;
        [SerializeField] private float _acceleration = 0.01f;
        [SerializeField] private float _gravity = 0.5f;

        [SerializeField] private float _jumpSpeed = 2;

        [SerializeField] private Transform _playerVisuals;
        [SerializeField] private float _rotationAirSpeed = 100.0f;
        [SerializeField] private float _rotationGroundSpeed = 10.0f;
        [SerializeField] private float _groundedDistance = 0.1f;
        
        [SerializeField] private float _minAirTimeToCheckForDeath = 0.1f;
        [SerializeField, Range(-1,1)] private float _dotProductDeathThreshold = 0.1f;
        
        [SerializeField] private UIManager _uiManager;

        private float _rotationInput;
        private float _timeInAir;
        public float _speed;

        private bool _isGrounded;
        
        private Vector2 _movement;

        private void OnEnable()
        {
            _speed = _startSpeed;
            SubscribeToInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            InputManager.MoveInput += OnMove;
            InputManager.JumpInput += OnJump;
        }

        private void UnsubscribeFromInput()
        {
            InputManager.MoveInput -= OnMove;
            InputManager.JumpInput -= OnJump;
        }

        private void OnMove(Vector2 direction)
        {
            _rotationInput = direction.x;
        }

        private void OnJump(bool held)
        {
            if (held && _isGrounded)
                _movement.y = _jumpSpeed;
            else if (_movement.y > 0)
                _movement.y /= 5.0f;
        }
        
        private void Update()
        {
            bool wasGrounded = _isGrounded;
            
            _speed += _acceleration * Time.deltaTime;
            _movement.x = Time.deltaTime * _speed;
            
            float x = transform.position.x + _movement.x;
            float groundY = _terrainManager.GetTerrainHeightAtX(x);

            if (groundY >= transform.position.y && _movement.y <= 0)
            {
                transform.position = new Vector3(x, groundY, 0);
                _movement.y = 0;
            }
            else
            {
                _movement.y -= _gravity * Time.deltaTime;
                transform.position = new Vector3(x, Mathf.Max(transform.position.y + _movement.y * Time.deltaTime, groundY), 0);
            }
            
            _isGrounded = (transform.position.y - groundY) < _groundedDistance;

            if (_isGrounded)
            {
                Vector3 normal = _terrainManager.GetNormalAtPosition(x);
                if (!wasGrounded && _timeInAir > _minAirTimeToCheckForDeath)
                {
                    if (CheckForDeath(normal))
                    {
                        Die();
                        return;
                    }
                    
                }
                
                _timeInAir = 0;
                LerpTransformUp(normal);
            }
            else
            {
                _timeInAir += Time.deltaTime;
                Rotate();
            }
            
            _uiManager.UpdateDistanceTravelled(transform.position.x);
        }

        private bool CheckForDeath(Vector3 normal)
        {
            return Vector3.Dot(_playerVisuals.up, normal) < _dotProductDeathThreshold;
        }

        private void Die()
        {
            UnsubscribeFromInput();
            _uiManager.ShowEndScreen();
        }

        private void Rotate()
        {
            _playerVisuals.localEulerAngles = new Vector3(0, 0, _playerVisuals.localEulerAngles.z - _rotationAirSpeed * _rotationInput * Time.deltaTime);
        }
        
        private void LerpTransformUp(Vector3 targetUp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_playerVisuals.forward, targetUp);
            _playerVisuals.rotation = Quaternion.Slerp(_playerVisuals.rotation, targetRotation, Time.deltaTime * _rotationGroundSpeed);
        }
    }
}