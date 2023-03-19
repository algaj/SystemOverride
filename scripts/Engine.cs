using Godot;
using System;

namespace SpaceThing
{
    public enum EngineThrustEffortDirection
    {
        None, Forward, Backward, Left, Right
    }

    public enum EngineTurnEffortDirection
    {
        None, Left, Right
    }

    public partial class Engine : Node2D
    {

        [Export]
        GpuParticles2D _engineThrustParticles;

        [Export]
        float _maxThrustForce;

        [Export]
        float _maxThrustVectoringAngle;

        [Export]
        EngineThrustEffortDirection _thrustEffortDirection = EngineThrustEffortDirection.None;

        [Export]
        EngineTurnEffortDirection _turnEffortDirection = EngineTurnEffortDirection.None;

        [Export]
        PointLight2D _engineLight;

        float _baseRotation;

        public EngineThrustEffortDirection ThrustEffortDirection { get { return _thrustEffortDirection; } }

        public EngineTurnEffortDirection TurnEffortDirection { get { return _turnEffortDirection; } }

        public float ThrustFactor { get; set; }

        bool _isEngineEnabled = false;

        public bool IsEngineEnabled
        {
            get
            {
                return _isEngineEnabled;
            }
            set
            {
                _isEngineEnabled = value;

                _engineThrustParticles.Emitting = _isEngineEnabled;

                if (_engineLight != null)
                {
                    _engineLight.Visible = _isEngineEnabled;
                }
            }
        }

        float _thrusterVectoringEffort = 0.0f;


        public float ThrusterVectoringEffort
        {
            get
            {
                return _thrusterVectoringEffort;
            }
            set
            {
                Debug.Assert(value >= -1.0f && value <= 1.0f, "value >= -1.0f && value <= 1.0f");

                _thrusterVectoringEffort = value;

                Rotation = _baseRotation - Mathf.DegToRad(_maxThrustVectoringAngle * _thrusterVectoringEffort);
            }
        }

        public RigidBody2D ParentRigidbody { private get; set; }

        public override void _Ready()
        {
            _baseRotation = Rotation;
            IsEngineEnabled = false;
        }

        public override void _PhysicsProcess(double delta)
        {
            Debug.Assert(ParentRigidbody != null, "ParentRigidbody != null");

            if (IsEngineEnabled)
            {
                ParentRigidbody.ApplyForce(CalculateThrustDirection() * _maxThrustForce * ThrustFactor, GlobalPosition - ParentRigidbody.GlobalPosition);
            }
        }

        Vector2 CalculateThrustDirection()
        {
            return (ToGlobal(Vector2.Up / Scale) - GlobalPosition).Normalized();
        }
    }
}