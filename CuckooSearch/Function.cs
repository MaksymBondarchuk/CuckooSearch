using System;
using System.Collections.Generic;

namespace CuckooSearch
{
	public class Function
	{
		public Func<List<double>, double> Expression { get; set; }

		public double BoundLower { get; set; }

		public double BoundUpper { get; set; }

		public double Dimensions { get; set; }

		public double LambdaMin { get; set; } = 3;

		public double LambdaMax { get; set; } = 30;

		public double AlphaMin { get; set; } = 1;

		public double AlphaMax { get; set; } = 1;

		public int IterationsNumber { get; set; } = 10000;
	}
}