using Godot;
using System;

namespace SpaceThing
{
	public partial class PlayerCamera : Camera2D
	{
		[Export]
		private Node2D target;

		public override void _Ready()
		{
		}

		public override void _Process(double delta)
		{
			GlobalPosition = target.GlobalPosition;
			//Rotation = target.Rotation;
		}
	}
}
