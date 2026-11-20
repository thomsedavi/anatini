namespace Anatini.Server.Enums
{
    [Flags]
    public enum Status
    {
        None = 0,
        Draft = 1,
        Published = 2,
        Archived = 4
    }
}
