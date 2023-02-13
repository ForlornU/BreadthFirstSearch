using UnityEngine;

[CreateAssetMenu(fileName = "MoveData", menuName = "ScriptableObjects/Character Move Data", order = 1)]
public class CharacterMoveData : ScriptableObject
{
    public int MaxMove = 8;
    public float MoveSpeed = 0.5f;
}
