using System;
using System.Diagnostics;
using System.Linq;

namespace CuckooSearch
{
	public class Algorithm
	{
		#region Properties: private

		private const double AbandonmentFraction = .25;

		public readonly Population Population = new Population();
		private Function _function;

		private readonly Random _random = new Random();
		private int _iteration;
		private int _lastImprovementOn;
		private Bird _best;

		#endregion

		#region Initialization

		public void Initialize(int hostsNumber, int cuckoosNumber, Function function)
		{
			_function = function;

			Population.Hosts.Clear();
			for (int i = 0; i < hostsNumber; i++)
			{
				var host = new Bird();
				host.MoveToRandomLocation(_function, _random);
				Population.Hosts.Add(host);
			}

			Population.Cuckoos.Clear();
			for (int i = 0; i < cuckoosNumber; i++)
			{
				var cuckoo = new Bird();
				cuckoo.MoveToRandomLocation(_function, _random);
				Population.Cuckoos.Add(cuckoo);
			}

			_iteration = 0;
			_lastImprovementOn = 0;
			_best = new Bird();
		}

		#endregion

		public void Run(int hostsNumber, int cuckoosNumber, Function function)
		{
			var stopwatch = Stopwatch.StartNew();

			Initialize(hostsNumber, cuckoosNumber, function);

			// double vMax = Math.Abs(Function.BoundUpper - Function.BoundLower) * .1;
			// const double wLow = 0.1;

			for (int iter = 1; iter <= function.IterationsNumber; iter++)
			{
				Iteration(function);
			}

			stopwatch.Stop();
			Console.WriteLine($"Best = {_best.Fx,-16:0.000000000000}");
			Console.WriteLine($"\nLast improvement was on iteration #{_lastImprovementOn}. Time elapsed: {stopwatch.Elapsed}");
			Console.WriteLine("Coordinates of the best");
			foreach (double x in _best.X)
			{
				Console.Write($"{x,20}");
			}

			Console.WriteLine();
		}

		public void Iteration(Function function)
		{
			_iteration++;
			
			Population.Cuckoos = Population.Cuckoos.OrderBy(c => c.Fx).ToList();

			const double alpha = 1;
			// double alpha = function.AlphaMax * Math.Pow(function.AlphaMin / function.AlphaMax, _iteration);

			for (int i = 0; i < Population.Cuckoos.Count; i++)
			{
				Bird cuckoo = Population.Cuckoos[i];
				// Console.WriteLine($"Fx = {cuckoo.Fx,-16:0.000000000000} X={cuckoo.X[0]} Y={cuckoo.X[1]}");
				
				#region Move Cuckoo

				double lambda = 1.5;
				// double lambda = function.LambdaMax - i * (function.LambdaMax - function.LambdaMin) / (Population.Cuckoos.Count /*- 1*/);

				for (int d = 0; d < _function.Dimensions; d++)
				{
					// double lambda = 3 - 2 * _random.NextDouble();
					// double randomPart = Math.Pow(_iteration, -lambda);
					// double randomPart = function.BoundLower + _random.NextDouble() * (function.BoundUpper - function.BoundLower);
					// cuckoo.X[d] += randomPart;

					// double randomPart = alpha * (_random.NextDouble() - .5) * LevyRandom(lambda, alpha);
					// double walk = alpha * Math.Pow(_iteration, -lambda) * randomPart;
					double walk = LevyRandom(lambda, alpha);
					cuckoo.X[d] += walk;

					// Ensure boundaries
					cuckoo.X[d] = Math.Max(cuckoo.X[d], _function.BoundLower);
					cuckoo.X[d] = Math.Min(cuckoo.X[d], _function.BoundUpper);
				}

				cuckoo.Fx = function.Expression(cuckoo.X);
				// Console.WriteLine($"Fx = {cuckoo.Fx,-16:0.000000000000} X={cuckoo.X[0]} Y={cuckoo.X[1]}");
				// Console.WriteLine($"Walked {dist,-8:0.00000} Lambda = {lambda,-7:0.00000} Fx = {cuckoo.Fx,-16:0.000000000000} X={cuckoo.X[0]} Y={cuckoo.X[1]}");

				#endregion

				#region Attack Host

				Bird host = Population.Hosts[_random.Next(Population.Hosts.Count)];
				if (cuckoo.Fx < host.Fx)
				{
					host.X.Clear();
					host.X.AddRange(cuckoo.X);
					host.Fx = cuckoo.Fx;
					_lastImprovementOn = _iteration;
				}

				#endregion

				#region Abandon worst nests

				Population.Hosts = Population.Hosts.OrderByDescending(h => h.Fx).ToList();
				int abandonFirstNumber = Convert.ToInt32(AbandonmentFraction * Population.Hosts.Count);
				for (int j = 0; j < abandonFirstNumber; j++)
				{
					Population.Hosts[j].MoveToRandomLocation(function, _random);
				}
				Population.Hosts = Population.Hosts.OrderByDescending(h => h.Fx).ToList();

				#endregion

				_best = Population.Hosts.Last();
			}
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