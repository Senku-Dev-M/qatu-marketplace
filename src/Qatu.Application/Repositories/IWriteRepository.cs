namespace Qatu.Application;

public interface IWriteRepository
{
    void Add<T>(T entity) where T : class;
}
