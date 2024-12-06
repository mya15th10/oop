using UnityEngine;

// Manages all in-game GUI-related components
public class GameInfo : MonoBehaviour
{
    #region Variables
    private static int turnCount; //Số lượt chơi đã diễn ra.
    private static float screenHeight;
    private static float screenWidth;
    private static float labelWidth;
    private static float labelHeight;
    private static string winner;
    private static string player1Name;
    private static string player2Name;

    private static GUIStyle player1Style = new GUIStyle();
    private static GUIStyle player2Style = new GUIStyle();
    private static GUIStyle turnStyle = new GUIStyle();
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeGameInfo();
    }

    private void OnGUI()
    {
        RenderTurnCount();
        RenderPlayerNames();
    }
    #endregion

    #region Public Static Methods
    // Cập nhật màu sắc của tên người chơi tùy theo lượt chơi. Nếu người chơi 1 đang lượt,
    // tên người chơi 1 sẽ có màu xanh lá (green), còn người chơi 2 có màu xám (gray), và ngược lại.
    public static void UpdatePlayerColors()
    {
        if (!Player.WhoseTurn())
        {
            player1Style.normal.textColor = Color.green;
            player2Style.normal.textColor = Color.gray;
        }
        else
        {
            player1Style.normal.textColor = Color.gray;
            player2Style.normal.textColor = Color.green;
        }
    }

    // Check if the game is a draw
    public static bool IsDraw()
    {
        return turnCount >= 25;
    }

    // Increment the turn counter
    public static void IncrementTurnCount()
    {
        turnCount++;
    }

    // Set the game's winner
    public static void SetWinner(string winnerName)
    {
        winner = winnerName;
    }

    // Get the game's winner
    public static string GetWinner()
    {
        return winner;
    }
    #endregion

    #region Private Methods
    // Initialize game information and styles
    private void InitializeGameInfo()
    {
        turnCount = 0;
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        labelWidth = screenWidth / 10;
        labelHeight = screenHeight / 10;

        player1Style.fontSize = 20;
        player2Style.fontSize = 20;
        turnStyle.normal.textColor = Color.blue;

        SetInitialPlayerColors();
        winner = "";
    }

    // Assign initial colors to player names
    private void SetInitialPlayerColors()
    {
        if (!Player.IsAIFirst())
        {
            player1Style.normal.textColor = Color.green;
            player2Style.normal.textColor = Color.gray;
        }
        else
        {
            player1Style.normal.textColor = Color.gray;
            player2Style.normal.textColor = Color.green;
        }
    }

    // Render the turn count label on the UI
    private void RenderTurnCount()
    {
        GUI.Label(
            new Rect(
                (Screen.width / 2) - (labelWidth / 2) + (labelWidth / 10), // loc x
                (Screen.height / 11) - (labelHeight / 2) - (labelHeight / 4), // loc y
                labelWidth,
                labelHeight
            ),
            $"Turn Count: {turnCount}"
        );
    }

    // Render player names on the UI
    private void RenderPlayerNames()
    {
        GUI.Label(
            new Rect(
                (Screen.width / 8) - (labelWidth / 2),
                (Screen.height / 2) - (labelHeight / 2),
                labelWidth,
                labelHeight
            ),
            Player.GetPlayer1Name(),
            player1Style
        );

        GUI.Label(
            new Rect(
                Screen.width - (Screen.width / 5 - labelWidth / 2),
                (Screen.height / 2) - (labelHeight / 2),
                labelWidth,
                labelHeight
            ),
            Player.GetPlayer2Name(),
            player2Style
        );
    }
    #endregion
}
