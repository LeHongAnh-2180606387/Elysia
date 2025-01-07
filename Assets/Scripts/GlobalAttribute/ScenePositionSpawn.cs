using UnityEngine;

class ScenePositionSpawn
{
    // City Scene
    private Vector3 cityFloorOnePosition = new Vector3(-13.68948f, 0.11f, -29.25412f); 

    private Vector3 cityFloorTwoPosition = new Vector3(-13.39f, 6.09f, -29.18f); 

    private Vector3 cityFloorThreePosition = new Vector3(-13.47f, 12.04f, -29.1f);

    // Training Scene
    private Vector3 trainingNewPlayerPosition = new Vector3(-24.0011f, 0.2000008f, 14.1664f);

    private Vector3 trainingPosition = new Vector3(-49.61f, 0.1900006f, -32.35f); 


    // Room Scene
    Vector3 roomPosition = new Vector3(-13.051f, 0f, -1.78f);

    public Vector3 getCityFloorOnePosition()
    {
        return cityFloorOnePosition;
    }

    public Vector3 getCityFloorTwoPosition()
    {
        return cityFloorTwoPosition;
    }
    public Vector3 getCityFloorThreePosition()
    {
        return cityFloorThreePosition;
    }
    public Vector3 getTrainingNewPlayerPosition()
    {
        return trainingNewPlayerPosition;
    }
    public Vector3 getTrainingPosition()
    {
        return trainingPosition;
    }
    public Vector3 getRoomPosition()
    {
        return roomPosition;
    }
}