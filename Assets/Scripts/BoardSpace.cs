using UnityEngine;

// BoardSpace represents an individual space on the game board.
// This class handles the state of the space and updates game state when a move is made.
public class BoardSpace : MonoBehaviour
{
    #region Variables

    // Horizontal and vertical values representing the position of this space on the board
    private int h_val;
    private int v_val;

    // Name of this space, used to extract position values
    private string thisSpaceName;
    private bool winConditionMet;


    #endregion

    #region Unity Lifecycle Methods

    // Called when the script is initialized
    private void Start()
    {
        InitializeSpace();
    }

    #endregion

    #region Public Methods

    // Updates the state of the space based on the player's move
    public bool UpdateSpaceState(char player)
    {
        string expectedTag = (player == 'x') ? "x_Occupied" : "o_Occupied";
        Debug.Log($"=== Verifying space {this.name} ===");
        Debug.Log($"Before update - Tag: {this.gameObject.tag}");

        // Cập nhật tag
        this.gameObject.tag = expectedTag;

        // Verify ngay sau khi update
        Debug.Log($"After update - Tag: {this.gameObject.tag}");
        if (this.gameObject.tag != expectedTag)
        {
            Debug.LogError($"Tag update failed! Expected: {expectedTag}, Got: {this.gameObject.tag}");
        }

        // Kiểm tra điều kiện thắng
        bool consecutive = IsConsecutive(h_val, v_val, player);
        Debug.Log($"IsConsecutive check result for position ({h_val},{v_val}): {consecutive}");

        if (consecutive)
        {
            for (int i = 0; i < BoardArray.nWinStates; i++)
            {
                Debug.Log($"Checking win state {i} for position ({h_val},{v_val})");
                if (BoardArray.GetWCB(h_val, v_val, i))
                {
                    if (player == 'x')
                    {
                        BoardArray.IncrementWCSX(i);
                        int newCount = BoardArray.GetWCSX(i);
                        Debug.Log($"Updated WCSX for state {i} to {newCount}");
                    }
                    else
                    {
                        BoardArray.IncrementWCSO(i);
                        int newCount = BoardArray.GetWCSO(i);
                        Debug.Log($"Updated WCSO for state {i} to {newCount}");
                    }
                }
            }
        }
        return true;
    }

    //Check if there is a consecutive string of players
    private bool IsConsecutive(int x, int y, char player)
    {
        string targetTag = (player == 'x') ? "x_Occupied" : "o_Occupied";
        Debug.Log($"=== Starting consecutive check for player {player} at position ({x},{y}) ===");

        // Kiểm tra dọc
        int verticalCount = 1;
        Debug.Log($"Starting vertical check from ({x},{y})");

        // Kiểm tra hướng lên
        for (int i = y - 1; i >= 0; i--)
        {
            string position = $"piece{x},{i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking up - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                verticalCount++;
                Debug.Log($"Match found up! Current vertical count: {verticalCount}");
            }
            else
            {
                Debug.Log($"No match up - Expected {targetTag} but found {currentTag}");
                break;
            }
        }

        // Kiểm tra hướng xuống
        for (int i = y + 1; i < BoardArray.nDown; i++)
        {
            string position = $"piece{x},{i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking down - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                verticalCount++;
                Debug.Log($"Match found down! Current vertical count: {verticalCount}");
            }
            else
            {
                Debug.Log($"No match down - Expected {targetTag} but found {currentTag}");
                break;
            }
        }

        Debug.Log($"Final vertical count: {verticalCount}");
        if (verticalCount >= 4)
        {
            Debug.Log($"Found vertical win for {player} with {verticalCount} pieces!");
            return true;
        }

        // Kiểm tra ngang
        int horizontalCount = 1;
        Debug.Log($"Starting horizontal check from ({x},{y})");

        // Kiểm tra sang trái
        for (int i = x - 1; i >= 0; i--)
        {
            string position = $"piece{i},{y}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking left - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                horizontalCount++;
                Debug.Log($"Match found left! Current horizontal count: {horizontalCount}");
            }
            else
            {
                Debug.Log($"No match left - Expected {targetTag} but found {currentTag}");
                break;
            }
        }

        // Kiểm tra sang phải
        for (int i = x + 1; i < BoardArray.nAcross; i++)
        {
            string position = $"piece{i},{y}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking right - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                horizontalCount++;
                Debug.Log($"Match found right! Current horizontal count: {horizontalCount}");
            }
            else
            {
                Debug.Log($"No match right - Expected {targetTag} but found {currentTag}");
                break;
            }
        }

        Debug.Log($"Final horizontal count: {horizontalCount}");
        if (horizontalCount >= 4)
        {
            Debug.Log($"Found horizontal win for {player} with {horizontalCount} pieces!");
            return true;
        }

        // Kiểm tra chéo chính
        int mainDiagonalCount = 1;
        Debug.Log($"Starting main diagonal check from ({x},{y})");

        // Kiểm tra chéo chính hướng lên
        for (int i = 1; x - i >= 0 && y - i >= 0; i++)
        {
            string position = $"piece{x - i},{y - i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking main diagonal up - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                mainDiagonalCount++;
                Debug.Log($"Match found on main diagonal up! Count: {mainDiagonalCount}");
            }
            else break;
        }

        // Kiểm tra chéo chính hướng xuống
        for (int i = 1; x + i < BoardArray.nAcross && y + i < BoardArray.nDown; i++)
        {
            string position = $"piece{x + i},{y + i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking main diagonal down - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                mainDiagonalCount++;
                Debug.Log($"Match found on main diagonal down! Count: {mainDiagonalCount}");
            }
            else break;
        }

        Debug.Log($"Final main diagonal count: {mainDiagonalCount}");
        if (mainDiagonalCount >= 4)
        {
            Debug.Log($"Found main diagonal win for {player} with {mainDiagonalCount} pieces!");
            return true;
        }

        // Kiểm tra chéo phụ
        int antiDiagonalCount = 1;
        Debug.Log($"Starting anti-diagonal check from ({x},{y})");

        // Kiểm tra chéo phụ hướng lên
        for (int i = 1; x - i >= 0 && y + i < BoardArray.nDown; i++)
        {
            string position = $"piece{x - i},{y + i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking anti-diagonal up - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                antiDiagonalCount++;
                Debug.Log($"Match found on anti-diagonal up! Count: {antiDiagonalCount}");
            }
            else break;
        }

        // Kiểm tra chéo phụ hướng xuống
        for (int i = 1; x + i < BoardArray.nAcross && y - i >= 0; i++)
        {
            string position = $"piece{x + i},{y - i}";
            string currentTag = PlayerClass.GetBoardState(position);
            Debug.Log($"Checking anti-diagonal down - Position: {position}, Tag: {currentTag}");

            if (currentTag == targetTag)
            {
                antiDiagonalCount++;
                Debug.Log($"Match found on anti-diagonal down! Count: {antiDiagonalCount}");
            }
            else break;
        }

        Debug.Log($"Final anti-diagonal count: {antiDiagonalCount}");
        if (antiDiagonalCount >= 4)
        {
            Debug.Log($"Found anti-diagonal win for {player} with {antiDiagonalCount} pieces!");
            return true;
        }

        Debug.Log($"No winning condition found at position ({x},{y})");
        return false;
    }


    #endregion

    #region Private Methods

    // Initialize the space's position and state based on its name
    private void InitializeSpace()
    {
        // Get the name of the space (e.g., "piece1,2")
        thisSpaceName = this.name;

        // Extract horizontal (h_val) and vertical (v_val) values from the name
        // Assuming the name format is always "pieceX,Y"
        h_val = thisSpaceName[5] - '0'; // Convert character to integer
        v_val = thisSpaceName[7] - '0'; // Convert character to integer
    }
    public static void PrintBoardState()
    {
        Debug.Log("=== Current Board State ===");
        for (int y = 0; y < BoardArray.nDown; y++)
        {
            for (int x = 0; x < BoardArray.nAcross; x++)
            {
                GameObject space = GameObject.Find($"piece{x},{y}");
                if (space != null)
                {
                    Debug.Log($"Position ({x},{y}): Name={space.name}, Tag={space.tag}");
                }
                else
                {
                    Debug.Log($"Position ({x},{y}): Object not found");
                }
            }
        }
        Debug.Log("========================");
    }
    #endregion
}