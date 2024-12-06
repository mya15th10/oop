using UnityEngine;
using System.IO;

// The History class manages the game's player history, including win/loss/draw records.
// It handles reading, writing, and updating player statistics.
public class History : MonoBehaviour
{
    #region Constants

    private const char DELIM = ',';
    private const string gameHistory = "TTT_Game_History.txt";
    private const string guestName = "Guest";

    private const int numRecords = 100;
    private const int numOfFields = 4;
    private const int maxCount = 999;
    private const int minCount = 0;

    private const int nameIndex = 0;
    private const int winIndex = 1;
    private const int lossIndex = 2;
    private const int drawIndex = 3;

    #endregion

    #region Static Variables

    private static int playerIndex;
    private static int currentRecords;
    private static string readIn;
    private static string[] inputLine;
    private static bool playerFound;
    private static string[,] PlayerHistory = new string[numRecords, numOfFields];

    #endregion

    #region Public Static Methods

    // Populate the PlayerHistory array with data from the history file
    public static void PopulatePlayerHistory()
    {
        if (File.Exists(gameHistory))
        {
            using (var reader = new StreamReader(new FileStream(gameHistory, FileMode.Open, FileAccess.Read)))
            {
                int count = 0;
                readIn = reader.ReadLine();

                while (count < numRecords && readIn != null)
                {
                    inputLine = readIn.Split(DELIM);
                    for (int j = 0; j < numOfFields; j++)
                    {
                        PlayerHistory[count, j] = inputLine[j];
                    }
                    count++;
                    readIn = reader.ReadLine();
                }

                currentRecords = count;

                // Initialize unused spots in the PlayerHistory array
                for (int k = count; k < numRecords; k++)
                {
                    for (int i = 0; i < numOfFields; i++)
                    {
                        PlayerHistory[k, i] = "";
                    }
                }
            }
        }
    }

    // Update player history based on the game's result
    public static void UpdatePlayerHistory(string playerName, string winner)
    {
        playerFound = false;
        string[] newPlayer = new string[numOfFields];

        if (playerName != guestName)
        {
            // Check if player already exists in the history
            for (int i = 0; i < currentRecords; i++)
            {
                if (PlayerHistory[i, nameIndex] == playerName)
                {
                    playerIndex = i;
                    playerFound = true;
                    break;
                }
            }

            // Add a new player if not found
            if (!playerFound)
            {
                playerIndex = currentRecords;
                currentRecords++;
                newPlayer[nameIndex] = playerName;
            }

            // Update statistics based on game result
            if (playerName == winner) // Winner
            {
                IncrementOrInitialize(winIndex, "1", newPlayer);
            }
            else if (GameInfo.IsDraw()) // Draw
            {
                IncrementOrInitialize(drawIndex, "1", newPlayer);
            }
            else // Loser
            {
                IncrementOrInitialize(lossIndex, "1", newPlayer);
            }

            // Add new player to the PlayerHistory array
            if (!playerFound)
            {
                for (int n = 0; n < numOfFields; n++)
                {
                    PlayerHistory[playerIndex, n] = newPlayer[n];
                }
            }
        }
    }

    // Write the current PlayerHistory data back to the history file
    public static void WriteHistoryFile()
    {
        using (var writer = new StreamWriter(new FileStream(gameHistory, FileMode.Create, FileAccess.Write)))
        {
            for (int i = 0; i < currentRecords; i++)
            {
                string entry = string.Join(DELIM.ToString(), PlayerHistory[i, 0], PlayerHistory[i, 1], PlayerHistory[i, 2], PlayerHistory[i, 3]);
                writer.WriteLine(entry);
            }
        }
    }

    // Display the non-empty entries in PlayerHistory to the Console
    public static void DisplayArray()
    {
        for (int i = 0; i < currentRecords; i++)
        {
            Debug.Log(string.Join(" , ", PlayerHistory[i, 0], PlayerHistory[i, 1], PlayerHistory[i, 2], PlayerHistory[i, 3]));
        }
    }

    // Get a formatted string of all player history entries
    public static string GetPlayerHistoryEntry()
    {
        var result = string.Empty;

        for (int i = 0; i < currentRecords; i++)
        {
            result += $"{PlayerHistory[i, nameIndex]}\r\n" +
                      $"{PlayerHistory[i, winIndex]} / {PlayerHistory[i, lossIndex]} / {PlayerHistory[i, drawIndex]}\r\n\r\n";
        }

        return result;
    }

    // Get a specific player's name from the history by index
    public static string GetPlayerHistoryNames(int index)
    {
        return PlayerHistory[index, nameIndex] ?? string.Empty;
    }

    // Get the total number of current records
    public static int GetCurrentRecords()
    {
        return currentRecords;
    }

    #endregion

    #region Private Static Methods

    // Increment the value of a stat or initialize it for a new player
    private static void IncrementOrInitialize(int statIndex, string initialValue, string[] newPlayer)
    {
        if (playerFound)
        {
            PlayerHistory[playerIndex, statIndex] = IncrementValue(PlayerHistory[playerIndex, statIndex]);
        }
        else
        {
            for (int i = 0; i < numOfFields; i++)
            {
                newPlayer[i] = "0";
            }
            newPlayer[statIndex] = initialValue;
        }
    }

    // Increment a numeric string value within bounds
    private static string IncrementValue(string value)
    {
        int number = int.TryParse(value, out var parsed) ? parsed : 0;
        return Mathf.Clamp(number + 1, minCount, maxCount).ToString();
    }

    #endregion
}
