using Godot;
using System;

namespace SpaceThing
{
	public partial class Projectile : Node2D
	{

		[Export]
		GpuParticles2D _ImpactParticles;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}