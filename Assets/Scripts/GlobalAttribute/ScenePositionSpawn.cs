using UnityEngine;

class ScenePositionSpawn
{
    private Vector3 cityPosition = new Vector3(-21.310199737548829f, -0.010000050067901612f, -2.3771989345550539f);
    private Quaternion cityrRotation = Quaternion.identity;
    public Vector3 getCityPosition()
    {
        return cityPosition;
    }
    public Quaternion getCityRotation()
    {
        return cityrRotation;
    }
}