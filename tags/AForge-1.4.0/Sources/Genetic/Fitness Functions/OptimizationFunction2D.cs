// AForge Genetic Library
//
// Copyright � Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;
	using AForge;

	/// <summary>Base class for two dimenstional function optimization</summary>
	///
	/// <remarks>The class is aimed to be used for two dimensional function
	/// optimization problems. It implements all methods of <see cref="IFitnessFunction"/>
	/// interface and requires overriding only one method -
	/// <see cref="OptimizationFunction"/>, which represents the
	/// function to optimize. <b>Note</b>: the optimization function should be greater
	/// then 0 on the specified optimization range.<br/><br/>
	/// The class works only with binary chromosomes (<see cref="BinaryChromosome"/>).</remarks>
	/// 
	/// <example>The following sample illustrates the usage of <c>OptimizationFunction1D</c> class:
	/// <code>
	/// // define optimization function
	/// public class UserFunction : OptimizationFunction2D
	/// {
	///		public UserFunction( ) :
	///			base( new DoubleRange( -4, 4 ), new DoubleRange( -4, 4 ) ) { }
	///
	/// 	public override double OptimizationFunction( double x, double y )
	///		{
	///			return ( Math.Cos( y ) * x * y ) / ( 2 - Math.Sin( x ) );
	///		}
	/// }
	/// ...
	/// // create genetic population
	/// Population population = new Population( 40,
	///		new BinaryChromosome( 32 ),
	///		new UserFunction( ),
	///		new EliteSelection( ) );
	///	// run one epoch of the population
	///	population.RunEpoch( );
	/// </code>
	/// </example>
	///
	public abstract class OptimizationFunction2D : IFitnessFunction
	{
		/// <summary>
		/// Optimization modes
		/// </summary>
		///
		/// <remarks>The enumeration defines optimization modes for
		/// the two dimensional function optimization.</remarks> 
		///
		public enum Modes
		{
			/// <summary>
			/// Search for function's maximum value
			/// </summary>
			Maximization,
			/// <summary>
			/// Search for function's minimum value
			/// </summary>
			Minimization
		}
		
		// optimization ranges
		private DoubleRange	rangeX = new DoubleRange( 0, 1 );
		private DoubleRange	rangeY = new DoubleRange( 0, 1 );
		// optimization mode
		private Modes mode = Modes.Maximization;

		/// <summary>
		/// X variable's optimization range
		/// </summary>
		/// 
		/// <remarks>Defines function's X range. The function's extreme will
		/// be searched in this range only.
		/// </remarks>
		/// 
		public DoubleRange RangeX
		{
			get { return rangeX; }
			set { rangeX = value; }
		}

		/// <summary>
		/// Y variable's optimization range
		/// </summary>
		/// 
		/// <remarks>Defines function's Y range. The function's extreme will
		/// be searched in this range only.
		/// </remarks>
		/// 
		public DoubleRange RangeY
		{
			get { return rangeY; }
			set { rangeY = value; }
		}

		/// <summary>
		/// Optimization mode
		/// </summary>
		///
		/// <remarks>Defines optimization mode - what kind of extreme to search</remarks> 
		///
		public Modes Mode
		{
			get { return mode; }
			set { mode = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="OptimizationFunction2D"/> class
		/// </summary>
		///
		/// <param name="rangeX">Specifies X variable's range</param>
		/// <param name="rangeY">Specifies Y variable's range</param>
		///
		public OptimizationFunction2D( DoubleRange rangeX, DoubleRange rangeY )
		{
			this.rangeX = rangeX;
			this.rangeY = rangeY;
		}


		/// <summary>
		/// Evaluates chromosome
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome to evaluate</param>
		/// 
		/// <returns>Returns chromosome's fitness value</returns>
		///
		/// <remarks>The method calculates fitness value of the specified
		/// chromosome.</remarks>
		///
		public double Evaluate( IChromosome chromosome )
		{
			double[] xy;

			// do native translation first
			xy = TranslateNative( chromosome );
			// get function value
			double functionValue = OptimizationFunction( xy[0], xy[1] );
			// return fitness value
			return ( mode == Modes.Maximization ) ? functionValue : 1 / functionValue;
		}

		/// <summary>
		/// Translates genotype to phenotype 
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome, which genoteype should be
		/// translated to phenotype</param>
		///
		/// <returns>Returns chromosome's fenotype - the actual solution
		/// encoded by the chromosome</returns> 
		/// 
		/// <remarks>The method returns object, which represents function's
		/// input point encoded by the specified chromosome. The object's type is
		/// array of two double values.</remarks>
		///
		public object Translate( IChromosome chromosome )
		{
			// do native translation first
			return TranslateNative( chromosome );
		}

		/// <summary>
		/// Translates genotype to phenotype 
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome, which genoteype should be
		/// translated to phenotype</param>
		///
		/// <returns>Returns chromosome's fenotype - the actual solution
		/// encoded by the chromosome</returns> 
		/// 
		/// <remarks>The method returns array of two double values, which
		/// represent function's input point (X and Y) encoded by the specified
		/// chromosome.</remarks>
		///
		public double[] TranslateNative( IChromosome chromosome )
		{
			// get chromosome's value
			ulong	val = ((BinaryChromosome) chromosome).Value;
			// chromosome's length
			int		length = ((BinaryChromosome) chromosome).Length;
			// length of X component
			int		xLength = length / 2;
			// length of Y component
			int		yLength = length - xLength;
			// X maximum value - equal to X mask
			ulong	xMax = 0xFFFFFFFFFFFFFFFF >> ( 64 - xLength );
			// Y maximum value
			ulong	yMax = 0xFFFFFFFFFFFFFFFF >> ( 64 - yLength );
			// X component
			double	xPart = val & xMax;
			// Y component;
			double	yPart = val >> xLength;

			// translate to optimization's funtion space
			double[] ret = new double[2];

			ret[0] = xPart * rangeX.Length / xMax + rangeX.Min;
			ret[1] = yPart * rangeY.Length / yMax + rangeY.Min;

			return ret;
		}

		/// <summary>
		/// Function to optimize
		/// </summary>
		///
		/// <param name="x">Function X input value</param>
		/// <param name="y">Function Y input value</param>
		/// 
		/// <returns>Returns function output value</returns>
		/// 
		/// <remarks>The method should be overloaded by inherited class to
		/// specify the optimization function.</remarks>
		///
		public abstract double OptimizationFunction( double x, double y );
	}
}
