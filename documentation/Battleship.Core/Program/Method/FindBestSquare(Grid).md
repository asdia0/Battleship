# FindBestSquare(Grid) Method

<sub>Class: [Program](../Program.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Finds the best [square](../../Square/Square.md) to search.

```cs
public static (Square, Dictionary<Int, Int>) FindBestSquare(Grid player)
```

## Parameters

`player`: [Grid](../../Grid/Grid.md)  
The player's grid.

## Returns

([Square](../../Square/Square.md), Dictionary<Int, Int>)

Returns the best square as well as the probability map of all the [squares](../../Square/Square.md) on the [grid](../../Grid/Grid.md).