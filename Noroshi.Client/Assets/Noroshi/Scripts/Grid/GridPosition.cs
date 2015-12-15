using System;

namespace Noroshi.Grid
{	
    public struct GridPosition
    {
        public readonly ushort HorizontalIndex;
        public readonly ushort VerticalIndex;

        public GridPosition(ushort horizontalIndex, ushort verticalIndex)
        {
            HorizontalIndex = horizontalIndex;
            VerticalIndex   = verticalIndex;
        }

        public GridPosition BuildNextGrid(Direction direction, int range = 1)
        {
            var hdiff = (direction == Direction.Right) ? range : (direction == Direction.Left) ? -range : 0;
            var vdiff = (direction == Direction.Up   ) ? range : (direction == Direction.Down) ? -range : 0;
            return new GridPosition((ushort)(HorizontalIndex + hdiff), (ushort)(VerticalIndex + vdiff));
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", HorizontalIndex, VerticalIndex);
        }
    }
}
