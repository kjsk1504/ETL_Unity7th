using UnityEngine;

public static class GameComponentExtensions
{
    public const float groundDetectDistance = 0.5f;
    public static bool isGrounded(this Component component)
    {
        return Physics.Raycast(component.transform.position,
                               Vector3.down, 
                               groundDetectDistance, 
                               1 << LayerMask.NameToLayer("Ground"));
    }
}
