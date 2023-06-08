using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepo _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepo repo, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderInDb = await _repo.GetByIdAsync(request.Id);

            if (orderInDb == null)
            {
                _logger.LogInformation($"Order {orderInDb.Id} is not found in db.");
                throw new NotFoundException(nameof(orderInDb), request.Id);
            }

            await _repo.DeleteAsync(orderInDb);

            _logger.LogInformation($"Order {orderInDb.Id} is successfully deleted.");
        }
    }
}
