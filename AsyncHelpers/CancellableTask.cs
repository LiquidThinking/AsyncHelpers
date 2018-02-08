using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHelpers
{
	public class CancellableTask : TaskCompletionSource<bool>, IDisposable
	{
		private CancellationTokenRegistration registration;

		public CancellableTask( CancellationToken cancelToken )
		{
			registration = cancelToken.Register( OnCancellation, false );
		}

		private void OnCancellation()
		{
			SetCanceled();
		}

		private readonly bool disposed = false;

		public void Dispose()
		{
			if ( !disposed )
			{
				registration.Dispose();
			}
		}
	}
}