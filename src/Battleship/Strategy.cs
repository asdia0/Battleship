namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class contains methods for the various strategies.
    /// </summary>
    public static class Strategy
    {
        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        /// <param name="opponent">The opponent's grid.</param>
        /// <returns>The square to attack.</returns>
        public static Square Random(Grid opponent)
        {
            return opponent.UnsearchedSquares[new Random().Next(opponent.UnsearchedSquares.Count)];
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        /// <param name="opponent">The opponent's grid.</param>
        /// <returns>The square to attack.</returns>
        public static Square HuntTarget(Grid opponent)
        {
            Square attackedSquare;
            List<Square> hitSquares = opponent.Squares.Where(i => i.Status == SquareStatus.Hit).ToList();
            List<Square> adjHitSquares = new ();

            foreach (Square hitSquare in hitSquares)
            {
                adjHitSquares.AddRange(hitSquare.AdjacentSquares.Where(i => i.Status == SquareStatus.Unsearched));
            }

            if (adjHitSquares.Any())
            {
                // target
                attackedSquare = adjHitSquares[new Random().Next(adjHitSquares.Count)];
            }
            else
            {
                // hunt
                attackedSquare = opponent.UnsearchedSquares[new Random().Next(opponent.UnsearchedSquares.Count)];
            }

            return attackedSquare;
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        /// <param name="opponent">The opponent's grid.</param>
        /// <returns>The square to attack.</returns>
        public static Square Optimal(Grid opponent)
        {
            /*
             * MUST
             * 1. A hit square only has 1 unsearched adjacent square
             * 2. A ship only has 1 possible arrangement
             * MIGHT
             * 3. A square has a hit adjacent square
             */

            // 1. A hit square only has 1 unsearched adjacent square
            foreach (Square square in opponent.Squares)
            {
                if (square.Status != SquareStatus.Hit)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new ();

                foreach (Square adjSquare in square.AdjacentSquares)
                {
                    if (!adjSquare.Searched)
                    {
                        unsearchedSquares.Add(adjSquare);
                    }
                }

                if (unsearchedSquares.Count == 1)
                {
                    return unsearchedSquares[0];
                }
            }

            // 2. A ship only has 1 possible arrangement
            foreach (Ship ship in opponent.OperationalShips)
            {
                HashSet<HashSet<int>> arrangements = ship.HitArrangements;

                if (arrangements.Count == 1)
                {
                    foreach (int id in arrangements.First().Where(i => opponent.Squares[i].Status == SquareStatus.Unsearched))
                    {
                        return opponent.Squares[id];
                    }
                }
            }

            // 3. A square has a hit adjacent square
            Dictionary<Square, int> probability = new ();
            HashSet<Square> adjacentSquares = new ();

            foreach (Square hitSquare in opponent.Squares.Where(i => i.Status == SquareStatus.Hit))
            {
                adjacentSquares.UnionWith(hitSquare.AdjacentSquares.Where(i => i.Status == SquareStatus.Unsearched));
            }

            if (adjacentSquares.Count == 0)
            {
                // All unsearched squares
                foreach (Square square in opponent.Squares.Where(i => i.Searched == false))
                {
                    probability.Add(square, 0);
                }
            }
            else
            {
                // All squares to search
                foreach (Square square in adjacentSquares)
                {
                    probability.Add(square, 0);
                }
            }

            // Get probability
            foreach (Ship ship in opponent.OperationalShips)
            {
                foreach (HashSet<int> arrangement in ship.HitArrangements)
                {
                    foreach (int squareID in arrangement)
                    {
                        Square sq = opponent.Squares[squareID];
                        if (probability.ContainsKey(sq))
                        {
                            probability[sq]++;
                        }
                    }
                }
            }

            return probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        /// <param name="p2">The opponent's grid..</param>
        /// <param name="shipList">The list of ships to consider.</param>
        /// <returns>The square to attack.</returns>
        public static Square Optimal(Grid p2, List<Ship> shipList)
        {
            // 1. A hit square only has 1 unsearched adjacent square
            foreach (Square square in p2.Squares)
            {
                if (square.Status != SquareStatus.Hit)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new ();

                foreach (Square adjSquare in square.AdjacentSquares)
                {
                    if (!adjSquare.Searched)
                    {
                        unsearchedSquares.Add(adjSquare);
                    }
                }

                if (unsearchedSquares.Count == 1)
                {
                    return unsearchedSquares[0];
                }
            }

            Dictionary<Square, int> probability = new ();
            HashSet<Square> adjacentSquares = new ();

            foreach (Square hitSquare in p2.Squares.Where(i => i.Status == SquareStatus.Hit))
            {
                adjacentSquares.UnionWith(hitSquare.AdjacentSquares.Where(i => i.Status == SquareStatus.Unsearched));
            }

            if (adjacentSquares.Count == 0)
            {
                // All unsearched squares
                foreach (Square square in p2.Squares.Where(i => i.Searched == false))
                {
                    probability.Add(square, 0);
                }
            }
            else
            {
                // All squares to search
                foreach (Square square in adjacentSquares)
                {
                    probability.Add(square, 0);
                }
            }

            // Get probability
            foreach (Ship ship in shipList)
            {
                foreach (HashSet<int> arrangement in ship.HitArrangements)
                {
                    foreach (int squareID in arrangement)
                    {
                        Square sq = p2.Squares[squareID];
                        if (probability.ContainsKey(sq))
                        {
                            probability[sq]++;
                        }
                    }
                }
            }

            return probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }
}
