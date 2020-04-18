using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CuckooSearch;

namespace CuckooFlights
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region DI

		public MainWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Radio Buttons

		private void SphereRb_Checked(object sender, RoutedEventArgs e)
		{
			DrawFunction(new Function
			{
				Expression = x => { return x.Sum(t => t * t); },
				BoundLower = -100,
				BoundUpper = 100,
				Dimensions = 2,
				LambdaMax = 3,
				LambdaMin = 0.1,
			});
		}

		private void AckleyRb_Checked(object sender, RoutedEventArgs e)
		{
			DrawFunction(new Function
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
			});
		}

		private void GriewankRb_Checked(object sender, RoutedEventArgs e)
		{
			DrawFunction(new Function
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
			});
		}

		private void RastriginRb_Checked(object sender, RoutedEventArgs e)
		{
			DrawFunction(new Function
			{
				Expression = x => { return x.Sum(t => t * t - 10 * Math.Cos(2 * Math.PI * t) + 10); },
				BoundLower = -5.12,
				BoundUpper = 5.12,
				Dimensions = 30,
				IterationsNumber = 150000
			});
		}

		private void RosenbrockRb_Checked(object sender, RoutedEventArgs e)
		{
			DrawFunction(new Function
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
			});
		}

		#endregion

		private void DrawFunction(Function function)
		{
			int width = Convert.ToInt32(FunctionImage.Height);
			int height = Convert.ToInt32(FunctionImage.Width);
			var pixels = new byte[height, width, 4];
		}

		private Color GetColor(Function function, double max, double min, double x, double y)
		{
			double value = function.Expression(new List<double> {x, y});
			
			return Colors.White;
		}
	}
}