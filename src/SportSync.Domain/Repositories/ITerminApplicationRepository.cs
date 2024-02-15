using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface ITerminApplicationRepository
{
    void Insert(TerminApplication terminApplication);
}