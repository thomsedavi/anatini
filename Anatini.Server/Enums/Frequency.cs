namespace Anatini.Server.Enums
{
    [Flags]
    public enum Frequency
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 4,
        Yearly = 8
    }
}
