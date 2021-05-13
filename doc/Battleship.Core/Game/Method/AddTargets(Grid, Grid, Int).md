# AddTargets([Grid](../../Grid/Grid.md), [Grid](../../Grid/Grid.md), Int) Method

<sub>Class: [Game](../Game.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Adds [squares](../../Square/Square.md) adjacent to a hit square to [`Player.ToSearch`](../../Grid/Field/ToSearch.md).

```cs
private void AddTargets(Grid p1, Grid p2, Int squareID)
```

## Parameters

`p1`: [Grid](../../Grid/Grid.md)  
Player 1.

`p2`: [Grid](../../Grid/Grid.md)  
Player 2.

`squareID`: Int  
The ID of the hit [square](../../Square/Square.md).
