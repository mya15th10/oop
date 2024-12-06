using UnityEngine;
using UnityEngine.SceneManagement;


// EndScreen handles the post-game UI and functionality, such as displaying the winner and navigation options.
public class EndScreen : MonoBehaviour
{
    #region Variables

    // Screen and UI element dimensions
    private static float screenHeight;
    private static float screenWidth;
    private static float buttonWidth;
    private static float buttonHeight;
    private static float labelWidth;
    private static float labelHeight;

    // Style for the win message label
    private static GUIStyle winLabelText = new GUIStyle();

    // Array of possible win messages
    private static string[] winMsgArr = new string[numMsgs]
    {
        " so remarkable!", " is the sovereign of the world!",
        " is superior to the other individual!",
        " is the dominant force in the world!",
        "is truly exceptional at Tic Tac Toe!", " is Tic Tac Toe-tally AWESOME :D",
        ": You have emerged victorious!"
    };

    // Game state information
    private static string player1Name;
    private static string player2Name;
    private static string winner;
    private static string winMsg;
    private static bool draw;

    private const int numMsgs = 7;

    #endregion

    #region Unity Lifecycle Methods

    // Called when the script instance is being loaded
    private void Start()
    {
        InitializeUI();
        SetWinMsg();
        Debug.Log("Instantiated EndScreen");
    }

    // Draw the UI elements for the end screen
    private void OnGUI()
    {
        // Display the winner message
        GUI.Label(
            new Rect(
                (screenWidth / 2) - (labelWidth / 2),
                (screenHeight / 2) - (labelHeight / 0.75f),
                labelWidth,
                labelHeight
            ),
            GameInfo.GetWinner() + winMsg,
            winLabelText
        );

        // Retry button: Reload the gameplay scene
        if (GUI.Button(
            new Rect(
                (screenWidth / 2) - (buttonWidth / 2),
                (screenHeight / 2) - (buttonHeight / 1.25f),
                buttonWidth,
                buttonHeight
            ),
            "Retry?"
        ))
        {
            SceneManager.LoadScene("Gameplay");
            
        }

        // Main Menu button: Navigate back to the main menu
        if (GUI.Button(
            new Rect(
                (screenWidth / 2) - (buttonWidth / 2),
                (screenHeight / 2) + (buttonHeight / 1.25f),
                buttonWidth,
                buttonHeight
            ),
            "Main Menu"
        ))
        {
            SceneManager.LoadScene("Menu");

        }
    }

    #endregion

    #region Static Methods

    // Executes history system updates
    public static void ExecuteHistorySystem()
    {
        History.PopulatePlayerHistory();
        History.UpdatePlayerHistory(Player.GetPlayer1Name(), GameInfo.GetWinner());
        History.UpdatePlayerHistory(Player.GetPlayer2Name(), GameInfo.GetWinner());
        History.DisplayArray();
        History.WriteHistoryFile();
    }

    #endregion

    #region Private Methods

    // Initializes the UI dimensions and styles
    private void InitializeUI()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        buttonWidth = screenWidth / 4;
        buttonHeight = screenHeight / 12;
        labelWidth = screenWidth / 2;
        labelHeight = screenHeight / 5;

        winLabelText.fontSize = 36;
        winLabelText.normal.textColor = Color.yellow;
        GUI.backgroundColor = Color.gray;
        winLabelText.alignment = TextAnchor.UpperCenter;
    }

    // Sets the win message based on the game result
    private void SetWinMsg()
    {
        if (GameInfo.IsDraw())
        {
            winMsg = "All participants have been defeated.";
        }
        else
        {
            System.Random random = new System.Random();
            winMsg = winMsgArr[random.Next(numMsgs)];
        }
    }

    #endregion
}
