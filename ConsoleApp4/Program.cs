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
            for (int col = 0; col < Columns - 3; col++) using System;

public class GameController
    {
        // Game variables
        private GameBoard gameBoard;  // Instance of the game board
        private IPlayer currentPlayer;  // Current player
        private IPlayer player1;  // Player 1
        private IPlayer player2;  // Player 2

        public GameController()
        {
            // Initialize game variables
            gameBoard = new GameBoard();  // Create a new game board
            player1 = new HumanPlayer("Player 1");  // Create Player 1
            player2 = new HumanPlayer("Player 2");  // Create Player 2
            currentPlayer = player1;  // Set Player 1 as the current player
        }

        public void StartGame()
        {
            Console.WriteLine("Connect Four");
            Console.WriteLine();

            ChooseGameMode();  // Prompt the user to choose the game mode

            bool gameover = false;

            // Main game loop
            while (!gameover)
            {
                try
                {
                    currentPlayer.MakeMove(gameBoard);  // Current player makes a move
                    gameBoard.DisplayBoard();  // Display the updated game board

                    // Check win conditions
                    if (gameBoard.CheckWin(currentPlayer.GetToken()))
                    {
                        Console.WriteLine($"{currentPlayer.GetName()} wins!");
                        gameover = true;
                    }
                    else if (gameBoard.IsBoardFull())
                    {
                        Console.WriteLine("It's a draw!");
                        gameover = true;
                    }
                    else
                    {
                        currentPlayer = (currentPlayer == player1) ? player2 : player1;  // Switch to the other player
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid column number.");
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

            // Set the game mode based on the user's choice
            if (choice == 1)
            {
                player2 = new ComputerPlayer();  // Set Player 2 as a computer player
            }
            else
            {
                Console.WriteLine("Player 1, enter your name: ");
                string name1 = Console.ReadLine();
                player1 = new HumanPlayer(name1);  // Set Player 1 as a human player

                Console.WriteLine("Player 2, enter your name: ");
                string name2 = Console.ReadLine();
                player2 = new HumanPlayer(name2);  // Set Player 2 as a human player
            }
        }

        // Player interface
        public interface IPlayer
        {
            void MakeMove(GameBoard gameBoard);
            char GetToken();
            string GetName();
        }

        // HumanPlayer class implementing the Player interface
        public class HumanPlayer : IPlayer
        {
            private string name;  // Player name

            public HumanPlayer(string playerName)
            {
                name = playerName;
            }

            public void MakeMove(GameBoard gameBoard)
            {
                int column = -1;

                while (column < 0 || column >= GameBoard.Columns || gameBoard.IsColumnFull(column))
                {
                    try
                    {
                        Console.Write($"{name}, enter the column number to place your token (0-6): ");
                        column = int.Parse(Console.ReadLine());

                        if (column < 0 || column >= GameBoard.Columns || gameBoard.IsColumnFull(column))
                        {
                            Console.WriteLine("Invalid column number. Please try again.");// Exception for entering a out of range number
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid column number."); //Exception for not entering another type of input instead of a number
                    }
                }

                gameBoard.PlaceToken(column, GetToken());
            }

            public char GetToken()
            {
                return 'X';  // Return the player's token
            }

            public string GetName()
            {
                return name;  // Return the player's name
            }
        }

        // ComputerPlayer class implementing the Player interface
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

                // Generate a random column number until a valid column is found
                do
                {
                    column = random.Next(0, GameBoard.Columns);
                }
                while (gameBoard.IsColumnFull(column));

                Console.WriteLine($"Computer chooses column {column}");
                gameBoard.PlaceToken(column, GetToken());  // Place the token on the game board
            }

            public char GetToken()
            {
                return 'O';  // Return the player's token
            }

            public string GetName()
            {
                return "Computer";  // Return the player's name
            }
        }

        // GameBoard class
        public class GameBoard
        {
            private const int Rows = 6;  // Number of rows on the game board
            public const int Columns = 7;  // Number of columns on the game board
            private char[,] board;  // 2D array representing the game board

            public GameBoard()
            {
                board = new char[Rows, Columns];  // Create a new game board
                InitializeBoard();  // Initialize the game board
            }

            private void InitializeBoard()
            {
                // Set all positions on the game board to empty
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        board[row, col] = '-';
                    }
                }
            }

            public void DisplayBoard()
            {
                Console.WriteLine("Connect Four");
                Console.WriteLine();

                // Display the game board
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        Console.Write(board[row, col] + " ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }

            public bool IsColumnFull(int column)
            {
                return board[0, column] != '-';  // Check if the top row of a column is filled
            }

            public bool IsBoardFull()
            {
                // Check if all columns are full
                for (int col = 0; col < Columns; col++)
                {
                    if (!IsColumnFull(col))
                        return false;
                }

                return true;  // The board is full
            }

            public void PlaceToken(int column, char token)
            {
                // Place the token in the lowest available position in the chosen column
                for (int row = Rows - 1; row >= 0; row--)
                {
                    if (board[row, column] == '-')
                    {
                        board[row, column] = token;
                        break;
                    }
                }
            }

            public bool CheckWin(char token)
            {
                // Check horizontal win condition
                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns - 3; col++)
                    {
                        if (board[row, col] == token &&
                            board[row, col + 1] == token &&
                            board[row, col + 2] == token &&
                            board[row, col + 3] == token)
                        {
                            return true;  // Four consecutive tokens horizontally
                        }
                    }
                }

                // Check vertical win condition
                for (int row = 0; row < Rows - 3; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        if (board[row, col] == token &&
                            board[row + 1, col] == token &&
                            board[row + 2, col] == token &&
                            board[row + 3, col] == token)
                        {
                            return true;  // Four consecutive tokens vertically
                        }
                    }
                }

                // Check diagonal win condition (down-right)
                for (int row = 0; row < Rows - 3; row++)
                {
                    for (int col = 0; col < Columns - 3; col++)
                    {
                        if (board[row, col] == token &&
                            board[row + 1, col + 1] == token &&
                            board[row + 2, col + 2] == token &&
                            board[row + 3, col + 3] == token)
                        {
                            return true;  // Four consecutive tokens diagonally (down-right)
                        }
                    }
                }

                // Check diagonal win condition (up-right)
                for (int row = 3; row < Rows; row++)
                {
                    for (int col = 0; col < Columns - 3; col++)
                    {
                        if (board[row, col] == token &&
                            board[row - 1, col + 1] == token &&
                            board[row - 2, col + 2] == token &&
                            board[row - 3, col + 3] == token)
                        {
                            return true;  // Four consecutive tokens diagonally (up-right)
                        }
                    }
                }

                return false;  // No win condition found
            }
        }

        public class ConnectFourGame
        {
            public static void Main(string[] args)
            {
                GameController gameController = new GameController();
                gameController.StartGame();  // Start the Connect Four game
            }
        }
    }

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
