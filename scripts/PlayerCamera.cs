using Godot;
using System;

namespace SpaceThing
{
	public partial class PlayerCamera : Camera2D
	{
		[Export]
		Node2D target;

		[Export]
		float minZoom = 0.2f;

		[Export]
		float maxZoom = 2.0f;

		[Export]
		float zoomStepFactor = 1.2f;

		Vector2 _targetZoom = Vector2.One;

		public override void _Ready()
		{
		}

		public override void _Process(double delta)
		{
			GlobalPosition = target.GlobalPosition;
			

			if (Input.IsActionJustReleased(InputActions.ZoomIn))
            {
				_targetZoom /= zoomStepFactor;
            }

			if (Input.IsActionJustReleased(InputActions.ZoomOut))
            {
				_targetZoom *= zoomStepFactor;
            }

			/*if (Zoom.X < _targetZoom.X)
            {
				Zoom += _targetZoom * (float)delta;
            } 
			else if (Zoom.X > _targetZoom.X)
            {
				Zoom -= _targetZoom * (float)delta;
			}*/

			Zoom = _targetZoom;

			if (Zoom.X < minZoom)
            {
				Zoom = new Vector2(minZoom, minZoom);
            }
			else if (Zoom.X > maxZoom)
            {
				Zoom = new Vector2(maxZoom, maxZoom);
            }


		}
	}
}
