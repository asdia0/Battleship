using System;
using System.Collections.Generic;
using System.Text;

namespace Battleships
{
    using System.Linq;
    using System.Text.RegularExpressions;
    class Game
    {
        public static string update = "";

        public static int move = 0;
        public static bool turn = true;
        // true = player, false = opponent

        public static Grid player;

        public static Grid opponent;

        public Game()
        {
            player = new Grid();
            opponent = new Grid();
        }

        public void StartGame()
        {
            SetUp();
            Attack();
            EndGame();
        }

        public static void SetUp()
        {
            Random rnd = new Random();

            // replace with rng later
            opponent.AddShip(opponent.squares[0], new Ship(opponent, "Carrier"), true);
            opponent.AddShip(opponent.squares[10], new Ship(opponent, "Battleship"), true);
            opponent.AddShip(opponent.squares[20], new Ship(opponent, "Cruiser"), true);
            opponent.AddShip(opponent.squares[30], new Ship(opponent, "Submarine"), true);
            opponent.AddShip(opponent.squares[40], new Ship(opponent, "Destroyer"), true);

            player.AddShip(player.squares[0], new Ship(player, "Carrier"), true);
            player.AddShip(player.squares[10], new Ship(player, "Battleship"), true);
            player.AddShip(player.squares[20], new Ship(player, "Cruiser"), true);
            player.AddShip(player.squares[30], new Ship(player, "Submarine"), true);
            player.AddShip(player.squares[40], new Ship(player, "Destroyer"), true);

            //while (player.ships.Count != 5)
            //{
            //    try
            //    {
            //        Console.WriteLine("Please enter the ID of the Square where you wish to place your ship on");
            //        int id = int.Parse(Console.ReadLine());
            //        Console.WriteLine("Please enter the type of ship you wish to place");
            //        string type = Console.ReadLine();
            //        Console.WriteLine("Please enter the alingment of the ship.");
            //        string al = Console.ReadLine();
            //        bool align = true;
            //        if (al == "horizontal")
            //        {
            //            align = true;
            //        }
            //        if (al == "vertical")
            //        {
            //            align = false;
            //        }
            //        player.AddShip(player.squares[id], new Ship(player, type), align);
            //    }
            //    catch
            //    {
            //    }
            //}
        }

        public static void EndGame()
        {
            if (opponent.ships.Count == 0)
            {
                Console.WriteLine("Congratulations on winning!");
            }
            else if (player.ships.Count == 0)
            {
                Console.WriteLine("F.");
            }
        }

        public static void Attack()
        {
            while (player.ships.Count > 0 && opponent.ships.Count > 0)
            {
                UpdateConsole();
                Random rnd = new Random();

                if (turn)
                {
                    bool isAvail = false;

                    int id = 0;

                    while (isAvail == false)
                    {
                        Console.WriteLine("Please select a square to attack");
                        id = int.Parse(Console.ReadLine());

                        if (!opponent.squares[id].beenSearched)
                        {
                            isAvail = true;
                        }
                    }

                    player.Search(opponent.squares[id]);

                    turn = false;
                }
                else
                {
                    int rannum = 0;

                    bool isAvail = false;
                    while (isAvail == false)
                    {
                        rannum = rnd.Next(100);
                        if (!player.squares[rannum].beenSearched)
                        {
                            isAvail = true;
                        }
                    }

                    opponent.Search(player.squares[rannum]);

                    move++;

                    turn = true;
                }
            }
        }

        public static void UpdateConsole()
        {
            string upd = update;
            update = "";

            string visual = "";

            foreach (Square sq in player.squares)
            {
                if (sq.hasShip == true)
                {
                    visual += "x";
                }
                else if (sq.beenSearched)
                {
                    visual += "o";
                }
                else
                {
                    visual += ".";
                }
            }

            string enem = "";

            foreach (Square sq in opponent.squares)
            {
                if (sq.beenSearched)
                {
                    if (sq.hadShip == true)
                    {
                        enem += "x";
                    }
                    else
                    {
                        enem += "o";
                    }
                }
                else
                {
                    enem += ".";
                }
            }

            List<string> groups = (from Match m in Regex.Matches(visual, ".{1,10}")
                                   select m.Value).ToList();

            List<string> lst = (from Match m in Regex.Matches(enem, ".{1,10}")
                                select m.Value).ToList();

            Console.Clear();
            Console.WriteLine($"Move {move}{Environment.NewLine}");
            Console.WriteLine($"You have {player.ships.Count} ships left.{Environment.NewLine}");
            Console.WriteLine($"Your opponent has {opponent.ships.Count} ships left.{Environment.NewLine}");
            Console.WriteLine(string.Join(Environment.NewLine, groups.ToArray()));
            Console.WriteLine();
            Console.WriteLine(string.Join(Environment.NewLine, lst.ToArray()));
            Console.WriteLine();
            Console.WriteLine(upd);

            foreach (Ship p in player.ships)
            {
                foreach (Square s in p.occupiedSquares)
                {
                    Console.Write($"{s.id} ");
                }
                Console.WriteLine();
            }
        }
    }
}
