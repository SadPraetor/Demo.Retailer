using System.Diagnostics;

namespace Demo.Retailer.MigrationService
{

	public class ActionTimer : IDisposable
	{

		private readonly long _timestamp;
		private readonly Action<TimeSpan> _onElapsed;

		public ActionTimer(Action<TimeSpan> onElapsed)
		{			
			_timestamp = Stopwatch.GetTimestamp();
			_onElapsed = onElapsed;
		}
		public void Dispose()
		{
			_onElapsed(TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _timestamp));
		}
	}
}
