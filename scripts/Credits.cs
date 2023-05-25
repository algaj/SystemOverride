using Godot;
using System;

namespace SystemOverride {
    public partial class Credits : Node
    {
        // String to avoid circular dependency
        [Export(PropertyHint.File)]
        string _mainMenuScenePath;

        [Export]
        Button _returnToMainMenuButton;

        public override void _Ready()
        {
            _returnToMainMenuButton.ButtonDown += _returnToMainMenuButton_ButtonDown;
        }

        private void _returnToMainMenuButton_ButtonDown()
        {
            PackedScene mainMenu = GD.Load<PackedScene>(_mainMenuScenePath);
            GetTree().ChangeSceneToPacked(mainMenu);
        }
    }
}