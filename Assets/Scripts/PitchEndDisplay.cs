using System.Collections.Generic;
using UnityEngine;

public class PitchEndDisplay : MonoBehaviour
{
    public Transform visual;
    public List<Transform> pitches;

    public void AddPitch(Vector3 _pitch){
        Transform instance = Instantiate(visual, _pitch, Quaternion.identity);
        pitches.Add(instance);
    }
    public void Clear(){
        foreach(Transform i in pitches){
            Destroy(i.gameObject);
        }
        pitches.Clear();
    }
}
