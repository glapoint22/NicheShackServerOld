using System.Linq;
using Website.Interfaces;
using DataAccess.Models;

namespace Website.Classes
{
    public class CategoryDetailDTO : CategoryDTO, ISelect<Category, CategoryDetailDTO>
    {
        public string Icon { get; set; }


        // ..................................................................................Set Select.....................................................................
        public IQueryable<CategoryDetailDTO> SetSelect(IQueryable<Category> source)
        {
            return source.Select(x => new CategoryDetailDTO
            {
                Id = x.Id,
                Name = x.Name,
                Niches = x.Niches
                    .OrderBy(y => y.Name)
                    .Select(y => new NicheDTO
                    {
                        Id = y.Id,
                        Name = y.Name
                    })
            });
        }
    }
}
