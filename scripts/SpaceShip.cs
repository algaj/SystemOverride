using Godot;
using System;
using System.Collections.Generic;

namespace SystemOverride
{

    public partial class Spaceship : RigidBody2D
    {

        [ExportCategory("General Settings")]
        [Export]
        int _maxHealth = 100;

        public int MaxHealth { get { return _maxHealth; } }

        [ExportCategory("Turn Regulator")]
        [Export]
        float _turnProporcionalGain = 0.0f;

        [Export]
        float _turnIntegralGain = 0.0f;

        [Export]
        float _turnDerivativeGain = 0.0f;


        [ExportCategory("Child Nodes")]
        /// <summary>
        /// All engines need to be children of this node.
        /// </summary>
        [Export]
        Node2D _enginesRoot;

        /// <summary>
        /// All weapons need to be children of this node.
        /// </summary>
        [Export]
        Node2D _weaponsRoot;

        [Export]
        AudioStreamPlayer2D _impactSfx;

        [Export]
        Node2D _damagedVariant;

        [ExportCategory("Assets")]

        [Export]
        PackedScene _spaceshipExplositionFX;

        [Export]
        PackedScene _spaceshipExplostionSfx;

        public int Health { get; private set; }

        public bool IsDestroyed { get; private set; } = false;


        [Signal]
        public delegate void ScreenShakeRequestedEventHandler(float screenShakeFactor);


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

        List<Engine> _engines = new List<Engine>();

        List<Weapon> _weapons = new List<Weapon>();

        RandomNumberGenerator _rng = new RandomNumberGenerator();

        Gravity _gravity;

        public override void _Ready()
        {
            Debug.Assert(_impactSfx != null, "_impactSfx != null");

            _gravity = GetNode<Gravity>("/root/Gravity");

            _rng.Randomize();

            Health = _maxHealth;

            _turnPidController = new PidController(_turnProporcionalGain, _turnIntegralGain, _turnDerivativeGain, 1.0f, -1.0f);

            int engineCount = _enginesRoot.GetChildCount();
            Debug.Assert(engineCount > 0, "engineCount > 0");

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
                weapon.ScreenShakeRequested += OnScreenShakeRequestedFromWeapons;
                _weapons.Add(weapon);
            }
        }

        public override void _PhysicsProcess(double delta)
        {

            if (IsDestroyed)
            {
                return;
            }

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
                    engine.ThrustFactor = Mathf.Abs(target_turn_effort);
                }
                else
                {
                    engine.ThrustFactor = 1.0f;
                }
            }

            ApplyCentralForce(_gravity.CalculateGravityForce(GlobalPosition));
        }

        private void OnScreenShakeRequestedFromWeapons(float screenShakeFactor)
        {
            EmitSignal(SignalName.ScreenShakeRequested, screenShakeFactor);
        }


        /// <summary>
        /// Fire weapons (only if the weapons are ready to fire).
        /// </summary>
        /// <param name="weaponGroupIndex">There could be more then one weapon of the same type. 
        /// Each type has it's index. Invalid value is ignored. </param>
        public void FireWeapons(int weaponGroupIndex)
        {
            if (IsDestroyed)
            {
                return;
            }

            foreach (var weapon in _weapons)
            {
                if (weapon.WeaponGroup == WeaponGroup.Primary)
                {
                    weapon.Fire();
                }
            }
        }

        public void SwitchToCollisionLayer(int layer)
        {
            CollisionLayer = 1u << (layer - 1);
            CollisionMask = 0xFFFF;

            foreach (var weapon in _weapons)
            {
                weapon.SwitchToCollisionLayer(layer);
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsDestroyed)
            {
                _impactSfx.PitchScale = Mathf.Lerp(0.9f, 1.2f, _rng.Randf());
                _impactSfx.Play();
                return;
            }

            Health -= damage;

            EmitSignal(SignalName.ScreenShakeRequested, (float)damage / MaxHealth * 10.0f);

            _impactSfx.PitchScale = Mathf.Lerp(0.9f, 1.2f, _rng.Randf());
            _impactSfx.Play();

            if (Health <= 0)
            {
                DestroySpaceship();
            }
        }

        public void DestroySpaceship()
        {

            var spaceshipExplosionSfx = _spaceshipExplostionSfx.Instantiate<AfterFreeSfx>();
            GetTree().Root.AddChild(spaceshipExplosionSfx);

            spaceshipExplosionSfx.GlobalPosition = GlobalPosition;

            /* TODO: Queue free particles */
            GpuParticles2D fx = _spaceshipExplositionFX.Instantiate<GpuParticles2D>();
            AddChild(fx);

            fx.GlobalPosition = GlobalPosition;
            fx.Emitting = true;

            _damagedVariant.Modulate = new Color(0.2f, 0.2f, 0.2f);

            IsDestroyed = true;

            foreach (var engine in _engines)
            {
                engine.IsEngineEnabled = false;
            }
        }
    }
}