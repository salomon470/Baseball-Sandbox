using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GM_PitchRecognition : MonoBehaviour, IGameMode
{
    public Pitcher pitcher;
    bool isLive = false;
    float timeSinceRelease = 0;

    public struct PitchGuess
    {
        public int SelectedPitch;   // The pitch digit they guessed
        public float ReactionTime;  // How many seconds into the pitch they guessed
        public bool IsCorrect;      // Was it right?

        public PitchGuess(int selectedPitch = -1, float reactionTime = 0f, bool isCorrect = false)
        {
            SelectedPitch = selectedPitch;
            ReactionTime = reactionTime;
            IsCorrect = isCorrect;
        }
    }
    List<PitchGuess> guesses = new List<PitchGuess>();

    // 1. ALL EVENT SUBSCRIPTIONS GO HERE
    void OnEnable()
    {
        Ball.OnPlateCrossed += EndRound;
        pitcher.OnPitchReleased += PitchReleased;
    }

    // 2. ALWAYS CLEAN UP HERE
    void OnDisable()
    {
        Ball.OnPlateCrossed -= EndRound;
        pitcher.OnPitchReleased -= PitchReleased;
    }
    public void Initialize()
    {
        throw new System.NotImplementedException();
    }
    public void StartRound()
    {
        guesses.Add(new PitchGuess(-1,0f,false));
        timeSinceRelease = 0f;

        //pick random pitch?
        pitcher.Pitch();
        Debug.Log("Round Start");
    }

    public void PitchReleased()
    {
        Debug.Log("PitchReleased");
        isLive = true;
    }

    private void SubmitGuess(int _guess, float _time)
    {
        var lastGuess = guesses[^1];
        lastGuess.SelectedPitch = _guess;
        lastGuess.IsCorrect = (_guess == pitcher.currentPitch + 1);
        lastGuess.ReactionTime = _time;
        guesses[^1] = lastGuess;
    }

    public void EndRound()
    {
        //Show results
        isLive = false;

        Debug.Log($"{guesses[^1].IsCorrect} | Reaction Time: {guesses[^1].ReactionTime}s");
    }
    public void Cleanup()
    {
        throw new System.NotImplementedException();
    }

    void Update()
    {
        if (isLive)
        {
            timeSinceRelease += Time.deltaTime;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame && guesses[^1].SelectedPitch == -1)
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
                                SubmitGuess(digit, timeSinceRelease);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}