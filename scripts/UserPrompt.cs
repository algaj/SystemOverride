using Godot;
using System;
using System.Collections.Generic;

public partial class UserPrompt : RichTextLabel
{
    [Export]
	AudioStreamPlayer _audioStreamPlayer;

	Timer _timer;

	Queue<string> _textQueue = new Queue<string>();

	string _textToPush;

	int _nextCharIndex = 0;

	bool _finishedReading = false;

	int _state = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = new Timer();
		_timer.WaitTime = 0.05f;
        _timer.Timeout += _timer_Timeout;
		AddChild(_timer);
		PrintText("[b]Zephyr[/b] :: ");
		PrintTextSlowly("#@#&*$^#&*!!!\n");
	}


    public override void _Process(double delta)
    {
        if (_finishedReading)
        {
			PrintText("[b]Zephyr[/b] :: ");
			PrintTextSlowly("#@#&*$^#&*!!!\n");
			_finishedReading = false;
		}
    }

	private void _timer_Timeout()
	{
		_finishedReading = _nextCharIndex >= _textToPush.Length;

		if (!_finishedReading)
		{
			AppendText(_textToPush[_nextCharIndex].ToString());
			_nextCharIndex++;

			_audioStreamPlayer.Play();

			if (_nextCharIndex < _textToPush.Length && _textToPush[_nextCharIndex] == ' ')
			{
				_timer.WaitTime = 0.1f;
			}
			else
			{
				_timer.WaitTime = 0.05f;
			}
		}

		if (!_timer.IsStopped())
		{
			_timer.Stop();
		}
		_timer.Start();
	}

	public void PrintTextSlowly(string text)
    {
		_nextCharIndex = 0;
		if (!_timer.IsStopped())
		{
			_timer.Stop();
		}
		_timer.Start();

		_textToPush = text;
	}

	public void PrintText(string text)
	{
		AppendText(text);
	}
}
