using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rearWheel;
    [SerializeField]
    private Rigidbody2D frontWheel;
    [SerializeField]
    private Rigidbody2D vehicleBody;
    private InputAction moveAction;
    public float torqueCoefficient = 40000f;
    public float brakingCoefficient = 10f;
    public float powerRatio = 1f;
    public float lockupThreshold = 10f;
    public float reversePenalty = 0.2f;
    public float reverseTimeout = 0.5f;
    public float reverseThreshold = 5f;
    public float maxRpm = 12000f;
    public float revAccelerationRate = 0.25f;
    public float revDecayRate = 1f;
    public float fuelCapacity = 100f;
    public float fuelConsumptionCoefficient = 2f;
    public float refuellingCoefficient = 10f;
    private float currentFuel;
    private float torqueFraction; //Dependent on revFraction, do not update manually
    private float revFraction = 0f;
    private const float initTorqueFraction = 0.36f; //worked out via graphing
    private const float peakRevCoef = 0.8f; //linked to torque fraction: DO NOT CHANGE without updating initial torque fraction
    private float reverseTimer = 0f;
    private float throttleValue;
    private float forwardVelocity;
    private bool reversedState;
    private bool throttling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentFuel = fuelCapacity;
        reverseTimer = 0f;
        revFraction = 0f;
        torqueFraction = initTorqueFraction;
        throttling = false;
        reversedState = false;
        // rearWheel = transform.Find("Rear Wheel").gameObject.GetComponent<Rigidbody2D>();
        // frontWheel = transform.Find("Front Wheel").gameObject.GetComponent<Rigidbody2D>();
        // vehicleBody = transform.Find("Vehicle Body").gameObject.GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        throttling = false;
        throttleValue = moveAction.ReadValue<Vector2>().x;
        throttleValue = Mathf.Max(-reversePenalty, throttleValue);
        forwardVelocity = vehicleBody.linearVelocityX * Mathf.Cos(Mathf.Deg2Rad * vehicleBody.rotation) + vehicleBody.linearVelocityY * Mathf.Sin(Mathf.Deg2Rad * vehicleBody.rotation);
        UpdateReverseState();
        if (!reversedState)
        {
            if (throttleValue >= 0)
            {
                ApplyDrivingTorque();
            }
            else
            {
                ApplyBraking();
            }
        }
        else
        {
            if (throttleValue <= 0)
            {
                ApplyDrivingTorque();
            }
            else
            {
                ApplyBraking();
            }
        }
        UpdateTorqueFraction();
        ConsumeFuel(torqueFraction);
        LimitRpm();
    }

    private bool ReverseStateCheck()
    { //Checks if reverse state should be entered based on input relative to the velocity of the vehicle and a timeout
        if (forwardVelocity <= 0 && throttleValue < 0)
        {
            return true;
        }
        else if (((forwardVelocity <= 0 && throttleValue <= 0) || throttleValue < 0) && Mathf.Abs(forwardVelocity) < reverseThreshold)
        {
            reverseTimer += Time.fixedDeltaTime;
            if (reverseTimer > reverseTimeout)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        reverseTimer = 0f;
        return false;
    }

    private void UpdateReverseState()
    { //serves to mimic the drop in revs on gear change
        bool isReversing = ReverseStateCheck();
        if (reversedState != isReversing)
        {
            revFraction /= 2;
        }
        reversedState = isReversing;
    }

    private void ApplyDrivingTorque()
    {
        rearWheel.freezeRotation = false;
        frontWheel.freezeRotation = false;
        if (currentFuel > 0)
        {
            rearWheel.AddTorque(-throttleValue * torqueCoefficient * torqueFraction * powerRatio * Time.fixedDeltaTime);
            frontWheel.AddTorque(-throttleValue * torqueCoefficient * torqueFraction * (1 - powerRatio) * Time.fixedDeltaTime);
        }
        if (throttleValue != 0)
        {
            throttling = true;
        }
    }

    private void ApplyBraking()
    {
        if (Mathf.Abs(rearWheel.angularVelocity) > lockupThreshold)
        { //TODO: Add condition to check if wheels are on ground
            rearWheel.AddTorque(-throttleValue * torqueCoefficient * brakingCoefficient * Time.fixedDeltaTime);
            frontWheel.AddTorque(-throttleValue * torqueCoefficient * brakingCoefficient * Time.fixedDeltaTime);
        }
        else
        {
            rearWheel.freezeRotation = true;
            frontWheel.freezeRotation = true;
        }
    }

    private void LimitRpm()
    {
        rearWheel.angularVelocity = Mathf.Clamp(rearWheel.angularVelocity, -maxRpm, maxRpm);
        frontWheel.angularVelocity = Mathf.Clamp(frontWheel.angularVelocity, -maxRpm, maxRpm);
    }

    private void UpdateTorqueFraction()
    {
        if (throttling && currentFuel > 0)
        {
            revFraction += revAccelerationRate * Time.fixedDeltaTime;
        }
        else
        {
            revFraction -= revDecayRate * Time.fixedDeltaTime;
        }
        revFraction = Mathf.Clamp(revFraction, 0, 1);
        torqueFraction = 1 - Mathf.Pow(revFraction - peakRevCoef, 2);
    }

    private void ConsumeFuel(float torqueFraction)
    {
        if (currentFuel > 0)
        {
            currentFuel -= torqueFraction * fuelConsumptionCoefficient * Time.fixedDeltaTime;
        }
        else
        {
            currentFuel = 0;
        }
    }

    public int GetFuel()
    {
        return (int)currentFuel;
    }

    public float GetRevFraction()
    {
        return revFraction;
    }

    public void RefuelVehicle()
    {
        if (currentFuel < fuelCapacity)
        {
            currentFuel += refuellingCoefficient * Time.fixedDeltaTime;
        }
        Debug.Log("Refuelling...");
    }
}
