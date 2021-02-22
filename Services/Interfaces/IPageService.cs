using Services.Classes;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPageService
    {
        Task<Page> GePage(string pageContent, QueryParams queryParams);
    }
}
