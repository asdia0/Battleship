# Program Class

<sub>Namespace: [Battleship.Core](../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Allows user to interact with the program.

```cs
public class Program
```

## Fields

| Field | Description |
|-------|-------------|
| [`delayTimer`](Field/delayTimer.md) | Timer for [`Delay(Double)`](Method/Delay(Double).md). |
| [`player1`](Field/player1.md) | Represents how many wins player 1 had in [`Simulate(Int)`](Method/Simulate(Int).md). |
| [`player2`](Field/player2.md) | Represents how many wins player 2 had in [`Simulate(Int)`](Method/Simulate(Int).md). |
| [`sumMoves`](Field/sumMoves.md) | Represents the total number of [moves](../Move/Move.md). |

## Methods
| Method | Description |
| ------ | ----------- |
| [`Delay(Double)`](Method/Delay(Double).md) | Delays the program. |
| [`Find()`](Method/Find().md) | Finds the best [square](../Square/Square.md). |
| [`FindBestSquare(Grid)`](Method/FindBestSquare(Grid).md) | Finds the best [square](../Square/Square.md) to search. |
| [`HighestHCS(List<Square>)`](Method/HighestHCS(List[Square]).md) | Gets the [square](../Square/Square.md) with the highest number of Hit Connected Squares in a list. |
| [`Main()`](Method/Main().md) | Entry point of the program. |
| [`Median(List<Dynamic>)`](Method/Median(List[Dynamic]).md) | Gets the median of a list. |
| [`Mode(List<Dynamic>)`](Method/Mode(List[Dynamic]).md) | Gets the mode of a list. |
| [`Simulate(Int)`](Method/Simulate(Int).md)| Simulates [games](../Game/Game.md). |