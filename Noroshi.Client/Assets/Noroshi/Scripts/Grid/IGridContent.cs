namespace Noroshi.Grid
{
    public interface IGridContent
    {
        GridPosition? GetGridPosition();
        void SetGridPosition(GridPosition gridPosition);
        void RemoveGridPosition();
    }
}
