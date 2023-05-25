using Godot;
using System;

namespace SystemOverride
{
    public partial class GameLogic : Node
    {
        [Export]
        AIController _aiController;

        // String to avoid circular dependency
        [Export(PropertyHint.File)]
        string _mainMenuScenePath;

        [Export]
        Label _scoreLabel;

        [Export]
        ProgressBar _progressBar;

        int _spaceshipsDestroyed = 0;
        int _lastRequiedSpaceshipsDestroyed = 0;
        int _requiedSpaceshipDestroyedForNextLabel = 5;

        string[] _labels = { "pilot", "space ace", "determined", "supercalifragilisticexpidalidoucious" };

        int _currentLabelIndex = 0;

        public override void _Ready()
        {
            _aiController.AISpaceshipDestroyed += _aiController_AISpaceshipDestroyed;
            UpdateScoreLabel(_spaceshipsDestroyed);
        }

        private void UpdateScoreLabel(int score)
        {
            if (score >= _requiedSpaceshipDestroyedForNextLabel)
            {
                _currentLabelIndex++;
                _lastRequiedSpaceshipsDestroyed = _requiedSpaceshipDestroyedForNextLabel;
                _requiedSpaceshipDestroyedForNextLabel += (int)((float)_requiedSpaceshipDestroyedForNextLabel * (float)1.5f);

                if (_currentLabelIndex >= _labels.Length)
                {
                    // TODO: winning condition
                }
            }
            _progressBar.Value = ((float)(score - _lastRequiedSpaceshipsDestroyed) / (float)(_requiedSpaceshipDestroyedForNextLabel - _lastRequiedSpaceshipsDestroyed)) * 100.0f;
            _scoreLabel.Text = _labels[_currentLabelIndex] + " ( " + score.ToString() + " / " + _requiedSpaceshipDestroyedForNextLabel.ToString() + " )";
        }

        private void _aiController_AISpaceshipDestroyed()
        {
            _spaceshipsDestroyed++;
            UpdateScoreLabel(_spaceshipsDestroyed);
        }

        public override void _Process(double delta)
        {
            if (Input.IsKeyPressed(Key.Escape))
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
                PackedScene mainMenu = GD.Load<PackedScene>(_mainMenuScenePath);
                GetTree().ChangeSceneToPacked(mainMenu);
            }

            if (Input.IsKeyPressed(Key.R))
            {
                // TODO: restart the game
            }
        }
    }
}