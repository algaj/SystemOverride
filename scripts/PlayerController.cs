using Godot;
using System;

namespace SystemOverride
{
	public partial class PlayerController : Node2D
	{
		[Export]
		Spaceship _spaceship;

		[Export]
		Line2D _line;

		[Export]
		Line2D _circle;

		[Export]
		Line2D _realDirection;

		[Export]
		Sprite2D _arrow;

		[Export]
		Line2D _connectingLine;

		[Export]
		Sprite2D _cursor;

		[Export]
		Line2D _healthRing;

		[Export]
		PlayerCamera _playerCamera;

		[Export]
		ProgressBar _healthBar;

		float _circleRadius = 170.0f;

		int _circleLineCount = 64;

		Vector2 _lastSpaceshipPosition = Vector2.Zero;
		Vector2 _lastSpaceshipDirection = Vector2.Zero;


		public override void _Ready()
		{
			_spaceship.SwitchToCollisionLayer(1);

            _spaceship.ScreenShakeRequested += _spaceship_ScreenShakeRequested;

			for (int i = 0; i < _circleLineCount + 1; i++)
            {
				float angle = (float)i * Mathf.Pi * 2.0f / (float)_circleLineCount;
				float x = (int)(Mathf.Cos(angle) * _circleRadius);
				float y = (int)(Mathf.Sin(angle) * _circleRadius);
				_circle.AddPoint(new Vector2(x, y));
            }

			_lastSpaceshipPosition = _spaceship.GlobalPosition;

			Input.SetCustomMouseCursor(null);
			Input.MouseMode = Input.MouseModeEnum.Hidden;

			_healthBar.Value = ((float)_spaceship.Health / (float)_spaceship.MaxHealth) * 100.0f;
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

			_realDirection.ClearPoints();
			_realDirection.AddPoint(_line.ToLocal(_spaceship.GlobalPosition));
			_realDirection.AddPoint(_line.ToLocal(_spaceship.GlobalPosition + _lastSpaceshipDirection * _circleRadius));

			_arrow.GlobalPosition = _spaceship.GlobalPosition + _lastSpaceshipDirection * _circleRadius;
			_arrow.LookAt(_arrow.GlobalPosition + new Vector2(-_lastSpaceshipDirection.Y, _lastSpaceshipDirection.X));

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

			_circle.GlobalPosition = _spaceship.GlobalPosition;
			_healthRing.GlobalPosition = _spaceship.GlobalPosition;

			_connectingLine.ClearPoints();
			_connectingLine.AddPoint(_line.ToLocal(_spaceship.GlobalPosition + _lastSpaceshipDirection * _circleRadius));
			_connectingLine.AddPoint(_line.ToLocal(_line.ToLocal(globalMousePos)));

			_cursor.GlobalPosition = globalMousePos;

			UpdateHealthBar((float)_spaceship.Health / (float)_spaceship.MaxHealth);
		}

		public void UpdateHealthBar(float healthFactor)
        {
			_healthRing.ClearPoints();
			for (int i = 0; i < (int)((_circleLineCount + 1) * healthFactor); i++)
			{
				float angle = (float)i * Mathf.Pi * 2.0f / (float)_circleLineCount;
				float x = (int)(Mathf.Cos(angle) * _circleRadius);
				float y = (int)(Mathf.Sin(angle) * _circleRadius);
				_healthRing.AddPoint(new Vector2(x, y));
			}

			_healthBar.Value = ((float)_spaceship.Health / (float)_spaceship.MaxHealth) * 100.0f;
		}
    }
}