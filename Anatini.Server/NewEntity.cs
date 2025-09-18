namespace Anatini.Server
{
    public abstract class NewEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
