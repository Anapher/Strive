using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace Strive.Core.Services.Poll.Utilities
{
    public class PollAnswerAggregatorWrapper
    {
        private readonly IComponentContext _context;

        public PollAnswerAggregatorWrapper(IComponentContext context)
        {
            _context = context;
        }

        public ValueTask<PollResults> AggregateAnswers(PollInstruction instruction,
            IEnumerable<PollAnswerWithKey> answers)
        {
            var genericMethod = GetType().GetMethod(nameof(GenericAggregateAnswers),
                BindingFlags.NonPublic | BindingFlags.Instance);

            var instructionType = instruction.GetType();
            var baseType = GetBaseType(instructionType, typeof(PollInstruction<>));

            genericMethod = genericMethod!.MakeGenericMethod(instructionType, baseType);

            return (ValueTask<PollResults>) genericMethod.Invoke(this, new object[] {instruction, answers})!;
        }

        private ValueTask<PollResults> GenericAggregateAnswers<TInstruction, TAnswer>(PollInstruction instruction,
            IEnumerable<PollAnswerWithKey> answers) where TAnswer : PollAnswer
            where TInstruction : PollInstruction<TAnswer>
        {
            if (instruction is not TInstruction typedInstruction)
                throw new ArgumentException(
                    $"The generic parameter {nameof(TAnswer)} must match the {nameof(instruction)}");

            var aggregator = _context.Resolve<IPollAnswerAggregator<TInstruction, TAnswer>>();
            return aggregator.Aggregate(typedInstruction, answers.ToDictionary(x => x.Key, x => (TAnswer) x.Answer));
        }

        private static Type GetBaseType(Type type, Type baseType)
        {
            while (type.BaseType != null)
            {
                type = type.BaseType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                {
                    return type.GenericTypeArguments[0];
                }
            }

            throw new InvalidOperationException("Base type was not found");
        }
    }
}
