using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public TypeObject typeObject;

}
public enum TypeObject
{
    SpawnPossiton,
    Coin,
    Crystal,
    People
}