namespace Battleship.Core
{
    /// <summary>
    /// Types of statuses for a <see cref="Square"/>.
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
    }
}
