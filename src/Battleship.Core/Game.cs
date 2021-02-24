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
        /// List of moves made.
        /// </summary>
        public List<Move> MoveList = new List<Move>();

        /// <summary>
        /// The winner of the game.
        /// </summary>
        public bool Winner;

        /// <summary>
        /// Number of moves.
        /// </summary>
        public int Move;

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
            }

            this.EndGame();
        }

        public void CreateGame(int algo1, int algo2)
        {
            List<string> Algorithms = new List<string>()
            {
                "Random",
                "Hunt Target",
                "Probability Density",
            };

            this.StartGame();

            while (this.player1.Ships.Count > 0 && this.player2.Ships.Count > 0)
            {
                if (this.turn)
                {
                    switch (algo1)
                    {
                        case 0:
                            this.Random();
                            break;
                        case 1:
                            this.HuntTarget();
                            break;
                        case 2:
                            this.ProbabilityDensity();
                            break;
                    }
                }
                else
                {
                    switch (algo2)
                    {
                        case 0:
                            this.Random();
                            break;
                        case 1:
                            this.HuntTarget();
                            break;
                        case 2:
                            this.ProbabilityDensity();
                            break;
                    }
                }
            }

            this.EndGame();
        }

        /// <summary>
        /// Converts a <see cref="Game"/> to a <see cref="string"/>.
        /// </summary>
        /// <returns>The stringified version of the game.</returns>
        public override string ToString()
        {
            string res = $"[Player 1 \"{this.player1}\"]\n[Player 2 \"{this.player2}\"]\n\n";

            foreach (Move move in this.MoveList)
            {
                res += $"{move.Player}: ({move.X},{move.Y})\n";
            }

            return res;
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        private void StartGame()
        {
            List<int> shipTypes = new List<int>()
            {
                5,
                4,
                3,
                3,
                2,
            };

            foreach (int length in shipTypes)
            {
                Random rnd = new Random();

                while (true)
                {
                    bool horizontal = false;

                    if (rnd.Next(2) == 0)
                    {
                        horizontal = true;
                    }

                    if (this.player1.AddShip(this.player1.UnoccupiedSquares[rnd.Next(this.player1.UnoccupiedSquares.Count)], new Ship(this.player1, length, 1), horizontal))
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

                    if (this.player2.AddShip(this.player2.UnoccupiedSquares[rnd.Next(this.player2.UnoccupiedSquares.Count)], new Ship(this.player2, length, 1), horizontal))
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
            string playername;
            Grid p1;
            Grid p2;

            this.Move++;

            if (this.turn)
            {
                playername = "Player 1";
                p1 = this.player1;
                p2 = this.player2;
            }
            else
            {
                playername = "Player 2";
                p1 = this.player2;
                p2 = this.player1;
            }

            Square attackedSq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

            this.Search(p1, p2, attackedSq);

            this.MoveList.Add(new Move(playername, attackedSq.ToCoor()));

            if (attackedSq.HadShip == false)
            {
                this.turn ^= true;
            }
        }

        /// <summary>
        /// Attacks an enmy square that is adjacent to a hit square. Implements parity.
        /// </summary>
        private void HuntTarget()
        {
            string playername;
            Grid p1;
            Grid p2;

            this.Move++;

            if (this.turn)
            {
                playername = "Player 1";
                p1 = this.player1;
                p2 = this.player2;
            }
            else
            {
                playername = "Player 2";
                p1 = this.player2;
                p2 = this.player1;
            }

            // HUNT
            if (p1.ToSearch.Count == 0)
            {
                Square attackedSq = p2.UnsearchedSquares[new Random().Next(p2.UnsearchedSquares.Count)];

                this.Search(p1, p2, attackedSq);

                this.MoveList.Add(new Move(playername, attackedSq.ToCoor()));

                if (attackedSq.HadShip == false)
                {
                    this.turn ^= true;
                }
            }

            // TARGET
            else
            {
                Square attackedSq = p1.ToSearch.First();

                this.Search(p1, p2, attackedSq);

                this.MoveList.Add(new Move(playername, attackedSq.ToCoor()));

                if (attackedSq.HadShip == false)
                {
                    this.turn ^= true;
                }
            }
        }

        /// <summary>
        /// Attacks an enemy square based on previous searches. Searches for all enemy ships at the same time.
        /// </summary>
        private void ProbabilityDensity()
        {
            string playername;
            Square attackedSq;
            Grid p1;
            Grid p2;

            if (this.turn)
            {
                p1 = this.player1;
                p2 = this.player2;
                playername = "Player 1";

                this.Move++;
            }
            else
            {
                playername = "Player 2";
                p1 = this.player2;
                p2 = this.player1;
            }

            /*
             * DEFINITE
             * 1. If a 'hit' square is surrounded by misses in 3 directions, there must be a ship pointing in the fourth direction.
             * 2. If a certain spot on the board could only hold one certain ship, no other ships can cross any of those squares.

             * DISQUALIFY
             * 3. Misses and sunk ships are obstructions
             * 4. Ships that are not sunk can't be located entirely on 'hit' squares.
             */

            foreach (Square sq in p2.Squares)
            {
                if (sq.BeenSearched && sq.HadShip == true && sq.IsSunk != true)
                {
                    List<Square> potentialSqs = new List<Square>();

                    foreach (Square square in sq.GetAdjacentSquares())
                    {
                        // is a valid square if it has not been searched
                        if (!sq.BeenSearched)
                        {
                            potentialSqs.Add(sq);
                        }
                    }

                    if (potentialSqs.Count == 1)
                    {
                        p1.ToAttack.Add(potentialSqs.First());
                    }
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
                attackedSq = Program.HighestHCS(p1.ToAttack.ToList()).First();
            }
            else
            {
                Dictionary<int, double> probability = new Dictionary<int, double>();

                // domain: unsearched squares
                if (!p1.ToSearch.Any())
                {
                    List<Square> l = Program.HighestHCS(p2.UnsearchedSquares);

                    foreach (Square sq in l)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in p2.Ships)
                    {
                        List<List<int>> arrL = ship.GetArrangements();
                        foreach (List<int> arr in arrL)
                        {
                            foreach (int sqID in arr)
                            {
                                if (l.Contains(p2.Squares[sqID]))
                                {
                                    probability[sqID] += (double)decimal.Divide(1, arrL.Count);
                                }
                            }
                        }
                    }
                }

                // domain: squares adjacent to hit squares
                else
                {
                    List<Square> l = Program.HighestHCS(p1.ToSearch.ToList());

                    foreach (Square sq in l)
                    {
                        probability.Add(sq.ID, 0);
                    }

                    foreach (Ship ship in p2.Ships)
                    {
                        List<List<int>> arrL = ship.GetArrangements();
                        foreach (List<int> arr in arrL)
                        {
                            foreach (int sqID in arr)
                            {
                                if (l.Contains(p2.Squares[sqID]))
                                {
                                    probability[sqID]++;
                                }
                            }
                        }
                    }
                }

                double[] source = probability.Values.ToArray();
                int i = 0;
                double[][] result = source.GroupBy(s => i++ / 10).Select(g => g.ToArray()).ToArray();

                attackedSq = p2.Squares[(int)probability.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
            }

            this.Search(p1, p2, attackedSq);

            this.MoveList.Add(new Move(playername, attackedSq.ToCoor()));

            if (attackedSq.HadShip != true)
            {
                this.turn ^= true;
            }
        }

        private string PrintProbability(Dictionary<int, int> probability, int chunkSize)
        {
            int[] source = probability.Values.ToArray();
            int i = 0;
            int[][] result = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();

            return System.Text.Json.JsonSerializer.Serialize(result).Replace("],[", "],\n [");
        }

        private string PrintLists(Grid player, string playerName)
        {
            string text = "\n  Attack: ";
            foreach (Square sq in player.ToAttack)
            {
                text += $"{sq.ID} ";
            }

            text += "\n  Search: ";
            foreach (Square sq in player.ToSearch)
            {
                text += $"{sq.ID} ";
            }

            return $"{playerName} Move {this.Move}: {text}";
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
                foreach (Square sq in p2.Squares[squareID].GetAdjacentSquares())
                {
                    if (!sq.BeenSearched && !p1.ToSearch.Contains(sq))
                    {
                        p1.ToSearch.Add(sq);
                    }
                }
            }
        }

        /// <summary>
        /// Searches a square.
        /// </summary>
        /// <param name="p1">Player 1's grid.</param>
        /// <param name="p2">Player 2's grid.</param>
        /// <param name="sq">The square to search.</param>
        private void Search(Grid p1, Grid p2, Square sq)
        {
            if (sq.BeenSearched)
            {
                throw new Exception($"Square {sq.ID} has already been searched.");
            }

            sq.BeenSearched = true;
            p2.UnsearchedSquares.Remove(sq);
            p1.ToAttack.Remove(sq);
            p1.ToSearch.Remove(sq);

            if (sq.HasShip == true)
            {
                this.AddTargets(p1, p2, sq.ID);

                sq.HasShip = false;
                sq.IsHit = true;

                foreach (Ship sp in sq.Grid.Ships.ToList())
                {
                    if (sp.CurrentOccupiedSquares.Contains(sq))
                    {
                        sp.CurrentOccupiedSquares.Remove(sq);

                        if (sp.CurrentOccupiedSquares.Count == 0)
                        {
                            p2.Ships.Remove(sp);

                            sp.IsSunk = true;

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

        private void OnlyOneArrangement(Grid p1, Grid p2)
        {
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
        }
    }
}
