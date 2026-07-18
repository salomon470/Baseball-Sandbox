using UnityEngine;

[CreateAssetMenu(fileName = "NewPitchProfile", menuName = "Baseball/Pitch Profile")]
public class PitchProfile : ScriptableObject
{
    [Header("Display Info")]
    public string pitchName;
    
    [Header("Base Physical Stats")]
    [Tooltip("Target speed in MPH")]
    public float velocity;
    
    [Tooltip("Rotations Per Minute")]
    public float RPM;
    
    [Tooltip("Clock Style Axis; in degrees, 180 backspin, 90 1st base side, 270 3rd base, 0 topspin")]
    public float spinDirection;
    [Tooltip("0 = 100% efficiency, +- 90 = 0% efficiency; Efficiency = Arccos (gyro deg)")]
    public float gyroDegree;

    public Vector3 spinAxis()
    {
        float xyMagnitude = Mathf.Cos(Mathf.Deg2Rad * gyroDegree);
        return new Vector3(xyMagnitude * Mathf.Cos(Mathf.Deg2Rad * spinDirection),
            xyMagnitude * -Mathf.Sin(Mathf.Deg2Rad * spinDirection),
            Mathf.Sin(Mathf.Deg2Rad * spinDirection)).normalized;
    }
}