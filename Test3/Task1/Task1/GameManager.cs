using System;
using System.Collections.Generic;
using System.Text;

namespace Task1
{
    /// <summary>
    /// Game manager that controls the gameplay.
    /// </summary>
    public class GameManager
    {
        private Field field;
        private Cell currentSign;

        public GameManager()
        {
            field = new Field();
            currentSign = Cell.X;
        }

        /// <summary>
        /// Return current sign as a string.
        /// </summary>
        public string GetSign()
            => currentSign.ToString();

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void Reset()
        {
            field.Reset();
            currentSign = Cell.X;
        }

        /// <summary>
        /// Tries to make turn for the specified coordinates.
        /// </summary>
        public bool TryMakeTurn(int i, int j)
        {
            if (field.Add(i, j, currentSign))
            {
                ChangeSign();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns gae status which is same as field's.
        /// </summary>
        public Field.Status GetStatus()
            => field.GetStatus();

        /// <summary>
        /// Changes sign from cross to zero and in reverse.
        /// </summary>
        private void ChangeSign()
        {
            currentSign = currentSign == Cell.X ? Cell.O : Cell.X;
        }
    }
}
