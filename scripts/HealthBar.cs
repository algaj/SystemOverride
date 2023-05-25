using Godot;
using System;
using System.Collections.Generic;

namespace SystemOverride {
    public partial class HealthBar : Line2D
    {

        [Export]
        float _healthBarCircleRadius = 250;

        [Export]
        PackedScene _healthSegment;

        [Export]
        Spaceship _target;

        [Export]
        float _hpPerSegment = 10.0f;

        [Export]
        Gradient _segmentGradient;


        const int _circleSegmentCount = 128;

        float _health = 0.0f;

        List<Sprite2D> _healthSegments = new List<Sprite2D>();

        public float Health {
            get
            {
                return _health;
            }
            set
            {
                if (_health != value)
                {
                    _health = value;
                    OnHealthChanged();
                }
            }
        }

        private void OnHealthChanged()
        {
            foreach (Sprite2D segment in _healthSegments)
            {
                segment.QueueFree();
            }

            _healthSegments.Clear();

            int healthSegmentCount = Mathf.CeilToInt(_health / _hpPerSegment) - 1;

            float angleStep = Mathf.DegToRad(360.0f / (float)(healthSegmentCount + 1));

            float angleTotal = healthSegmentCount * angleStep;

            float startingAngle = -angleTotal / 2.0f + Mathf.Pi / 2.0f;

            for (int i = 0; i <= healthSegmentCount; i++)
            {
                float x = Mathf.Cos(i * angleStep + startingAngle) * _healthBarCircleRadius;
                float y = Mathf.Sin(i * angleStep + startingAngle) * _healthBarCircleRadius;
                Sprite2D segment = _healthSegment.Instantiate<Sprite2D>();
                segment.Modulate = _segmentGradient.Sample(healthSegmentCount / 5.0f);
                AddChild(segment);
                segment.Position = new Vector2(x, y);
                _healthSegments.Add(segment);
            }
        }

        public override void _Ready()
        {
            CreateHealthBarCircle();
            Health = _target.Health;
        }

        public override void _Process(double delta)
        {
            GlobalPosition = _target.GlobalPosition;
            Health = _target.Health;
        }

        private void CreateHealthBarCircle()
        {
            // Calculate the angle between each segment
            float angleStep = 2.0f * Mathf.Pi / _circleSegmentCount;

            var points = new Vector2[_circleSegmentCount + 1];
            for (int i = 0; i <= _circleSegmentCount; i++)
            {
                float x = Mathf.Cos(i * angleStep) * _healthBarCircleRadius;
                float y = Mathf.Sin(i * angleStep) * _healthBarCircleRadius;
                points[i] = new Vector2(x, y);
            }

            // Set the points for the Line2D
            points[_circleSegmentCount] = new Vector2(_healthBarCircleRadius, 0); // Close the circle
            Points = points;
        }
    }
}