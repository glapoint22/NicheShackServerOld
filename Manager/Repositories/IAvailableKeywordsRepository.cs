using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public interface IAvailableKeywordsRepository : IRepository<Keyword_In_KeywordGroup>
    {

        Task Trumpy(int keywordId, string keywordName);
    }
}
