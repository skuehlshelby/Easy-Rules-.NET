using EasyRules;

namespace Tests.Core
{
    public class AbstractTest
    {
        protected IFacts Facts = new Facts()
        {
            { "fact1", string.Empty },
            { "fact2", string.Empty }
        };
    }
}
