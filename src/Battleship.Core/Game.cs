namespace Battleship.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a game of Battleship.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The winner of the game.
        /// </summary>
        public bool Winner;

        /// <summary>
        /// Number of moves.
        /// </summary>
        public int Move = 0;

        /// <summary>
        /// Player 1's grid.
        /// </summary>
        private Grid player1;

        /// <summary>
        /// Player 2's grid.
        /// </summary>
        private Grid player2;

        private bool turn = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.player1 = new Grid();
            this.player2 = new Grid();
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void CreateGame()
        {
            this.StartGame();

            while (this.player1.Ships.Count > 0 && this.player2.Ships.Count > 0)
            {
                this.ProbabilityDensity();

                //if (this.turn)
                //{
                //    this.ProbabilityDensity();
                //}
                //else
                //{
                //    this.Random();
                //}
            }

            this.EndGame();
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        private void StartGame()
        {
            List<string> shipTypes = new List<string>()
            {
                "Carrier",
                "Battleship",
                "Cruiser",
                "Submarine",
                "Destroyer",
            };

            foreach (string type in shipTypes)
            {
                Random rnd = new Random();

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    if (this.player1.AddShip(this.player1.UnoccupiedSquares[rnd.Next(this.player1.UnoccupiedSquares.Count)], new Ship(this.player1, type), horizontal))
                    {
                        break;
                    }
                }

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    if (this.player2.AddShip(this.player2.UnoccupiedSquares[rnd.Next(this.player2.UnoccupiedSquares.Count)], new Ship(this.player2, type), horizontal))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Ends the game.
        /// </summary>
        private void EndGame()
        {
            if (this.player2.Ships.Count == 0)
            {
                this.Winner = true;
            }
            else if (this.player1.Ships.Count == 0)
            {
                this.Winner = false;
            }
        }

        /// <summary>
        /// Attacks an enemy square randomly.
        /// </summary>
        private void Random()
        {
            Grid p2;

            if (this.turn)
            {
                p2 = this.player2;

                this.turn = false;
            }
            else
            {
                p2 = this.player1;

                this.Move++;
                this.turn = true;
            }

            this.Search(p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)]);
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        private void HuntTarget()
        {
            Grid p1;
            Grid p2;

            if (this.turn)
            {
                p1 = this.player1;
                p2 = this.player2;

                this.turn = false;
            }
            else
            {
                p1 = this.player2;
                p2 = this.player1;

                this.Move++;
                this.turn = true;
            }

            // HUNT
            if (p1.ToSearch.Count == 0)
            {
                Square sq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

                this.Search(sq);

                this.AddTargets(p1, p2, sq.ID);
            }

            // TARGET
            else
            {
                this.Search(p1.ToSearch[0]);

                this.AddTargets(p1, p2, p1.ToSearch[0].ID);

                p1.ToSearch.Remove(p1.ToSearch[0]);
            }
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        private void ProbabilityDensity()
        {
            Square attackedSq;
            Grid p1;
            Grid p2;

            if (this.turn)
            {
                p1 = this.player1;
                p2 = this.player2;

                this.turn = false;
            }
            else
            {
                p1 = this.player2;
                p2 = this.player1;

                this.Move++;
                this.turn = true;
            }

            /*
             * DEFINITE
             * 1. If a 'hit' square is surrounded by misses in 3 directions, there must be a ship pointing in the fourth direction.
             * 2. If a certain spot on the board could only hold one certain ship, no other ships can cross any of those squares.

             * DISQUALIFY
             * 3. Misses and sunk ships are obstructions
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            // 1.
            foreach (Square sq in p2.Squares)
            {
                if (sq.BeenSearched && sq.HadShip == true && sq.IsSunk != true)
                {
                    this.TooManyMisses(p1, sq);
                }
            }

            // 2.
            foreach (Ship ship in p2.Ships)
            {
                List<List<int>> arrL = ship.GetArrangements();

                if (arrL.Count == 1)
                {
                    foreach (int sqID in arrL.First())
                    {
                        Square sq = p2.Squares[sqID];

                        if (!sq.BeenSearched && !p1.ToAttack.Contains(sq))
                        {
                            p1.ToAttack.Add(sq);
                        }
                    }
                }
            }

            if (p1.ToAttack.Any())
            {
                attackedSq = p1.ToAttack.First();
            }
            else
            {
                Dictionary<int, int> probability = new Dictionary<int, int>();

                if (!p1.ToSearch.Any())
                {
                    // domain: p2.UnsearchedSquares

                    foreach (Square sq in p2.Squares)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in p2.Ships)
                    {
                        foreach (List<int> arr in ship.GetArrangements())
                        {
                            foreach (int sqID in arr)
                            {
                                if (p2.UnsearchedSquares.Contains(p2.Squares[sqID]))
                                {
                                    probability[sqID]++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // domain: p1.toSearch

                    foreach (Square sq in p2.Squares)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in p2.Ships)
                    {
                        foreach (List<int> arr in ship.GetArrangements())
                        {
                            foreach (int sqID in arr)
                            {
                                if (p1.ToSearch.Contains(p2.Squares[sqID]))
                                {
                                    probability[sqID]++;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(this.PrintProbability(probability, 10));

                attackedSq = p2.Squares[probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
            }

            this.Search(attackedSq);

            this.AddTargets(p1, p2, attackedSq.ID);

            p1.ToAttack.Remove(attackedSq);
        }

        private string PrintProbability(Dictionary<int, int> probability, int chunkSize)
        {
            int[] source = probability.Values.ToArray();
            int i = 0;
            int[][] result = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();

            return System.Text.Json.JsonSerializer.Serialize(result).Replace("],[", "],\n [");
        }

        /// <summary>
        /// Adds squares to the target list.
        /// </summary>
        /// <param name="p1">Player 1.</param>
        /// <param name="p2">Player 2.</param>
        /// <param name="squareID">Square to check's ID.</param>
        private void AddTargets(Grid p1, Grid p2, int squareID)
        {
            if (p2.Squares[squareID].BeenSearched && p2.Squares[squareID].HadShip == true && p2.Squares[squareID].IsSunk != true)
            {
                List<int> sqID = new List<int>()
                {
                    squareID - 1,
                    squareID + 1,
                    squareID - Settings.GridWidth,
                    squareID + Settings.GridWidth,
                };

                foreach (int id in sqID)
                {
                    if (id > -1 && id < (Settings.GridHeight * Settings.GridWidth))
                    {
                        Square sq = p2.Squares[id];

                        if (!sq.BeenSearched && !p1.ToSearch.Contains(sq))
                        {
                            p1.ToSearch.Add(sq);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="sq">The square to search.</param>
        private void Search(Square sq)
        {
            if (sq.BeenSearched)
            {
                throw new Exception($"Square {sq.ID} has already been searched.");
            }

            sq.BeenSearched = true;
            sq.Grid.UnsearchedSquares.Remove(sq);

            if (sq.HasShip == true)
            {
                sq.HasShip = false;
                sq.IsHit = true;

                foreach (Ship sp in sq.Grid.Ships.ToList())
                {
                    if (sp.CurrentOccupiedSquares.Contains(sq))
                    {
                        sp.CurrentOccupiedSquares.Remove(sq);

                        if (sp.CurrentOccupiedSquares.Count == 0)
                        {
                            sq.Grid.Ships.Remove(sp);

                            foreach (Square square in sp.OriginalOccupiedSquares)
                            {
                                square.IsSunk = true;
                            }
                        }
                    }
                }
            }
            else
            {
                sq.IsMiss = true;
            }
        }

        private void TooManyMisses(Grid p1, Square sq)
        {
            if (sq.HadShip == true && sq.IsSunk != true)
            {
                List<int> sqID = new List<int>()
                {
                    sq.ID - 1,
                    sq.ID + 1,
                    sq.ID - Settings.GridWidth,
                    sq.ID + Settings.GridWidth,
                };

                List<Square> potentialSqs = new List<Square>();


                foreach (int id in sqID)
                {
                    if (id > -1 && id < (Settings.GridHeight * Settings.GridWidth))
                    {
                        // is a valid square if it has not been searched
                        if (!sq.Grid.Squares[id].BeenSearched)
                        {
                            potentialSqs.Add(sq.Grid.Squares[id]);
                        }
                    }
                }


                if (potentialSqs.Count == 1)
                {
                    p1.ToAttack.Add(potentialSqs.First());
                }
            }
        }
    }
}
