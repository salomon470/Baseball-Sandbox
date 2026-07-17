using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiningBallUI : MonoBehaviour
{
    public int dir = 1;
    public float offset;
    public SliderValueMatch slider;
    void Update()
    {
        transform.eulerAngles = new Vector3(0,0,dir*float.Parse(slider.inputField.text) + offset);
    }
}
