using Godot;
using System;

public partial class Bullet : Node2D
{
	[Export]
	private PackedScene bulletImpactFXScene;
	

	private float aliveTime = 0.0f;

	private Area2D bulletArea;

	public override void _Ready()
	{
		bulletArea = GetNode<Area2D>("./Area2D");
        bulletArea.BodyEntered += BulletArea_BodyEntered;
	}

    private void BulletArea_BodyEntered(Node2D body)
    {
		GpuParticles2D fx = bulletImpactFXScene.Instantiate<GpuParticles2D>();
		GetNode("/root").AddChild(fx);

		fx.GlobalPosition = GlobalPosition;
		fx.Emitting = true;
		QueueFree();
	}

    public override void _Process(double delta)
	{
		aliveTime += (float)delta;

		MoveLocalY(-(float)delta * 1000.0f);

		if (aliveTime > 5.0f)
        {
			QueueFree();
        }
	}
}
