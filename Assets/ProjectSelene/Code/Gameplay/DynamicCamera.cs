using System;
using ProjectSelene.Code.CustomPhysics;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class DynamicCamera : MonoBehaviour
{
    [SerializeField] private CustomRigidbody rb;
    [SerializeField] private Transform player;

    [Header("Distance/Offset")]
    [SerializeField] private Vector3 minOffset = new Vector3(0, 3, -0.5f);
    [SerializeField] private Vector3 maxOffset = new Vector3(0, 4, -8f);
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeThreshold = 5f;  // speed where shake starts
    [SerializeField] private float maxShakeAmount = 0.3f;
    [SerializeField] private float shakeSpeed = 25f;

    private float _currentSpeed;
    private Vector3 _smoothDampVelocity; // camera's smoothing state (not the RB velocity!)

    private void FixedUpdate()
    {
        // Only read motion data here so it stays in sync with physics
        _currentSpeed = rb != null ? rb.Speed : 0f;
    }

    private void LateUpdate()
    {
        // 1) Distance/offset interpolation
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, _currentSpeed);
        Vector3 targetOffset = Vector3.Lerp(minOffset, maxOffset, t);

        // Base follow target (no shake yet)
        Vector3 followTarget = player.position + targetOffset;

        // 2) Smooth only the base position
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            followTarget,
            ref _smoothDampVelocity,
            smoothTime
        );

        // 3) Compute shake separately so SmoothDamp doesn't erase it
        Vector3 shakeOffset = Vector3.zero;
        if (_currentSpeed > shakeThreshold)
        {
            float shakeFactor = Mathf.InverseLerp(shakeThreshold, maxSpeed, _currentSpeed);
            float shakeAmount = maxShakeAmount * shakeFactor;

            // Perlin-based 2D shake (X/Y). Keep Z at 0 so distance doesn't wobble.
            float nx = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f;

            shakeOffset = new Vector3(nx, ny, 0f) * shakeAmount;
        }

        // 4) Apply shake AFTER smoothing
        transform.position = smoothedPosition + shakeOffset;

        // 5) Keep looking at the player
        transform.LookAt(player);
    }
    }
}