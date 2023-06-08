using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepo : RepoBase<Order>, IOrderRepo
    {
        public OrderRepo(OrderContext context) : base(context)
        {    
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await _contex.Orders
                .Where (o => o.UserName == userName)
                .ToListAsync();
        }
    }
}
