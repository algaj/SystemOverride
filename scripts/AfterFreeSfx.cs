using Godot;

namespace SystemOverride
{

    /// <summary>
    /// A class that provides a convenient way to play a sound effect 
    /// and automatically free the associated node from memory after playback is finished.
    /// </summary>
    public partial class AfterFreeSfx : AudioStreamPlayer2D
    {
        // Override method called when the node is ready to be used.
        public override void _Ready()
        {
            // Attach a handler to the Finished signal.
            Finished += OnFinished;

            // Start playing the sound effect.
            Play();
        }

        // Handler for the Finished signal.
        private void OnFinished()
        {
            // Free the node from memory.
            QueueFree();
        }
    }
}