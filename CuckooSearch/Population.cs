using System.Collections.Generic;

namespace CuckooSearch
{
	public class Population
	{
		public List<Bird> Hosts { get; set; } = new List<Bird>();
		public List<Bird> Cuckoos { get; set; } = new List<Bird>();

		public void SortByFitness(Function function)
		{
			Cuckoos.Sort((x, y) => function.Expression(x.X).CompareTo(function.Expression(y.X)));
		}

		public void SortByFitnessDescending(Function function)
		{
			Cuckoos.Sort((x, y) => function.Expression(y.X).CompareTo(function.Expression(x.X)));
		}
	}
}