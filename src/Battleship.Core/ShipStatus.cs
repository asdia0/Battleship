namespace Battleship.Core
{
    /// <summary>
    /// Types of <see cref="Ship"/> statuses.
    /// </summary>
    public enum ShipStatus
    {
        /// <summary>
        /// Ship is operational (<see cref="Ship.UnsearchedSquares"/> > 0).
        /// </summary>
        Operational,

        /// <summary>
        /// Ship has been sunk (<see cref="Ship.UnsearchedSquares"/> = 0).
        /// </summary>
        Sunk,
    }
}
