namespace Battleship.Core
{
    using System;

    /// <summary>
    /// Defines an exception thrown in this project.
    /// </summary>
    public class BattleshipException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattleshipException"/> class.
        /// </summary>
        public BattleshipException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleshipException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        public BattleshipException(string message)
            : base(message)
        { }
    }
}
