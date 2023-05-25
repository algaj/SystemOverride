using Godot;
using System;
using System.Diagnostics;

public partial class MainMenu : Control
{
	[Export]
	Button newGameButton;

	[Export]
	Button creditsButton;

	[Export]
	Button exitButton;

    [Export]
    PackedScene gameScene;

    [Export]
    PackedScene creditsScene;



	public override void _Ready()
	{
		Debug.Assert(newGameButton != null, "newGameButton != null");
		Debug.Assert(creditsButton != null, "creditsButton != null");
		Debug.Assert(exitButton != null, "exitButton != null");
        Debug.Assert(creditsScene != null, "creditsScene != null");

        newGameButton.ButtonDown += NewGameButton_ButtonDown;
        creditsButton.ButtonDown += CreditsButton_ButtonDown;
        exitButton.ButtonDown += ExitButton_ButtonDown;
	}

    private void ExitButton_ButtonDown()
    {
        GetTree().Quit();
    }

    private void CreditsButton_ButtonDown()
    {
        GetTree().ChangeSceneToPacked(creditsScene);
    }

    private void NewGameButton_ButtonDown()
    {
        GetTree().ChangeSceneToPacked(gameScene);
    }
}
