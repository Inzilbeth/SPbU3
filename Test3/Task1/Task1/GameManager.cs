using System;
using System.Collections.Generic;
using System.Text;

namespace Task1
{
    public class GameManager
    {
        private Field field;
        private Cell currentSign;

        public GameManager()
        {
            field = new Field();
            currentSign = Cell.X;
        }

        public string GetSign()
            => currentSign.ToString();

        public void Reset()
        {
            field.Reset();
            currentSign = Cell.X;
        }

        public bool TryMakeTurn(int i, int j)
        {
            if (field.Add(i, j, currentSign))
            {
                ChangeSign();
                return true;
            }

            return false;
        }

        public Field.Status GetStatus()
            => field.GetStatus();

        private void ChangeSign()
        {
            currentSign = currentSign == Cell.X ? Cell.O : Cell.X;
        }
    }
}
