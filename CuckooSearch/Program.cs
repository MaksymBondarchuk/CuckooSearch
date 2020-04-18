using System;
using System.Linq;

namespace CuckooSearch
{
	internal static class Program
	{
		private static void Main()
		{
			// ReSharper disable once UnusedVariable
			var sphere = new Function
			{
				Expression = x => { return x.Sum(t => t * t); },
				BoundLower = -100,
				BoundUpper = 100,
				Dimensions = 2,
				LambdaMax = 3,
				LambdaMin = 0.1,
			};

			// ReSharper disable once UnusedVariable
			var ackley = new Function
			{
				Expression = x =>
				{
					double _1NaD = 1.0 / x.Count;
					return -20 * Math.Exp(-0.2 * Math.Sqrt(_1NaD * x.Sum(t => t * t))) -
						Math.Exp(_1NaD * x.Sum(t => Math.Cos(2 * Math.PI * t))) + 20 + Math.E;
				},
				BoundLower = -32.768,
				BoundUpper = 32.768,
				Dimensions = 20
			};

			// ReSharper disable once UnusedVariable
			var griewank = new Function
			{
				Expression = x =>
				{
					double mul = 1.0;
					for (int i = 0; i < x.Count; i++)
						mul *= Math.Cos(x[i] / Math.Sqrt(i + 1));
					return x.Sum(t => t * t / 4000) - mul + 1;
				},
				BoundLower = -600,
				BoundUpper = 600,
				Dimensions = 50
			};

			// ReSharper disable once UnusedVariable
			var rastrigin = new Function
			{
				Expression = x => { return x.Sum(t => t * t - 10 * Math.Cos(2 * Math.PI * t) + 10); },
				BoundLower = -5.12,
				BoundUpper = 5.12,
				Dimensions = 30,
				IterationsNumber = 150000
			};

			// ReSharper disable once UnusedVariable
			var rosenbrock = new Function
			{
				Expression = x =>
				{
					double res = .0;
					for (int i = 0; i < x.Count - 1; i++)
						res += 100 * Math.Pow(x[i + 1] - x[i] * x[i], 2) + (x[i] - 1) * (x[i] - 1);
					return res;
				},
				BoundLower = -5,
				BoundUpper = 10,
				Dimensions = 30
			};


			var algorithm = new Algorithm();

			algorithm.Run(hostsNumber: 50, cuckoosNumber: 5, function: sphere);
		}
	}
}