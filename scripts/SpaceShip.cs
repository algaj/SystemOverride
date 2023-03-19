using Godot;
using System;
using System.Collections.Generic;

namespace SpaceThing
{
    public partial class Spaceship : RigidBody2D
    {
        [Export]
        Node2D _enginesRoot;

        [Export]
        Node2D _weaponsRoot;

        [Export]
        float turnProporcionalGain = 0.0f;

        [Export]
        float turnIntegralGain = 0.0f;

        [Export]
        float turnDerivativeGain = 0.0f;

        Vector2 _targetMovementEffort = Vector2.Zero;

        /// <summary>
        /// Target thrust in local directions. Each axis is normalized to <-1, 1>.
        /// Negative Y axis means forward, X axis is the right direction.
        /// To represent no movement, use zero vector.
        /// </summary>
        public Vector2 TargetMovementEffort
        {
            get
            {
                return _targetMovementEffort;
            }

            set
            {
                Debug.Assert(value.X <= 1.0f && value.X >= -1.0f, "value.X <= 1.0f && value.X >= -1.0f");
                Debug.Assert(value.Y <= 1.0f && value.Y >= -1.0f, "value.Y <= 1.0f && value.Y >= -1.0f");

                _targetMovementEffort = value;
            }
        }

        public float TurnProcessOffset { get; set; }

        PidController _turnPidController;

        DebugDraw debugDraw;

        List<Engine> _engines = new List<Engine>();

        List<Weapon> _weapons = new List<Weapon>();

        public override void _Ready()
        {
            debugDraw = GetNode<DebugDraw>("/root/DebugDraw");

            _turnPidController = new PidController(turnProporcionalGain, turnIntegralGain, turnDerivativeGain, 1.0f, -1.0f);

            int engineCount = _enginesRoot.GetChildCount();

            for (int i = 0; i < engineCount; i++)
            {
                var engine = _enginesRoot.GetChild<Engine>(i);
                _engines.Add(engine);
                engine.ParentRigidbody = this;
               
            }

            int weaponCount = _weaponsRoot.GetChildCount();

            for (int i = 0; i < weaponCount; i++)
            {
                var weapon = _weaponsRoot.GetChild<Weapon>(i);
                _weapons.Add(weapon);
            }
        }

        public override void _Process(double delta)
        {
#if false
            float turnAxisValue = Input.GetAxis(InputActions.Left, InputActions.Right);

            engineParticles.Rotation = engineParticlesDefaultRotation + turnAxisValue * 0.3f;

            if (Input.IsActionPressed(InputActions.Up))
            {
                engineParticles.Emitting = true;
                engineParticles.Visible = true;
            }
            else
            {
                engineParticles.Emitting = false;
                engineParticles.Visible = false;
            }

            timeSinceLastShot += (float)delta;

            if (Input.IsActionPressed(InputActions.Shoot))
            {
                if (timeSinceLastShot > shootInterval)
                {
                    timeSinceLastShot = 0.0f;
                    Bullet bullet = bulletScene.Instantiate<Bullet>();

                    GetNode("/root").AddChild(bullet);
                    bullet.GlobalPosition = gunPosition.GlobalPosition;
                    bullet.GlobalRotation = gunPosition.GlobalRotation;
                }
            }
#endif
        }

        public override void _PhysicsProcess(double delta)
        {
            _turnPidController.ProcessVariable = TurnProcessOffset;

            float target_turn_effort = (float)_turnPidController.ControlVariable(TimeSpan.FromSeconds(delta));


            foreach (var engine in _engines)
            {
                bool enableEngine = false;
                bool usedForTurning = false;

                switch (engine.ThrustEffortDirection)
                {
                    case EngineThrustEffortDirection.Forward:
                        if (TargetMovementEffort.Y < 0.0f)
                        {
                            enableEngine = true;
                        }
                        break;
                    case EngineThrustEffortDirection.Backward:
                        if (TargetMovementEffort.Y > 0.0f)
                        {
                            enableEngine = true;
                        }
                        break;
                    case EngineThrustEffortDirection.Left:
                        if (TargetMovementEffort.X < 0.0f)
                        {
                            enableEngine = true;
                        }
                        break;
                    case EngineThrustEffortDirection.Right:
                        if (TargetMovementEffort.X > 0.0f)
                        {
                            enableEngine = true;
                        }
                        break;
                }

                switch (engine.TurnEffortDirection)
                {
                    case EngineTurnEffortDirection.Left:
                        if (target_turn_effort < 0.0f)
                        {
                            enableEngine = true;
                            usedForTurning = true;
                        }
                        break;
                    case EngineTurnEffortDirection.Right:
                        if (target_turn_effort > 0.0f)
                        {
                            enableEngine = true;
                            usedForTurning = true;
                        }
                        break;
                }


                engine.IsEngineEnabled = enableEngine;

                if (usedForTurning)
                {
                    engine.ThrustFactor =Mathf.Abs(target_turn_effort);
                }
                else
                {
                    engine.ThrustFactor = 1.0f;
                }
            }
        }


        /// <summary>
        /// Fire weapons (only if the weapons are ready to fire).
        /// </summary>
        /// <param name="weaponGroupIndex">There could be more then one weapon of the same type. 
        /// Each type has it's index. Invalid value is ignored. </param>
        public void FireWeapons(int weaponGroupIndex)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.WeaponGroupIndex == weaponGroupIndex)
                {
                    weapon.Fire();
                }
            }
        }
    }
}