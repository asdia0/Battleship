# Ship Class

<sub>Namespace: [Battleship.Core](../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Defines a ship.

```cs
public class Ship
```

## Constructors

| Constructor | Description |
| ----------- | ----------- |
| [`Ship(Grid, Int, Int)`](Constructor/Ship(Grid,%20Int,%20Int).md) | Initializes a new instance of the [Ship](Ship.md) class. |
| [`Ship(Int, Int, Int)`](Constructor/Ship(Int,%20Int,%20int).md) | Initializes a new instance of the [Ship](Ship.md) class without a [grid](../Grid/Grid.md). |

## Fields

| Field | Description |
|-------|-------------|
| [`Alignment`](Field/Alignment.md) | The alignment of the [ship](Ship.md). |
| [`Breadth`](Field/Breadth.md) | The breadth of the [ship](Ship.md). |
| [`CurrentOccupiedSquares`](Field/CurrentOccupiedSquares.md) | The unsunk [squares](../Square/Square.md) the [ship](Ship.md) occupies. |
| [`Grid`](Field/Grid.md) | The [grid](../Grid/Grid.md) the [ship](Ship.md) is on. |
| [`ID`](Field/ID.md) | The ID of the [ship](Ship.md). |
| [`IsSunk`](Field/IsSunk.md) | Determines if the [ship](Ship.md) is sunk. |
| [`Length`](Field/Length.md) | The length of the [ship](Ship.md). |
| [`Name`](Field/Name.md) | The name of the [ship](Ship.md) |
| [`OriginalOccupiedSquares`](Field/OriginalOccupiedSquares.md) | All the [squares](../Square/Square.md) the [ship](Ship.md) has ever occupied. |

## Methods

| Method | Description |
| ------ | ----------- |
| [`CanFit(Square, Bool)`](Method/CanFit(Square,%20Bool).md) | Checks if the [ship](Ship.md) can fit on a [grid](../Grid/Grid.md) at a given position. |
| [`GetArrangements()`](Method/GetArrangements().md) | Gets all the possible ways the [ship](Ship.md) can be arranged on a [grid](../Grid/Grid.md). |
| [`IncreaseProbability(Dictionary<Int, Int>, Square, Bool)`](Method/IncreaseProbability(Dictionary[Int,%20Int],%20Square,%20Bool).md) | Increases the probability of the [squares](../Square/Square.md) the [ship](Ship.md) could fit on. |