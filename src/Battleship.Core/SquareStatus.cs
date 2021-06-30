namespace Battleship.Core
{
    /// <summary>
    /// Types of <see cref="Square"/> statuses.
    /// </summary>
    public enum SquareStatus
    {
        /// <summary>
        /// Square is unsearched (<see cref="Square.Searched"/> = false).
        /// </summary>
        Unsearched,

        /// <summary>
        /// Square is a miss (<see cref="Square.Searched"/> = true AND <see cref="Square.Ship"/> = null).
        /// </summary>
        Miss,

        /// <summary>
        /// Square was hit (<see cref="Square.Searched"/> = true AND <see cref="Square.Ship"/> != null).
        /// </summary>
        Hit,

        /// <summary>
        /// Square's ship was sunk (<see cref="Square.Searched"/> = true AND <see cref="Square.Ship"/>.Sunk == true).
        /// </summary>
        Sunk,
    }
}
