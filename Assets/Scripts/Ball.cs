using System;
using UnityEditor;
using UnityEngine;

public struct PitchData{
    public float velocity;
    public Vector3 spinAxis;
    public float RPM;

    public PitchData(float _velocity, Vector3 _spinAxis, float _RPM){
        velocity = _velocity;
        spinAxis = _spinAxis.normalized;
        RPM = _RPM;
    }
}

public class Ball : MonoBehaviour
{
    public Transform visual;
    public Vector3 velocity = Vector3.zero;
    float mass = 0.141748f;
    float radius = 0.037f;

    public float airDensity = 1.225f;
    public PitchData pitchData;
    public BallType ballType;

    // New fields to track break (displacement due to lift)
    [NonSerialized] public Vector3 displacementDueToLift = Vector3.zero;
    private Vector3 _velocityDueToLift = Vector3.zero;
    public float reynoldsNumber;

    public void Initialize(float _aidDensity, PitchData _pitchData)
    {
        mass = ballType.mass;
        radius = ballType.radius;

        airDensity = _aidDensity;
        pitchData = _pitchData;
        velocity = _pitchData.velocity * transform.forward;
    }
    
    void Update()
    {        
        // Apply gravity
        AddForce(mass * Physics.gravity * Time.deltaTime);

        // Calculate and apply lift force
        Vector3 liftForce = GetLift() * Time.deltaTime;
        AddForce(liftForce);

        // Track velocity and displacement caused by lift
        Vector3 deltaVLift = liftForce / mass;
        _velocityDueToLift += deltaVLift;
        displacementDueToLift += _velocityDueToLift * Time.deltaTime;

        // Update position
        transform.position += velocity * Time.deltaTime;

        AddForce(GetDrag() * Time.deltaTime);

        visual.Rotate(Quaternion.LookRotation(transform.forward, Vector3.up)*pitchData.spinAxis, pitchData.RPM * 6 * Time.deltaTime, Space.World);
    }
    void AddForce(Vector3 force)
    {
        velocity += force / mass;
    }

    public Vector3 GetLift()
    {
        // Convert RPM to angular velocity (rad/s)
        float spin = (pitchData.RPM * 2f * Mathf.PI) / 60f;

        Quaternion rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        // Rotate the spin axis into world space
        Vector3 _spinAxis = rotation * pitchData.spinAxis;
        
        // Normalize vectors
        Vector3 normalizedSpin = _spinAxis.normalized;
        Vector3 normalizedVelocity = velocity.normalized;

        // Calculate spin efficiency
        float dotProduct = Vector3.Dot(normalizedSpin, normalizedVelocity);
        Vector3 parallelSpin = dotProduct * normalizedVelocity;
        Vector3 perpendicularSpin = normalizedSpin - parallelSpin;
        float spinEfficiency = perpendicularSpin.magnitude;
        Vector3 realSpinAxis = (perpendicularSpin.magnitude > 0.0001f) ?
                              perpendicularSpin.normalized : Vector3.zero;

        // Debug visualization
        float debugScale = 2f;  // Adjust based on your scene scale
        Debug.DrawRay(transform.position, velocity.normalized * debugScale, Color.red);
        Debug.DrawRay(transform.position, _spinAxis.normalized * debugScale, Color.green);
        Debug.DrawRay(transform.position, realSpinAxis * debugScale, Color.blue);

        // Cross product gives lift direction
        Vector3 liftDirection = Vector3.Cross(realSpinAxis, velocity.normalized).normalized;
        // Cross-sectional area
        float area = Mathf.PI * radius * radius;

        float spinParameter = (radius * spin) / velocity.magnitude;
        //float effectiveCl = Mathf.Min(Cl, 0.5f * spinParameter);
        float effectiveCl = ballType.ClCurve.Evaluate(spinParameter);

        // Magnus force formula: 0.5 * ρ * A * Cₗ * v²
        float forceMagnitude = 0.5f * airDensity * area * effectiveCl * velocity.sqrMagnitude * spinEfficiency;

        Debug.DrawRay(transform.position, liftDirection*forceMagnitude, Color.magenta);
        return liftDirection * forceMagnitude;
    }

    public Vector3 GetDrag(){
        float area = Mathf.PI * radius * radius;
        float magnitude = 0.5f * airDensity * velocity.magnitude * velocity.magnitude * area * ballType.CdCurve.Evaluate(velocity.magnitude/60);
        return -velocity.normalized * magnitude;
    }
}