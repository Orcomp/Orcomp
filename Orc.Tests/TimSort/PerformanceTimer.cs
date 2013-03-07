﻿namespace Orc.Tests.TimSort
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Threading;

    #region class PerformanceTimer

	/// <summary>
	/// Encapsulates access to system performance counter.
	/// </summary>
	public class PerformanceTimer
	{
		#region fields

		private int m_StartedCount;
		private long m_StartTime;
		private long m_StopTime;
		private long m_Frequency;
		private long m_ElapsedTime;

		#endregion

		#region imports

		[DllImport("kernel32.dll")]
		internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("kernel32.dll")]
		internal static extern bool QueryPerformanceFrequency(out long lpFrequency);

		#endregion

		#region properties

		/// <summary>
		/// Returns the duration of the timer (in seconds)
		/// </summary>
		/// <value>The elapsed time in seconds.</value>
		public double Elapsed
		{
			get { return (double)(this.m_ElapsedTime) / (double)this.m_Frequency; }
		}

		/// <summary>
		/// Gets the elapsed TimeSpan.
		/// </summary>
		/// <value>The elapsed TimeSpan.</value>
		public TimeSpan ElapsedTimeSpan
		{
			get { return TimeSpan.FromSeconds(this.Elapsed); }
		}

		#endregion

		#region class PerformanceTimerScope

		/// <summary>
		/// Utility class to use with <c>using</c> keyword. 
		/// See <see cref="PerformanceTimer.Scope()"/> for more information.
		/// Basicly, class which starts the timer when instantiated and stops then timer when disposed.
		/// </summary>
		internal class PerformanceTimerScope: IDisposable
		{
			#region fields

			private readonly PerformanceTimer m_Timer;

			#endregion

			#region constructor

			/// <summary>
			/// Initializes a new instance of the <see cref="PerformanceTimerScope"/> class.
			/// </summary>
			/// <param name="timer">The timer to control.</param>
			public PerformanceTimerScope(PerformanceTimer timer)
			{
				this.m_Timer = timer;
				timer.Start();
			}

			#endregion

			#region IDisposable Members

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// In this case it just stops controlled timer.
			/// </summary>
			public void Dispose()
			{
				this.m_Timer.Stop();
			}

			#endregion
		}

		#endregion

		#region constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceTimer"/> class.
		/// Timers is not started.
		/// </summary>
		public PerformanceTimer()
		{
			this.Reset();
		}

		#endregion

		#region public interface

		/// <summary>
		/// Resets the timer. Timer is not started.
		/// </summary>
		public void Reset()
		{
			this.m_StartTime = this.m_StartTime = this.m_ElapsedTime = 0;
			this.m_StartedCount = 0;
		}

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void Start()
		{
			if (this.m_StartedCount == 0)
			{
				// lets waiting threads do their work
				// this thread will more likely go uninterrupted when doing measurement
				Thread.Sleep(0);

				if (!QueryPerformanceFrequency(out this.m_Frequency))
#if (SILVERLIGHT)
                    throw new ExternalException("QueryPerformanceFrequency failed.");
#else
					throw new Win32Exception();
#endif

				if (!QueryPerformanceCounter(out this.m_StartTime))
#if (SILVERLIGHT)
                    throw new ExternalException("QueryPerformanceCounter failed.");
#else
					throw new Win32Exception();
#endif
                    
			}
			this.m_StartedCount++;
		}

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void Start(bool reset)
		{
			if (reset)
			{
				if (this.m_StartedCount > 0)
				{
					throw new ArgumentException("Cannot restart nested timer");
				}
				this.Reset();
			}
			this.Start();
		}

		/// <summary>
		/// Stops the timer. Elapsed time is added to total elapsed time.
		/// </summary>
		public double Stop()
		{
			if (this.m_StartedCount <= 0)
				return this.Elapsed;

			QueryPerformanceCounter(out this.m_StopTime);
			this.m_StartedCount--;
			this.m_ElapsedTime += this.m_StopTime - this.m_StartTime;
			this.m_StartTime = this.m_StopTime;
			return this.Elapsed;
		}

		/// <summary>
		/// Simple utility to use with <c>using</c> keyword. See example.
		/// Timer is started when object is created and stops when object is disposed.
		/// </summary>
		/// <example><code>
		/// timer = new PerformanceTimer();
		/// while (!queue.Empty)
		/// {
		///   object element = queue.Dequeue();
		///   using (timer.Scope())
		///   {
		///     operation(element); // only time of this line counts towards total time
		///   }
		/// }
		/// </code></example>
		/// <returns><see cref="IDisposable"/> object. You don't have to handle it.</returns>
		public IDisposable Scope()
		{
			return new PerformanceTimerScope(this);
		}

		/// <summary>Measures the time spent executing action.</summary>
		/// <param name="name">The name.</param>
		/// <param name="repeat">Number of repetiotions.</param>
		/// <param name="action">The action.</param>
		/// <param name="size">The size.</param>
		public static void Debug(string name, int repeat, Action action, double size = 0)
		{
			var timer = new PerformanceTimer();
			using (timer.Scope())
			{
				action();
			}
			var time = timer.Elapsed / repeat;
			Console.WriteLine("{0}: {1:0.0000}ms ({2:0.00}/s)", name, time * 1000, size / time);
		}

		/// <summary>Measures the time spent executing action.</summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="name">The name.</param>
		/// <param name="repeat">Number of repetiotions.</param>
		/// <param name="action">The action.</param>
		/// <param name="size">The size.</param>
		/// <returns>Value returned by action.</returns>
		public static T Debug<T>(string name, int repeat, Func<T> action, double size = 0)
		{
			var result = default(T);
			var timer = new PerformanceTimer();
			using (timer.Scope())
			{
				for (int i = 0; i < repeat; i++)
				{
					result = action();
				}
			}
			var time = timer.Elapsed / repeat;
			Console.WriteLine("{0}: {1:0.0000}ms ({2:0.00}/s)", name, time * 1000, size / time);
			return result;
		}

		#endregion
	}

	#endregion

}
