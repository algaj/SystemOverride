using Godot;
using System;

namespace SystemOverride
{
    /// <summary>
    /// The engines in these groups get activated at once. If the engine points backwards, it's thrust direction is forward
    /// because it makes the spaceship go forward.
    /// </summary>
    public enum EngineMoveDirectionGroup
    {
        None, Forward, Backward, Left, Right
    }

    /// <summary>
    /// The engines in these groups get activated together. If the engine is offseted to the right side of the spaceship,
    /// it's turn direction group will be Left, because the engine will turn the spaceship to the left.
    /// </summary>
    public enum EngineTurnDirectionGroup
    {
        None, Left, Right
    }


    public partial class Engine : Node2D
    {
        // Exports

        [Export]
        public string EngineName { get; private set; }

        [Export]
        public EngineMoveDirectionGroup MoveDirectionGroup { get; private set; }

        [Export]
        public EngineTurnDirectionGroup TurnDirectionGroup { get; private set; }

        [Export]
        public float EngineForce { get; private set; }

        [Export]
        AudioStreamPlayer2D _optionalEngineAudioStreamPlayer = null;

        [Export]
        GpuParticles2D _optionalEngineParticles2D = null;

        [Export]
        Light2D _optionalEngineLight = null;

        // Properties

        public float EngineForceMultiplier { get; set; } = 1.0f;

        public bool EngineEnabled { get; set; }

        // Constants

        // State


        // Methods

        /// <summary>
        /// Adds thrust force to the spaceship. Needs to be called inside physics process.
        /// </summary>
        /// <param name="spaceship">Owner of the engine.</param>
        /// <param name="forceFactor">Value between 0.0 to 1.0 specifiing how much thrust to allow from the engine.</param>
        public void ApplyForceToOwner(Spaceship spaceship, float forceFactor)
        {
            Debug.Assert(forceFactor >= 0.0f && forceFactor <= 1.0f, "forceFactor >= 0.0f && forceFactor <= 1.0f");
            Debug.Assert(IsInstanceValid(spaceship), "IsInstanceValid(spaceship)");

            if (EngineEnabled)
            {
                spaceship.ApplyForce(CalculateThrustDirection() * EngineForce * EngineForceMultiplier,
                    GlobalPosition - spaceship.GlobalPosition);
            }

            // Activate effects if the engine is enabled.

            if (IsInstanceValid(_optionalEngineLight) && forceFactor > 0.0f)
            {
                _optionalEngineLight.Enabled = EngineEnabled;
            }

            if (IsInstanceValid(_optionalEngineParticles2D) && forceFactor > 0.0f)
            {
                _optionalEngineParticles2D.Emitting = EngineEnabled;
            }

            if (IsInstanceValid(_optionalEngineAudioStreamPlayer) && forceFactor > 0.0f)
            {
                _optionalEngineAudioStreamPlayer.Playing = EngineEnabled;
            }

        }

        /// <summary>
        /// Calculates direction in which the thruster adds force to the spaceship.
        /// </summary>
        Vector2 CalculateThrustDirection()
        {
            return (ToGlobal(Vector2.Up / Scale) - GlobalPosition).Normalized();
        }
    }
}