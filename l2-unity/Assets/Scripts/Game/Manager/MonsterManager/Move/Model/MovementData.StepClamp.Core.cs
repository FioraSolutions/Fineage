using UnityEngine;

public partial class MovementData
{
    private void CharacterMove(Entity entity , Vector3 direction , float speed)
    {
        CharacterController character = GetControllerToTypeEntity(entity);
        // Match PlayerController: gravity (Y) is integrated separately so the
        // vertical velocity is NOT scaled by run speed. Combining them made the
        // monsters/NPCs bob "light" — the faster they ran, the weaker gravity felt.
        Vector3 planar = new Vector3(direction.x, 0f, direction.z);
        float vy = ApplyGravity(Time.deltaTime);
        StartMove(character, planar, speed, vy);
    }

    private void StartMove(CharacterController character, Vector3 direction, float speed, float verticalVelocity)
    {
        if (character == null || !character.enabled || !character.gameObject.activeInHierarchy)
            return;

        if (character.radius <= 0.001f || character.height <= 0.001f)
            return;

        Vector3 planar = direction * speed * Time.deltaTime;
        planar.y = verticalVelocity * Time.deltaTime;
        character.Move(planar);
    }

    private float ApplyGravity(float deltaTime)
    {
        var character = GetControllerToTypeEntity(_entity);

        if (character == null)
            return 0f;

        if (character.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }
}
