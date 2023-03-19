using Godot;
using System;

namespace SpaceThing
{
	public partial class Weapon : Node2D
	{
		[Export]
		int _weaponGroupIndex = 0;

		[Export]
		float _fireInterval;

		[Export]
		GpuParticles2D _fireParticles;

		[Export]
		Timer _fireTimer;

		[Export]
		Projectile _projectile;

		public int WeaponGroupIndex { get { return _weaponGroupIndex; } }

		public override void _Ready()
		{
		}

		public override void _Process(double delta)
		{
		}

		public void Fire()
        {

        }
	}
}