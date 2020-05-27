using System;
using System.Collections.Generic;
using System.Globalization;

namespace CuckooSearch
{
	public class Bird
	{
		public Bird()
		{
		}

		public Bird(Bird bird)
		{
			X = new List<double>();
			X.AddRange(bird.X);
			Fx = bird.Fx;
		}

		public List<double> X { get; } = new List<double>();

		public double Fx { get; set; } = double.MaxValue;

		public void MoveToRandomLocation(Function function, Random random)
		{
			X.Clear();
			for (var j = 0; j < function.Dimensions; j++)
			{
				double rnd = function.BoundLower + random.NextDouble() * (function.BoundUpper - function.BoundLower);
				X.Add(rnd);
			}

			Fx = function.Expression(X);
		}

		public override string ToString()
		{
			return Fx.ToString(CultureInfo.InvariantCulture);
		}
	}
}