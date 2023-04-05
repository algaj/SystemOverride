using Godot;
using System;

public partial class AfterFreeGFX : GpuParticles2D
{
	public override void _Ready()
	{
		
	}


	public override void _Process(double delta)
	{
		if (!Emitting)
        {
			QueueFree();
        }
	}
}
