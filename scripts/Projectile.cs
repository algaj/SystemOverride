using Godot;
using System;

namespace SpaceThing
{
	public partial class Projectile : Node2D
	{

		[Export]
		PackedScene _ImpactParticlesScene;

		[Export]
		Area2D _collisionArea;

		public Vector2 InitialVelocity { get; set; } = Vector2.Zero;

		Vector2 _localInitialVelocity = Vector2.Zero;

		float _aliveTime = 0.0f;

		public override void _Ready()
		{
            _collisionArea.BodyEntered += _collisionArea_BodyEntered;
			_localInitialVelocity = ToLocal(GlobalPosition + InitialVelocity);

		}

        private void _collisionArea_BodyEntered(Node2D body)
        {
			var spaceship = body as Spaceship;

			if (spaceship != null)
            {
				GpuParticles2D fx = _ImpactParticlesScene.Instantiate<GpuParticles2D>();
				GetNode("/root").AddChild(fx);

				fx.GlobalPosition = GlobalPosition;
				fx.Emitting = true;
				QueueFree();
			}
		}

		public override void _Process(double delta)
		{
			_aliveTime += (float)delta;

			MoveLocalY(-1000.0f * (float)delta);

			if (_aliveTime > 5.0f)
			{
				QueueFree();
			}
		}
	}
}