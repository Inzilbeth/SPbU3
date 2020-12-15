using System;
using System.Collections.Generic;
using System.Text;

namespace Task1
{
    public class Field
    {
        public enum Status
        {
            InProgress,
            CrossWon,
            ZeroWon,
            Draw
        }

        private Cell[,] array;
        private Status status;

        public Field()
        {
            array = new Cell[3, 3];
            Reset();
        }

        public Status GetStatus()
            => status;

        public void Reset()
        {
            status = Status.InProgress;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    array[i, j] = Cell.Empty;
                }
            }
        }

        public bool Add(int i, int j, Cell type)
        {
            if (type == Cell.Empty ||
                array[i, j] != Cell.Empty ||
                !IsOnField(i, j) ||
                status != Status.InProgress)
            {
                return false;
            }
            else
            {
                array[i, j] = type;
            }

            UpdateStatus();
            return true;
        }

        private bool IsOnField(int i, int j)
            => i >= 0 && i < 3 && j >= 0 && j < 3;

        private void UpdateStatus()
        {
            for (int i = 0; i < 3; i++)
            {
                if (array[i, 0] != Cell.Empty && array[i, 0] == array[i, 1] && array[i, 1] == array[i, 2])
                {
                    status = array[i, 0] == Cell.X ? Status.CrossWon : Status.ZeroWon;
                    return;
                }

                if (array[0, i] != Cell.Empty && array[0, i] == array[1, i] && array[1, i] == array[2, i])
                {
                    status = array[0, i] == Cell.X ? Status.CrossWon : Status.ZeroWon;
                    return;
                }
            }

            if (array[0, 0] != Cell.Empty && array[0, 0] == array[1, 1] && array[1, 1] == array[2, 2])
            {
                status = array[0, 0] == Cell.X ? Status.CrossWon : Status.ZeroWon;
                return;
            }

            if (array[2, 0] != Cell.Empty && array[2, 0] == array[1, 1] && array[1, 1] == array[0, 2])
            {
                status = array[2, 0] == Cell.X ? Status.CrossWon : Status.ZeroWon;
                return;
            }

            int count = 0;
            foreach (var cell in array)
            {
                if (cell != Cell.Empty)
                {
                    count++;
                }
            }

            if (count == 9)
            {
                status = Status.Draw;
                return;
            }

            status = Status.InProgress;
        }
    }
}
