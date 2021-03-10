# IncreaseProbability(Dictionary<Int, Int>, [Square](../../Square/Square.md), Bool) Method

<sub>Class: [Ship](../Ship.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Increases the probability of the [squares](../../Square/Square.md) the [ship](../Ship.md) could fit on.

```cs
public Dictionary<Int, Int> IncreaseProbability(Dictionary<Int, Int> probability, Square sq, Bool alignment)
```
## Parameters

`probability`: Dictionary<Int, Int>  
A dictionary containing the probability of [squares](../../Square/Square.md) having a [ship](../Ship.md) on it.

`sq`: [Square](../../Square/Square.md)  
[Square](../../Square/Square.md) to place the [ship](../Ship.md) on.

`alignment`: Bool  
[Alignment](../Field/Alignment.md) of [ship](../Ship.md).

## Returns

Dictionary<Int, Int>

The updated `probability` dictionary.