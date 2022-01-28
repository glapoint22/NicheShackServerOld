using System.Collections.Generic;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using Website.ViewModels;
using Services.Classes;

namespace Website.Repositories
{
    public interface IListRepository : IRepository<List>
    {
        Task<IEnumerable<ListViewModel>> GetLists(string customerId);
        Task<IEnumerable<ListProductViewModel>> GetListProducts(IEnumerable<int> collaborators, string customerId, string sort);
        Task<List<string>> GetRecipientIds(EmailType emailType, string listId, string customerId);
        Task<EmailParams> GetEmailParams(string customerId, string listId, string host, List<string> recipientIds, int? productId = null);
        Task<string> FirstList(string customerId);
    }
}
