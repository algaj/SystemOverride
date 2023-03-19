using Godot;
using System;

namespace SpaceThing
{
	public partial class PlayerController : Node
	{
		[Export]
		Spaceship spaceship;


		public override void _Ready()
		{
			
		}

		public override void _Process(double delta)
		{

			float yAxisEffort = Input.GetAxis(InputActions.MoveForward, InputActions.MoveBackward);
			float xAxisEffort = Input.GetAxis(InputActions.MoveLeft, InputActions.MoveRight);

			spaceship.TargetMovementEffort = new Vector2(xAxisEffort, yAxisEffort);

			float turnEffort = Input.GetAxis(InputActions.TurnLeft, InputActions.TurnRight);

			spaceship.TargetTurnEffort = turnEffort;
		}


	}
}