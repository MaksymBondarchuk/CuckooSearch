using System;
using System.Linq;

namespace CuckooSearch.Factories
{
    public static class FunctionsFactory
    {
        public static Function Sphere { get; } = new Function
        {
            Expression = x => { return x.Sum(t => t * t); },
            BoundLower = -100,
            BoundUpper = 100,
            Dimensions = 2
        };
        
        public static Function Ackley { get; } = new Function
        {
            Expression = x =>
            {
                double _1NaD = 1.0 / x.Count;
                return -20 * Math.Exp(-0.2 * Math.Sqrt(_1NaD * x.Sum(t => t * t))) -
                    Math.Exp(_1NaD * x.Sum(t => Math.Cos(2 * Math.PI * t))) + 20 + Math.E;
            },
            BoundLower = -32.768,
            BoundUpper = 32.768,
            Dimensions = 2
        };
        
        public static Function Griewank { get; } = new Function
        {
            Expression = x =>
            {
                var mul = 1.0;
                for (var i = 0; i < x.Count; i++)
                    mul *= Math.Cos(x[i] / Math.Sqrt(i + 1));
                return x.Sum(t => t * t / 4000) - mul + 1;
            },
            BoundLower = -100,
            BoundUpper = 100,
            Dimensions = 2
        };
        
        public static Function Rastrigin { get; } = new Function
        {
            Expression = x => { return 10 * x.Count + x.Sum(t => t * t - 10 * Math.Cos(2 * Math.PI * t)); },
            BoundLower = -5,
            BoundUpper = 5,
            Dimensions = 2,
            IterationsNumber = 150000
        };
        
        public static Function Rosenbrock { get; } = new Function
        {
            Expression = x =>
            {
                var res = .0;
                for (var i = 0; i < x.Count - 1; i++)
                {
                    res += 100 * Math.Pow(x[i + 1] - x[i] * x[i], 2) + (x[i] - 1) * (x[i] - 1);
                }

                return res;
            },
            BoundLower = -2.048,
            BoundUpper = 2.048,
            Dimensions = 2
        };
    }
}