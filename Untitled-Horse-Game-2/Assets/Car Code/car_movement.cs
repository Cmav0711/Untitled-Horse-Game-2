using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class car_movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float motorForce = 35f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float brakeForce = 45f;

    [Header("Steering")]
    [SerializeField] private float steeringPower = 3.2f;
    [SerializeField] private float steeringAtSpeed = 0.08f;
    [SerializeField] private float minSteeringFactorAtTopSpeed = 0.35f;
    [SerializeField] private float minSpeedToSteer = 0.5f;

    [Header("Grip / Drift")]
    [SerializeField] private float normalLateralGrip = 6f;
    [SerializeField] private float driftLateralGrip = 2f;
    [SerializeField] private float driftSteeringMultiplier = 1.4f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private float groundCheckRadius = 0.35f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Air Control")]
    [SerializeField] private float airYawTorque = 40f;
    [SerializeField] private float airPitchTorque = 25f;

    [Header("Upright Assist")]
    [SerializeField] private float uprightStrength = 18f;
    [SerializeField] private float uprightDamping = 2.5f;

    private Rigidbody rb;
    private float throttleInput;
    private float steeringInput;
    private bool isBraking;
    private bool isDrifting;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.centerOfMass = new Vector3(0f, -0.35f, 0f);
        rb.constraints = RigidbodyConstraints.None;
    }

    private void Update()
    {
        throttleInput = GetThrottleInput();
        steeringInput = GetSteeringInput();
        isBraking = IsBrakePressed();
        isDrifting = IsDriftPressed();
    }

    private void FixedUpdate()
    {
        UpdateGroundedState();

        if (!isGrounded)
        {
            ApplyAirControl();
            return;
        }

        ApplyForwardDrive();
        ApplySteering();
        ApplyLateralGrip();
        ApplyUprightAssist();
    }

    private void ApplyAirControl()
    {
        Vector3 yawTorque = Vector3.up * steeringInput * airYawTorque;
        Vector3 pitchTorque = transform.right * -throttleInput * airPitchTorque;
        rb.AddTorque(yawTorque + pitchTorque, ForceMode.Acceleration);
    }

    private void ApplyUprightAssist()
    {
        Vector3 tiltAxis = Vector3.Cross(transform.up, Vector3.up);
        rb.AddTorque(tiltAxis * uprightStrength - rb.angularVelocity * uprightDamping, ForceMode.Acceleration);
    }

    private void UpdateGroundedState()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        isGrounded = Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out _, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void ApplyForwardDrive()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (Mathf.Abs(forwardSpeed) < maxSpeed || Mathf.Sign(throttleInput) != Mathf.Sign(forwardSpeed))
        {
            Vector3 force = transform.forward * throttleInput * motorForce;
            rb.AddForce(force, ForceMode.Acceleration);
        }

        if (isBraking)
        {
            Vector3 brakeVector = -rb.linearVelocity * brakeForce;
            rb.AddForce(brakeVector, ForceMode.Acceleration);
        }
    }

    private void ApplySteering()
    {
        float signedForwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float forwardSpeed = Mathf.Abs(signedForwardSpeed);

        // Real-car style: no steering response while fully stopped.
        if (forwardSpeed < minSpeedToSteer)
        {
            return;
        }

        float steeringScale = Mathf.Max(minSteeringFactorAtTopSpeed, 1f - forwardSpeed * steeringAtSpeed);
        float driftMultiplier = isDrifting ? driftSteeringMultiplier : 1f;
        float reverseSteering = Mathf.Sign(signedForwardSpeed);

        float turnAmount = steeringInput * reverseSteering * steeringPower * steeringScale * driftMultiplier * Time.fixedDeltaTime * 50f;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void ApplyLateralGrip()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(rb.linearVelocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);

        float targetGrip = isDrifting ? driftLateralGrip : normalLateralGrip;
        Vector3 adjustedRightVelocity = Vector3.Lerp(rightVelocity, Vector3.zero, targetGrip * Time.fixedDeltaTime);

        rb.linearVelocity = forwardVelocity + adjustedRightVelocity;
    }

    private static float GetThrottleInput()
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        float forward = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) ? 1f : 0f;
        float reverse = (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) ? 1f : 0f;
        return forward - reverse;
    }

    private static float GetSteeringInput()
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        float right = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) ? 1f : 0f;
        float left = (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) ? 1f : 0f;
        return right - left;
    }

    private static bool IsBrakePressed()
    {
        return Keyboard.current != null &&
               (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
    }

    private static bool IsDriftPressed()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
    }
}
