namespace Battleship
{
    /// <summary>
    /// Types of Battleship strategies.
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// Moves are made at random.
        /// </summary>
        Random,

        /// <summary>
        /// If there are no hit squares, moves are made at random. Else, a random square adjacent to a hit square will be searched.
        /// </summary>
        HuntTarget,

        /// <summary>
        /// Goes through all possible ship arrangments and finds the square with the highest proability of having a ship on it.
        /// </summary>
        ProbabilityDensity,
    }
}
