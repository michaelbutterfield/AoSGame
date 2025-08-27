using Godot;

public partial class MainMenuController : Control
{
    [Export]
    public PackedScene GameScene { get; set; }

    [Export]
    public PackedScene LobbyScene { get; set; }

    [Export]
    public PackedScene ArmyBuilderScene { get; set; }
    
    [Export]
    public PackedScene BattleplanSelectionScene { get; set; }

    public override void _Ready()
    {
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        var singlePlayerButton = GetNode<Button>("VBoxContainer/SinglePlayerButton");
        var battleplanButton = GetNode<Button>("VBoxContainer/BattleplanButton");
        var armyBuilderButton = GetNode<Button>("VBoxContainer/ArmyBuilderButton");
        var hostButton = GetNode<Button>("VBoxContainer/HostButton");
        var joinButton = GetNode<Button>("VBoxContainer/JoinButton");
        var settingsButton = GetNode<Button>("VBoxContainer/SettingsButton");
        var quitButton = GetNode<Button>("VBoxContainer/QuitButton");

        singlePlayerButton.Pressed += OnSinglePlayerPressed;
        battleplanButton.Pressed += OnBattleplanPressed;
        armyBuilderButton.Pressed += OnArmyBuilderPressed;
        hostButton.Pressed += OnHostPressed;
        joinButton.Pressed += OnJoinPressed;
        settingsButton.Pressed += OnSettingsPressed;
        quitButton.Pressed += OnQuitPressed;
    }

    private void OnSinglePlayerPressed()
    {
        GD.Print("MainMenu: Starting single player game");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartSinglePlayerGame();
        }

        // Load game scene
        if (GameScene != null)
        {
            GetTree().ChangeSceneToPacked(GameScene);
        }
        else
        {
            GD.PrintErr("MainMenu: GameScene not set!");
        }
    }

    private void OnBattleplanPressed()
    {
        GD.Print("MainMenu: Opening Battleplan Selection");
        
        if (BattleplanSelectionScene != null)
        {
            GetTree().ChangeSceneToPacked(BattleplanSelectionScene);
        }
        else
        {
            GD.PrintErr("MainMenu: BattleplanSelectionScene not set!");
        }
    }

    private void OnArmyBuilderPressed()
    {
        GD.Print("MainMenu: Opening Army Builder");
        
        if (ArmyBuilderScene != null)
        {
            GetTree().ChangeSceneToPacked(ArmyBuilderScene);
        }
        else
        {
            GD.PrintErr("MainMenu: ArmyBuilderScene not set!");
        }
    }

    private void OnHostPressed()
    {
        GD.Print("MainMenu: Starting multiplayer host");
        
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.HostGame();
        }

        // Load lobby scene
        if (LobbyScene != null)
        {
            GetTree().ChangeSceneToPacked(LobbyScene);
        }
        else
        {
            GD.PrintErr("MainMenu: LobbyScene not set!");
        }
    }

    private void OnJoinPressed()
    {
        GD.Print("MainMenu: Opening join dialog");
        ShowJoinDialog();
    }

    private void OnSettingsPressed()
    {
        GD.Print("MainMenu: Opening settings");
        ShowSettingsDialog();
    }

    private void OnQuitPressed()
    {
        GD.Print("MainMenu: Quitting game");
        GetTree().Quit();
    }

    private void ShowJoinDialog()
    {
        var dialog = new AcceptDialog();
        dialog.Title = "Join Game";
        dialog.DialogText = "Enter server address:";
        
        var input = new LineEdit();
        input.PlaceholderText = "127.0.0.1";
        input.Text = "127.0.0.1";
        
        dialog.AddChild(input);
        AddChild(dialog);
        
        dialog.Confirmed += () =>
        {
            string address = input.Text;
            if (!string.IsNullOrEmpty(address))
            {
                if (NetworkManager.Instance != null)
                {
                    NetworkManager.Instance.JoinGame(address);
                }
                
                if (LobbyScene != null)
                {
                    GetTree().ChangeSceneToPacked(LobbyScene);
                }
            }
            dialog.QueueFree();
        };
        
        dialog.Canceled += () => dialog.QueueFree();
        dialog.PopupCentered();
    }

    private void ShowSettingsDialog()
    {
        var dialog = new AcceptDialog();
        dialog.Title = "Settings";
        dialog.DialogText = "Settings not implemented yet.";
        dialog.OkButtonText = "OK";
        
        AddChild(dialog);
        dialog.Confirmed += () => dialog.QueueFree();
        dialog.PopupCentered();
    }
}
