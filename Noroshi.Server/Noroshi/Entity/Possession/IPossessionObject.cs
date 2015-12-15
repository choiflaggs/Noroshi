namespace Noroshi.Server.Entity.Possession
{
    public interface IPossessionObject : IPossessionParam
    {
        uint PossessingNum { get; }
        Core.WebApi.Response.Possession.PossessionObject ToResponseData();
    }
}
