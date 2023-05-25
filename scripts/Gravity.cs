using Godot;
using System;
using System.Collections.Generic;

namespace SystemOverride
{
	public partial class Gravity : Node
	{
		public class GravityPoint
		{
			public GravityPoint(Node2D node, float gravityForce)
			{
				Node = node;
				GravityForce = gravityForce;
			}

			public Node2D Node { get; set; }
			public float GravityForce { get; set; }
		}

		private const float _maxDistance = 100000.0f;

		private List<GravityPoint> gravityPoints = new List<GravityPoint>();

		public override void _Process(double delta)
		{
		}

		public void RegisterGravityPoint(Node2D node, float gravityForce)
		{
			gravityPoints.Add(new GravityPoint(node, gravityForce));
		}

		public void UnregisterAllGravityPoints()
		{
			gravityPoints.Clear();
		}

		public Vector2 CalculateGravityForce(Vector2 pos)
		{
			Vector2 result = Vector2.Zero;

			foreach (var gravityPoint in gravityPoints)
			{
				float distance = gravityPoint.Node.GlobalPosition.DistanceTo(pos);

				Vector2 direction = pos.DirectionTo(gravityPoint.Node.GlobalPosition);

				result += Mathf.Clamp(Mathf.Lerp(1.0f, 0.0f, distance / _maxDistance), 0.0f, 1.0f) * direction * gravityPoint.GravityForce;
			}

			return result;
		}
	}
}