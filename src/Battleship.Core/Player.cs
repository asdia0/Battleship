namespace Battleship.Core
{
    /// <summary>
    /// Defines a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Determines whether <see cref="Grid"/> has been set.
        /// </summary>
        private bool GridSet = false;

        /// <summary>
        /// Determines whether <see cref="Name"/> has been set.
        /// </summary>
        private bool NameSet = false;

        /// <summary>
        /// <see cref="Grid"/>'s value.
        /// </summary>
        private Grid _Grid;

        /// <summary>
        /// <see cref="Name"/>'s value.
        /// </summary>
        private string _Name;

        /// <summary>
        /// Gets or sets the player's grid.
        /// </summary>
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
                else
                {
                    throw new BattleshipException("Grid has already been set.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
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
                else
                {
                    throw new BattleshipException("Name has already been set.");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="grid">The player's grid.</param>
        public Player(string name, Grid grid)
        {
            this.Name = name;
            this.Grid = grid;
        }
    }
}
