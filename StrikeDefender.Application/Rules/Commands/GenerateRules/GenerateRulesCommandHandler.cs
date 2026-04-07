using ErrorOr;
using MediatR;
using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Common.Helpers;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Rules.RuleDTO;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Rules;

namespace StrikeDefender.Application.Rules.Commands.GenerateRules
{
    public class GenerateRulesCommandHandler(
      IAiEngineService aiEngineService,
      IGenericRepository<SuccessfulAttack> attackRepository,
      IGenericRepository<WafRule> rulesRepository,
      IUnitOfWork unitOfWork)
      : IRequestHandler<GenerateRulesCommand, ErrorOr<TestGenerationRules>>
    {
        private readonly IAiEngineService _aiEngineService = aiEngineService;
        private readonly IGenericRepository<SuccessfulAttack> _attackRepository = attackRepository;
        private readonly IGenericRepository<WafRule> _rulesRepository = rulesRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ErrorOr<TestGenerationRules>> Handle(
            GenerateRulesCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return Error.Validation("Prompt", "Prompt cannot be empty");

            var attacks = await _attackRepository.Query()
                .Where(a => !a.IsBlocked)
                .OrderByDescending(a => a.AttackResult.Createdon)
                .Take(5)
                .Include(a => a.Attack)
                .ToListAsync(cancellationToken);

            if (attacks.Count == 0)
                return Error.NotFound("Attacks.NotFound", "No attacks available");
            var aiResult = await _aiEngineService.GenerateRulesAsync(
                attacks,
                request.Prompt,
                cancellationToken);

            if (aiResult.IsError)
                return aiResult.Errors;

            var wafRules = new List<WafRule>();

            foreach (var ruleContent in aiResult.Value)
            {
                var ruleOrError = WafRule.Create(ruleContent);

                if (ruleOrError.IsError)
                    continue;

                wafRules.Add(ruleOrError.Value);
            }

            await _rulesRepository.AddRangeAsync(wafRules);
            await _unitOfWork.CommitChangesAsync();

            var rulesDto = wafRules
                .Select(r => new RuleWithIdDto(
                    r.Id,
                    r.RuleContent))
                .ToList();

            return new TestGenerationRules(
                rulesDto,
                attacks.Select(a => new AttackPayloadDto(
                    a.AttackId,
                    a.Attack.Payload)).ToList());
        }







    }









    }