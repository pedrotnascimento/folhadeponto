using Repository.DataAccessLayer;


namespace Repository.RepositoryInterfaces
{
    public interface ITimeMomentRepository
    {
        void Create(TimeMomentDAL timeMoment);
        IList<TimeMomentDAL> QueryByUserIdAndDate(int userId, DateTime date);
        List<TimeMomentDAL> QueryByUserIdAndMonth(int id, DateTime month);
    }
}
