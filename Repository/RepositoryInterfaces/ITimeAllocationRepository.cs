using Repository.DataAccessLayer;
using Repository.Tables;

namespace Repository.RepositoryInterfaces
{
    public interface ITimeAllocationRepository
    {
        void Create(TimeAllocationDAL timeAlocation);
        TimeAllocationDAL? GetByDate(DateTime dateTime);
        void Update(TimeAllocationDAL timeAlocation);
    }
}
