using Godot;
using System;

namespace SpaceThing
{
	public partial class PlayerController : Node2D
	{
		[Export]
		Spaceship _spaceship;

		[Export]
		Line2D _line;


		public override void _Ready()
		{

		}

		public override void _Process(double delta)
		{

			float yAxisEffort = Input.GetAxis(InputActions.MoveForward, InputActions.MoveBackward);
			float xAxisEffort = Input.GetAxis(InputActions.MoveLeft, InputActions.MoveRight);

			_spaceship.TargetMovementEffort = new Vector2(xAxisEffort, yAxisEffort);

			Vector2 globalMousePos = GetGlobalMousePosition();

			Vector2 targetDirection = (globalMousePos - _spaceship.GlobalPosition).Normalized();

			float facingDirectionOffset = -_spaceship.ToLocal(targetDirection + _spaceship.GlobalPosition).X;

			_spaceship.TurnProcessOffset = facingDirectionOffset;

			_line.ClearPoints();

			_line.AddPoint(_line.ToLocal(_spaceship.GlobalPosition));
			_line.AddPoint(_line.ToLocal(globalMousePos));

			if (Input.IsActionPressed(InputActions.PrimaryWeapon))
            {
				_spaceship.FireWeapons(0);
            }
		}
    }
}