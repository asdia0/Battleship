# TooManyMisses([Grid](../../Grid/Grid.md), [Square](../../Square/Square.md)) Method

<sub>Class: [Game](../Game.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Adds a [square](../../Square/Square.md) to [`Player.ToAttack`](../../Grid/Field/ToAttack.md) if the hit square only has one unsearched adjacent square.

```cs
private void TooManyMisses(Grid p1, Square sq)
```

## Parameters

`p1`: [Grid](../../Grid/Grid.md)  
Player 2.

`sq`: [Square](../../Square/Square.md)  
The [square](../../Square/Square.md) to check.
