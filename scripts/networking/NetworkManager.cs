using Godot;
using System;
using System.Collections.Generic;

public partial class NetworkManager : Node
{
    [Signal]
    public delegate void PlayerConnectedEventHandler(int playerId);

    [Signal]
    public delegate void PlayerDisconnectedEventHandler(int playerId);

    [Signal]
    public delegate void LobbyUpdatedEventHandler();

    [Signal]
    public delegate void GameStartedEventHandler();

    public static NetworkManager Instance { get; private set; }

    [Export]
    public int Port { get; set; } = 7777;

    [Export]
    public string ServerAddress { get; set; } = "127.0.0.1";

    [Export]
    public int MaxPlayers { get; set; } = 2;

    public bool IsHost { get; private set; } = false;
    public bool IsClient { get; private set; } = false;
    public bool IsConnected { get; private set; } = false;
    public int LocalPlayerId { get; private set; } = 1;

    private ENetMultiplayerPeer _multiplayerPeer;
    private Dictionary<int, PlayerInfo> _connectedPlayers = new Dictionary<int, PlayerInfo>();

    public override void _Ready()
    {
        Instance = this;
        SetupMultiplayer();
    }

    private void SetupMultiplayer()
    {
        _multiplayerPeer = new ENetMultiplayerPeer();
        
        // Connect multiplayer signals
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public void HostGame()
    {
        GD.Print("NetworkManager: Starting host...");
        
        _multiplayerPeer.CreateServer(Port, MaxPlayers);
        Multiplayer.MultiplayerPeer = _multiplayerPeer;
        
        IsHost = true;
        IsClient = false;
        LocalPlayerId = 1;
        
        // Add host as player 1
        AddLocalPlayer();
        
        GD.Print($"NetworkManager: Host started on port {Port}");
    }

    public void JoinGame(string address)
    {
        GD.Print($"NetworkManager: Connecting to {address}...");
        
        _multiplayerPeer.CreateClient(address, Port);
        Multiplayer.MultiplayerPeer = _multiplayerPeer;
        
        IsHost = false;
        IsClient = true;
        LocalPlayerId = 2; // Client is always player 2 in 2-player game
        
        GD.Print($"NetworkManager: Attempting to connect to {address}:{Port}");
    }

    public void Disconnect()
    {
        if (_multiplayerPeer != null)
        {
            _multiplayerPeer.Close();
        }
        
        IsHost = false;
        IsClient = false;
        IsConnected = false;
        _connectedPlayers.Clear();
        
        GD.Print("NetworkManager: Disconnected");
    }

    private void OnPeerConnected(long id)
    {
        GD.Print($"NetworkManager: Peer {id} connected");
        
        if (IsHost)
        {
            // Send current game state to new player
            RpcId(id, nameof(ReceiveGameState), GetGameStateData());
        }
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"NetworkManager: Peer {id} disconnected");
        
        if (_connectedPlayers.ContainsKey((int)id))
        {
            _connectedPlayers.Remove((int)id);
            EmitSignal(SignalName.PlayerDisconnected, (int)id);
            EmitSignal(SignalName.LobbyUpdated);
        }
    }

    private void OnConnectedToServer()
    {
        GD.Print("NetworkManager: Connected to server");
        IsConnected = true;
        
        // Add local player to the game
        AddLocalPlayer();
    }

    private void OnConnectionFailed()
    {
        GD.PrintErr("NetworkManager: Failed to connect to server");
        IsConnected = false;
    }

    private void OnServerDisconnected()
    {
        GD.Print("NetworkManager: Disconnected from server");
        IsConnected = false;
        _connectedPlayers.Clear();
    }

    private void AddLocalPlayer()
    {
        var playerInfo = new PlayerInfo
        {
            Id = LocalPlayerId,
            Name = $"Player {LocalPlayerId}",
            ArmyName = "Stormcast Eternals", // Default army
            IsReady = false
        };
        
        _connectedPlayers[LocalPlayerId] = playerInfo;
        
        if (IsHost)
        {
            // Host immediately has the player
            EmitSignal(SignalName.PlayerConnected, LocalPlayerId);
            EmitSignal(SignalName.LobbyUpdated);
        }
        else
        {
            // Client needs to request to join
            RpcId(1, nameof(RequestJoinGame), playerInfo);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void RequestJoinGame(PlayerInfo playerInfo)
    {
        if (!IsHost) return;
        
        int senderId = Multiplayer.GetRemoteSenderId();
        GD.Print($"NetworkManager: Player {senderId} requesting to join as {playerInfo.Name}");
        
        // Accept the player
        _connectedPlayers[senderId] = playerInfo;
        EmitSignal(SignalName.PlayerConnected, senderId);
        EmitSignal(SignalName.LobbyUpdated);
        
        // Notify all clients about the new player
        Rpc(nameof(ReceivePlayerJoined), playerInfo);
    }

    [Rpc]
    private void ReceivePlayerJoined(PlayerInfo playerInfo)
    {
        if (IsHost) return; // Host already has this info
        
        _connectedPlayers[playerInfo.Id] = playerInfo;
        EmitSignal(SignalName.PlayerConnected, playerInfo.Id);
        EmitSignal(SignalName.LobbyUpdated);
    }

    [Rpc]
    private void ReceiveGameState(GameStateData gameState)
    {
        // Update local game state with received data
        GD.Print("NetworkManager: Received game state from host");
        
        // Apply the game state to GameManager
        if (GameManager.Instance != null)
        {
            ApplyGameState(gameState);
        }
    }

    private GameStateData GetGameStateData()
    {
        return new GameStateData
        {
            CurrentGameState = GameManager.Instance.CurrentGameState,
            CurrentTurnPhase = GameManager.Instance.CurrentTurnPhase,
            CurrentPlayerTurn = GameManager.Instance.CurrentPlayerTurn,
            Players = GameManager.Instance.Players
        };
    }

    private void ApplyGameState(GameStateData gameState)
    {
        // Apply the received game state
        // This would sync the game state across all clients
    }

    public void SetPlayerReady(bool isReady)
    {
        if (_connectedPlayers.ContainsKey(LocalPlayerId))
        {
            _connectedPlayers[LocalPlayerId].IsReady = isReady;
            
            if (IsHost)
            {
                EmitSignal(SignalName.LobbyUpdated);
                CheckGameStart();
            }
            else
            {
                RpcId(1, nameof(UpdatePlayerReady), LocalPlayerId, isReady);
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void UpdatePlayerReady(int playerId, bool isReady)
    {
        if (!IsHost) return;
        
        if (_connectedPlayers.ContainsKey(playerId))
        {
            _connectedPlayers[playerId].IsReady = isReady;
            EmitSignal(SignalName.LobbyUpdated);
            CheckGameStart();
        }
    }

    private void CheckGameStart()
    {
        if (!IsHost) return;
        
        bool allReady = true;
        foreach (var player in _connectedPlayers.Values)
        {
            if (!player.IsReady)
            {
                allReady = false;
                break;
            }
        }
        
        if (allReady && _connectedPlayers.Count == MaxPlayers)
        {
            GD.Print("NetworkManager: All players ready, starting game!");
            Rpc(nameof(StartGame));
        }
    }

    [Rpc]
    private void StartGame()
    {
        GD.Print("NetworkManager: Game starting!");
        EmitSignal(SignalName.GameStarted);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    // Unit movement synchronization
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void MoveUnit(int unitId, Vector3 newPosition)
    {
        // Validate the move (check if it's the player's turn, etc.)
        if (!IsValidMove(unitId, newPosition)) return;
        
        // Apply the move to all clients
        Rpc(nameof(ApplyUnitMove), unitId, newPosition);
    }

    [Rpc]
    private void ApplyUnitMove(int unitId, Vector3 newPosition)
    {
        // Find and move the unit
        var unit = GameManager.Instance.AllUnits.Find(u => u.Id == unitId);
        if (unit != null)
        {
            unit.SetPosition(newPosition);
        }
    }

    private bool IsValidMove(int unitId, Vector3 newPosition)
    {
        // Add validation logic here
        // Check if it's the player's turn, if the move is legal, etc.
        return true;
    }

    // Dice rolling synchronization
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RollDice(int diceCount, int diceSides)
    {
        var results = new int[diceCount];
        var random = new Random();
        
        for (int i = 0; i < diceCount; i++)
        {
            results[i] = random.Next(1, diceSides + 1);
        }
        
        Rpc(nameof(ReceiveDiceResults), results);
    }

    [Rpc]
    private void ReceiveDiceResults(int[] results)
    {
        GD.Print($"NetworkManager: Dice results: {string.Join(", ", results)}");
        // Handle dice results in the game
    }

    public List<PlayerInfo> GetConnectedPlayers()
    {
        return new List<PlayerInfo>(_connectedPlayers.Values);
    }

    public bool IsLocalPlayerTurn()
    {
        return GameManager.Instance.CurrentPlayerTurn == LocalPlayerId;
    }
}

public class PlayerInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ArmyName { get; set; } = "";
    public bool IsReady { get; set; } = false;
}

public class GameStateData
{
    public GameManager.GameState CurrentGameState { get; set; }
    public GameManager.TurnPhase CurrentTurnPhase { get; set; }
    public int CurrentPlayerTurn { get; set; }
    public Dictionary<int, PlayerData> Players { get; set; } = new Dictionary<int, PlayerData>();
}
