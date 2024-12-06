using UnityEngine;
using UnityEngine.SceneManagement;


// UserInterface manages the game's menus and navigation, including player options, scores, and AI settings.
public class UserInterface : MonoBehaviour
{
    #region Variables

    // Screen and button dimensions
    private float screenHeight;
    private float screenWidth;
    private float buttonHeight;
    private float buttonWidth;

    // Scroll view and scores UI elements
    private Vector2 scrollViewVector = Vector2.zero;
    private const string scoresHeading = "Player Name\r\nWins/Losses/Draws\r\n\r\n";
    private string playerHistory;

    // State management for different screens
    private static bool menuScreen = true;
    private static bool startScreen = false;
    private static bool scoresScreen = false;
    private static bool aboutScreen = false;
    private static bool returningP1 = false;
    private static bool returningP2 = false;
    private static bool newP1Screen = false;
    private static bool newP2Screen = false;
    private static bool aiScreen = false;
    private static bool isThereAI = false;
    private static bool doesP1GoFirst = false;
    private static bool doesAIGoFirst = false;
    private static bool isPlayer1 = false;
    private static bool isPlayer2 = false;
    private static bool aiDifficultySelected = false;


    // Player-related variables
    private static string[] returningPlayers;
    private static string stringToEdit;
    private static string tempName;
    private static string player1Name;
    private static string player2Name;

    // AI settings
    private static int selectedGridIndex = -1;
    private static int aiDifficultyLevel = 0; // 0 = Easy, 1 = Normal, 2 = Hard

    // GUI styles
    private static GUIStyle scoreLabelText = new GUIStyle();

    #endregion

    #region Unity Lifecycle Methods

    // Initialize screen dimensions and player history
    private void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        buttonHeight = screenHeight * 0.1f;
        buttonWidth = screenWidth * 0.4f;

        InitializeStrings();
        InitializePlayerHistory();

        scoreLabelText.alignment = TextAnchor.UpperCenter;

    }

    private void OnGUI()
    {
        if (menuScreen)
        {
            ShowMainMenu();
        }
        else if (returningP1)
        {
            ShowReturningPlayerMenu(isPlayer1: true);
        }
        else if (returningP2)
        {
            ShowReturningPlayerMenu(isPlayer1: false);
        }
        else if (aiScreen)
        {
            ShowAIMenu();
        }
        else if (startScreen)
        {
            ShowStartMenu();
        }
        else if (scoresScreen)
        {
            ShowScoresMenu();
        }
        else if (aboutScreen)
        {
            ShowAboutMenu();
        }
        else if (newP1Screen)
        {
            ShowNewPlayerMenu(isPlayer1: true);
        }
        else if (newP2Screen)
        {
            ShowNewPlayerMenu(isPlayer1: false);
        }
        
    }

    #endregion

    #region Menu Display Methods

    private void ShowMainMenu()
    {
        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.4f, buttonWidth, buttonHeight), "Start"))
        {
            TransitionToScreen("Start");
            SceneManager.LoadScene("Start");
        }

        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.5f, buttonWidth, buttonHeight), "Scores"))
        {
            TransitionToScreen("Scores");
            SceneManager.LoadScene("Scores");
        }

        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.6f, buttonWidth, buttonHeight), "About"))
        {

            TransitionToScreen("About");
            SceneManager.LoadScene("About");
        }

        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.7f, buttonWidth, buttonHeight), "Quit"))
        {
            Application.Quit();
        }
    }

    private void ShowStartMenu()
    {
        // Player 1 options
        ShowPlayerOptions("Player1", isPlayer1: true, 0.1f);

        // Player 2 options
        ShowPlayerOptions("Player2", isPlayer1: false, 0.65f);

        // Ready to play
        if (isPlayer1 && isPlayer2 && (doesP1GoFirst || doesAIGoFirst))
        {
            if (GUI.Button(new Rect(screenWidth * 0.66f, screenHeight * 0.63f, buttonWidth * 0.58f, buttonHeight * 0.7f), "Play!"))
            {
                StartGame();
            }
        }
        // Player can quit the game

        if (GUI.Button(new Rect(screenWidth * 0.66f, screenHeight * 0.73f, buttonWidth * 0.58f, buttonHeight * 0.7f), "Quit"))
        {
            Application.Quit();
        }
        // Radio Buttons for who goes first
        GUI.Box(new Rect(screenWidth * 0.4f, screenHeight * 0.65f, screenWidth * 0.2f, screenHeight * 0.2f), "Who goes first?");

        doesP1GoFirst = GUI.Toggle(new Rect(screenWidth * 0.43f, screenHeight * 0.7f, 100, 30), doesP1GoFirst, "Player 1");
        if (doesP1GoFirst)
        {
            doesAIGoFirst = false;
        }

        if (isThereAI)
        {
            doesAIGoFirst = GUI.Toggle(new Rect(screenWidth * 0.43f, screenHeight * 0.75f, 100, 30), doesAIGoFirst, "AI");
            if (doesAIGoFirst)
            {
                doesP1GoFirst = false;
            }
        }

        //nút Main Menu
        if (GUI.Button(new Rect(screenWidth * 0.66f, screenHeight * 0.83f, buttonWidth * 0.58f, buttonHeight * 0.7f), "Main Menu"))
        {
            TransitionToScreen("Menu");
        }
    }

    private void ShowScoresMenu()
    {
        scrollViewVector = GUI.BeginScrollView(new Rect(screenWidth * 0.3f, screenHeight * 0.25f, screenWidth * 0.6f, screenHeight * 0.6f), scrollViewVector, new Rect(0, 0, 400, 2000));
        playerHistory = GUI.TextArea(new Rect(-screenWidth * 0.05f, 0, screenWidth * 0.5f, screenHeight), playerHistory, scoreLabelText);
        GUI.EndScrollView();

        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.85f, buttonWidth, buttonHeight), "Main Menu"))
        {
            TransitionToScreen("Menu");
        }
    }

    private void ShowAboutMenu()
    {
        if (GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.85f, buttonWidth, buttonHeight), "Main Menu"))
        {
            TransitionToScreen("Menu");
        }
    }

    private void ShowAIMenu()
    {
        // Điều chỉnh vị trí các nút để chúng không bị chồng lên nhau
        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.2f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Easy"))
        {
            SetAIDifficulty(0, "Easy Bob");
        }
        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.3f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Normal"))
        {
            SetAIDifficulty(1, "Cunning Clive");
        }
        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.4f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Hard"))
        {
            SetAIDifficulty(2, "Master Shifu");
        }

        // Thêm nút Return
        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.5f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Return"))
        {
            aiScreen = false;
            startScreen = true;
        }
    }

    #endregion

    #region Helper Methods

    private void TransitionToScreen(string screenName)
    {
        menuScreen = screenName == "Menu";
        startScreen = screenName == "Start";
        scoresScreen = screenName == "Scores";
        aboutScreen = screenName == "About";
    }
    private void ShowNewPlayerMenu(bool isPlayer1)
    {
        string label = isPlayer1 ? "New Player 1" : "New Player 2";
        GUI.Box(new Rect(screenWidth * 0.375f, screenHeight * 0.05f, screenWidth * 0.25f, screenHeight * 0.5f), label);

        GUI.Label(new Rect(screenWidth * 0.4f, screenHeight * 0.135f, buttonWidth * 0.65f, buttonHeight * 0.5f),
                  isPlayer1 ? "P1 Name (no commas):" : "P2 Name (no commas):");

        stringToEdit = GUI.TextField(new Rect(screenWidth * 0.4f, screenHeight * 0.2f, buttonWidth * 0.5f, buttonHeight * 0.4f),
                                      stringToEdit, 25);

        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.3f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Enter"))
        {
            if (isPlayer1)
            {
                player1Name = stringToEdit;
                UserInterface.isPlayer1 = true;
            }
            else
            {
                player2Name = stringToEdit;
                UserInterface.isPlayer2 = true;
            }

            stringToEdit = "";
            newP1Screen = false;
            newP2Screen = false;
            startScreen = true;
        }

        if (GUI.Button(new Rect(screenWidth * 0.4f, screenHeight * 0.4f, buttonWidth * 0.5f, buttonHeight * 0.4f), "Return"))
        {
            newP1Screen = false;
            newP2Screen = false;
            startScreen = true;
        }
    }

    private void ShowReturningPlayerMenu(bool isPlayer1)
    {
        string label = isPlayer1 ? "Returning P1" : "Returning P2";
        GUI.Box(new Rect(screenWidth * 0.25f, screenHeight * 0.1f, screenWidth * 0.5f, screenHeight * 0.8f), label);

        scrollViewVector = GUI.BeginScrollView(new Rect(screenWidth * 0.28f, screenHeight * 0.15f, screenWidth * 0.44f, screenHeight * 0.6f),
                                                scrollViewVector, new Rect(0, 0, screenWidth * 0.42f, screenHeight * 2.5f));

        GUIStyle textStyle = new GUIStyle(GUI.skin.button);
        textStyle.fontSize = 14;
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleLeft;
        if ((selectedGridIndex = GUI.SelectionGrid(new Rect(0, 0, screenWidth * 0.42f, screenHeight * 2.5f),
                                                   selectedGridIndex, returningPlayers, 1, textStyle)) >= 0)
        {
            string selectedName = returningPlayers[selectedGridIndex];

            if (isPlayer1)
            {
                player1Name = selectedName;
                UserInterface.isPlayer1 = true;
            }
            else
            {
                player2Name = selectedName;
                UserInterface.isPlayer2 = true;
            }

            selectedGridIndex = -1;
            returningP1 = false;
            returningP2 = false;
            startScreen = true;
        }

        GUI.EndScrollView();

        if (GUI.Button(new Rect(screenWidth * 0.28f, screenHeight * 0.77f, screenWidth * 0.44f, buttonHeight * 0.7f), "Return"))
        {
            returningP1 = false;
            returningP2 = false;
            startScreen = true;
        }
    }

    private void ShowPlayerOptions(string playerLabel, bool isPlayer1, float positionX)
    {
        // Tạo hộp hiển thị nhãn cho từng người chơi
        GUI.Box(new Rect(screenWidth * positionX, screenHeight * 0.05f, screenWidth * 0.25f, screenHeight * 0.5f), playerLabel);

        // Nút "New User" cho phép người chơi tạo tài khoản mới
        if (GUI.Button(new Rect(screenWidth * (positionX + 0.01f), screenHeight * 0.1f, buttonWidth * 0.58f, buttonHeight * 0.5f), "New User"))
        {
            if (isPlayer1)
            {
                newP1Screen = true;
                startScreen = false; // Ensure we leave the start screen
            }
            else
            {
                newP2Screen = true;
                startScreen = false;
            }
        }

        // the ability to choose from a list of saved names
        if (GUI.Button(new Rect(screenWidth * (positionX + 0.01f), screenHeight * 0.15f, buttonWidth * 0.58f, buttonHeight * 0.5f), "Returning User"))
        {
            if (isPlayer1)
            {
                returningP1 = true;
                startScreen = false;
            }
            else
            {
                returningP2 = true;
                startScreen = false;
            }
        }
        // Nút "Guest" cho phép chơi với tư cách khách
        if (GUI.Button(new Rect(screenWidth * (positionX + 0.01f), screenHeight * 0.20f, buttonWidth * 0.58f, buttonHeight * 0.5f), "Guest"))
        {
            if (isPlayer1)
            {
                UserInterface.isPlayer1 = true; // Truy cập biến tĩnh bằng tên lớp
            }
            else
            {
                UserInterface.isPlayer2 = true;
            }
        }

        //Player 2 will AI
        if (GUI.Button(new Rect(screenWidth * (positionX + 0.01f), screenHeight * 0.25f, buttonWidth * 0.58f, buttonHeight * 0.5f), "AI"))
        {
            UserInterface.isThereAI = true;
            UserInterface.aiScreen = true;
            UserInterface.startScreen = false; // Tắt màn hình Start

            if (!isPlayer1)
            {
                UserInterface.isPlayer2 = true;
            }

        }
    }


    private void SetAIDifficulty(int level, string name)
    {
        aiDifficultyLevel = level;
        player2Name = name;
        aiScreen = false;    // Tắt màn hình AI
        startScreen = true;  // Quay lại màn hình Start
    }

    private void StartGame()
    {
        Debug.Log($"Starting game with: {player1Name}, {player2Name}");
        new Player(player1Name, player2Name, isThereAI, doesAIGoFirst, aiDifficultyLevel);
        SceneManager.LoadScene("Gameplay");
    }

    private void InitializeStrings()
    {
        stringToEdit = "";
        player1Name = "";
        player2Name = "";
        playerHistory = "";
    }

    private void InitializePlayerHistory()
    {
        History.PopulatePlayerHistory();
        PopulateReturningPlayers();
        playerHistory = scoresHeading + History.GetPlayerHistoryEntry();
    }

    private void PopulateReturningPlayers()
    {
        int currentRecords = History.GetCurrentRecords();
        returningPlayers = new string[currentRecords];
        for (int i = 0; i < currentRecords; i++)
        {
            returningPlayers[i] = History.GetPlayerHistoryNames(i);
        }
    }

    #endregion
}
