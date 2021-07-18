namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class contains methods for ship layouts.
    /// </summary>
    public static class Layout
    {
        /// <summary>
        /// Gets the optimal layout.
        /// </summary>
        /// <param name="length">The length of the grid.</param>
        /// <param name="breadth">The breadth of the grid.</param>
        /// <param name="shipList">The list of ships to clone.</param>
        /// <returns>A grid with the optimal layout.</returns>
        public static Grid Optimal(int length, int breadth, List<Ship> shipList)
        {
            Grid res = new (length, breadth);

            foreach (Ship ship in shipList)
            {
                res.Ships.Add(new Ship(res, ship.Length));
            }

            shipList = res.Ships.ToList();

            while (shipList.Any())
            {
                foreach (Ship ship in shipList.ToList())
                {
                    HashSet<HashSet<int>> arrs = ship.NoHitArrangements;

                    HashSet<int> intersection = arrs
                           .Skip(1)
                           .Aggregate(
                               new HashSet<int>(arrs.First()),
                               (h, e) => { h.IntersectWith(e); return h; });

                    int numActualArrs = intersection.Any() ? 1 : 0;

                    if (numActualArrs == 0)
                    {
                        HashSet<HashSet<int>> arrCopy = arrs.ToHashSet();

                        foreach (HashSet<int> arr1 in arrCopy)
                        {
                            foreach (HashSet<int> arr2 in arrCopy)
                            {
                                if (!arrCopy.Contains(arr2))
                                {
                                    continue;
                                }

                                if (arr1 == arr2)
                                {
                                    continue;
                                }

                                if (arr1.Intersect(arr2).Any())
                                {
                                    numActualArrs++;
                                    arrCopy.Remove(arr2);
                                }
                            }

                            if (!arrCopy.Contains(arr1))
                            {
                                continue;
                            }
                        }
                    }

                    if (numActualArrs <= shipList.Where(i => i.Length == ship.Length).Count())
                    {
                        HashSet<int> arr = ship.NoHitArrangements.Last();

                        HashSet<Square> shipSquares = arr.Select(i => res.Squares[i]).ToHashSet();

                        ship.Squares = shipSquares;

                        foreach (Square shipSq in shipSquares)
                        {
                            shipSq.Ship = ship;
                            shipList.Remove(ship);
                        }
                    }
                }

                Strategy.Optimal(res, shipList).Searched = true;
            }

            return res;
        }

        /// <summary>
        /// Get a random layout.
        /// </summary>
        /// <param name="length">The length of the grid.</param>
        /// <param name="breadth">The breadth of the grid.</param>
        /// <param name="shipList">The list of ships to clone.</param>
        /// <returns>A grid with a random layout.</returns>
        public static Grid Random(int length, int breadth, List<Ship> shipList)
        {
            Grid res = new (length, breadth);

            Random rnd = new ();

            int counter = 0;

            while (res.Ships.Count != shipList.Count)
            {
                if (counter >= shipList.Count)
                {
                    foreach (Ship localShip in res.Ships)
                    {
                        foreach (Square localSq in localShip.Squares)
                        {
                            int id = localSq.ID;
                            res.Squares[id] = null;
                            res.Squares[id] = new Square(res, id);
                        }
                    }

                    res.Ships.Clear();
                    counter = 0;
                }

                Ship ship = shipList[counter];

                counter++;
                bool horizontal = false;

                if (rnd.Next(2) == 0)
                {
                    horizontal = true;
                }

                Ship addShip = new (res, ship.Length);

                if (res.AddShip(res.UnoccupiedSquares[rnd.Next(res.UnoccupiedSquares.Count)], addShip, horizontal ? Alignment.Horizontal : Alignment.Vertical))
                {
                    continue;
                }
            }

            return res;
        }
    }
}
