using EasyRules;
using System;
using Xunit;

namespace Tests.Api
{

	public class RulesTest
	{
		[Fact]
		public void FactsMustHaveUniqueName()
		{
			var facts = new Facts()
			{
				{ "foo", 1 },
				{ "foo", 2 }
			};

			Assert.Single(facts);
			Assert.True(facts.IsTrue<int>("foo", f => f == 1));
		}

		[Fact]
	public void TestAdd()
		{
			var fact1 = new Fact<int>("foo", 1);
			var fact2 = new Fact<int>("bar", 2);

			var facts = new Facts
			{
				fact1,
				fact2
			};

			Assert.Contains(fact1, facts);
			Assert.Contains(fact2, facts);
		}

		[Fact]
		public void TestPut()
		{
			var facts = new Facts
			{
				{ "foo", 1 },
				{ "bar", 2 }
			};

			Assert.Contains(new Fact<int>("foo", 1), facts);
			Assert.Contains(new Fact<int>("bar", 2), facts);
		}

		[Fact]
	public void TestRemove()
		{
			Fact<Integer> foo = new Fact<>("foo", 1);
			facts.add(foo);
			facts.remove(foo);

			assertThat(facts).isEmpty();
		}

		[Fact]
	public void TestRemoveByName()
		{
			Fact<Integer> foo = new Fact<>("foo", 1);
			facts.add(foo);
			facts.remove("foo");

			assertThat(facts).isEmpty();
		}

		[Fact]
	public void TestGet()
		{
			Fact<Integer> fact = new Fact<>("foo", 1);
			facts.add(fact);
			Integer value = facts.get("foo");
			assertThat(value).isEqualTo(1);
		}

		[Fact]
	public void TestGetFact()
		{
			Fact<Integer> fact = new Fact<>("foo", 1);
			facts.add(fact);
			Fact <?> retrievedFact = facts.getFact("foo");
			assertThat(retrievedFact).isEqualTo(fact);
		}

		[Fact]
	public void TestAsMap()
		{
			Fact<Integer> fact1 = new Fact<>("foo", 1);
			Fact<Integer> fact2 = new Fact<>("bar", 2);
			facts.add(fact1);
			facts.add(fact2);
			Map<String, Object> map = facts.asMap();
			assertThat(map).containsKeys("foo", "bar");
			assertThat(map).containsValues(1, 2);
		}

		[Fact]
	public void TestClear()
		{
			Facts facts = new Facts();
			facts.add(new Fact<>("foo", 1));
			facts.clear();
			assertThat(facts).isEmpty();
		}
	}
}
