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
        /// Determines whether <see cref="Game"/> has been set.
        /// </summary>
        private bool GameSet = false;

        /// <summary>
        /// <see cref="Grid"/>'s value.
        /// </summary>
        private Grid _Grid;

        /// <summary>
        /// <see cref="Name"/>'s value.
        /// </summary>
        private string _Name;

        /// <summary>
        /// <see cref="Game"/>'s value.
        /// </summary>
        private Game _Game;

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
        /// Gets or sets the game the player is in.
        /// </summary>
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
                else
                {
                    throw new BattleshipException("Game has already been set.");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="grid">The player's grid.</param>
        /// <param name="game">The game the player is in.</param>
        public Player(string name, Grid grid, Game game)
        {
            this.Name = name;
            this.Grid = grid;
            this.Game = game;
        }
    }
}
