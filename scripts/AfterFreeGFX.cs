using Godot;

namespace SystemOverride
{

    /// <summary>
    /// Checks if the particle system has finished emitting and frees the node if true.
    /// </summary>
    public partial class AfterFreeGFX : GpuParticles2D
    {
        private const bool _emitOnStart = true;

        public override void _Ready()
        {
            Emitting = _emitOnStart;
        }

        public override void _Process(double delta)
        {
            DestroyIfFinished();
        }

        private void DestroyIfFinished()
        {

            // No idea if emitting turn false when the particles stop
            // emitting or when there are no particles.
            if (!Emitting)
            {
                QueueFree();
            }
        }
    }
}