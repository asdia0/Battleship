namespace Battleship
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Layout
    {
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
                            (h, e) => { h.IntersectWith(e); return h; }
                        );

                    int numActualArrs = intersection.Any() ? 1 : 0;

                    if (numActualArrs == 0)
                    {

                        foreach (HashSet<int> arr1 in arrs.ToList())
                        {
                            foreach (HashSet<int> arr2 in arrs.ToList())
                            {
                                if (arr1 == arr2)
                                {
                                    continue;
                                }

                                if (arr1.Intersect(arr2).Any())
                                {
                                    numActualArrs++;
                                    arrs.Remove(arr1);
                                    arrs.Remove(arr2);
                                }
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

                (Square square, decimal prob) = Strategy.ProbabilityDensity(res, shipList);
                square.Searched = true;
            }

            return res;
        }

        public static void Random()
        {

        }
    }
}
