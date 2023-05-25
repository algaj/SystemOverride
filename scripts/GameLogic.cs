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

        // String to avoid circular dependency
        [Export(PropertyHint.File)]
        string _gameScenePath;

        [Export]
        Label _scoreLabel;

        [Export]
        ProgressBar _progressBar;

        [Export]
        Label _timerLabel;

        [Export]
        PlayerController _playerController;

        [Export]
        Panel _winningPanel;

        [Export]
        Panel _loosingPanel;

        [Export]
        Button _loosingPanelButton;

        [Export]
        Button _winningPanelButton;

        [Export]
        Label _winningPanelLabel;

        int _spaceshipsDestroyed = 0;
        int _lastRequiedSpaceshipsDestroyed = 0;
        int _requiedSpaceshipDestroyedForNextLabel = 1;

        string[] _labels = { "shocked", "getting it", "making them pay", "destroying their fleet", "furious", "determined pilot", "dangerous ace" };

        int _currentLabelIndex = 0;

        Gravity _gravity;

        float _stopwatchTime = 0.0f;

        public override void _Ready()
        {
            _gravity = GetNode<Gravity>("/root/Gravity");
            _aiController.AISpaceshipDestroyed += _aiController_AISpaceshipDestroyed;
            _playerController.PlayerSpaceshipDestroyed += _playerController_PlayerSpaceshipDestroyed;
            _winningPanelButton.Pressed += _winningPanelButton_Pressed;
            _loosingPanelButton.Pressed += _loosingPanelButton_Pressed;
            UpdateScoreLabel(_spaceshipsDestroyed);
        }

        private void _loosingPanelButton_Pressed()
        {
            RestartGame();
        }

        private void _winningPanelButton_Pressed()
        {
            RestartGame();
        }

        private void _playerController_PlayerSpaceshipDestroyed()
        {
            // Loosing condition

            if (!_winningPanel.Visible)
            {
                _playerController.Visible = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;
                _loosingPanel.Show();
            }
        }

        private void UpdateScoreLabel(int score)
        {
            if (score >= _requiedSpaceshipDestroyedForNextLabel)
            {
                _currentLabelIndex++;
                _lastRequiedSpaceshipsDestroyed = _requiedSpaceshipDestroyedForNextLabel;
                _requiedSpaceshipDestroyedForNextLabel += (int)((float)_requiedSpaceshipDestroyedForNextLabel * (float)1.0f);

                if (_currentLabelIndex >= _labels.Length)
                {
                    // winning condition

                    if (!_loosingPanel.Visible)
                    {
                        _playerController.Visible = false;
                        Input.MouseMode = Input.MouseModeEnum.Visible;
                        _winningPanelLabel.Text = "You have won in " + _stopwatchTime + " seconds.";
                        _winningPanel.Show();
                    }

                }
            }

            if (_currentLabelIndex >= _labels.Length) {
                return;
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
            _stopwatchTime += (float)delta;
            _timerLabel.Text = _stopwatchTime.ToString("000.00");

            if (Input.IsActionJustPressed(InputActions.Exit))
            {
                ExitToMainMenu();
            }

            if (Input.IsActionJustPressed(InputActions.Restart))
            {
                RestartGame();
            }
        }

        private void RestartGame()
        {
            _gravity.UnregisterAllGravityPoints();
            PackedScene gameScene = GD.Load<PackedScene>(_gameScenePath);
            GetTree().ChangeSceneToPacked(gameScene);
        }

        private void ExitToMainMenu()
        {
            _gravity.UnregisterAllGravityPoints();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            PackedScene mainMenu = GD.Load<PackedScene>(_mainMenuScenePath);
            GetTree().ChangeSceneToPacked(mainMenu);
        }
    }
}