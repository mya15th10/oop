using UnityEngine;

public class AIHardClass : AIMediumClass
{
    #region Fields
    private readonly int[,] weightedMap = new int[,] {
        {3, 4, 3, 4, 3},
        {4, 6, 6, 6, 4},
        {3, 6, 8, 6, 3},
        {4, 6, 6, 6, 4},
        {3, 4, 3, 4, 3}
    };
    #endregion

    #region Constructors
    public AIHardClass() : base() { }

    public AIHardClass(GameObject piece, char pShape) : base(piece, pShape)
    {
        gamePiece = piece;
        shape = pShape;
        opponentShape = (shape == 'o') ? 'x' : 'o';
    }
    #endregion

    #region Main Logic
    public override bool GetMove()
    {
        convertBoardToArray();

        Vector3[] moves = new[] {
            canIWin(),           // Priority 1: Win if possible
            canIBlock(),         // Priority 2: Block opponent's win
            block2InARow(),      // Priority 3: Block 2 in a row in inner 9
            block2InARowEdges(), // Priority 4: Block 2 in a row on edges
            blockSean(),         // Priority 5: Special blocking strategy
            takeEdges(),         // Priority 6: Take strategic edges
            findHighestWeight()  // Priority 7: Take highest weighted position
        };

        Vector3 nullVector = new Vector3(-1, 0.1f, -1);

        // Try each move in priority order
        foreach (Vector3 move in moves)
        {
            if (move != nullVector)
            {
                return ExecuteMove(move);
            }
        }

        // If no strategic move is found, make a random move
        return MakeRandomMove();
    }

    public override int convertBoardToArray()
    {
        for (int n = 0; n < 5; n++)
        {
            for (int b = 0; b < 5; b++)
            {
                gameBoardSpace = GameObject.Find($"piece{n},{b}");
                if (gameBoardSpace.tag == "x_Occupied")
                {
                    theBoard[n, b] = 'x';
                    weightedMap[n, b] = 0;
                }
                else if (gameBoardSpace.tag == "o_Occupied")
                {
                    theBoard[n, b] = 'o';
                    weightedMap[n, b] = 0;
                }
                else
                    theBoard[n, b] = '-';
            }
        }
        return 0;
    }
    #endregion

    #region Move Execution
    private bool ExecuteMove(Vector3 position)
    {
        Instantiate(gamePiece, position, Quaternion.identity);
        gameBoardSpace = GameObject.Find($"piece{(int)position.x},{(int)position.z}");
        gameBoardSpace.tag = $"{shape}_Occupied";

        BoardSpace thisSpace = gameBoardSpace.GetComponent<BoardSpace>();
        thisSpace.UpdateSpaceState(shape);

        GameInfo.IncrementTurnCount();
        Player.CheckWinState();
        GameInfo.UpdatePlayerColors();

        return true;
    }

    private bool MakeRandomMove()
    {
        System.Random ran = new System.Random();
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            int tempX = ran.Next(5);
            float tempY = 0.1f;
            int tempZ = ran.Next(5);

            gameBoardSpace = GameObject.Find($"piece{tempX},{tempZ}");
            if (gameBoardSpace != null &&
                gameBoardSpace.tag != "x_Occupied" &&
                gameBoardSpace.tag != "o_Occupied")
            {
                return ExecuteMove(new Vector3(tempX, tempY, tempZ));
            }
            attempts++;
        }
        return false;
    }
    #endregion

    #region Strategic Moves
    public Vector3 findHighestWeight()
    {
        int highest = 0;
        int highestX = -1;
        int highestZ = -1;

        for (int n = 0; n < 5; n++)
        {
            for (int b = 0; b < 5; b++)
            {
                if (weightedMap[n, b] > highest)
                {
                    highest = weightedMap[n, b];
                    highestX = n;
                    highestZ = b;
                }
            }
        }

        return new Vector3(highestX, 0.1f, highestZ);
    }

    public Vector3 block2InARow()
    {
        int x = -1;
        int z = -1;
        Vector3 blockingVector;

        for (int n = 1; n < 4; n++)
        {

            if (theBoard[n, 1] == opponentShape && theBoard[n, 2] == opponentShape && theBoard[n, 3] == '-')
            {
                x = n;
                z = 3;
            }
            else if (theBoard[n, 2] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 1] == '-')
            {
                x = n;
                z = 1;
            }
            else if (theBoard[n, 1] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 2] == '-')
            {
                x = n;
                z = 2;
            }

            else if (theBoard[1, n] == opponentShape && theBoard[2, n] == opponentShape && theBoard[3, n] == '-')
            {
                x = 3;
                z = n;
            }
            else if (theBoard[2, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[1, n] == '-')
            {
                x = 1;
                z = n;
            }
            else if (theBoard[1, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[2, n] == '-')
            {
                x = 2;
                z = n;
            }
        }

        if (theBoard[1, 1] == opponentShape && theBoard[2, 2] == opponentShape && theBoard[3, 3] == '-')
        {
            x = 3;
            z = 3;
        }
        else if (theBoard[2, 2] == opponentShape && theBoard[3, 3] == opponentShape && theBoard[1, 1] == '-')
        {
            x = 1;
            z = 1;
        }
        else if (theBoard[1, 1] == opponentShape && theBoard[3, 3] == opponentShape && theBoard[2, 2] == '-')
        {
            x = 2;
            z = 2;
        }

        else if (theBoard[1, 3] == opponentShape && theBoard[2, 2] == opponentShape && theBoard[3, 1] == '-')
        {
            x = 3;
            z = 1;
        }
        else if (theBoard[2, 2] == opponentShape && theBoard[3, 1] == opponentShape && theBoard[1, 3] == '-')
        {
            x = 1;
            z = 3;
        }
        else if (theBoard[1, 3] == opponentShape && theBoard[3, 1] == opponentShape && theBoard[2, 2] == '-')
        {
            x = 2;
            z = 2;
        }

        blockingVector = new Vector3(x, 0.1f, z);

        return blockingVector;
    }

    public Vector3 block2InARowEdges()
    {
        int x = -1;
        int z = -1;
        Vector3 blockingVector;

        for (int n = 0; n < 5; n += 4)
        {

            if (theBoard[n, 1] == opponentShape && theBoard[n, 2] == opponentShape && theBoard[n, 3] == '-')
            {
                x = n;
                z = 3;
            }
            else if (theBoard[n, 2] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 1] == '-')
            {
                x = n;
                z = 1;
            }
            else if (theBoard[n, 1] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 2] == '-')
            {
                x = n;
                z = 2;
            }

            else if (theBoard[1, n] == opponentShape && theBoard[2, n] == opponentShape && theBoard[3, n] == '-')
            {
                x = 3;
                z = n;
            }
            else if (theBoard[2, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[1, n] == '-')
            {
                x = 1;
                z = n;
            }
            else if (theBoard[1, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[2, n] == '-')
            {
                x = 2;
                z = n;
            }
        }

        blockingVector = new Vector3(x, 0.1f, z);

        return blockingVector;
    }

    public Vector3 takeEdges()
    {
        int x = -1;
        int z = -1;
        Vector3 winningVector;

        for (int n = 0; n < 5; n += 4)
        {

            if (theBoard[n, 1] == shape && theBoard[n, 2] == shape && theBoard[n, 3] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
            {
                x = n;
                z = 3;
            }
            else if (theBoard[n, 2] == shape && theBoard[n, 3] == shape && theBoard[n, 1] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
            {
                x = n;
                z = 1;
            }
            else if (theBoard[n, 1] == shape && theBoard[n, 3] == shape && theBoard[n, 2] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
            {
                x = n;
                z = 2;
            }
            /////////////////////////////////////
            else if (theBoard[1, n] == shape && theBoard[2, n] == shape && theBoard[3, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
            {
                x = 3;
                z = n;
            }
            else if (theBoard[2, n] == shape && theBoard[3, n] == shape && theBoard[1, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
            {
                x = 1;
                z = n;
            }
            else if (theBoard[1, n] == shape && theBoard[3, n] == shape && theBoard[2, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
            {
                x = 2;
                z = n;
            }
        }

        winningVector = new Vector3(x, 0.1f, z);

        return winningVector;
    }

    public Vector3 blockSean()
    {
        Vector3 blocker;
        int x = -1;
        int z = -1;

        if (theBoard[1, 2] == opponentShape)
        {
            if (theBoard[0, 1] == '-')
            {
                x = 0;
                z = 1;
            }
            else if (theBoard[0, 3] == '-')
            {
                x = 0;
                z = 3;
            }
        }
        if (theBoard[3, 2] == opponentShape)
        {
            if (theBoard[4, 1] == '-')
            {
                x = 4;
                z = 1;
            }
            else if (theBoard[4, 3] == '-')
            {
                x = 4;
                z = 3;
            }
        }
        if (theBoard[2, 1] == opponentShape)
        {
            if (theBoard[1, 0] == '-')
            {
                x = 1;
                z = 0;
            }
            else if (theBoard[3, 0] == '-')
            {
                x = 3;
                z = 0;
            }
        }
        if (theBoard[2, 3] == opponentShape)
        {
            if (theBoard[1, 4] == '-')
            {
                x = 1;
                z = 4;
            }
            else if (theBoard[3, 4] == '-')
            {
                x = 3;
                z = 4;
            }
        }

        blocker = new Vector3(x, 0.1f, z);
        Debug.Log("blockSean playing at (" + x + "," + z + ")");
        return blocker;
    }
    #endregion
}