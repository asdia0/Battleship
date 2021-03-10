# Grid Class

<sub>Namespace: [Battleship.Core](../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Represents a grid.

```cs
public class Grid
```

## Constructors

| Constructor | Description |
|-------------|-------------|
| [`Grid()`](Constructor/Grid().md) | Initializes a new instance of the [Grid](Grid.md) class. |
| [`Grid(Grid)`](Constructor/Grid(Grid).md) | Clones a [Grid](Grid.md). |
| [`Grid(String)`](Constructor/Grid(String).md) | Initializes a new instance of the [Grid](Grid.md) class from a String. |

## Fields

| Field | Description |
|-------|-------------|
| [`OriginalShips`](Field/OriginalShips.md) | A list of all [ships](../Ship/Ship.md). |
| [`SearchedSquares`](Field/SearchedSquares.md) | A list of all searched [squares](../Square/Square.md). |
| [`Ships`](Field/Ships.md) | A list of active [ships](../Ship/Ship.md). |
| [`Squares`](Field/Squares.md) | A list of [squares](../Square/Square.md). |
| [`ToAttack`](Field/ToAttack.md) | A list of enemy [squares](../Square/Square.md) that have a ship. |
| [`ToSearch`](Field/ToSearch.md) | A list of enemy [squares](../Square/Square.md) that may have a ship. |
| [`UnoccupiedSquares`](Field/UnoccupiedSquares.md) | A list of unoccupied [squares](../Square/Square.md). |
| [`UnsearchedSquares`](Field/UnsearchedSquares.md) | A list of unsearched [squares](../Square/Square.md). |

## Methods

| Method | Description |
|--------|-------------|
| [`AddDefaultShips()`](Method/AddDefaultShips().md) | Adds the [ships](../Ship/Ship.md) in the original game of Battleship. |
| [`AddShip(Square, Ship, Bool)`](Method/AddShip(Square,%20Ship,%20Bool).md) | Adds a [ship](../Ship/Ship.md). |
| [`AddShipsRandomly(List<Ship>)`](Method/AddShipsRandomly(List[Ship]).md) | Adds a [ship](../Ship/Ship.md) at a random position. |
| [`AddSquares()`](Method/AddSquares().md) | Adds [squares](../Square/Square.md). |
| [`ToString()`](Method/ToString().md) | Converts the [grid](Grid.md) to a String. |
