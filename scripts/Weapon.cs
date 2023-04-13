using Godot;
using System;

namespace SystemOverride
{
    /// <summary>
    /// Used to group weapons that fire together on the same input action.
    /// </summary>
    public enum WeaponGroup
    {
        Primary,
        Secondary,
    }

    public partial class Weapon : Node2D
    {
        [ExportGroup("General settings")]

        /// <summary>
        /// Each group of weapons can be fired separately.
        /// </summary>
        [Export]
        public WeaponGroup WeaponGroup { get; private set; }

        /// <summary>
        /// Max delay between shots.
        /// </summary>
        [Export]
        float _fireIntervalMax;

        /// <summary>
        /// Min delay between shots.
        /// </summary>
        [Export]
        float _fireIntervalMin;

        /// <summary>
        /// Spread angle of fire in degrees. 
        /// </summary>
        [Export]
        float _fireSpreadAngle = 0.0f;

        /// <summary>
        /// Min value of random pitch scale of the weapon fire sound effect.
        /// </summary>
        [Export]
        float _fireSfxPitchFactorMin = 0.5f;


        /// <summary>
        /// Max value of random pitch scale of the weapon fire sound effect.
        /// </summary>
        [Export]
        float _fireSfxPitchFactorMax = 2.0f;

        [Export]
        float _recoilMaxOffset = 0.0f;

        [Export]
        float _recoilDuration = 0.1f;

        [Export]
        float _screenShakeFactor = 0.05f;

        [ExportGroup("Assets")]

        /// <summary>
        /// Projectile that gets spawned when the weapon fires.
        /// Forward: Negative Y
        /// </summary>
        [Export]
        PackedScene _projectileAsset;

        [ExportGroup("Child Nodes")]

        /// <summary>
        /// Audio stream player used for fire sounds.
        /// </summary>
        [Export]
        AudioStreamPlayer2D _audioStreamPlayer;

        /// <summary>
        /// Particles that are emitted when the weapon fires.
        /// </summary>
        [Export]
        GpuParticles2D _fireParticles;

        /// <summary>
        /// Timer used to time the fire rate.
        /// </summary>
        [Export]
        Timer _fireTimer;

        /// <summary>
        /// Barrel of the weapon that gets recoiled back.
        /// </summary>
        [Export]
        Node2D _barrel;

        [Export]
        Node2D _barrelEnd;

        [Signal]
        public delegate void ScreenShakeRequestedEventHandler(float screenShakeFactor);

        /// <summary>
        /// Specifies which layer needs to be ignored by projectiles (to avoid firing into itself).
        /// </summary>
        int _currentCollisionLayer = 0;

        bool _reloaded = true;

        RandomNumberGenerator _rng = new RandomNumberGenerator();

        Tween _recoilTween;
        Vector2 _recoilBasePosition;
        Vector2 _previousPosition = Vector2.Zero;
        Vector2 _velocity = Vector2.Zero;

        public override void _Ready()
        {
            Debug.Assert(_fireIntervalMin <= _fireIntervalMax, "_fireIntervalMin < _fireIntervalMax");
            Debug.Assert(_fireIntervalMin >= 0.0f, "_fireIntervalMin >= 0.0f");
            Debug.Assert(_fireIntervalMax >= 0.0f, "_fireIntervalMax >= 0.0f");
            Debug.Assert(_fireTimer != null, "_fireTimer != null");
            Debug.Assert(_projectileAsset != null, "_projectileAsset != null");
            Debug.Assert(_fireSpreadAngle <= 360.0f, "_fireSpreadAngle <= 360.0f");
            Debug.Assert(_fireParticles != null, "_fireParticles != null");
            Debug.Assert(_audioStreamPlayer != null, "_audioStreamPlayer != null");
            Debug.Assert(_barrel != null, "_barrel != null");

            _recoilBasePosition = _barrel.Position;

            _rng.Randomize();
            _fireTimer.Timeout += _fireTimer_Timeout;
            _fireTimer.OneShot = true;
        }

        public override void _Process(double delta)
        {
            _velocity = (GlobalPosition - _previousPosition) / (float)delta;

            _previousPosition = GlobalPosition;
        }

        /// <summary>
        /// Reload the weapon when the timer runs out.
        /// </summary>
        private void _fireTimer_Timeout()
        {
            _reloaded = true;
        }


        /// <summary>
        /// Fire the weapon only if it is already reloaded.
        /// </summary>
        public void Fire()
        {
            if (_reloaded)
            {
                EmitSignal(SignalName.ScreenShakeRequested, _screenShakeFactor);

                _reloaded = false;
                _fireTimer.Start(Mathf.Lerp(_fireIntervalMin, _fireIntervalMax, _rng.Randf()));

                _fireParticles.Emitting = true;
                
                var projectile = _projectileAsset.Instantiate<Projectile>();
                projectile.GlobalPosition = _barrelEnd.GlobalPosition;
                projectile.GlobalRotation = GlobalRotation + Mathf.DegToRad(_fireSpreadAngle * (_rng.Randf() - 0.5f));
                projectile.InitialVelocity = _velocity;
                projectile.SwitchToCollisionLayer(_currentCollisionLayer);
                GetTree().Root.AddChild(projectile);
                _audioStreamPlayer.PitchScale = Mathf.Lerp(_fireSfxPitchFactorMin, _fireSfxPitchFactorMax, _rng.Randf());
                _audioStreamPlayer.Play();

                if (_recoilTween != null)
                    _recoilTween.Kill();

                _recoilTween = GetTree().CreateTween().BindNode(_barrel).SetTrans(Tween.TransitionType.Elastic);
                _recoilTween.TweenMethod(new Callable(_barrel, MethodName.SetPosition), _recoilBasePosition, _recoilBasePosition + new Vector2(0.0f, _recoilMaxOffset), _recoilDuration * 0.5f);
                _recoilTween.TweenMethod(new Callable(_barrel, MethodName.SetPosition), _recoilBasePosition + new Vector2(0.0f, _recoilMaxOffset), _recoilBasePosition, _recoilDuration * 0.5f);
            }
        }

        /// <summary>
        /// Avoid firing into the selected collision layer.
        /// </summary>
        /// <param name="layer">Layer index starting from 1.</param>
        public void SwitchToCollisionLayer(int layer)
        {
            Debug.Assert(layer > 0 && layer <= 32, "layer > 1 && layer <= 32");
            _currentCollisionLayer = layer;
        }
    }
}