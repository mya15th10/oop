using UnityEngine;

// The BoardArray class manages the game board's structure and win condition tracking.
public class BoardArray : MonoBehaviour
{
    #region Constants and Static Variables

    // Board dimensions and total number of possible win states
    public const int nAcross = 5;      // Number of game spaces across the board
    public const int nDown = 5;        // Number of game spaces down the board
    public const int nWinStates = 40;  // Number of possible win states

    // 3D array to track which win conditions apply to each board coordinate
    private static bool[,,] winCheckBoard;

    // Arrays to keep track of each player's progress toward win conditions
    private static int[] winCheckSumX; // For player X
    private static int[] winCheckSumO; // For player O

    #endregion

    #region Unity Lifecycle Methods

    // Called when the script is first run, typically at game start
    private void Start()
    {
        InitializeBoardState();
        InitializeWinConditions();

        Debug.Log("Board Initialized");
    }

    #endregion

    #region Initialization Methods

    // Initializes the board state arrays
    private void InitializeBoardState()
    {
        // Initialize arrays
        winCheckSumX = new int[nWinStates];
        winCheckSumO = new int[nWinStates];
        winCheckBoard = new bool[nAcross, nDown, nWinStates];

        // Set default values for winCheckBoard (all spaces start as false)
        for (int x = 0; x < nAcross; x++)
        {
            for (int y = 0; y < nDown; y++)
            {
                for (int z = 0; z < nWinStates; z++)
                {
                    winCheckBoard[x, y, z] = false;
                }
            }
        }
    }

    private void InitializeWinConditions()
    {
        int winStateIndex = 0;
        Debug.Log("Starting InitializeWinConditions");

        // Các điều kiện chiến thắng ngang
        for (int i = 0; i < nDown; i++)
        {

            for (int j = 0; j <= nAcross - 4; j++)
            {
                Debug.Log($"Initializing horizontal win condition at row {i}, starting column {j}");
                for (int k = 0; k < 4; k++)
                {
                    winCheckBoard[j + k, i, winStateIndex] = true;
                    Debug.Log($"Set winCheckBoard[{j + k},{i},{winStateIndex}] = true");

                }
                winStateIndex++;
            }
        }

        // Các điều kiện chiến thắng dọc
        Debug.Log($"Starting vertical conditions at winStateIndex: {winStateIndex}");

        for (int i = 0; i < nAcross; i++)
        {
            for (int j = 0; j <= nDown - 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    winCheckBoard[i, j + k, winStateIndex] = true;
                    Debug.Log($"Set winCheckBoard[{i},{j + k},{winStateIndex}] = true");

                }
                winStateIndex++;
            }
        }

        // Các điều kiện chiến thắng chéo chính
        Debug.Log($"Starting main diagonal conditions at winStateIndex: {winStateIndex}");

        for (int i = 0; i <= nAcross - 4; i++)
        {
            for (int j = 0; j <= nDown - 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    winCheckBoard[i + k, j + k, winStateIndex] = true;
                }
                winStateIndex++;
            }
        }

        // Các điều kiện chiến thắng chéo phụ
        Debug.Log($"Starting anti-diagonal conditions at winStateIndex: {winStateIndex}");

        for (int i = 0; i <= nAcross - 4; i++)
        {
            for (int j = 4; j < nDown; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    winCheckBoard[i + k, j - k, winStateIndex] = true;
                }
                winStateIndex++;
            }
        }
        Debug.Log($"Final winStateIndex: {winStateIndex}");
        // In giá trị winCheckBoard sau khi thiết lập
        PrintWinCheckBoard();  // Gọi hàm in toàn bộ mảng

        // Reset winCheckSum arrays
        for (int j = 0; j < nWinStates; j++)
        {
            winCheckSumX[j] = 0;
            winCheckSumO[j] = 0;
        }
    }

    #endregion

    #region Public Static Methods

    // Increment the win condition sum for player X
    public static void IncrementWCSX(int winStateIndex)
    {
        winCheckSumX[winStateIndex]++;
        Debug.Log($"Incremented WCSX at index {winStateIndex}, new value: {winCheckSumX[winStateIndex]}");

    }

    // Increment the win condition sum for player O
    public static void IncrementWCSO(int winStateIndex)
    {
        winCheckSumO[winStateIndex]++;
        Debug.Log($"Incremented WCSO at index {winStateIndex}, new value: {winCheckSumO[winStateIndex]}");

    }

    // Get the win condition sum for player X at the given index
    public static int GetWCSX(int winStateIndex)
    {
        return winCheckSumX[winStateIndex];


    }

    // Get the win condition sum for player O at the given index
    public static int GetWCSO(int winStateIndex)
    {
        return winCheckSumO[winStateIndex];
    }

    // Check if a specific board coordinate is part of a given win condition
    public static bool GetWCB(int x, int y, int winStateIndex)
    {
        return winCheckBoard[x, y, winStateIndex];
    }
    private void PrintWinCheckBoard()
    {
        for (int y = 0; y < nDown; y++)
        {
            string row = "";
            for (int x = 0; x < nAcross; x++)
            {
                row += winCheckBoard[x, y, 0] + " ";  // In ra giá trị của winCheckBoard
            }
            Debug.Log("Row " + y + ": " + row);
        }
    }

    // Kiểm tra điều kiện chiến thắng sau mỗi lượt đi



    #endregion
}