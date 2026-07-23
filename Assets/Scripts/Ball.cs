using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    bool isPitchCompelted = false;

    public float airDensity = 1.225f;
    public PitchData pitchData;
    public BallType ballType;
    public bool freeze = false;

    // New fields to track break (displacement due to lift)
    [NonSerialized] public Vector3 displacementDueToLift = Vector3.zero;
    private Vector3 _velocityDueToLift = Vector3.zero;
    public float reynoldsNumber;

    public static event Action OnPlateCrossed;

    public void Initialize(float _aidDensity, PitchData _pitchData)
    {
        mass = ballType.mass;
        radius = ballType.radius;

        airDensity = _aidDensity;
        pitchData = _pitchData;
        velocity = _pitchData.velocity * transform.forward;
    }
    
    void FixedUpdate()
    {        
        if (transform.position.z >= 18.44f && freeze)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 18.44f);
            return;
        }
        // Apply gravity
        AddForce(mass * Physics.gravity * Time.fixedDeltaTime);

        // Calculate and apply lift force
        Vector3 liftForce = BallPhysics.CalculateLift(velocity, pitchData, ballType, airDensity, transform.forward) * Time.fixedDeltaTime;
        AddForce(liftForce);

        // Track velocity and displacement caused by lift
        Vector3 deltaVLift = liftForce / mass;
        _velocityDueToLift += deltaVLift;
        displacementDueToLift += _velocityDueToLift * Time.fixedDeltaTime;

        // Update position
        transform.position += velocity * Time.fixedDeltaTime;

        AddForce(BallPhysics.CalculateDrag(velocity,ballType,airDensity) * Time.fixedDeltaTime);

        visual.Rotate(Quaternion.LookRotation(transform.forward, Vector3.up)*pitchData.spinAxis, pitchData.RPM * 6 * Time.fixedDeltaTime, Space.World);

        if(transform.position.z >= 18.44f && !isPitchCompelted)
        {
            isPitchCompelted = true;
            OnPlateCrossed?.Invoke();
        }
    }
    void AddForce(Vector3 force)
    {
        velocity += force / mass;
    }
}