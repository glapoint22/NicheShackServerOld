using DataAccess.Interfaces;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.Classes;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{
    public class AdditionalInfoRepository<T> : Repository<T>, IAdditionalInfoRepository<T> where T : class, IAdditionalInfo
    {
        private readonly NicheShackContext context;

        public AdditionalInfoRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        public async Task Update(AdditionalInfoViewModel additionalInfoViewModel)
        {
            DbSet<T> dbSet = context.Set<T>();

            T additionalInfo = await dbSet
                 .AsNoTracking()
                 .Where(x => x.Id == additionalInfoViewModel.Id)
                 .SingleOrDefaultAsync();

            if (additionalInfo != null)
            {
                additionalInfo.IsRecurring = additionalInfoViewModel.IsRecurring;
                additionalInfo.ShippingType = additionalInfoViewModel.ShippingType;
                additionalInfo.TrialPeriod = additionalInfoViewModel.RecurringPayment.TrialPeriod;
                additionalInfo.Price = additionalInfoViewModel.RecurringPayment.Price;
                additionalInfo.RebillFrequency = additionalInfoViewModel.RecurringPayment.RebillFrequency;
                additionalInfo.TimeFrameBetweenRebill = additionalInfoViewModel.RecurringPayment.TimeFrameBetweenRebill;
                additionalInfo.SubscriptionDuration = additionalInfoViewModel.RecurringPayment.SubscriptionDuration;

                dbSet.Update(additionalInfo);
                await context.SaveChangesAsync();
            }
        }




        public async Task<int> Post(T additionalInfo)
        {
            DbSet<T> dbSet = context.Set<T>();

            dbSet.Add(additionalInfo);

            await context.SaveChangesAsync();

            return additionalInfo.Id;
        }




        public async Task Delete(int id)
        {
            DbSet<T> dbSet = context.Set<T>();

            T additionalInfo = await dbSet
                 .FindAsync(id);

            if (additionalInfo != null)
            {
                dbSet.Remove(additionalInfo);
                await context.SaveChangesAsync();
            }
        }
    }
}
