using System;
using System.ComponentModel;

namespace IconMaker.ProgressDialog
{
	public class ProgressDialogContext
	{
		public BackgroundWorker Worker { get; }
		public DoWorkEventArgs Arguments { get; }

		public ProgressDialogContext(BackgroundWorker worker, DoWorkEventArgs arguments)
		{
		    Worker = worker ?? throw new ArgumentNullException(nameof(worker));
			Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
		}

		public bool CheckCancellationPending()
		{
			if(Worker.WorkerSupportsCancellation && Worker.CancellationPending)
				Arguments.Cancel = true;

			return Arguments.Cancel;
		}

		public void ThrowIfCancellationPending()
		{
			if(CheckCancellationPending())
				throw new ProgressDialogCancellationExcpetion();
		}

		public void Report(string message = null, string subLabel = null, double? value = null, double? max = null, double? min = null)
		{
		    if (Worker.WorkerReportsProgress)
		        Worker.ReportProgress(0, new ProgressStatus
		        {
		            Message = message,
                    SubLabel = subLabel,
                    Maximum = max,
                    Minimum = min,
                    Value = value,
		        });
		}

		public void ReportWithCancellationCheck(string message = null, string subLabel = null, double? value = null, double? max = null, double? min = null)
		{
			ThrowIfCancellationPending();

		    if (Worker.WorkerReportsProgress)
		        Worker.ReportProgress(0, new ProgressStatus
		        {
		            Message = message,
		            SubLabel = subLabel,
		            Maximum = max,
		            Minimum = min,
		            Value = value,
		        });
		}
	}
}
