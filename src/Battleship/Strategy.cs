namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Strategy
    {
        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        /// <returns>The square to attack.</returns>
        public static Square Random(Grid p2)
        {
            return p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        /// <returns>The square to attack.</returns>
        public static Square HuntTarget(Grid p1, Grid p2)
        {
            Square attackedSquare;

            if (p1.ToSearch.Count == 0)
            {
                // HUNT
                attackedSquare = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];
            }
            else
            {
                // TARGET
                attackedSquare = p1.ToSearch.ToList()[new Random().Next(p1.ToSearch.Count)];
            }

            return attackedSquare;
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        /// <returns>The square to attack.</returns>
        public static Square ProbabilityDensity(Player player, Grid p1, Grid p2)
        {
            /*
             * MUST
             * 1. A hit square only has 1 unsearched adjacent square
             * 2. A ship only has 1 possible arrangement
             * MIGHT
             * 3. A square has a hit adjacent square
             * MUST NOT
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            if (p1.ToAttack.Any())
            {
                return p1.ToAttack.First();
            }

            // 1. A hit square only has 1 unsearched adjacent square
            foreach (Square square in p2.Squares)
            {
                if (square.Status != SquareStatus.Hit)
                {
                    continue;
                }

                List<Square> unsearchedSquares = new();

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
            foreach (Ship ship in p2.OperationalShips)
            {
                HashSet<HashSet<int>> arrangements = ship.Arrangements;

                if (arrangements.Count == 1)
                {
                    foreach (int id in arrangements.First().Where(i => p2.Squares[i].Status == SquareStatus.Unsearched))
                    {
                        p1.ToAttack.Add(p2.Squares[id]);
                        return p1.ToAttack.First();
                    }
                }
            }

            // 3. A square has a hit adjacent square
            Dictionary<Square, int> probability = new();

            if (p1.ToSearch.Count == 0)
            {
                // All unsearched squares
                foreach (Square square in p2.Squares)
                {
                    {
                        if (!square.Searched)
                        {
                            probability.Add(square, 0);
                        }
                    }
                }
            }
            else
            {
                // All squares to search
                foreach (Square square in p1.ToSearch)
                {
                    probability.Add(square, 0);
                }
            }

            // Get probability
            foreach (Ship ship in p2.OperationalShips)
            {
                foreach (HashSet<int> arrangement in ship.Arrangements)
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
