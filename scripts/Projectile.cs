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

		float _aliveTime = 0.0f;

		public override void _Ready()
		{
            _collisionArea.BodyEntered += _collisionArea_BodyEntered;
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

			MoveLocalY(-(float)delta * 1000.0f);

			if (_aliveTime > 5.0f)
			{
				QueueFree();
			}
		}
	}
}