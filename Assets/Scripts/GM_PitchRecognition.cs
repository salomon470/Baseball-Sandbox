using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GM_PitchRecognition : MonoBehaviour
{
    public Pitcher pitcher;
    int pitches = 1;

    bool pitching = false;
    int guess = -1;
    float guessTime = 0f;
    int prevPitch = -1;

    void Start()
    {
        pitches = pitcher.pitches.Length;
        foreach (PitchProfile profile in pitcher.pitches)
        {
            //display sum shit
        }
    }

    void Update()
    {
        if (pitcher.IsBallInAir && pitching && guess == -1)
        {
            prevPitch = pitcher.currentPitch + 1;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                // Find the key that was triggered this frame
                foreach (var key in Keyboard.current.allKeys)
                {
                    if (key.wasPressedThisFrame)
                    {
                        // Get the key display name (e.g., "1", "2")
                        string keyName = key.displayName;

                        // Try to parse it directly to an integer
                        if (int.TryParse(keyName, out int digit))
                        {
                            if (digit <= pitcher.pitches.Length)
                            {
                                guess = digit;
                                guessTime = pitcher.airTime;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 2. EVALUATE GUESS (Only when the pitch is active, but the ball has just landed)
        if (pitching && !pitcher.IsWindup && !pitcher.IsBallInAir)
        {
            // This block runs EXACTLY ONCE the frame the ball disappears/crosses the plate
            OnGuess();
        }

        if (guess == -1)
            pitching = pitcher.IsBallInAir || pitcher.IsWindup;
    }

    void OnGuess()
    {
        if (prevPitch != -1)
        {
            if (guess == prevPitch)
            {
                Debug.Log("Correct: " + guessTime);
            }
            else
            {
                Debug.Log("Wrong");
            }
        }
        pitching = false;
        guess = -1;
        prevPitch = -1;
    }

    public void NextPitch()
    {
        if (pitching)
            return;

        pitcher.Pitch();
    }
}
