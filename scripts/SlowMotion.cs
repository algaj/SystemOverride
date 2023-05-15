using Godot;
using System;

namespace SystemOverride
{
	public partial class SlowMotion : Node
	{
		float _timeScale = 1.0f;

		float _recoveryRate = 0.8f;


		public override void _Process(double delta)
		{
			_timeScale += _recoveryRate * (float)delta;
			if (_timeScale > 1.0f)
			{
				_timeScale = 1.0f;
			}
			Godot.Engine.TimeScale = _timeScale;
		}

		public void TriggerSlowMotion()
		{
			if (_timeScale > 0.6f)
			{
				_timeScale = 0.6f;
			}
		}

        public void TriggerSlowMotionHard()
        {
            _timeScale = 0.2f;
        }
    }
}