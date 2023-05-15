using Godot;
using System;

namespace SystemOverride
{
	public partial class PlayerController : Node2D
	{
		[Export]
		Spaceship _spaceship;

		[Export]
		Line2D _shipToCursorLine;

		[Export]
		Sprite2D _directionArrow;

		[Export]
		Sprite2D _cursor;

		[Export]
		PlayerCamera _playerCamera;

		float _circleRadius = 170.0f;

		int _circleLineCount = 64;

		Vector2 _lastSpaceshipPosition = Vector2.Zero;
		Vector2 _lastSpaceshipDirection = Vector2.Zero;


		public override void _Ready()
		{
			_spaceship.SwitchToCollisionLayer(1);

            _spaceship.ScreenShakeRequested += _spaceship_ScreenShakeRequested;

			_lastSpaceshipPosition = _spaceship.GlobalPosition;

			Input.SetCustomMouseCursor(null);
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}

        private void _spaceship_ScreenShakeRequested(float screenShakeFactor)
        {
			_playerCamera.ScreenShakeFactor += screenShakeFactor;

		}

        public override void _Process(double delta)
		{
			if (!IsInstanceValid(_spaceship))
			{
				return;
			}

			_lastSpaceshipDirection = (_spaceship.GlobalPosition - _lastSpaceshipPosition).Normalized();
			_lastSpaceshipPosition = _spaceship.GlobalPosition;

			_directionArrow.GlobalPosition = _spaceship.GlobalPosition + _lastSpaceshipDirection * _circleRadius;
			_directionArrow.LookAt(_directionArrow.GlobalPosition + new Vector2(-_lastSpaceshipDirection.Y, _lastSpaceshipDirection.X));

			float yAxisEffort = Input.GetAxis(InputActions.PositiveThrust, InputActions.NegativeThrust);
			float xAxisEffort = Input.GetAxis(InputActions.SideThrustLeft, InputActions.SideThrustRight);

			_spaceship.TargetMovementEffort = new Vector2(xAxisEffort, yAxisEffort);

			Vector2 globalMousePos = GetGlobalMousePosition();

			Vector2 targetDirection = (globalMousePos - _spaceship.GlobalPosition).Normalized();

			float facingDirectionOffset = -_spaceship.ToLocal(targetDirection + _spaceship.GlobalPosition).X;

			_spaceship.TurnProcessOffset = facingDirectionOffset;

			_shipToCursorLine.ClearPoints();

			_shipToCursorLine.AddPoint(_shipToCursorLine.ToLocal(_spaceship.GlobalPosition));
			_shipToCursorLine.AddPoint(_shipToCursorLine.ToLocal(globalMousePos));

			if (Input.IsActionPressed(InputActions.FireWeapons))
            {
				_spaceship.FireWeapons(0);
            }

			_cursor.GlobalPosition = globalMousePos;
		}
    }
}