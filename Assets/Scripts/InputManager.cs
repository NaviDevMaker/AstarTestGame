using UnityEngine;

public static class InputManager
{
    public static PressedKey GetKey()
    {
        if (Input.GetKey(KeyCode.W)) return PressedKey.Foward;
        if (Input.GetKey(KeyCode.S)) return PressedKey.Back;
        if (Input.GetKey(KeyCode.A)) return PressedKey.Left;
        if (Input.GetKey(KeyCode.D)) return PressedKey.Right;
        return PressedKey.None;
    }
    public static bool AttackButtonPressed() => Input.GetMouseButtonDown(0);
    public static bool PickUpItemButtonPressed() => Input.GetMouseButton(2);
}

public enum PressedKey
{
    None,
    Right,
    Left,
    Foward,
    Back
}


