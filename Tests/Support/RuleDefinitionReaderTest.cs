using EasyRules.Support;
using EasyRules.Support.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Support
{

    public sealed class RuleDefinitionReaderTest
    {
        [Theory, InlineData("adult-rule.json")]

        public void TestRuleDefinitionReadingFromFile(string fileName)
        {
			var ruleDefinitionReader = new JsonRuleDefinitionReader();

			// when
			using var stream = LoadEmbeddedResource(fileName);
			using var reader = new StreamReader(stream);
			var ruleDefinitions = ruleDefinitionReader.Read(reader);

			// then
			Assert.Single(ruleDefinitions);
			var adultRuleDefinition = ruleDefinitions.Single();
			Assert.NotNull(adultRuleDefinition);
			Assert.Equal("adult rule", adultRuleDefinition.Name);
			Assert.Equal("when age is greater than 18, then mark as adult", adultRuleDefinition.Description);
			Assert.Equal(1, adultRuleDefinition.Priority);
			Assert.Equal("person.age > 18", adultRuleDefinition.Condition);
			Assert.Collection(adultRuleDefinition.Actions, a => Assert.Equal("person.setAdult(true);", a));
		}

		private static Stream LoadEmbeddedResource(string resourceName)
		{
			var assembly = Assembly.GetAssembly(typeof(RuleDefinitionReaderTest));

			foreach (var manifestResourceName in assembly?.GetManifestResourceNames() ?? [])
			{
				if (manifestResourceName.EndsWith(resourceName))
				{
					return assembly?.GetManifestResourceStream(manifestResourceName)!;
				}
			}

			throw new ArgumentException($"No resource found whose name ends with '{resourceName}'");
		}
    }
}
