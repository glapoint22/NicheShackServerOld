using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class AvailableKeywordsRepository : Repository<Keyword_In_KeywordGroup>, IAvailableKeywordsRepository
    {

        private readonly NicheShackContext context;

        public AvailableKeywordsRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }


        public async Task Trumpy(int keywordId, string keywordName) 
        {
            var keywordGroupId = await context.Keywords_In_KeywordGroup.Where(x => x.KeywordId == keywordId).Select(x => x.KeywordGroupId).FirstOrDefaultAsync();
            var keywords = await context.Keywords_In_KeywordGroup.Where(x => x.KeywordGroupId == keywordGroupId).Select(x => x.Keyword.Name).ToListAsync();

            var fuck = keywords.Any(x => x.ToUpper() == keywordName.ToUpper());

            //var filterOption = await unitOfWork.FilterOptions.Get(x => x.Name == filterOptionName && x.Filter.Id == parentFilterId);


            //var musky = await context.Keywords_In_KeywordGroup.Join(context.Keywords, x => x.KeywordId, y => y.Id, (a, b) =>  new { A = a, B = b }  );



            var alita = true;
        }
    }
}