namespace HomeCinema.Entities
{
    /// <summary>
    /// Interface is used to mark entities as system defaults.
    /// </summary>
    public interface ISystemDefault
    {
        bool IsSystemDefault { get; set; }
    }
}