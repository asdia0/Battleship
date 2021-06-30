namespace Battleship.Core
{
    public class Player
    {
        private bool GridSet = false;

        private bool NameSet = false;

        private bool GameSet = false;

        private Grid _Grid;

        private string _Name;

        private Game _Game;

        public Grid Grid
        {
            get
            {
                return this._Grid;
            }

            set
            {
                if (!this.GridSet)
                {
                    this._Grid = value;
                }
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                if (!this.NameSet)
                {
                    this._Name = value;
                }
            }
        }

        public Game Game
        {
            get
            {
                return this._Game;
            }

            set
            {
                if (!this.GameSet)
                {
                    this._Game = value;
                }
            }
        }
    }
}
