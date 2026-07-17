using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PitchHandler : MonoBehaviour
{
    public Ball ballPrefab;
    public List<Ball> balls;
    public PitchEndDisplay pitchEndDisplay;
    [Header("Environment Parameters")]
    public float airDensity = 1.225f;

    [Header("Pitch Parameters")]
    public Transform startPoint;
    public PitchData pitchData;
    [Header("Inputs Fields")]
    public TMP_InputField velocityInput;
    public TMP_InputField angleX;
    public TMP_InputField angleY;
    public TMP_InputField positionX;
    public TMP_InputField positionY;
    public TMP_InputField positionZ;
    public TMP_InputField RPMInput;
    public TMP_InputField spinInput;
    public TMP_InputField gyroInput;

    [Header("Target")]
    bool isSetTarget = false;
    public float targetRange = 5f;
    public Camera targetCam;
    public Transform target;
    public bool targetLock = false;
    [Header("Visualisers")]
    public Transform spinAxisVisualiser;

    public void SpawnBall()
    {
        Ball ball = Instantiate(ballPrefab, startPoint.position, Quaternion.identity);
        ball.transform.forward = startPoint.forward;
        ball.Initialize(airDensity, pitchData);

        balls.Add(ball);
    }

    void Update()
    {
        SetParameters();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBall();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            pitchEndDisplay.Clear();
        }

        for (int i = 0; i < balls.Count; i++)
        {
            BallControl(balls[i]);
        }
        SetTarget();
        TargetCompensation();
    }
    public void TargetLock(){
        targetLock = false;
    }
    void SetTarget()
    {
        if (Input.GetMouseButtonDown(0))
            targetLock = true;

        if (targetLock)
            return;
        // Get the mouse position in screen coordinates.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -targetCam.transform.position.z;

        // Convert the screen coordinates to a point in world space.
        // We need to provide a Z value for the distance from the camera.
        // A Z of 10 is a common starting point, but you might need to adjust it.
        Vector3 worldPosition = targetCam.ScreenToWorldPoint(mousePosition);

        // Set the Z coordinate to 0.
        worldPosition.z = 0f;

        // Move the object (the GameObject this script is attached to) to the calculated position.
        target.position = worldPosition;
    }

    void BallControl(Ball ball)
    {
        if (ball.transform.position.z >= 0)
        {
            Debug.Log($"Total Break: {ball.displacementDueToLift}");

            pitchEndDisplay.AddPitch(ball.transform.position);

            Destroy(ball.gameObject);
            balls.Remove(ball);
        }
    }
    void SetParameters()
    {
        pitchData.velocity = float.Parse(velocityInput.text) / 2.237f;
        //startPoint.rotation = Quaternion.Euler(new Vector3(float.Parse(angleX.text), float.Parse(angleY.text), 0));
        startPoint.localPosition = new Vector3(float.Parse(positionX.text), float.Parse(positionY.text), float.Parse(positionZ.text));
        pitchData.RPM = float.Parse(RPMInput.text);

        float xyMagnitude = Mathf.Cos(Mathf.Deg2Rad * float.Parse(gyroInput.text));
        pitchData.spinAxis = new Vector3(xyMagnitude * Mathf.Cos(Mathf.Deg2Rad * float.Parse(spinInput.text)),
            xyMagnitude * -Mathf.Sin(Mathf.Deg2Rad * float.Parse(spinInput.text)),
            Mathf.Sin(Mathf.Deg2Rad * float.Parse(gyroInput.text))).normalized;

        Debug.DrawRay(spinAxisVisualiser.position, pitchData.spinAxis);

        spinAxisVisualiser.up = Quaternion.LookRotation(startPoint.forward, Vector3.up) * pitchData.spinAxis;
    }

    //Calculation does not include any air physics, i.e no darg, lift, ect
    void TargetCompensation()
    {
        float dist = Vector3.Distance(target.position, startPoint.position);
        float time = dist / pitchData.velocity;

        //gravity acceleration and time to get distance
        float gravityDisp = 0.5f * Physics.gravity.magnitude * time * time;

        //gravityDisp added to intended target for gyro compensation
        Vector3 aimTarget = target.position + Vector3.up * gravityDisp;

        startPoint.forward = aimTarget - startPoint.position;
    }
}
