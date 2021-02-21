namespace Battleship.Core
{
    public class Move
    {
        public string player;

        public int x;

        public int y;

        public Move(string player, (int, int) tuple)
        {
            this.player = player;

            this.x = tuple.Item1;
            this.y = tuple.Item2;
        }
    }
}
