using UnityEngine;

[CreateAssetMenu(fileName = "Ball Type", menuName = "New Ball Type", order = 1)]
public class BallType : ScriptableObject
{
    public float mass = 0.141748f;
    public float radius = 0.037f;

    public AnimationCurve ClCurve;
    public AnimationCurve CdCurve;
}
