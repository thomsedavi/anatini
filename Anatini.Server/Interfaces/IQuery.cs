namespace Anatini.Server.Interfaces
{
    internal interface IQuery<T>
    {
        Task<T> ExecuteAsync();
    }
}
