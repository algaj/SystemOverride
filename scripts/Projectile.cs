using Godot;
using System;

namespace SystemOverride
{
    public partial class Projectile : Node2D
    {
        [ExportGroup("General Settings")]

        [Export]
        int _damage = 10;

        [Export]
        float _aliveTime = 10.0f;

        [Export]
        float _velocity = 1000.0f;

        [ExportGroup("Assets")]

        [Export]
        PackedScene _impactParticlesScene;

        [ExportGroup("Child Nodes")]

        [Export]
        Area2D _collisionArea;

        [Export]
        Timer _aliveTimer;

        public Vector2 InitialVelocity { get; set; }

        public override void _Ready()
        {
            Debug.Assert(_damage > 0, "_damage > 0");
            Debug.Assert(_aliveTime > 0, "_aliveTime > 0");
            Debug.Assert(_velocity > 0, "_velocity > 0");
            Debug.Assert(_impactParticlesScene != null, "_impactParticlesScene != null");
            Debug.Assert(_collisionArea != null, "_collisionArea != null");
            Debug.Assert(_aliveTimer != null, "_aliveTimer != null");

            _collisionArea.BodyEntered += _collisionArea_BodyEntered;

            _aliveTimer.Timeout += _aliveTimer_Timeout;

            _aliveTimer.Start(_aliveTime);
        }

        private void _aliveTimer_Timeout()
        {
            QueueFree();
        }

        private void _collisionArea_BodyEntered(Node2D body)
        {
            if (body is Spaceship spaceship)
            {
                GpuParticles2D fx = _impactParticlesScene.Instantiate<GpuParticles2D>();
                GetTree().Root.AddChild(fx);

                fx.GlobalPosition = GlobalPosition;
                fx.Emitting = true;

                spaceship.TakeDamage(_damage);

                QueueFree();
            }
        }

        public override void _Process(double delta)
        {
            const float weaponVelocityModificationFactor = 0.1f;
            // Calculate the direction in which to move the node based on its current orientation and the desired direction of movement.
            Translate((Transform.BasisXform(Vector2.Up).Normalized() * _velocity 
                + InitialVelocity * weaponVelocityModificationFactor) * (float)delta);
        }

        public void SwitchToCollisionLayer(int layer)
        {
            _collisionArea.CollisionLayer = 0;
            _collisionArea.CollisionMask = ~(1u << (layer - 1));
        }
    }
}