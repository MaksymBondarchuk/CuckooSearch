using System;
using System.Diagnostics;
using System.Linq;
using CuckooSearch.Enums;

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
        private double _lambda;
        private double _alpha;
        private WalkType _walkType;

        #endregion

        #region Initialization

        public void Initialize(int hostsNumber, int cuckoosNumber, Function function, WalkType walkType, double alpha, double lambda)
        {
            _function = function;

            Population.Hosts.Clear();
            for (var i = 0; i < hostsNumber; i++)
            {
                var host = new Bird();
                host.MoveToRandomLocation(_function, _random);
                Population.Hosts.Add(host);
            }

            Population.Cuckoos.Clear();
            for (var i = 0; i < cuckoosNumber; i++)
            {
                var cuckoo = new Bird();
                cuckoo.MoveToRandomLocation(_function, _random);
                Population.Cuckoos.Add(cuckoo);
            }

            _iteration = 0;
            _lastImprovementOn = 0;
            _best = new Bird();
            _lambda = lambda;
            _alpha = alpha;
            _walkType = walkType;
        }

        #endregion

        public void Run(int hostsNumber, int cuckoosNumber, Function function, WalkType walkType)
        {
            var stopwatch = Stopwatch.StartNew();

            Initialize(hostsNumber, cuckoosNumber, function, walkType, 1, 1.5);

            // double vMax = Math.Abs(Function.BoundUpper - Function.BoundLower) * .1;
            // const double wLow = 0.1;

            for (var iter = 1; iter <= function.IterationsNumber; iter++)
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

            double alpha = _alpha;

            for (var i = 0; i < Population.Cuckoos.Count; i++)
            {
                Bird cuckoo = Population.Cuckoos[i];

                #region Move Cuckoo

                double lambda = _lambda; // 1.5;

                for (var d = 0; d < _function.Dimensions; d++)
                {
                    double walk = _walkType switch
                    {
                        WalkType.Levy => LevyRandom(lambda, alpha),
                        WalkType.Mantegna => MantegnaRandom(alpha),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    cuckoo.X[d] += walk;

                    // Ensure boundaries
                    cuckoo.X[d] = Math.Max(cuckoo.X[d], _function.BoundLower);
                    cuckoo.X[d] = Math.Min(cuckoo.X[d], _function.BoundUpper);
                }

                cuckoo.Fx = function.Expression(cuckoo.X);

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
                var abandonFirstNumber = Convert.ToInt32(AbandonmentFraction * Population.Hosts.Count);
                for (var j = 0; j < abandonFirstNumber; j++)
                {
                    Population.Hosts[j].MoveToRandomLocation(function, _random);
                }

                Population.Hosts = Population.Hosts.OrderByDescending(h => h.Fx).ToList();
                Population.Cuckoos = Population.Cuckoos.OrderByDescending(h => h.Fx).ToList();

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

            double log = Math.Log(w);
            w = Math.Sqrt((-2.0 * log) / w);
            double y = x1 * w;
            return mue + sigma * y;
        }

        // ReSharper disable once UnusedMember.Local
        private double MantegnaRandom(double alpha)
        {
            double sigmaX = SpecialFunction.lgamma(alpha + 1) * Math.Sin(Math.PI * alpha * .5);
            double divider = SpecialFunction.lgamma((alpha + 1) * .5) * alpha * Math.Pow(2.0, (alpha - 1) * .5);
            sigmaX /= divider;
            double alpha1 = 1.0 / alpha;
            sigmaX = Math.Pow(Math.Abs(sigmaX), alpha1);
            double x = GaussianRandom(0, sigmaX);
            double y = Math.Abs(GaussianRandom(0, 1.0));
            return x / Math.Pow(y, alpha1);
        }
    }
}