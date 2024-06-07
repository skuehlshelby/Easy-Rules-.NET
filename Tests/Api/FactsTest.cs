using EasyRules;
using Xunit;

namespace Tests.Api
{
	public class FactsTest
	{
		[Fact]
		public void FactsMustHaveUniqueName()
		{
			var facts = new Facts()
			{
				Fact.Create("foo", 1).Key(out var foo),
				Fact.Create("foo", 2)
			};

			Assert.Single(facts);
			Assert.True(facts.True(foo, f => f == 1));
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
			
			Assert.Contains(new Fact<int>("foo", 1), facts, EquateFactsByName.Instance);
			Assert.Contains(new Fact<int>("bar", 2), facts, EquateFactsByName.Instance);
		}

		[Fact]
		public void TestRemove()
		{
			var foo = new Fact<int>("foo", 1);
			var facts = new Facts()
			{
				foo
			};

			facts.Remove(foo);

			Assert.Empty(facts);
		}

		[Fact]
		public void TestRemoveByName()
		{
			var foo = Fact.Create("foo", 1);

			var facts = new Facts()
			{
				foo
			};

			facts.Remove("foo");

			Assert.Empty(facts);
		}

		[Fact]
		public void TestGet()
		{
			var fact = new Fact<int>("foo", 1);

			var facts = new Facts()
			{
				fact
			};

			var value = facts.Get(fact.Key());

			Assert.Equal(1, value);
		}

		[Fact]
		public void TestGetFact()
		{
			var fact = new Fact<int>("foo", 1);

			var facts = new Facts()
			{
				fact
			};

			var retrievedFact = facts.GetFactByName("foo");
			Assert.Equal(fact, retrievedFact);
		}

		[Fact]
		public void TestClear()
		{
			var facts = new Facts
			{
				{ "foo", 1 },
				{ "bar", 2 }
			};

			facts.Clear();

			Assert.Empty(facts);
		}
	}
}
