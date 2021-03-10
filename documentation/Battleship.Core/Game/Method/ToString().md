# ToString() Method

<sub>Class: [Game](../Game.md)  
Namespace: [Battleship.Core](../../Battleship.Core.md)  
Assembly: Battleship.Core.dll</sub>

Converts the [game](../Game.md) to a String.

```cs
public String ToString()
```

## Returns

String  
Both player's [grids](../../Grid/Grid.md) and the list of [moves](../../Move/Move.md) in the following format:

```txt
[Player 1 "Player 1's grid"]
[Player 2 "Player 2's grid"]

Player 1: (x, y) # Coordinate of first square searched.
Player 2: (x, y) # Coordinate of second square searched.
```
