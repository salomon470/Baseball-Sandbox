using UnityEngine;
using TMPro;

public class GM_PitchRecognition : MonoBehaviour
{
    public Pitcher pitcher;
    int pitches = 1;

    bool pitching = false;
    int guess = -1;

    void Start()
    {
        pitches = pitcher.pitches.Length;
        foreach (PitchProfile profile in pitcher.pitches){
            //display sum shit
        }
    }

    void Update()
    {
        if (pitching)
        {
            
        }
    }

    void OnGuess()
    {
        
    }

    public void NextPitch()
    {
        
    }
}
