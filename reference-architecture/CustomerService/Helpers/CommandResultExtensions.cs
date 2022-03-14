using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Helpers
{
    public static class CommandResultExtensions
    {
        public static ActionResult ToActionResult(this CommandResult result, object? entity = null)
        {
            switch (result.Outcome)
            {
                case CommandOutcome.Accepted:
                    if (entity != null) return new OkObjectResult(entity);
                    return new OkResult();
                case CommandOutcome.Conflict:
                    return new ConflictResult();
                case CommandOutcome.NotFound:
                    return new NotFoundResult();
                default:
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}