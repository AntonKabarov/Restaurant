using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using RestaurantIntegrationGrpc.Command;
using Sms.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantIntegrationGrpc.Services
{
    public class SmsTestGrpcService : SmsTestService.SmsTestServiceBase
    {
        private readonly IMediator _mediator;

        public SmsTestGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetMenuResponse> GetMenu(
            BoolValue request,
            ServerCallContext context)
        {
            var query = new GetMenuQuery(request.Value);
            return await _mediator.Send(query, context.CancellationToken);
        }

        public override async Task<SendOrderResponse> SendOrder(
            Order request,
            ServerCallContext context)
        {
            var command = new SendOrderCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }
    }
}
