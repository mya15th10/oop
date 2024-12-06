using UnityEngine;
using System;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

// AIClass represents the AI player in the game
// Inherits from PlayerClass and overrides specific behaviors like making a move
public class AIClass : PlayerClass

{
    #region Constructors

    // Default constructor
    public AIClass() { }

    // Constructor with parameters to initialize game piece and player shape
    public AIClass(GameObject piece, char pShape)
    {
        gamePiece = piece; // The game piece object used by the AI
        shape = pShape;    // The shape ('x' or 'o') representing this player
    }

    #endregion

    #region AI Logic

    // This method defines how the AI determines and makes its move
    public override bool GetMove()
    {
        int AIPieceCoordX, AIPieceCoordZ;
        float AIPieceCoordY;
        System.Random random = new System.Random();

        int maxAttempts = 100; // Thêm giới hạn số lần thử
        int attempts = 0;

        do
        {
            AIPieceCoordX = random.Next(BoardArray.nAcross);
            AIPieceCoordY = Player.pieceHeightFromBoard;
            AIPieceCoordZ = random.Next(BoardArray.nAcross);

            gameBoardSpace = GameObject.Find($"piece{AIPieceCoordX},{AIPieceCoordZ}");

            // Thêm kiểm tra null
            if (gameBoardSpace != null)
            {
                Debug.Log($"AI attempting to place piece at ({AIPieceCoordX}, {AIPieceCoordZ})");

                if (gameBoardSpace.tag != "o_Occupied" &&
                    gameBoardSpace.tag != "x_Occupied" &&
                    !GameInfo.IsDraw())
                {
                    Vector3 boardLocation = new Vector3(AIPieceCoordX, AIPieceCoordY, AIPieceCoordZ);
                    Instantiate(gamePiece, boardLocation, Quaternion.identity);

                    if (shape == 'x')
                    {
                        gameBoardSpace.tag = "x_Occupied";
                    }
                    else if (shape == 'o')
                    {
                        gameBoardSpace.tag = "o_Occupied";
                    }

                    BoardSpace thisSpace = gameBoardSpace.GetComponent<BoardSpace>();
                    if (thisSpace != null)  // Thêm kiểm tra null cho BoardSpace
                    {
                        thisSpace.UpdateSpaceState(shape);
                        GameInfo.IncrementTurnCount();
                        Player.CheckWinState();
                        GameInfo.UpdatePlayerColors();
                        return true;
                    }
                }
            }

            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("AI couldn't find a valid move after maximum attempts");
                return false;
            }

        } while ((gameBoardSpace == null) ||
                 (gameBoardSpace.tag == "x_Occupied") ||
                 (gameBoardSpace.tag == "o_Occupied"));

        return false;
    }

    #endregion
}
