using Services.Classes;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPageService
    {
        Task<PageContent> GePage(string pageContent, QueryParams queryParams);
    }
}
