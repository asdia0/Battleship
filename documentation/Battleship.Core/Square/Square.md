# Square Class

<sub>Namespace: [Battleship.Core](../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Defines a square.

```cs
public class Square
```

## Constructors

| Constructor | Description |
| ----------- | ----------- |
| [`Square(Grid, Int)`](Constructor/Square(Grid,%20Int).md) | Initializes a new instance of the [Square](Square.md) class. |

## Fields

| Field | Description |
|-------|-------------|
| [`BeenSearched`](Field/BeenSearched.md) | Determines if the [square](Square.md) has been searched. |
| [`Grid`](Field/Grid.md) | The [grid](../Grid/Grid.md) the [square](Square.md) is on. |
| [`HadShip`](Field/HadShip.md) | Determines if the [square](Square.md) has ever had a [ship](../Ship/Ship.md) on it. |
| [`HasShip`](Field/HasShip.md) | Determines if the [square](Square.md) has a [ship](../Ship/Ship.md) on it. |
| [`ID`](Field/ID) | The ID of the [square](Square.md). |
| [`IsHit`](Field/IsHit.md) | Determines if the [square](square.md) has been hit. |
| [`IsMiss`](Field/IsMiss.md) | Determines if the [Square](square.md) is a miss. |
| [`IsSunk`](Field/IsSunk.md) | Determines if the [Square](square.md) has been sunk. |
| [`Ship`](Field/Ship.md) | The [ship](../Ship/Ship.md) occupying the [square](Square.md). |

## Methods
| Method | Description |
| ------ | ----------- |
| [`GetAdjacentSquares()`](Method/GetAdjacentSquares().md) | Gets the [square](Square.md)'s neighbours. |
| [`GetNumberOfHitAdjacentSquares()`](Method/GetNumberOfHitAdjacentSquares().md) | Gets the number of adjacent [square](Square.md) that are [hit](Field/IsHit.md). |
| [`GetNumberOfHitConnectedSquares()`](Method/GetNumberOfHitConnectedSquares().md) | Gets the number of connected [square](Square.md) that are [hit](Field/IsHit.md). |
| [`ToCoor()`](Method/ToCoor().md) | Gets the x- and y-coordinates of the [square](Square.md). |