using UnityEngine;

public static class BallPhysics
{
    public static Vector3 CalculateLift(Vector3 velocity, PitchData pitchData, BallType ballType, float airDensity, Vector3 initialForward)
    {
        float spin = (pitchData.RPM * 2f * Mathf.PI) / 60f;
        float radius = ballType.radius;
        float area = Mathf.PI * radius * radius;

        Quaternion rotation = Quaternion.LookRotation(initialForward, Vector3.up);
        Vector3 worldSpinAxis = (rotation * pitchData.spinAxis).normalized;
        Vector3 normalizedVelocity = velocity.normalized;

        float dotProduct = Vector3.Dot(worldSpinAxis, normalizedVelocity);
        Vector3 parallelSpin = dotProduct * normalizedVelocity;
        Vector3 perpendicularSpin = worldSpinAxis - parallelSpin;
        float spinEfficiency = perpendicularSpin.magnitude;

        Vector3 realSpinAxis = (spinEfficiency > 0.0001f) ? perpendicularSpin.normalized : Vector3.zero;
        if (realSpinAxis == Vector3.zero) return Vector3.zero;

        Vector3 liftDirection = Vector3.Cross(realSpinAxis, normalizedVelocity).normalized;
        float spinParameter = (radius * spin) / velocity.magnitude;
        float effectiveCl = ballType.ClCurve.Evaluate(spinParameter);

        return liftDirection * (0.5f * airDensity * area * effectiveCl * velocity.sqrMagnitude * spinEfficiency);
    }

    public static Vector3 CalculateDrag(Vector3 velocity, BallType ballType, float airDensity)
    {
        float radius = ballType.radius;
        float area = Mathf.PI * radius * radius;
        float magnitude = 0.5f * airDensity * velocity.sqrMagnitude * area * ballType.CdCurve.Evaluate(velocity.magnitude / 60f);
        return -velocity.normalized * magnitude;
    }
}