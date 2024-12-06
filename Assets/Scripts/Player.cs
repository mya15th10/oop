/*This Player class manages all aspects related to players and the game,
 * including creating players, determining turns, checking win conditions, and handling AI. 
 * It also manages player movements and determines the final outcome of the game. */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using Unity.VisualScripting;



// Player class manages the game's players, including human and AI players.
// It handles game state transitions, win conditions, and player actions.
public class Player : MonoBehaviour
{
    #region Public Variables

    public GameObject X_Piece; // GameObject for Player 1's piece (set in Unity Editor)
    public GameObject O_Piece; // GameObject for Player 2's piece (set in Unity Editor)

    #endregion

    #region Private Static Variables

    private static char p1Shape = 'x'; // Shape for Player 1
    private static char p2Shape = 'o'; // Shape for Player 2
    private static bool AIflag; // True if an AI opponent is selected
    private static bool AITurnFirst; // True if the AI goes first
    private static bool FirstAIturn = true; // Tracks if this is the AI's first turn
    private static bool altTurns = true; // Tracks whose turn it is: true = P1, false = P2/AI
    private static bool gameWon = false; // Tracks if the game has been won
    private static int AIlevel; // Difficulty level of the AI (0 = Easy, 1 = Medium, 2 = Hard)
    private static PlayerClass P1; // Player 1 instance
    private static PlayerClass P2; // Player 2 or AI instance
    private static string player1Name; // Name of Player 1
    private static string player2Name; // Name of Player 2
    private static string[] AInames = { "Easy Bob", "Cunning Clive", "Master Shifu" }; // AI names

    #endregion

    #region Constants

    public const int maxTurns = 25; // Maximum number of turns
    public const float pieceHeightFromBoard = 0.1f; // Height of pieces above the board

    #endregion

    #region Constructor

    // Constructor to initialize players
    public Player() { }

    public Player(string p1name, string p2name, bool AI, bool AIfirst, int AIlvl)
    {
        player1Name = p1name;
        player2Name = p2name;
        AIflag = AI;
        AITurnFirst = AIfirst;
        AIlevel = AIlvl;
    }

    #endregion

    #region Unity Lifecycle Methods

    private void Start()
    {
        // Initialize game variables and create player objects
        gameWon = false;
        altTurns = true; // Player 1 starts first
        CreateObjects();
    }

    private void Update()
    {
        // Handle player moves based on mouse input
        if (Input.GetMouseButtonUp(0))
        {
            if (altTurns && P1.GetMove())
            {
                altTurns = false;
            }
            else if (!altTurns && P2.GetMove())
            {
                altTurns = true;
            }
        }

        // Check if the AI is supposed to take the first turn
        if (AITurnFirst && FirstAIturn)
        {
            FirstAIturn = false;
            if (P2.GetMove())
            {
                altTurns = true;
            }
        }

        // Check win conditions or if the game is a draw
        CheckWinConditions();
    }

    #endregion

    #region Player Management

    // Create player objects (Player 1 and Player 2/AI)
    private void CreateObjects()
    {
        // Tạo Player 1
        GameObject player1Object = new GameObject("Player1");
        P1 = player1Object.AddComponent<PlayerClass>();
        P1.Initialize(X_Piece, p1Shape);

        if (AIflag)
        {
            // Tạo AI Player
            GameObject player2Object = new GameObject("AIPlayer");
            switch (AIlevel)
            {
                case 0:
                    P2 = player2Object.AddComponent<AIClass>();
                    break;
                case 1:
                    P2 = player2Object.AddComponent<AIMediumClass>();
                    break;
                case 2:
                    P2 = player2Object.AddComponent<AIHardClass>();
                    break;
            }
            P2.Initialize(O_Piece, p2Shape);
        }
        else
        {
            // Tạo Player 2
            GameObject player2Object = new GameObject("Player2");
            P2 = player2Object.AddComponent<PlayerClass>();
            P2.Initialize(O_Piece, p2Shape);
        }
    }


    // Check if the AI goes first
    public static bool IsAIFirst()
    {
        return AITurnFirst;
    }

    // Determine whose turn it is
    public static bool WhoseTurn()
    {
        return altTurns;
    }

    // Get the name of Player 1
    public static string GetPlayer1Name()
    {
        return player1Name;
    }

    // Get the name of Player 2 or AI
    public static string GetPlayer2Name()
    {
        return AIflag ? AInames[AIlevel] : player2Name;
    }

    #endregion

    #region Game State Management

    // Check win conditions and handle game-over logic
    public static void CheckWinConditions()
    {
        if (gameWon || GameInfo.IsDraw())
        {
            if (GameInfo.IsDraw())
            {
                Debug.Log("The game is a draw!");
            }

            // Update player history and load the end screen
            EndScreen.ExecuteHistorySystem();
            SceneManager.LoadScene("End");

        }
    }

    // Check if either player has achieved a win condition
    public static void CheckWinState()
    {
        Debug.Log("CheckWinState started.");

        // Kiểm tra theo chiều dọc
        for (int x = 0; x < BoardArray.nAcross; x++)
        {
            int verticalX = 0;
            int verticalO = 0;

            for (int y = 0; y < BoardArray.nDown; y++)
            {
                string currentState = PlayerClass.GetBoardState($"piece{x},{y}");

                if (currentState == "x_Occupied")
                {
                    verticalX++;
                    verticalO = 0;
                    if (verticalX >= 4)
                    {
                        gameWon = true;
                        GameInfo.SetWinner(GetPlayer1Name());
                        return;
                    }
                }
                else if (currentState == "o_Occupied")
                {
                    verticalO++;
                    verticalX = 0;
                    if (verticalO >= 4)
                    {
                        gameWon = true;
                        GameInfo.SetWinner(GetPlayer2Name());
                        return;
                    }
                }
                else
                {
                    verticalX = 0;
                    verticalO = 0;
                }
            }
        }

        // Kiểm tra theo chiều ngang
        for (int y = 0; y < BoardArray.nDown; y++)
        {
            int horizontalX = 0;
            int horizontalO = 0;

            for (int x = 0; x < BoardArray.nAcross; x++)
            {
                string currentState = PlayerClass.GetBoardState($"piece{x},{y}");

                if (currentState == "x_Occupied")
                {
                    horizontalX++;
                    horizontalO = 0;
                    if (horizontalX >= 4)
                    {
                        gameWon = true;
                        GameInfo.SetWinner(GetPlayer1Name());
                        return;
                    }
                }
                else if (currentState == "o_Occupied")
                {
                    horizontalO++;
                    horizontalX = 0;
                    if (horizontalO >= 4)
                    {
                        gameWon = true;
                        GameInfo.SetWinner(GetPlayer2Name());
                        return;
                    }
                }
                else
                {
                    horizontalX = 0;
                    horizontalO = 0;
                }
            }
        }

        // Kiểm tra đường chéo chính (từ trái trên xuống phải dưới)
        for (int i = 0; i <= BoardArray.nAcross - 4; i++)
        {
            for (int j = 0; j <= BoardArray.nDown - 4; j++)
            {
                int diagonalX = 0;
                int diagonalO = 0;

                for (int k = 0; k < 4; k++)
                {
                    string currentState = PlayerClass.GetBoardState($"piece{i + k},{j + k}");

                    if (currentState == "x_Occupied")
                    {
                        diagonalX++;
                        diagonalO = 0;
                    }
                    else if (currentState == "o_Occupied")
                    {
                        diagonalO++;
                        diagonalX = 0;
                    }
                    else
                    {
                        diagonalX = 0;
                        diagonalO = 0;
                    }
                }

                if (diagonalX >= 4)
                {
                    gameWon = true;
                    GameInfo.SetWinner(GetPlayer1Name());
                    return;
                }
                if (diagonalO >= 4)
                {
                    gameWon = true;
                    GameInfo.SetWinner(GetPlayer2Name());
                    return;
                }
            }
        }

        // Kiểm tra đường chéo phụ (từ phải trên xuống trái dưới)
        for (int i = 4; i < BoardArray.nAcross; i++)
        {
            for (int j = 0; j <= BoardArray.nDown - 4; j++)
            {
                int diagonalX = 0;
                int diagonalO = 0;

                for (int k = 0; k < 4; k++)
                {
                    string currentState = PlayerClass.GetBoardState($"piece{i - k},{j + k}");

                    if (currentState == "x_Occupied")
                    {
                        diagonalX++;
                        diagonalO = 0;
                    }
                    else if (currentState == "o_Occupied")
                    {
                        diagonalO++;
                        diagonalX = 0;
                    }
                    else
                    {
                        diagonalX = 0;
                        diagonalO = 0;
                    }
                }

                if (diagonalX >= 4)
                {
                    gameWon = true;
                    GameInfo.SetWinner(GetPlayer1Name());
                    return;
                }
                if (diagonalO >= 4)
                {
                    gameWon = true;
                    GameInfo.SetWinner(GetPlayer2Name());
                    return;
                }
            }
        }
    }

    //This method checks whether a player's consecutive tiles are valid.
    private static bool ValidateConsecutive(char player, int winStateIndex)
    {
        Debug.Log($"Validating consecutive for player {player}, winStateIndex: {winStateIndex}");

        // Mảng để lưu trữ các vị trí thuộc về winState này
        List<(int x, int y)> positions = new List<(int x, int y)>();

        // Thu thập tất cả các vị trí thuộc về winState này
        for (int x = 0; x < BoardArray.nAcross; x++)
        {
            for (int y = 0; y < BoardArray.nDown; y++)
            {
                if (BoardArray.GetWCB(x, y, winStateIndex))
                {
                    Debug.Log($"Found position in win condition: ({x},{y})");

                    string expectedTag = (player == 'x') ? "x_Occupied" : "o_Occupied";
                    GameObject space = GameObject.Find($"piece{x},{y}");

                    if (space && space.CompareTag(expectedTag))
                    {
                        Debug.Log($"Position ({x},{y}) is occupied by {player}");

                        positions.Add((x, y));
                    }
                }
            }
        }

        // Nếu không đủ 4 vị trí, return false
        Debug.Log($"Total positions found: {positions.Count}");

        if (positions.Count < 4)
        {
            Debug.Log("Not enough positions for a win");

            return false;
        }

        // Sắp xếp các vị trí theo hướng phù hợp
        positions = SortPositionsByWinState(positions, winStateIndex);
        Debug.Log("Positions after sorting:");
        foreach (var pos in positions)
        {
            Debug.Log($"({pos.x},{pos.y})");
        }
        // Kiểm tra tính liên tiếp
        for (int i = 0; i < positions.Count - 1; i++)
        {
            var current = positions[i];
            var next = positions[i + 1];

            // Kiểm tra khoảng cách giữa hai vị trí
            int dx = Math.Abs(next.x - current.x);
            int dy = Math.Abs(next.y - current.y);
            Debug.Log($"Checking continuity between ({current.x},{current.y}) and ({next.x},{next.y}): dx={dx}, dy={dy}");

            // Nếu không liên tiếp (khoảng cách > 1), return false
            if (dx > 1 || dy > 1)
            {
                Debug.Log("Positions are not consecutive");

                return false;
            }
        }
        Debug.Log("All positions are consecutive");

        return true;
    }

    private static List<(int x, int y)> SortPositionsByWinState(List<(int x, int y)> positions, int winStateIndex)
    {
        // Xác định hướng của winState để sắp xếp cho phù hợp
        if (winStateIndex < BoardArray.nDown) // Hàng ngang 
        {
            return positions.OrderBy(p => p.x).ToList();
        }
        else if (winStateIndex < BoardArray.nDown + BoardArray.nAcross) // Hàng dọc
        {
            // Sửa lại cách sắp xếp cho cột
            return positions.OrderBy(p => p.y).ThenBy(p => p.x).ToList();
        }
        else if (winStateIndex < BoardArray.nDown + BoardArray.nAcross + (BoardArray.nAcross - 4)) // Chéo chính
        {
            return positions.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        }
        else // Chéo phụ 
        {
            return positions.OrderBy(p => p.x).ThenByDescending(p => p.y).ToList();
        }
    }


    #endregion
}