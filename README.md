\# Tic Tac Toe 5x5

### Project Description
Enhanced Unity implementation of Tic Tac Toe with a 5x5 grid, featuring AI opponents and player statistics tracking.

### Game Features
- 5x5 game board
- Win condition: 4 in a row
- Three AI difficulty levels
- Player statistics tracking
- Game history recording

### Game Modes
- Player vs Player
- Player vs AI 
- Guest mode for casual play

### AI Behaviors

#### Easy AI (AIClass)
- Makes random valid moves
- Basic move validation

#### Medium AI (AIMediumClass)
- Detects winning moves
- Blocks opponent's winning moves
- Falls back to random moves if no critical moves found

#### Hard AI (AIHardClass)
- Uses position weighting strategy
- Advanced blocking patterns
- Prioritizes strategic board positions
- Multiple layers of move evaluation

### Core Components
- BoardArray: Manages game board structure and win conditions
- BoardSpace: Handles individual space states and move validation
- PlayerClass: Base class for player move management
- GameInfo: Manages game state and UI information
- History: Handles player statistics and game history
- UserInterface: Controls menu systems and game flow
- EndScreen: Manages victory conditions and end-game states

### Project Structure
```
├── Scripts
│   ├── AI
│   │   ├── AIClass.cs
│   │   ├── AIMediumClass.cs
│   │   └── AIHardClass.cs
│   ├── Board
│   │   ├── BoardArray.cs
│   │   └── BoardSpace.cs
│   ├── Core
│   │   ├── Player.cs
│   │   └── PlayerClass.cs
│   └── UI
│       ├── GameInfo.cs
│       ├── History.cs
│       ├── EndScreen.cs
│       └── UserInterface.cs
```

### System Requirements
- Unity Engine
- .NET Framework compatibility

### Getting Started
1. Clone the repository
2. Open the project in Unity
3. Load the main scene
4. Players can:
   - Create a new profile
   - Select a returning player profile
   - Play as a guest
   - Play against AI

### Game Flow
1. Main Menu
2. Player Selection
3. Game Mode Selection
4. Gameplay
5. End Screen with Results
6. Stats Update

### EndScreen Features
- Dynamic victory messages
- Game statistics updates
- Retry option
- Return to main menu option
- Score tracking integration

### Statistics System
- Tracks wins, losses, and draws
- Persistent player profiles
- History file management
- Up to 100 player records
- CSV format storage

### Development
Built with:
- Unity Engine
- C# for game logic
- Unity GUI system for interface

### Contributing
1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request
