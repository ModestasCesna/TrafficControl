
namespace TrafficControl.Service
{
    public interface IFileService
    {
        Task<string> GetPath(Guid id);
        Task<Guid> Store(string fileName, Stream file);
    }
}