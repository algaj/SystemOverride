using Godot;
using System;

namespace SystemOverride
{
	public partial class Planet : Node2D
	{
		[Export]
		public string Name { get; private set; }

		[Export]
		public string Description { get; private set; }

        [Export]
		public int MaxHealth { get; private set; }

		[Export]
		float _orbitFrequency;

        [Export]
		Node2D _orbitPoint;

		[Export]
		Line2D _orbitCircle;

		public int Health { get; private set; }

		public override void _Ready()
		{
			Health = MaxHealth;

			const int circleLineCount = 512;

			float circleRadius = Mathf.Abs(GlobalPosition.DistanceTo(_orbitPoint.GlobalPosition));

			for (int i = 0; i < circleLineCount + 1; i++)
			{
				float angle = (float)i * Mathf.Pi * 2.0f / (float)circleLineCount;
				float x = (int)(Mathf.Cos(angle) * circleRadius);
				float y = (int)(Mathf.Sin(angle) * circleRadius);
				_orbitCircle.AddPoint(new Vector2(x, y));
			}

			_orbitCircle.GlobalScale = Vector2.One;
		}

		public override void _Process(double delta)
		{
			_orbitPoint.Rotate(2 * Mathf.Pi * _orbitFrequency * (float)delta / 60.0f);
			Rotation = 0.0f;
		}

		public void TakeDamage(int damage)
        {
			Health -= damage;

			if (Health <= 0)
            {
				DestroyPlanet();
            }
        }

		void DestroyPlanet()
        {

        }
	}
}