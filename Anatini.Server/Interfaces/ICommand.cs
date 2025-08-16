namespace Anatini.Server.Interfaces
{
    internal interface ICommand<T>
    {
        Task<T> ExecuteAsync();
    }
}
