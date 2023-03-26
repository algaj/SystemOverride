using Godot;
using System;

public partial class AfterFreeSfx : AudioStreamPlayer2D
{
	public override void _Ready()
	{
        Finished += AfterFreeSfx_Finished;

		Play();
	}

    private void AfterFreeSfx_Finished()
    {
        QueueFree();
    }
}
