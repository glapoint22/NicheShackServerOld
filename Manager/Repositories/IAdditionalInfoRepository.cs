using DataAccess.Interfaces;
using DataAccess.Repositories;
using Services.Classes;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IAdditionalInfoRepository<T> : IRepository<T> where T : class, IAdditionalInfo
    {
        Task Update(AdditionalInfoViewModel additionalInfoViewModel);

        Task<int> Post(T additionalInfo);

        Task Delete(int id);
    }
}
