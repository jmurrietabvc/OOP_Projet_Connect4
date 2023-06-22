using System;

public class GameController
{
    private GameBoard gameBoard; // Reference to the game board
    private IPlayer currentPlayer; // Reference to the current player
    private IPlayer player1; // Reference to player 1
    private IPlayer player2; // Reference to player 2

    public GameController()
    {
        gameBoard = new GameBoard(); // Create a new game board
        player1 = new HumanPlayer("Player 1"); // Create player 1 as a human player
        player2 = new HumanPlayer("Player 2"); // Create player 2 as a human player
        currentPlayer = player1; // Set player 1 as the current player
    }

    public void StartGame()
    {
        Console.WriteLine("Connect Four");
        Console.WriteLine();

        ChooseGameMode(); // Let the players choose the game mode

        bool gameover = false;

        while (!gameover)
        {
            currentPlayer.MakeMove(gameBoard); // Current player makes a move
            gameBoard.DisplayBoard(); // Display the game board

            if (gameBoard.CheckWin(currentPlayer.GetToken())) // Check if the current player wins
            {
                Console.WriteLine($"{currentPlayer.GetName()} wins!");
                gameover = true;
            }
            else if (gameBoard.IsBoardFull()) // Check if the game board is full (draw)
            {
                Console.WriteLine("It's a draw!");
                gameover = true;
            }
            else
            {
                currentPlayer = (currentPlayer == player1) ? player2 : player1; // Switch players
            }
        }
    }

    private void ChooseGameMode()
    {
        Console.WriteLine("Choose game mode:");
        Console.WriteLine("1. Play against the computer");
        Console.WriteLine("2. Play with two human players");

        int choice;
        while (true)
        {
            Console.Write("Enter your choice (1 or 2): ");
            if (int.TryParse(Console.ReadLine(), out choice) && (choice == 1 || choice == 2))
                break;
            Console.WriteLine("Invalid input. Please enter 1 or 2.");
        }

        if (choice == 1)
        {
            player2 = new ComputerPlayer(); // Set player 2 as a computer player
        }
        else
        {
            Console.WriteLine("Player 1, enter your name: ");
            string name1 = Console.ReadLine();
            player1 = new HumanPlayer(name1); // Set player 1 as a human player

            Console.WriteLine("Player 2, enter your name: ");
            string name2 = Console.ReadLine();
            player2 = new HumanPlayer(name2); // Set player 2 as a human player
        }
    }
}

public interface IPlayer
{
    void MakeMove(GameBoard gameBoard);
    char GetToken();
    string GetName();
}

public class HumanPlayer : IPlayer
{
    private string name;

    public HumanPlayer(string playerName)
    {
        name = playerName;
    }

    public void MakeMove(GameBoard gameBoard)
    {
        int column = -1;

        while (column < 0 || column >= GameBoard.Columns || gameBoard.IsColumnFull(column))
        {
            Console.Write($"{name}, enter the column number to place your token (0-6): ");
            int.TryParse(Console.ReadLine(), out column);
        }

        gameBoard.PlaceToken(column, GetToken());
    }

    public char GetToken()
    {
        return 'X';
    }

    public string GetName()
    {
        return name;
    }
}

public class ComputerPlayer : IPlayer
{
    private System.Random random;

    public ComputerPlayer()
    {
        random = new System.Random();
    }

    public void MakeMove(GameBoard gameBoard)
    {
        int column;

        do
        {
            column = random.Next(0, GameBoard.Columns);
        }
        while (gameBoard.IsColumnFull(column));

        Console.WriteLine($"Computer chooses column {column}");
        gameBoard.PlaceToken(column, GetToken());
    }

    public char GetToken()
    {
        return 'O';
    }

    public string GetName()
    {
        return "Computer";
    }
}

public class GameBoard
{
    private const int Rows = 6;
    public const int Columns = 7;
    private char[,] board; // 2D array to represent the game board

    public GameBoard()
    {
        board = new char[Rows, Columns]; // Initialize the game board
        InitializeBoard(); // Set the initial state of the board
    }

    private void InitializeBoard()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                board[row, col] = '-'; // Initialize each cell of the board with '-'
            }
        }
    }

    public void DisplayBoard()
    {
        Console.WriteLine("Connect Four");
        Console.WriteLine();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Console.Write(board[row, col] + " "); // Display the contents of each cell
            }
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public bool IsColumnFull(int column)
    {
        return board[0, column] != '-'; // Check if the top cell of the column is filled
    }

    public bool IsBoardFull()
    {
        for (int col = 0; col < Columns; col++)
        {
            if (!IsColumnFull(col))
                return false; // If any column is not full, the board is not full
        }

        return true; // All columns are full, the board is full
    }

    public void PlaceToken(int column, char token)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (board[row, column] == '-')
            {
                board[row, column] = token; // Place the token in the lowest available position in the column
                break;
            }
        }
    }

    public bool CheckWin(char token)
    {
        // Check horizontal
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                if (board[row, col] == token &&
                    board[row, col + 1] == token &&
                    board[row, col + 2] == token &&
                    board[row, col + 3] == token)
                {
                    return true; // Four consecutive tokens horizontally
                }
            }
        }

        // Check vertical
        for (int row = 0; row < Rows - 3; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (board[row, col] == token &&
                    board[row + 1, col] == token &&
                    board[row + 2, col] == token &&
                    board[row + 3, col] == token)
                {
                    return true; // Four consecutive tokens vertically
                }
            }
        }

        // Check diagonal (down-right)
        for (int row = 0; row < Rows - 3; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                if (board[row, col] == token &&
                    board[row + 1, col + 1] == token &&
                    board[row + 2, col + 2] == token &&
                    board[row + 3, col + 3] == token)
                {
                    return true; // Four consecutive tokens diagonally (down-right)
                }
            }
        }

        // Check diagonal (up-right)
        for (int row = 3; row < Rows; row++)
        {
            for (int col = 0; col < Columns - 3; col++)
            {
                if (board[row, col] == token &&
                    board[row - 1, col + 1] == token &&
                    board[row - 2, col + 2] == token &&
                    board[row - 3, col + 3] == token)
                {
                    return true; // Four consecutive tokens diagonally (up-right)
                }
            }
        }

        return false; // No win condition detected
    }
}

public class ConnectFourGame
{
    public static void Main(string[] args)
    {
        GameController gameController = new GameController();
        gameController.StartGame();
    }
}
