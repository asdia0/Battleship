# Game Class

<sub>Namespace: [Battleship.Core](../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Defines a game of Battleship.

```cs
public class Game
```

## Constructors

| Constructor | Description |
|-------------|-------------|
| [`Game()`](Constructor/Game().md) | Initializes a new instance of the [Game](Game.md) class. |

## Fields

| Field | Description |
|-------|-------------|
| [`MoveList`](Field/MoveList.md) | Represents the list of moves made. |
| [`Moves`](Field/Moves.md) | Represents the number of moves made. |
| [`Player1`](Field/Player1.md) | Represents the first player. |
| [`Player2`](Field/Player2.md) | Represents the second player. |
| [`Turn`](Field/Turn.md) | Represents which player is playing. |
| [`Winner`](Field/Winner.md) | Represents the winner. |

## Methods

| Method | Description |
|--------|-------------|
| [`AddTargets(Grid, Grid, Int)`](Method/AddTargets(Grid,%20Grid,%20Int).md) | Adds [squares](../Square/Square.md) adjacent to a hit square to [`Player.ToSearch`](../Grid/ToSearch.md). |
| [`CreateGame()`](Method/CreateGame().md) | Creates the [game](Game.md). |
| [`CreateGame(Int, Int)`](Method/CreateGame(Int,%20Int).md) | Creates the [game](Game.md) with set algorithms. |
| [`EndGame()`](Method/EndGame().md) | Ends the [game](Game.md). |
| [`HuntTarget()`](Method/HuntTarget().md) | Searches a [square](../Square/Square.md) using the Hunt Target strategy. |
| [`OnlyOneArrangement(Grid, Grid)`](Method/OnlyOneArrangement(Grid,%20Grid).md) | Adds [squares](../Square/Square.md) to [`Player.ToAttack`](../Grid/ToAttack.md) if a ship can only be in one position. |
| [`ProbabilityDensity()`](Method/ProbabilityDensity().md) | Searches a [square](../Square/Square.md) using the Probability Density strategy. |
| [`Random()`](Method/Random().md) | Searches a [square](../Square/Square.md) randomly. |
| [`Search(Grid, Grid, Square)`](Method/Search(Grid,%20Grid,%20Square).md) | Searches a [square](../Square/Square.md). |
| [`StartGame()`](Method/StartGame().md) | Sets up the [game](Game.md). |
| [`TooManyMisses(Grid, Square)`](Method/TooManyMisses(Grid,%20Square).md) | Adds a [square](../Square/Square.md) to [`Player.ToAttack`](../Grid/ToAttack.md) if the hit square only has one unsearched adjacent square. |
| [`ToString()`](Method/ToString().md) | Converts the [game](Game.md) to a String. |
