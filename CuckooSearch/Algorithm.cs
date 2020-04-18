using System;
using System.Diagnostics;
using System.Linq;

namespace CuckooSearch
{
	public class Algorithm
	{
		#region Properties: private

		private const int AbandonWorstNestsPercentage = 20;

		private readonly Population _population = new Population();
		private Function _function;

		private readonly Random _random = new Random();

		#endregion

		#region Initialization

		private void Initialize(int hostsNumber, int cuckoosNumber, Function function)
		{
			_function = function;

			_population.Hosts.Clear();
			for (int i = 0; i < hostsNumber; i++)
			{
				var host = new Bird();
				host.MoveToRandomLocation(_function, _random);
				_population.Hosts.Add(host);
			}

			_population.Cuckoos.Clear();
			for (int i = 0; i < cuckoosNumber; i++)
			{
				var cuckoo = new Bird();
				cuckoo.MoveToRandomLocation(_function, _random);
				_population.Cuckoos.Add(cuckoo);
			}
		}

		#endregion

		public void Run(int hostsNumber, int cuckoosNumber, Function function)
		{
			var stopwatch = Stopwatch.StartNew();

			Initialize(hostsNumber, cuckoosNumber, function);

			// double vMax = Math.Abs(Function.BoundUpper - Function.BoundLower) * .1;
			// const double wLow = 0.1;

			var best = new Bird();

			int lastImprovementOn = 0;
			for (int iter = 1; iter <= function.IterationsNumber; iter++)
			{
				_population.Cuckoos = _population.Cuckoos.OrderBy(c => c.Fx).ToList();

				double alpha = function.AlphaMax * Math.Pow(function.AlphaMin / function.AlphaMax, iter);

				for (int i = 0; i < _population.Cuckoos.Count; i++)
				{
					Bird cuckoo = _population.Cuckoos[i];

					#region Move Cuckoo

					double lambda = function.LambdaMax - i * (function.LambdaMax - function.LambdaMin) / (_population.Cuckoos.Count - 1);

					double dist = 0;
					for (int d = 0; d < _function.Dimensions; d++)
					{
						double randomPart = alpha * (_random.NextDouble() - .5) * LevyRandom(lambda, alpha);
						// cuckoo.X[d] += randomPart;

						// double randomPart = function.BoundLower + _random.NextDouble() * (function.BoundUpper - function.BoundLower);
						double walk = alpha * Math.Pow(iter, -lambda) * randomPart;
						dist += walk;
						cuckoo.X[d] += walk;

						// Ensure boundaries
						cuckoo.X[d] = Math.Max(cuckoo.X[d], _function.BoundLower);
						cuckoo.X[d] = Math.Min(cuckoo.X[d], _function.BoundUpper);
					}

					cuckoo.Fx = function.Expression(cuckoo.X);
					Console.WriteLine($"Walked {dist,-8:0.00000} Lambda = {lambda,-7:0.00000} Fx = {cuckoo.Fx,-16:0.000000000000} X={cuckoo.X[0]} Y={cuckoo.X[1]}");

					#endregion

					#region Attack Host

					Bird host = _population.Hosts[_random.Next(_population.Hosts.Count)];
					if (cuckoo.Fx < host.Fx)
					{
						host.X.Clear();
						host.X.AddRange(cuckoo.X);
						host.Fx = cuckoo.Fx;
						lastImprovementOn = iter;
					}

					#endregion

					#region Abandon worst nests

					// _population.Hosts = _population.Hosts.OrderByDescending(h => h.Fx).ToList();
					// int abandonFirstNumber = AbandonWorstNestsPercentage * _population.Hosts.Count / 100;
					//
					// for (int j = 0; j < abandonFirstNumber; j++)
					// {
					// 	_population.Hosts[j].MoveToRandomLocation(function, _random);
					// }
					// _population.Hosts = _population.Hosts.OrderByDescending(h => h.Fx).ToList();

					#endregion

					best = _population.Hosts.Last();
				}

				// Console.WriteLine($"#{iter,-4} Best = {best.Fx,-16:0.000000000000} Alpha = {alpha,-7:0.00000}");
			}

			stopwatch.Stop();
			Console.WriteLine($"Best = {best.Fx,-16:0.000000000000}");
			Console.WriteLine($"\nLast improvement was on iteration #{lastImprovementOn}. Time elapsed: {stopwatch.Elapsed}");
			Console.WriteLine("Coordinates of the best");
			foreach (double x in best.X)
			{
				Console.Write($"{x,20}");
			}

			Console.WriteLine();
		}

		private double LevyRandom(double lambda, double alpha)
		{
			double rnd = _random.NextDouble();
			double f = Math.Pow(rnd, -1 / lambda);
			return alpha * f * (_random.NextDouble() - .5);
		}

		private double GaussianRandom(double mue, double sigma)
		{
			double x1;
			double w;
			const int randMax = 0x7fff;
			do
			{
				x1 = 2.0 * _random.Next(randMax) / (randMax + 1) - 1.0;
				double x2 = 2.0 * _random.Next(randMax) / (randMax + 1) - 1.0;
				w = x1 * x1 + x2 * x2;
			} while (w >= 1.0);

			// ReSharper disable once IdentifierTypo
			double llog = Math.Log(w);
			w = Math.Sqrt((-2.0 * llog) / w);
			double y = x1 * w;
			return mue + sigma * y;
		}

		// ReSharper disable once UnusedMember.Local
		private double MantegnaRandom(double lambda)
		{
			double sigmaX = SpecialFunction.lgamma(lambda + 1) * Math.Sin(Math.PI * lambda * .5);
			double divider = SpecialFunction.lgamma(lambda * .5) * lambda * Math.Pow(2.0, (lambda - 1) * .5);
			sigmaX /= divider;
			double lambda1 = 1.0 / lambda;
			sigmaX = Math.Pow(Math.Abs(sigmaX), lambda1);
			double x = GaussianRandom(0, sigmaX);
			double y = Math.Abs(GaussianRandom(0, 1.0));
			return x / Math.Pow(y, lambda1);
		}
	}
}