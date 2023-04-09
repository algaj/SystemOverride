using Godot;
using System;

/// <summary>
/// Represents a sound effect that automatically frees itself from memory after it finishes playing.
/// </summary>
public partial class AfterFreeSfx : AudioStreamPlayer2D
{
    public override void _Ready()
	{
        Finished += AfterFreeSfx_Finished;

		Play();
	}

    /// <summary>
    /// Called when the sound effect finishes playing.
    /// </summary>
    private void AfterFreeSfx_Finished()
    {
        QueueFree();
    }
}
