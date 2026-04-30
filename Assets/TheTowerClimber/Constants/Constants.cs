
using UnityEngine;

[System.Serializable]
public struct DoorRequirement
{
    public KeyType requiredKeyType;
    public bool consumeKeyOnOpen;
}


//TIP : enum 을 쓴다
// 0번은 -> None
// 어느타입도 아니다
public enum ItemType
{
    None,
    Key,
    Consumable,
    Gold,
    Quest
}

public enum KeyType
{
    None,
    Red,
    Blue,
    Green
}

//DIRECTION
public enum DIRECTION
{
    Left, LeftUp, LeftDown,
    Right, RightUp, RightDown,
    Up,Down, None
}

/// <summary>8방향 enum을 월드 평면 Vector2로 바꾼다. 공격·이동 스크립트가 공통으로 사용한다.</summary>
public static class Direction8
{
    public static Vector2 ToVector2(DIRECTION direction)
    {
        return direction switch
        {
            DIRECTION.Left => Vector2.left,
            DIRECTION.Right => Vector2.right,
            DIRECTION.Up => Vector2.up,
            DIRECTION.Down => Vector2.down,
            DIRECTION.LeftUp => new Vector2(-1f, 1f).normalized,
            DIRECTION.LeftDown => new Vector2(-1f, -1f).normalized,
            DIRECTION.RightUp => new Vector2(1f, 1f).normalized,
            DIRECTION.RightDown => new Vector2(1f, -1f).normalized,
            _ => Vector2.zero
        };
    }
}

