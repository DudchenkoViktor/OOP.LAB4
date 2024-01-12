using System;
using System.Collections.Generic;

// Interfaces
public interface IGameCommandExecutor
{
    void Execute();
}

public interface IPlayerDisplayCommand : IGameCommandExecutor
{
}

public interface IAddPlayerCommand : IGameCommandExecutor
{
}

public interface IPlayerStatsCommand : IGameCommandExecutor
{
}

public interface IPlayGameCommand : IGameCommandExecutor
{
}

// Implementation Classes
public class PlayerDisplayCommand : IPlayerDisplayCommand
{
    private IPlayerRepository playerRepository;

    public PlayerDisplayCommand(IPlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public void Execute()
    {
        List<PlayerEntity> players = playerRepository.ReadAllPlayers();
        Console.WriteLine("All Players:");
        foreach (var player in players)
        {
            Console.WriteLine($"Player ID: {player.PlayerId}, Username: {player.UserName}, Current Rating: {player.CurrentRating}");
        }
        Console.WriteLine();
    }
}

public class AddPlayerCommand : IAddPlayerCommand
{
    private IPlayerRepository playerRepository;

    public AddPlayerCommand(IPlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public void Execute()
    {
        Console.Write("Enter player name: ");
        string playerName = Console.ReadLine();

        Console.Write("Enter initial rating: ");
        if (int.TryParse(Console.ReadLine(), out int initialRating))
        {
            Console.Write("Enter account type (Standard/HalfPointsDeducted/VictorySeriesBonus): ");
            string accountType = Console.ReadLine();

            BaseGameAccount newPlayer;
            switch (accountType.ToLower())
            {
                case "standard":
                    newPlayer = new StandardGameAccount(playerName, initialRating);
                    break;
                case "halfpointsdeducted":
                    newPlayer = new HalfPointsDeductedAccount(playerName, initialRating);
                    break;
                case "victoryseriesbonus":
                    newPlayer = new VictorySeriesBonusAccount(playerName, initialRating);
                    break;
                default:
                    Console.WriteLine("Invalid account type. Player not created.");
                    return;
            }

            playerRepository.CreatePlayer(new PlayerEntity { UserName = playerName, CurrentRating = initialRating });
            Console.WriteLine($"Player {playerName} with account type {accountType} created successfully.");
        }
        else
        {
            Console.WriteLine("Invalid initial rating. Player not created.");
        }
    }
}

public class PlayerStatsCommand : IPlayerStatsCommand
{
    private IPlayerRepository playerRepository;

    public PlayerStatsCommand(IPlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public void Execute()
    {
        Console.Write("Enter player name or ID: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int playerId))
        {
            PlayerEntity player = playerRepository.ReadAllPlayers().Find(p => p.PlayerId == playerId);
            if (player != null)
            {
                Console.WriteLine($"Player ID: {player.PlayerId}, Username: {player.UserName}, Current Rating: {player.CurrentRating}");
            }
            else
            {
                Console.WriteLine("Player not found.");
            }
        }
        else
        {
            PlayerEntity player = playerRepository.ReadAllPlayers().Find(p => p.UserName.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                Console.WriteLine($"Player ID: {player.PlayerId}, Username: {player.UserName}, Current Rating: {player.CurrentRating}");
            }
            else
            {
                Console.WriteLine("Player not found.");
            }
        }
    }
}

public class PlayGameCommand : IPlayGameCommand
{
    private IPlayerRepository playerRepository;
    private IGameRepository gameRepository;

    public PlayGameCommand(IPlayerRepository playerRepository, IGameRepository gameRepository)
    {
        this.playerRepository = playerRepository;
        this.gameRepository = gameRepository;
    }

    public void Execute()
    {
        Console.Write("Enter player name or ID: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int playerId))
        {
            PlayerEntity player = playerRepository.ReadAllPlayers().Find(p => p.PlayerId == playerId);
            if (player != null)
            {
                Console.Write("Enter opponent name: ");
                string opponentName = Console.ReadLine();

                Console.Write("Enter rating for the game: ");
                if (int.TryParse(Console.ReadLine(), out int rating))
                {
                    Console.Write("Enter game outcome (Win/Loss): ");
                    string outcome = Console.ReadLine();

                    playerRepository.CreatePlayer(new PlayerEntity { UserName = player.UserName, CurrentRating = player.CurrentRating });
                    gameRepository.CreateGame(new GameEntity { PlayerId = player.PlayerId, OpponentName = opponentName, Outcome = outcome, Rating = rating });
                    Console.WriteLine("Game recorded successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid rating. Game not recorded.");
                }
            }
            else
            {
                Console.WriteLine("Player not found.");
            }
        }
        else
        {
            PlayerEntity player = playerRepository.ReadAllPlayers().Find(p => p.UserName.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                Console.Write("Enter opponent name: ");
                string opponentName = Console.ReadLine();

                Console.Write("Enter rating for the game: ");
                if (int.TryParse(Console.ReadLine(), out int rating))
                {
                    Console.Write("Enter game outcome (Win/Loss): ");
                    string outcome = Console.ReadLine();

                    playerRepository.CreatePlayer(new PlayerEntity { UserName = player.UserName, CurrentRating = player.CurrentRating });
                    gameRepository.CreateGame(new GameEntity { PlayerId = player.PlayerId, OpponentName = opponentName, Outcome = outcome, Rating = rating });
                    Console.WriteLine("Game recorded successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid rating. Game not recorded.");
                }
            }
            else
            {
                Console.WriteLine("Player not found.");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        MyDbContext dbContext = new MyDbContext();
        IPlayerRepository playerRepository = dbContext;
        IGameRepository gameRepository = dbContext;

        IPlayerDisplayCommand playerDisplayCommand = new PlayerDisplayCommand(playerRepository);
        IAddPlayerCommand addPlayerCommand = new AddPlayerCommand(playerRepository);
        IPlayerStatsCommand playerStatsCommand = new PlayerStatsCommand(playerRepository);
        IPlayGameCommand playGameCommand = new PlayGameCommand(playerRepository, gameRepository);

        List<IGameCommandExecutor> commands = new List<IGameCommandExecutor>
        {
            playerDisplayCommand,
            addPlayerCommand,
            playerStatsCommand,
            playGameCommand
        };

        while (true)
        {
            Console.WriteLine("Select a command:");
            for (int i = 0; i < commands.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {commands[i].GetType().Name.Replace("Command", "")}");
            }

            if (int.TryParse(Console.ReadLine(), out int commandIndex) && commandIndex >= 1 && commandIndex <= commands.Count)
            {
                commands[commandIndex - 1].Execute();
            }
            else
            {
                Console.WriteLine("Invalid command index. Please try again.");
            }
        }
    }
}
