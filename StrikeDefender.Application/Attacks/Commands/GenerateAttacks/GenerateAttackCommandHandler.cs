using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Application.Attacks.Commands.GenerateAttacks
{
    public class GenerateAttackCommandHandler(
      IAiEngineService aiEngineService,
      IGenericRepository<Attack> attackRepository,
      IUnitOfWork unitOfWork)
      : IRequestHandler<GenerateAttackCommand, ErrorOr<List<AttackResponce>>>
    {
        private readonly IAiEngineService _aiEngineService = aiEngineService;
        private readonly IGenericRepository<Attack> _attackRepository = attackRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<List<AttackResponce>>> Handle(
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

            if (response.IsError)
                return response.Errors;

            var payloads = response.Value;

            var attacksToAdd = new List<Attack>();

            foreach (var payload in payloads)
            {
                var attackOrError = Attack.Create(payload, request.Type);

                if (attackOrError.IsError)
                    continue;

                attacksToAdd.Add(attackOrError.Value);
            }

            if (attacksToAdd.Count > 0)
            {
                await _attackRepository.AddRangeAsync(attacksToAdd);
                await _unitOfWork.CommitChangesAsync();
            }

            var result = attacksToAdd
                .Select(a => new AttackResponce(
                    a.Id,
                    a.Payload))
                .ToList();

            return result;
        }
    }
    }