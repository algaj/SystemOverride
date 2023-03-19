using Godot;
using System;

namespace SpaceThing
{
	public partial class Weapon : Node2D
	{
		[Export]
		int _weaponGroupIndex = 0;

		[Export]
		float _fireIntervalMax;

		[Export]
		float _fireIntervalMin;

		[Export]
		GpuParticles2D _fireParticles;

		[Export]
		Timer _fireTimer;

		[Export]
		PackedScene _projectileScene;

		[Export]
		float _fireSpreadAngle = 0.0f;

		public int WeaponGroupIndex { get { return _weaponGroupIndex; } }

		bool reloaded = true;

		RigidBody2D ignoredRigidbody;
		RandomNumberGenerator random = new RandomNumberGenerator();

		public override void _Ready()
		{
			random.Randomize();
            _fireTimer.Timeout += _fireTimer_Timeout;
			_fireTimer.OneShot = true;
		}

        private void _fireTimer_Timeout()
        {
			reloaded = true;
        }

		public void Fire()
        {
			if (reloaded)
			{
				reloaded = false;
				_fireTimer.Start(Mathf.Lerp(_fireIntervalMin, _fireIntervalMax, random.Randf()));

				_fireParticles.Emitting = true;

				var projectile = _projectileScene.Instantiate<Projectile>();
				GetNode("/root").AddChild(projectile);

				projectile.GlobalPosition = GlobalPosition;
				projectile.GlobalRotation = GlobalRotation + Mathf.DegToRad(_fireSpreadAngle * random.Randf());

			}
        }
	}
}