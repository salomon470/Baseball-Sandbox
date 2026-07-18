using UnityEngine;
using UnityEngine.Video;

public class Pitcher : MonoBehaviour
{
    public Ball ballPrefab;
    public VideoPlayer vid;
    public PitchProfile[] pitches;
    public int currentPitch = 0;
    public Vector2 releasePoint;
    public float releaseExtension;
    [Tooltip("Frame of vid that the ball is released")]
    public long releaseFrame;

    void Start()
    {
        // Tell the VideoPlayer to fire an event every time a new frame is ready
        vid.sendFrameReadyEvents = true;
        vid.frameReady += OnFrameReady;
    }

    void Update()
    {
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
        currentPitch = Random.Range(0,2);
        
        Ball ball = Instantiate(ballPrefab, new Vector3(releasePoint.x, releasePoint.y, releaseExtension), Quaternion.identity);
        ball.transform.forward = Vector3.forward; //temp
        ball.Initialize(1.225f, new PitchData(pitches[currentPitch].velocity / 2.237f, pitches[currentPitch].spinAxis(), pitches[currentPitch].RPM));
        Debug.Log(pitches[currentPitch].pitchName);
    }
}
