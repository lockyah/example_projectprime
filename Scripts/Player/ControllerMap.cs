using UnityEngine;

public class ControllerMap
{
    public enum ButtonState { Up, Down, Pressed, Released } //Press and Release are only on the first frame a button is held/released

    public Vector2 StickVector;
    public ButtonState Trigger;
    public ButtonState Grip;

    public ControllerMap()
    {
        StickVector = new Vector2();
        Trigger = ButtonState.Up;
        Grip = ButtonState.Up;
    }

    
}
