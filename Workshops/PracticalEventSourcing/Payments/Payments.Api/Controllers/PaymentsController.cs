using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Core.Commands;
using Core.Ids;
using Core.Queries;
using Payments.Api.Requests.Carts;
using Commands = Payments.Payments.Commands;

namespace Payments.Api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController: Controller
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly IIdGenerator idGenerator;

        public PaymentsController(
            ICommandBus commandBus,
            IQueryBus queryBus,
            IIdGenerator idGenerator)
        {
            Guard.Against.Null(commandBus, nameof(commandBus));
            Guard.Against.Null(queryBus, nameof(queryBus));
            Guard.Against.Null(idGenerator, nameof(idGenerator));

            this.commandBus = commandBus;
            this.queryBus = queryBus;
            this.idGenerator = idGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> RequestPayment([FromBody] RequestPaymentRequest request)
        {
            Guard.Against.Null(request, nameof(request));

            var paymentId = idGenerator.New();

            var command = Commands.RequestPayment.Create(
                paymentId,
                request.OrderId,
                request.Amount
            );

            await commandBus.Send(command);

            return Created("api/Payments", paymentId);
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompletePayment(Guid id)
        {
            var command = Commands.CompletePayment.Create(
                id
            );

            await commandBus.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DiscardPayment(Guid id, [FromBody] DiscardPaymentRequest request)
        {
            Guard.Against.Null(request, nameof(request));

            var command = Commands.DiscardPayment.Create(
                id,
                request.DiscardReason
            );

            await commandBus.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> TimeoutPayment(Guid id, [FromBody] TimeOutPaymentRequest request)
        {
            Guard.Against.Null(request, nameof(request));

            var command = Commands.TimeOutPayment.Create(
                id,
                request.TimedOutAt
            );

            await commandBus.Send(command);

            return Ok();
        }
    }
}
