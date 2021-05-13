# AddShip([Square](../../Square/Square.md), [Ship](../../Ship/Ship.md), Bool) Method

<sub>Class: [Grid](../Grid.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Adds a [ship](../../Ship/Ship.md).

```cs
public bool AddShip(Square square, Ship ship, bool alignment)
```

## Parameters

`square`: [Square](../../Square/Square.md)  
The [square](../../Square/Square.md) to add the [ship](../../Ship/Ship.md) on.

`ship`: [Ship](../../Ship/Ship.md)  
The [ship](../../Ship/Ship.md) to add.

`alignment`: Bool  
Determines the alignment of the [ship](../../Ship/Ship.md). Horizontal is `true`, vertical is `false`.

## Returns

Bool  
A success result. `true` if the process succeeded, `false` if the process failed.
