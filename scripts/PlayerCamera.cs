using Godot;
using System;
using static Godot.FastNoiseLite;

namespace SpaceThing
{
    public partial class PlayerCamera : Camera2D
    {
        [Export]
        Node2D _target;

        [Export]
        float _minZoom = 0.2f;

        [Export]
        float _maxZoom = 2.0f;

        [Export]
        float _zoomStepFactor = 1.2f;

        [Export]
        float _screenShakeRecoveryFactor = 1.0f;

        public float ScreenShakeFactor { get; set; } = 0.0f;

        Vector2 _targetZoom = Vector2.One;

        FastNoiseLite _noise = new FastNoiseLite();

        public override void _Ready()
        {
            _noise.NoiseType = NoiseTypeEnum.SimplexSmooth;
            _noise.FractalOctaves = 4;
            _noise.Frequency = 1.0f / 20.0f;
        }

        public override void _Process(double delta)
        {
            // Follow target
            GlobalPosition = _target.GlobalPosition;

            ProcessZoom();

            ProcessScreenShake((float)delta);
        }

        void ProcessZoom()
        {
            if (Input.IsActionJustReleased(InputActions.ZoomIn))
            {
                _targetZoom /= _zoomStepFactor;
                if (_targetZoom.X < _minZoom)
                {
                    _targetZoom = new Vector2(_minZoom, _minZoom);
                }
            }

            if (Input.IsActionJustReleased(InputActions.ZoomOut))
            {
                _targetZoom *= _zoomStepFactor;
                if (_targetZoom.X > _maxZoom)
                {
                    _targetZoom = new Vector2(_maxZoom, _maxZoom);
                }
            }

            Zoom = _targetZoom;
        }

        /// <summary>
        /// Processes screen shake effect based on Perlin noise algorithm.
        /// </summary>
        /// <param name="deltaTime">The time since the last frame in seconds.</param>
        void ProcessScreenShake(float deltaTime)
        {
            float xNoise = _noise.GetNoise1D((float)Time.GetTicksMsec() / 100.0f);
            float yNoise = _noise.GetNoise1D((float)Time.GetTicksMsec() / 100.0f + 15515.0f);
            Vector2 offset = new Vector2(xNoise, yNoise) * ScreenShakeFactor * 100.0f;
            Offset = offset;

            ScreenShakeFactor = Mathf.Clamp(ScreenShakeFactor - _screenShakeRecoveryFactor * ScreenShakeFactor * deltaTime, 0.0f, float.MaxValue);
        }
    }
}
