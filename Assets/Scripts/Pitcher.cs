using UnityEngine;
using UnityEngine.Video;

public class Pitcher : MonoBehaviour
{
    public Ball ballPrefab;
    public VideoPlayer vid;
    public PitchProfile[] pitches;
    public int currentPitch = 0;
    public Vector3 releasePoint;
    [Tooltip("Frame of vid that the ball is released")]
    public long releaseFrame;

    public Vector3 location;
    public Transform target;

    void Start()
    {
        // Tell the VideoPlayer to fire an event every time a new frame is ready
        vid.sendFrameReadyEvents = true;
        vid.frameReady += OnFrameReady;

        vid.transform.position = new Vector3(vid.transform.position.x, vid.transform.position.y, releasePoint.z);
    }

    void Update()
    {
        location = target.position;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vid.frame = 0;
            vid.Play();
        }
    }

    // This only runs ONCE per video frame, completely separate from your game's Update FPS
    void OnFrameReady(UnityEngine.Video.VideoPlayer source, long frameIdx)
    {
        if (frameIdx == releaseFrame)
        {
            SpawnBall();
        }
    }

    void OnDestroy()
    {
        // Clean up the event listener when the object is destroyed
        vid.frameReady -= OnFrameReady;
    }
    public void SpawnBall()
    {
        Vector3 start = releasePoint;
        PitchData currentPitchData = new PitchData(
            pitches[currentPitch].velocity / 2.237f,
            pitches[currentPitch].spinAxis(),
            pitches[currentPitch].RPM
        );

        Ball ball = Instantiate(ballPrefab, start, Quaternion.identity);

        ball.transform.forward = Calibrate(currentPitchData);
        
        ball.Initialize(1.225f, currentPitchData);
        ball.freeze = true;

        Debug.Log(pitches[currentPitch].pitchName);
    }

    Vector3 Calibrate(PitchData _pData)
    {
        DebugExtensions.DrawSphere(location, 0.1f, Color.red);
        Vector3 dir = location - releasePoint;

        // 1. Convert the velocity from MPH to meters per second once
        float speedMS = _pData.velocity;

        // 2. Setup the directional velocity vector
        Vector3 initialVelocity = dir.normalized * speedMS;

        // 3. Create the intermediate PitchData struct
        PitchData currentPitchData = _pData;

        // 4. Clean, easily readable SimParameters instantiation
        SimParameters parameters = new SimParameters(
            releasePoint,
            initialVelocity,
            currentPitchData,
            ballPrefab.ballType,
            1.225f,
            location.z
        );

        Vector3 err = Vector3.zero;

        // Define an array of colors matching your loop size
        Color[] iterationColors = new Color[] { Color.green, Color.yellow, Color.pink };
        
        //Iteration 0
        Vector3 sim = Simulate(parameters);
        sim.z = location.z;

        //Iteration 1+
        for (int i = 0; i < 3; i++)
        {
            err = location - sim;
            Debug.Log($"Iteration {i+1} Error: {err}");

            dir += err;
            parameters.initialVel = dir.normalized * speedMS;

            sim = Simulate(parameters);
            sim.z = location.z;
            
            // Grab the color based on the current loop index
            Color sphereColor = iterationColors[i];
            DebugExtensions.DrawSphere(sim, 0.05f, sphereColor, 4f);
        }

        return dir;
    }

    public struct SimParameters
    {
        public Vector3 startPos;
        public Vector3 initialVel;
        public PitchData pitchData;
        public BallType ballType;
        public float airDensity;
        public float targetZ;

        // Constructor to quickly build the struct in one line
        public SimParameters(Vector3 _startPos, Vector3 _initialVel, PitchData _pitchData, BallType _ballType, float _airDensity, float _targetZ = 18.44f)
        {
            startPos = _startPos;
            initialVel = _initialVel;
            pitchData = _pitchData;
            ballType = _ballType;
            airDensity = _airDensity;
            targetZ = _targetZ;
        }
    }
    static Vector3 Simulate(SimParameters config)
    {
        Vector3 pos = config.startPos;
        Vector3 vel = config.initialVel;
        Vector3 initialForward = config.initialVel.normalized;

        while (pos.z < config.targetZ)
        {
            // Apply gravity
            vel += Physics.gravity * Time.fixedDeltaTime;

            // Calculate and apply lift force
            Vector3 liftForce = BallPhysics.CalculateLift(vel, config.pitchData, config.ballType, config.airDensity, initialForward);
            vel += liftForce / config.ballType.mass * Time.fixedDeltaTime;

            // Update position
            pos += vel * Time.fixedDeltaTime;

            vel += BallPhysics.CalculateDrag(vel, config.ballType, config.airDensity) * Time.fixedDeltaTime / config.ballType.mass;
        }

        return pos;
    }
}
