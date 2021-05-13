# CanFit([Square](../../Square/Square.md), Bool) Method

<sub>Class: [Ship](../Ship.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Checks if the [ship](../Ship.md) can fit on a [grid](../../Grid/Grid.md) at a given position.

```cs
public Bool CanFit(Square sq, Bool alignment)
```

## Parameters

`sq`: [Square](../../Square/Square.md)  
The starting [square](../../Square/Square.md).

`alignment`: Bool  
Determines if the [ship](../Ship.md) is horizontally or vertically placed.

## Returns

Bool
`true` if the [ship](../Ship.md) can fit. `false` if otherwise.