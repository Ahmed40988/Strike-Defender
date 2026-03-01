using ErrorOr;
using MediatR;
using StrikeDefender.Application.Common.Interfaces;

namespace StrikeDefender.Application.Attacks.Commands.GenerateAttacks
{
    public class GenerateAttackCommandHandler(IAiEngineService aiEngineService) : IRequestHandler<GenerateAttackCommand, ErrorOr<List<string>>>
    {
        private readonly IAiEngineService _aiEngineService = aiEngineService;

        public async Task<ErrorOr<List<string>>> Handle(
           GenerateAttackCommand request,
           CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return Error.Validation("Prompt", "Prompt cannot be empty");
            }

            var response = await _aiEngineService.GenerateAttacksAsync(
                request.Prompt,
                cancellationToken);

            return response;
        }
    }
}
