using Godot;
using System;
using System.Collections.Generic;

namespace SystemOverride
{

    public partial class Spaceship : RigidBody2D
    {
        /// <summary>
        /// Normal bullet takes around 10 hp, when this number runs out the spaceship explodes.
        /// </summary>
        [Export]
        int _maxHealth = 100;

        [Export]
        float _despawnTime = 7.0f;

        [Export]
        AudioStreamPlayer2D _impactSfx;

        [Export]
        PackedScene _spaceshipExplositionFX;

        [Export]
        PackedScene _spaceshipExplostionSfx;

        public int MaxHealth { get { return _maxHealth; } }

        public int Health { get; private set; }

        public bool IsDestroyed { get; private set; } = false;


        [Signal]
        public delegate void ScreenShakeRequestedEventHandler(float screenShakeFactor);

        [Signal]
        public delegate void DamageTakenEventHandler();

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


        const float _turnProporcionalGain = 10.0f;

        const float _turnIntegralGain = 15.0f;

        const float _turnDerivativeGain = 5.0f;


        PidController _turnPidController;

        List<Engine> _engines = new List<Engine>();

        List<Weapon> _weapons = new List<Weapon>();

        RandomNumberGenerator _rng = new RandomNumberGenerator();

        Gravity _gravity;

        Node2D _enginesRoot;
        Node2D _weaponsRoot;
        Node2D _spaceshipSprite;

        Timer _despawnTimer;

        public override void _Ready()
        {
            // Check exports
            Debug.Assert(_impactSfx != null, "_impactSfx != null");

            // Initialize references

            _turnPidController = new PidController(_turnProporcionalGain, _turnIntegralGain, _turnDerivativeGain, 1.0f, -1.0f);

            _gravity = GetNode<Gravity>("/root/Gravity");
            _enginesRoot = GetNode<Node2D>("Engines");
            _weaponsRoot = GetNode<Node2D>("Weapons");
            _spaceshipSprite = GetNode<Node2D>("Sprite");

            _despawnTimer = new Timer();
            _despawnTimer.Timeout += OnDespawn;
           AddChild(_despawnTimer);
            _despawnTimer.WaitTime = _despawnTime;

            // Other initialization stuff

            _rng.Randomize();

            Health = _maxHealth;

            int engineCount = _enginesRoot.GetChildCount();

            // There should be atleast some engines... otherwise it's not really a spaceship anymore.
            Debug.Assert(engineCount > 0, "engineCount > 0");

            for (int i = 0; i < engineCount; i++)
            {
                var engine = _enginesRoot.GetChild<Engine>(i);
                _engines.Add(engine);

            }

            int weaponCount = _weaponsRoot.GetChildCount();
            // But there could be no weapons... probably.

            for (int i = 0; i < weaponCount; i++)
            {
                var weapon = _weaponsRoot.GetChild<Weapon>(i);
                weapon.ScreenShakeRequested += OnScreenShakeRequestedFromWeapons;
                _weapons.Add(weapon);
            }
        }

        public override void _PhysicsProcess(double delta)
        {

            _turnPidController.ProcessVariable = TurnProcessOffset;

            float target_turn_effort = (float)_turnPidController.ControlVariable(TimeSpan.FromSeconds(delta));

            foreach (var engine in _engines)
            {

                // If the spaceship is destroyed, dont control the engines.
                if (!IsDestroyed)
                {

                    bool usedForTurning = false;

                    bool enableEngine = engine.MoveDirectionGroup switch
                    {
                        EngineMoveDirectionGroup.Forward => TargetMovementEffort.Y < 0.0f,
                        EngineMoveDirectionGroup.Backward => TargetMovementEffort.Y > 0.0f,
                        EngineMoveDirectionGroup.Left => TargetMovementEffort.X < 0.0f,
                        EngineMoveDirectionGroup.Right => TargetMovementEffort.X > 0.0f,
                        _ => false
                    };


                    switch (engine.TurnDirectionGroup)
                    {
                        case EngineTurnDirectionGroup.Left when target_turn_effort < 0.0f:
                            enableEngine = true;
                            usedForTurning = true;
                            break;
                        case EngineTurnDirectionGroup.Right when target_turn_effort > 0.0f:
                            enableEngine = true;
                            usedForTurning = true;
                            break;
                    }


                    engine.EngineEnabled = enableEngine;

                    float forceFactor = usedForTurning ? Mathf.Abs(target_turn_effort) : 1.0f;

                    engine.ApplyForceToOwner(this, forceFactor);
                } 
                else
                {
                    // Keep the engine thrust at max if the spaceship is destroyed and the engine is still active.
                    engine.ApplyForceToOwner(this, 1.0f);
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

            EmitSignal(SignalName.ScreenShakeRequested, 14.0f);

            _impactSfx.PitchScale = Mathf.Lerp(0.9f, 1.2f, _rng.Randf());
            _impactSfx.Play();

            if (Health <= 0)
            {
                DestroySpaceship();
            }

            EmitSignal(SignalName.DamageTaken);
        }

        public void DestroySpaceship()
        {

            var spaceshipExplosionSfx = _spaceshipExplostionSfx.Instantiate<AfterFreeSfx>();
            spaceshipExplosionSfx.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(spaceshipExplosionSfx);

            /* TODO: Queue free particles */
            GpuParticles2D fx = _spaceshipExplositionFX.Instantiate<GpuParticles2D>();
            AddChild(fx);

            fx.GlobalPosition = GlobalPosition;
            fx.Emitting = true;

            // Make the sprite darker so it's easy to see that it's destroyed.
            _spaceshipSprite.Modulate = new Color(0.2f, 0.2f, 0.2f);

            IsDestroyed = true;

            // Disable all engines.
            foreach (var engine in _engines)
            {
                engine.EngineEnabled = false;
            }

            _despawnTimer.Start();
        }

        private void OnDespawn()
        {
            var spaceshipExplosionSfx = _spaceshipExplostionSfx.Instantiate<AfterFreeSfx>();
            spaceshipExplosionSfx.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(spaceshipExplosionSfx);

            /* TODO: Queue free particles */
            GpuParticles2D fx = _spaceshipExplositionFX.Instantiate<GpuParticles2D>();
            fx.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(fx);
            fx.Emitting = true;

            QueueFree();
        }
    }
}