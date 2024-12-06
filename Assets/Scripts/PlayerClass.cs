using System.Collections.Generic;
using UnityEngine;

// PlayerClass serves as the base class for both human players and AI players.
// It manages moves, game piece placement, and updates the board state.
public class PlayerClass : MonoBehaviour
{
    #region Protected Variables

    // Game piece and board space references
    protected GameObject gamePiece;       // The game piece this player places
    protected GameObject gameBoardSpace;  // The board space the player interacts with
    protected Transform myTransform;      // Transform of the clicked board space

    // Player attributes
    protected char shape;           // The shape representing this player ('x' or 'o')
    protected char opponentShape;   // The shape representing the opponent
    protected char[,] theBoard = new char[5, 5]; // The game board (5x5)

    private static Dictionary<string, string> boardState = new Dictionary<string, string>();


    #endregion

    #region Constructors

    // Default constructor
    public PlayerClass() { }

    // Constructor to initialize player with a game piece and shape
    public PlayerClass(GameObject piece, char pShape)
    {
        gamePiece = piece;
        shape = pShape;
        opponentShape = (shape == 'o') ? 'x' : 'o'; // Determine opponent's shape
    }

    #endregion

    #region Public Methods

    // This method is used to make a player's turn.
    // Returns true if the move is successful
    public virtual bool GetMove()
    {
        if (gamePiece == null)
        {
            Debug.LogError("Game piece not assigned!");
            return false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            gameBoardSpace = hit.transform.gameObject;
            myTransform = hit.transform;
            Debug.Log($"Current space tag before any operation: {gameBoardSpace.tag}");

            if (gameBoardSpace != null && myTransform != null)
            {
                Vector3 boardLocation = new Vector3(
                    myTransform.position.x,
                    myTransform.position.y + Player.pieceHeightFromBoard,
                    myTransform.position.z);

                // Ki?m tra tr?ng thái t? boardState
                string currentState = GetBoardState(gameBoardSpace.name);
                Debug.Log($"Current board state for {gameBoardSpace.name}: {currentState}");

                if (gameBoardSpace.tag == "Unoccupied" && !GameInfo.IsDraw())
                {
                    BoardSpace thisSpace = gameBoardSpace.GetComponent<BoardSpace>();
                    Debug.Log($"Got BoardSpace component: {(thisSpace != null)}");

                    if (thisSpace != null)
                    {
                        // C?p nh?t tr?ng thái tr??c khi g?i UpdateSpaceState
                        string newTag = (shape == 'x') ? "x_Occupied" : "o_Occupied";
                        gameBoardSpace.tag = newTag;
                        UpdateBoardState(gameBoardSpace.name, newTag);
                        Debug.Log($"Updated board state for {gameBoardSpace.name} to {newTag}");

                        bool updateSuccess = thisSpace.UpdateSpaceState(shape);
                        Debug.Log($"UpdateSpaceState result: {updateSuccess}");

                        if (updateSuccess)
                        {
                            if (gamePiece != null)
                            {
                                GameObject newPiece = Instantiate(gamePiece, boardLocation, Quaternion.identity);
                                Debug.Log($"Instantiated new piece at {boardLocation}");
                            }

                            // Debug: In ra tr?ng thái c?a các ô lân c?n
                            PrintNeighboringStates(thisSpace.GetComponent<BoardSpace>());

                            Player.CheckWinState();
                            GameInfo.UpdatePlayerColors();
                            GameInfo.IncrementTurnCount();
                            BoardSpace.PrintBoardState();

                            // Debug: In ra tr?ng thái cu?i cùng
                            Debug.Log($"Final board state for {gameBoardSpace.name}: {GetBoardState(gameBoardSpace.name)}");
                            return true;
                        }
                    }
                }
                else
                {
                    Debug.Log($"Move rejected - Space occupied: {gameBoardSpace.tag}, IsDraw: {GameInfo.IsDraw()}");
                }
            }
        }
        return false;
    }

    // Thêm hàm helper ?? debug tr?ng thái các ô lân c?n
    private void PrintNeighboringStates(BoardSpace space)
    {
        if (space != null)
        {
            int x = int.Parse(space.name.Split(',')[0].Replace("piece", ""));
            int y = int.Parse(space.name.Split(',')[1]);

            Debug.Log("=== Neighboring States ===");
            // Ki?m tra ô phía trên
            if (y > 0)
                Debug.Log($"Up: piece{x},{y - 1} -> {GetBoardState($"piece{x},{y - 1}")}");
            // Ki?m tra ô phía d??i
            if (y < BoardArray.nDown - 1)
                Debug.Log($"Down: piece{x},{y + 1} -> {GetBoardState($"piece{x},{y + 1}")}");
            // Ki?m tra ô bên trái
            if (x > 0)
                Debug.Log($"Left: piece{x - 1},{y} -> {GetBoardState($"piece{x - 1},{y}")}");
            // Ki?m tra ô bên ph?i
            if (x < BoardArray.nAcross - 1)
                Debug.Log($"Right: piece{x + 1},{y} -> {GetBoardState($"piece{x + 1},{y}")}");
            Debug.Log("========================");
        }
    }

    //This method initializes values ​​for players when they are created.
    public void Initialize(GameObject piece, char pShape)
    {
        gamePiece = piece;
        shape = pShape;
        opponentShape = (shape == 'o') ? 'x' : 'o';
    }


    public static void UpdateBoardState(string position, string tag)
    {
        boardState[position] = tag;
        Debug.Log($"Updated board state: {position} -> {tag}");
    }

    public static string GetBoardState(string position)
    {
        return boardState.ContainsKey(position) ? boardState[position] : "Unoccupied";
    }

    #endregion
}