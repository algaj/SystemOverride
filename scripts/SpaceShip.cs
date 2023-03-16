using Godot;
using System;

namespace SpaceThing
{
	public partial class SpaceShip : RigidBody2D
	{
		[Export]
		private Node2D enginePosition;

		[Export]
		private PackedScene bulletScene;

		[Export]
		private Node2D gunPosition;

		[Export]
		private GpuParticles2D engineParticles;

		private const float shootInterval = 0.1f;

		private DebugDraw debugDraw;

		private float engineParticlesDefaultRotation;

		private float timeSinceLastShot = 0.0f;

		public override void _Ready()
		{
			debugDraw = GetNode<DebugDraw>("/root/DebugDraw");

			engineParticles.Emitting = false;

			engineParticlesDefaultRotation = engineParticles.Rotation;
		}

        public override void _Process(double delta)
        {
			float turnAxisValue = Input.GetAxis(InputActions.Left, InputActions.Right);

			engineParticles.Rotation = engineParticlesDefaultRotation + turnAxisValue * 0.3f;

			if (Input.IsActionPressed(InputActions.Up))
            {
				engineParticles.Emitting = true;
				engineParticles.Visible = true;
            } 
			else
            {
				engineParticles.Emitting = false;
				engineParticles.Visible = false;
			}

			timeSinceLastShot += (float)delta;

			if (Input.IsActionPressed(InputActions.Shoot))
            {
				if (timeSinceLastShot > shootInterval)
                {
					timeSinceLastShot = 0.0f;
					Bullet bullet = bulletScene.Instantiate<Bullet>();

					GetNode("/root").AddChild(bullet);
					bullet.GlobalPosition = gunPosition.GlobalPosition;
					bullet.GlobalRotation = gunPosition.GlobalRotation;
                }
            }
		}

        public override void _PhysicsProcess(double delta)
        {
			if (Input.IsActionPressed(InputActions.Up))
            {
				float turnAxisValue = Input.GetAxis(InputActions.Left, InputActions.Right);

				Vector2 engineThrustDirection = (enginePosition.ToGlobal(Vector2.Up) - enginePosition.GlobalPosition).Rotated(turnAxisValue * 0.2f);

				ApplyForce(engineThrustDirection * 100.0f, GlobalPosition - enginePosition.GlobalPosition);
			}
		}
    }
}