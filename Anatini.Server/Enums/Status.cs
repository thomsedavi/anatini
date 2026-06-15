namespace Anatini.Server.Enums
{
    [Flags]
    public enum Status
    {
        None = 0,
        Published = 1,
        Draft = 2,
        Archived = 4
    }
}
