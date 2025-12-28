using MediatR;
using Sms.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Command
{
    public record GetMenuQuery(bool IncludeAll) : IRequest<GetMenuResponse>;

    public record SendOrderCommand(Order Order) : IRequest<SendOrderResponse>;

}
