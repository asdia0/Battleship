namespace Battleship.Core
{
    /// <summary>
    /// Defines a coordinate.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// Gets the X-coordinate.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y-coordinate.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="x">The x-coodinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Converts the <see cref="Position"/> into a <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="Position"/> as a <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"({this.X}, {this.Y})";
        }
    }
}
