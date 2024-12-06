using UnityEngine;

public class AIMediumClass : AIClass
{
    #region Fields
    #endregion

    #region Constructors
    public AIMediumClass() : base() { }

    public AIMediumClass(GameObject piece, char pShape) : base(piece, pShape)
    {
        SetupOpponentShape(pShape);
    }
    #endregion

    #region Main Logic
    public override bool GetMove()
    {
        convertBoardToArray();

        // Kiểm tra nước đi thắng hoặc chặn
        var winningMove = FindBestMove();
        if (winningMove != null)
        {
            return MakeMove(winningMove.Value);
        }

        // Nếu không có nước đi tốt, chọn ngẫu nhiên
        return MakeRandomMove();
    }
    #endregion

    #region Board Management
    private void SetupOpponentShape(char pShape)
    {
        opponentShape = (pShape == 'o') ? 'x' : 'o';
    }

    public virtual int convertBoardToArray()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                gameBoardSpace = GameObject.Find($"piece{x},{y}");
                theBoard[x, y] = GetBoardSpaceState(gameBoardSpace);
            }
        }
        return 0;
    }

    private char GetBoardSpaceState(GameObject space)
    {
        if (space.tag == "x_Occupied") return 'x';
        if (space.tag == "o_Occupied") return 'o';
        return '-';
    }
    #endregion

    #region Move Analysis
    private Vector3? FindBestMove()
    {
        // Thứ tự ưu tiên: Thắng > Chặn > Ngẫu nhiên
        var winningMove = canIWin();
        if (winningMove != new Vector3(-1, 0.1f, -1))
        {
            return winningMove;
        }

        var blockingMove = canIBlock();
        if (blockingMove != new Vector3(-1, 0.1f, -1))
        {
            return blockingMove;
        }

        return null;
    }

    private bool MakeMove(Vector3 position)
    {
        gameBoardSpace = GameObject.Find($"piece{(int)position.x},{(int)position.z}");

        if (IsValidMove(gameBoardSpace))
        {
            PlacePiece(position);
            UpdateGameState(gameBoardSpace);
            return true;
        }

        return false;
    }

    private bool MakeRandomMove()
    {
        System.Random random = new System.Random();
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            var position = new Vector3(
                random.Next(5),
                0.1f,
                random.Next(5)
            );

            gameBoardSpace = GameObject.Find($"piece{(int)position.x},{(int)position.z}");

            if (IsValidMove(gameBoardSpace))
            {
                PlacePiece(position);
                UpdateGameState(gameBoardSpace);
                return true;
            }

            attempts++;
        }

        return false;
    }

    private bool IsValidMove(GameObject space)
    {
        return space != null &&
               space.tag != "x_Occupied" &&
               space.tag != "o_Occupied" &&
               !GameInfo.IsDraw();
    }

    private void PlacePiece(Vector3 position)
    {
        Instantiate(gamePiece, position, Quaternion.identity);
    }

    private void UpdateGameState(GameObject space)
    {
        space.tag = $"{shape}_Occupied";
        var boardSpace = space.GetComponent<BoardSpace>();
        boardSpace.UpdateSpaceState(shape);
        GameInfo.IncrementTurnCount();
        Player.CheckWinState();
        GameInfo.UpdatePlayerColors();
    }
    #endregion

    #region Win Detection
    protected virtual Vector3 canIWin()
    {
        // Logic kiểm tra chiến thắng cho AI
        Vector3 myVector = new Vector3(-1, 0.1f, -1);
        int tempX, tempZ;

        #region VertsAndHoriz
        for (int lc = 0; lc < 5; lc++)
        {
            for (int inn = 0; inn < 2; inn++)
            {
                if (theBoard[lc, 0 + inn] == shape && theBoard[lc, 1 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 3 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 0 + inn] == shape && theBoard[lc, 1 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 2 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 2 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 0 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 1 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 1 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 1 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 0 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 0 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                if (theBoard[0 + inn, lc] == shape && theBoard[1 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == '-')
                {
                    tempX = 3 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[0 + inn, lc] == shape && theBoard[1 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[2 + inn, lc] == '-')
                {
                    tempX = 2 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[0 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[1 + inn, lc] == '-')
                {
                    tempX = 1 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[1 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[0 + inn, lc] == '-')
                {
                    tempX = 0 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
            }
        }
        
        #endregion

        #region diagonals
        //Main Diagonal
        for (int inn = 0; inn < 2; inn++)
        {
            //Bottom left to top right
            if (theBoard[0 + inn, 0 + inn] == shape && theBoard[1 + inn, 1 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == '-')
            {
                tempX = 3 + inn;
                tempZ = 3 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 0 + inn] == shape && theBoard[1 + inn, 1 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[2 + inn, 2 + inn] == '-')
            {
                tempX = 2 + inn;
                tempZ = 2 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 0 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[1 + inn, 1 + inn] == '-')
            {
                tempX = 1 + inn;
                tempZ = 1 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[1 + inn, 1 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[0 + inn, 0 + inn] == '-')
            {
                tempX = 0 + inn;
                tempZ = 0 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            //Top left to bottom right
            if (theBoard[0 + inn, 4 - inn] == shape && theBoard[1 + inn, 3 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == '-')
            {
                tempX = 3 + inn;
                tempZ = 1 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 4 - inn] == shape && theBoard[1 + inn, 3 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[2 + inn, 2 - inn] == '-')
            {
                tempX = 2 + inn;
                tempZ = 2 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 4 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[1 + inn, 3 - inn] == '-')
            {
                tempX = 1 + inn;
                tempZ = 3 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[1 + inn, 3 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[0 + inn, 4 - inn] == '-')
            {
                tempX = 0 + inn;
                tempZ = 4 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
        }
        //Diag 1
        if (theBoard[0, 1] == shape && theBoard[1, 2] == shape && theBoard[2, 3] == shape && theBoard[3, 4] == '-')
        {
            tempX = 3;
            tempZ = 4;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[0, 1] == shape && theBoard[1, 2] == shape && theBoard[3, 4] == shape && theBoard[2, 3] == '-')
        {
            tempX = 2;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[0, 1] == shape && theBoard[2, 3] == shape && theBoard[3, 4] == shape && theBoard[1, 2] == '-')
        {
            tempX = 1;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 2] == shape && theBoard[2, 3] == shape && theBoard[3, 4] == shape && theBoard[0, 1] == '-')
        {
            tempX = 0;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        // Diag 2
        if (theBoard[1, 0] == shape && theBoard[2, 1] == shape && theBoard[3, 2] == shape && theBoard[4, 3] == '-')
        {
            tempX = 4;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 0] == shape && theBoard[2, 1] == shape && theBoard[4, 3] == shape && theBoard[3, 2] == '-')
        {
            tempX = 3;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 0] == shape && theBoard[3, 2] == shape && theBoard[4, 3] == shape && theBoard[2, 1] == '-')
        {
            tempX = 2;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[2, 1] == shape && theBoard[3, 2] == shape && theBoard[4, 3] == shape && theBoard[1, 0] == '-')
        {
            tempX = 1;
            tempZ = 0;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        //Diag 3

        if (theBoard[4, 1] == shape && theBoard[3, 2] == shape && theBoard[2, 3] == shape && theBoard[1, 4] == '-')
        {
            tempX = 1;
            tempZ = 4;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[4, 1] == shape && theBoard[3, 2] == shape && theBoard[1, 4] == shape && theBoard[2, 3] == '-')
        {
            tempX = 2;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[4, 1] == shape && theBoard[2, 3] == shape && theBoard[1, 4] == shape && theBoard[3, 2] == '-')
        {
            tempX = 3;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 2] == shape && theBoard[2, 3] == shape && theBoard[1, 4] == shape && theBoard[4, 1] == '-')
        {
            tempX = 4;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }

        // Diag 4
        if (theBoard[3, 0] == shape && theBoard[2, 1] == shape && theBoard[1, 2] == shape && theBoard[0, 3] == '-')
        {
            tempX = 0;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 0] == shape && theBoard[2, 1] == shape && theBoard[0, 3] == shape && theBoard[1, 2] == '-')
        {
            tempX = 1;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 0] == shape && theBoard[1, 2] == shape && theBoard[0, 3] == shape && theBoard[2, 1] == '-')
        {
            tempX = 2;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[2, 1] == shape && theBoard[1, 2] == shape && theBoard[0, 3] == shape && theBoard[3, 0] == '-')
        {
            tempX = 3;
            tempZ = 0;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        #endregion

        return myVector;
    }

    protected virtual Vector3 canIBlock()
    {
    // Logic chặn nước đi của người chơi
    {
        Vector3 myVector = new Vector3(-1, 0.1f, -1);
        int tempX, tempZ;

        #region VertsAndHoriz
        for (int lc = 0; lc < 5; lc++)
        {
            for (int inn = 0; inn < 2; inn++)
            {
                if (theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 3 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 2 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 2 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 1 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 1 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 0 + inn] == '-')
                {
                    tempX = lc;
                    tempZ = 0 + inn;
                    return new Vector3(tempX, 0.1f, tempZ);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                if (theBoard[0 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == '-')
                {
                    tempX = 3 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[0 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == '-')
                {
                    tempX = 2 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[0 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == '-')
                {
                    tempX = 1 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
                if (theBoard[1 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[0 + inn, lc] == '-')
                {
                    tempX = 0 + inn;
                    tempZ = lc;
                    return new Vector3(tempX, 0.1f, tempZ);
                }
            }
        }
        #endregion

        #region diagonals
        //Loop through the main diagonal
        for (int inn = 0; inn < 2; inn++)
        {
            //Bottom left to top right
            if (theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == '-')
            {
                tempX = 3 + inn;
                tempZ = 3 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == '-')
            {
                tempX = 2 + inn;
                tempZ = 2 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == '-')
            {
                tempX = 1 + inn;
                tempZ = 1 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[0 + inn, 0 + inn] == '-')
            {
                tempX = 0 + inn;
                tempZ = 0 + inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            //Top left to bottom right
            if (theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == '-')
            {
                tempX = 3 + inn;
                tempZ = 1 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == '-')
            {
                tempX = 2 + inn;
                tempZ = 2 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == '-')
            {
                tempX = 1 + inn;
                tempZ = 3 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
            if (theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[0 + inn, 4 - inn] == '-')
            {
                tempX = 0 + inn;
                tempZ = 4 - inn;
                return new Vector3(tempX, 0.1f, tempZ);
            }
        }
        //Check through short diagonals
        //Diag 1
        if (theBoard[0, 1] == opponentShape && theBoard[1, 2] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[3, 4] == '-')
        {
            tempX = 3;
            tempZ = 4;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[0, 1] == opponentShape && theBoard[1, 2] == opponentShape && theBoard[3, 4] == opponentShape && theBoard[2, 3] == '-')
        {
            tempX = 2;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[0, 1] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[3, 4] == opponentShape && theBoard[1, 2] == '-')
        {
            tempX = 1;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 2] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[3, 4] == opponentShape && theBoard[0, 1] == '-')
        {
            tempX = 0;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        //Diag 2
        if (theBoard[1, 0] == opponentShape && theBoard[2, 1] == opponentShape && theBoard[3, 2] == opponentShape && theBoard[4, 3] == '-')
        {
            tempX = 4;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 0] == opponentShape && theBoard[2, 1] == opponentShape && theBoard[4, 3] == opponentShape && theBoard[3, 2] == '-')
        {
            tempX = 3;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[1, 0] == opponentShape && theBoard[3, 2] == opponentShape && theBoard[4, 3] == opponentShape && theBoard[2, 1] == '-')
        {
            tempX = 2;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[2, 1] == opponentShape && theBoard[3, 2] == opponentShape && theBoard[4, 3] == opponentShape && theBoard[1, 0] == '-')
        {
            tempX = 1;
            tempZ = 0;
            return new Vector3(tempX, 0.1f, tempZ);
        }

        //Diag 3

        if (theBoard[4, 1] == opponentShape && theBoard[3, 2] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[1, 4] == '-')
        {
            tempX = 1;
            tempZ = 4;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[4, 1] == opponentShape && theBoard[3, 2] == opponentShape && theBoard[1, 4] == opponentShape && theBoard[2, 3] == '-')
        {
            tempX = 2;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[4, 1] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[1, 4] == opponentShape && theBoard[3, 2] == '-')
        {
            tempX = 3;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 2] == opponentShape && theBoard[2, 3] == opponentShape && theBoard[1, 4] == opponentShape && theBoard[4, 1] == '-')
        {
            tempX = 4;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }

        // Diag 4
        if (theBoard[3, 0] == opponentShape && theBoard[2, 1] == opponentShape && theBoard[1, 2] == opponentShape && theBoard[0, 3] == '-')
        {
            tempX = 0;
            tempZ = 3;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 0] == opponentShape && theBoard[2, 1] == opponentShape && theBoard[0, 3] == opponentShape && theBoard[1, 2] == '-')
        {
            tempX = 1;
            tempZ = 2;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[3, 0] == opponentShape && theBoard[1, 2] == opponentShape && theBoard[0, 3] == opponentShape && theBoard[2, 1] == '-')
        {
            tempX = 2;
            tempZ = 1;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        if (theBoard[2, 1] == opponentShape && theBoard[1, 2] == opponentShape && theBoard[0, 3] == opponentShape && theBoard[3, 0] == '-')
        {
            tempX = 3;
            tempZ = 0;
            return new Vector3(tempX, 0.1f, tempZ);
        }
        #endregion

        return myVector;
    }
}
    #endregion
}