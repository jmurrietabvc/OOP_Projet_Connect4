using System;

public class GameController
{
    private GameBoard gameBoard;
    private IPlayer currentPlayer;
    private IPlayer player1;
    private IPlayer player2;

    public GameController()
    {
        gameBoard = new GameBoard();
        player1 = new HumanPlayer("Player 1");
        player2 = new HumanPlayer("Player 2");
        currentPlayer = player1;
    }

    public void StartGame()
    {
        Console.WriteLine("Connect Four");
        Console.WriteLine();

        ChooseGameMode();

        bool gameover = false;

        while (!gameover)
        {
            currentPlayer.MakeMove(gameBoard);
            gameBoard.DisplayBoard();

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
                currentPlayer = (currentPlayer == player1) ? player2 : player1;
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
            player2 = new ComputerPlayer();
        }
        else
        {
            Console.WriteLine("Player 1, enter your name: ");
            string name1 = Console.ReadLine();
            player1 = new HumanPlayer(name1);

            Console.WriteLine("Player 2, enter your name: ");
            string name2 = Console.ReadLine();
            player2 = new HumanPlayer(name2);
        }
    }



namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        { 

            GameController gameController = new GameController();
        gameController.StartGame();
        }
}
}
