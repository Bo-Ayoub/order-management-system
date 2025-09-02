using MediatR;
using OrderManagement.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Features.Orders.Commands.ConfirmOrder
{
    public record ConfirmOrderCommand(Guid OrderId) : IRequest<Result>;
}
