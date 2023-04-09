using Godot;

/// <summary>
/// Automatically frees itself after it stops emitting particles.
/// </summary>
public partial class AfterFreeGFX : GpuParticles2D
{

    public override void _Ready()
    {
        Emitting = true;
    }

    public override void _Process(double delta)
	{
		if (!Emitting)
        {
			QueueFree();
        }
	}
}
