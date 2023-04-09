using Godot;
using System;
using System.Collections.Generic;

namespace SpaceThing
{
    public partial class AIController : Node2D
    {
        /// <summary>
        /// AI will try to target this point.
        /// </summary>
        [Export]
        Node2D _target;

        /// <summary>
        /// All spaceships that are children of this node will be controled by this AI.
        /// </summary>
        [Export]
        Node2D _spaceshipsRoot;

        List<Spaceship> _spaceships = new List<Spaceship>();

        public override void _Ready()
        {
            int spaceshipCount = _spaceshipsRoot.GetChildCount();

            for (int i = 0; i < spaceshipCount; i++)
            {
                var spaceship = _spaceshipsRoot.GetChild<Spaceship>(i);
                _spaceships.Add(spaceship);
                spaceship.SwitchToCollisionLayer(2);
            }
        }

        public override void _Process(double delta)
        {
            const float deviationFactor = 0.2f;
            const float maxFacingDirectionOffsetToFire = 0.1f;

            foreach (var spaceship in _spaceships)
            {
                if (!IsInstanceValid(spaceship))
                {
                    return;
                }

                var direction = (_target.GlobalPosition - spaceship.GlobalPosition).Normalized();

                var randomDeviation = new Vector2((float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0)) * deviationFactor;
                direction += randomDeviation;

                float facingDirectionOffset = -spaceship.ToLocal(direction + spaceship.GlobalPosition).X;

                spaceship.TurnProcessOffset = facingDirectionOffset;
                spaceship.TargetMovementEffort = Vector2.Up;


                if (facingDirectionOffset <= maxFacingDirectionOffsetToFire && spaceship.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition) < 40000000.0f)
                {
                    spaceship.FireWeapons(0);
                }
            }
        }
    }
}