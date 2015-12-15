using System;
using System.Collections.Generic;
using UniLinq;

namespace Noroshi.Grid
{
    public class GridContainer<T> where T : class, IGridContent
    {
        List<T> _contents = new List<T>();
        T[,] _container;
        public readonly ushort HorizontalSize;
        public readonly ushort VerticalSize;

        public GridContainer(ushort horizontalSize, ushort verticalSize)
        {
            HorizontalSize = horizontalSize;
            VerticalSize   = verticalSize;
            _container     = new T[HorizontalSize, VerticalSize];
        }

        public bool HasContent(ushort horizontalIndex, ushort verticalIndex)
        {
            return GetContent(horizontalIndex, verticalIndex) != null;
        }

        public bool HasContentByVerticalIndex(ushort verticalIndex)
        {
            return Enumerable.Range(0, HorizontalSize).Any(h => HasContent((ushort)h, verticalIndex));
        }

        public T GetContent(ushort horizontalIndex, ushort verticalIndex)
        {
            if (0 <= horizontalIndex && horizontalIndex < HorizontalSize)
            {
                if (0 <= verticalIndex && verticalIndex < VerticalSize)
                {
                    return _container[horizontalIndex, verticalIndex];
                }
            }
            return null;
        }

        public List<T> GetContents()
        {
            return new List<T>(_contents);
        }

        public IEnumerable<T> GetContentsByHorizontalIndexes(IEnumerable<ushort> horizontalIndexes)
        {
            var map = new Dictionary<ushort, bool>();
            foreach (var h in horizontalIndexes)
            {
                map.Add(h, true);
            }
            return _contents.Where(c => map.ContainsKey(c.GetGridPosition().Value.HorizontalIndex));
        }

        public IEnumerable<T> GetContentsByHorizontalRange(ushort horizontalIndex, Direction horizontalDirection, int minRange, int maxRange)
        {
            return GetContentsByHorizontalIndexes(GetHorizontalIndexesByRange(horizontalIndex, horizontalDirection, minRange, maxRange));
        }
        public IEnumerable<ushort> GetHorizontalIndexesByRange(ushort horizontalIndex, Direction horizontalDirection, int minRange, int maxRange)
        {
            var minIndex = 0;
            var maxIndex = 0;
            switch (horizontalDirection)
            {
                case Direction.Right:
                    minIndex = horizontalIndex + minRange;
                    maxIndex = horizontalIndex + maxRange;
                    break;
                case Direction.Left:
                    minIndex = horizontalIndex - maxRange;
                    maxIndex = horizontalIndex - minRange;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (minIndex < 0) minIndex = 0;
            if (HorizontalSize - 1 < maxIndex) maxIndex = HorizontalSize - 1;
            var count = maxIndex - minIndex + 1;
            return Enumerable.Range(minIndex, count).Select(i => (ushort)i);
        }

        public int GetHorizontalDistance(T content1, T content2)
        {
            return Math.Abs(content1.GetGridPosition().Value.HorizontalIndex - content2.GetGridPosition().Value.HorizontalIndex);
        }
        public Direction? GetHorizontalDirection(T fromContent, T toCotent)
        {
            var diff = toCotent.GetGridPosition().Value.HorizontalIndex - fromContent.GetGridPosition().Value.HorizontalIndex;
            return diff > 0 ? (Direction?)Direction.Right : diff < 0 ? (Direction?)Direction.Left : null;
        }

        public void SetContent(ushort horizontalIndex, ushort verticalIndex, T content)
        {
            if (HasContent(horizontalIndex, verticalIndex))
            {
                throw new InvalidOperationException();
            }
            if (0 <= horizontalIndex && horizontalIndex < HorizontalSize)
            {
                if (0 <= verticalIndex && verticalIndex < VerticalSize)
                {
                    _setContent(new GridPosition(horizontalIndex, verticalIndex), content);
                    return;
                }
            }
            throw new ArgumentOutOfRangeException();
        }
        void _setContent(GridPosition grid, T content)
        {
            _container[grid.HorizontalIndex, grid.VerticalIndex] = content;
            content.SetGridPosition(grid);
            _contents.Add(content);
        }

        public void MoveTo(T content, Direction direction)
        {
            var nextGrid = content.GetGridPosition().Value.BuildNextGrid(direction);
            MoveTo(content, nextGrid.HorizontalIndex, nextGrid.VerticalIndex);
        }
        public void MoveTo(T content, ushort horizontalIndex, ushort verticalIndex)
        {
            var prevGrid = content.GetGridPosition().Value;
            _moveContent(prevGrid.HorizontalIndex, prevGrid.VerticalIndex, horizontalIndex, verticalIndex);
        }
        T _moveContent(ushort fromHorizontalIndex, ushort fromVerticalIndex, ushort toHorizontalIndex, ushort toVerticalIndex)
        {
            var element = RemoveContent(fromHorizontalIndex, fromVerticalIndex);
            SetContent(toHorizontalIndex, toVerticalIndex, element);
            return element;
        }

        public T RemoveContent(ushort horizontalIndex, ushort verticalIndex)
        {
            var content = GetContent(horizontalIndex, verticalIndex);
            if (content == null)
            {
                throw new InvalidOperationException();
            }
            if (!RemoveContent(content))
            {
                throw new InvalidOperationException();
            }
            return content;
        }
        public bool RemoveContent(T content)
        {
            var grid = content.GetGridPosition().Value;
            _container[grid.HorizontalIndex, grid.VerticalIndex] = null;
            content.RemoveGridPosition();
            _contents.Remove(content);
            return true;
        }
        public List<T> RemoveAllContents()
        {
            var contents = GetContents();
            foreach (var content in contents)
            {
                RemoveContent(content);
            }
            return contents;
        }
    }
}
