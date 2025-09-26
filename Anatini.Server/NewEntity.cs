namespace Anatini.Server
{
    public abstract class NewEntity
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
    }
}
