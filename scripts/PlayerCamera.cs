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
            GlobalPosition = _target.GlobalPosition;

            ProcessZoom();

            ProcessScreenShake((float)delta);
        }

        void ProcessZoom()
        {
            if (Input.IsActionJustReleased(InputActions.ZoomIn))
            {
                _targetZoom /= _zoomStepFactor;
            }

            if (Input.IsActionJustReleased(InputActions.ZoomOut))
            {
                _targetZoom *= _zoomStepFactor;
            }

            Zoom = _targetZoom;

            if (Zoom.X < _minZoom)
            {
                Zoom = new Vector2(_minZoom, _minZoom);
            }
            else if (Zoom.X > _maxZoom)
            {
                Zoom = new Vector2(_maxZoom, _maxZoom);
            }
        }

        void ProcessScreenShake(float deltaTime)
        {
            float xNoise = _noise.GetNoise1D((float)Time.GetTicksMsec() / 100.0f);
            float yNoise = _noise.GetNoise1D((float)Time.GetTicksMsec() / 100.0f + 15515.0f);

            Offset = new Vector2(xNoise * ScreenShakeFactor * 100.0f, yNoise * ScreenShakeFactor * 100.0f);

            ScreenShakeFactor -= _screenShakeRecoveryFactor * ScreenShakeFactor * deltaTime;
            if (ScreenShakeFactor < 0.0f)
            {
                ScreenShakeFactor = 0.0f;
            }
        }
    }
}
