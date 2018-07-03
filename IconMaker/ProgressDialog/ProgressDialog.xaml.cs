//
// Parago Media GmbH & Co. KG, Jürgen Bäurle (jbaurle@parago.de)
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace IconMaker.ProgressDialog
{
    public partial class ProgressDialog
    {
        public static ProgressDialogContext Current { get; set; }

        volatile bool _isBusy;
        BackgroundWorker _worker;

        public string Label
        {
            get => TextLabel.Text;
            set => TextLabel.Text = value;
        }

        public string SubLabel
        {
            get => SubTextLabel.Text;
            set => SubTextLabel.Text = value;
        }

        public ProgressDialog(ProgressDialogSettings settings)
        {
            InitializeComponent();

            if (settings == null)
                settings = ProgressDialogSettings.WithLabelOnly;

            if (settings.ShowSubLabel)
            {
                Height = 140;
                MinHeight = 140;
                SubTextLabel.Visibility = Visibility.Visible;
            }
            else
            {
                Height = 110;
                MinHeight = 110;
                SubTextLabel.Visibility = Visibility.Collapsed;
            }

            CancelButton.Visibility = settings.ShowCancelButton ? Visibility.Visible : Visibility.Collapsed;
        }

        internal ProgressDialogResult Execute(object operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            ProgressDialogResult result = null;

            _isBusy = true;

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _worker.DoWork +=
                (s, e) =>
                {

                    try
                    {
                        Current = new ProgressDialogContext(s as BackgroundWorker, e);

                        switch (operation)
                        {
                            case Action action:
                                action();
                                break;
                            case Func<object> function:
                                e.Result = function();
                                break;
                            default:
                                throw new InvalidOperationException("Operation type is not supoorted");
                        }

                        // NOTE: Always do this check in order to avoid default processing after the Cancel button has been pressed.
                        // This call will set the Cancelled flag on the result structure.
                        Current.CheckCancellationPending();
                    }
                    catch (ProgressDialogCancellationExcpetion)
                    { }
                    catch (Exception)
                    {
                        if (!Current.CheckCancellationPending())
                            throw;
                    }
                    finally
                    {
                        Current = null;
                    }

                };

            _worker.RunWorkerCompleted +=
                (s, e) =>
                {

                    result = new ProgressDialogResult(e);

                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (SendOrPostCallback)delegate
                    {
                        _isBusy = false;
                        Close();
                    }, null);

                };

            _worker.ProgressChanged +=
                (s, e) =>
                {
                    if (_worker.CancellationPending)
                        return;

                    if (!(e.UserState is ProgressStatus state))
                        return;

                    Label = state.Message ?? Label;
                    SubLabel = state.SubLabel ?? SubLabel;
                    ProgressBar.Maximum = state.Maximum ?? ProgressBar.Maximum;
                    ProgressBar.Minimum = state.Minimum ?? ProgressBar.Minimum;

                    if (state.Value.HasValue)
                    {
                        if (double.IsPositiveInfinity(state.Value.Value))
                            ++ProgressBar.Value;
                        else if (double.IsNegativeInfinity(state.Value.Value))
                            --ProgressBar.Value;
                        else
                            ProgressBar.Value = state.Value.Value;
                    }
                    else
                        ProgressBar.IsIndeterminate = true;

                    if (state.Minimum.HasValue && state.Maximum.HasValue)
                        ProgressBar.IsIndeterminate = false;

                };

            _worker.RunWorkerAsync();

            ShowDialog();

            return result;
        }

        void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (_worker != null && _worker.WorkerSupportsCancellation)
            {
                SubLabel = "Please wait while process will be cancelled...";
                CancelButton.IsEnabled = false;
                _worker.CancelAsync();
            }
        }

        void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = _isBusy;
        }

        internal static ProgressDialogResult Execute(Window owner, string label, Action operation)
        {
            return ExecuteInternal(owner, label, operation, null);
        }

        internal static ProgressDialogResult Execute(Window owner, string label, Action operation, ProgressDialogSettings settings)
        {
            return ExecuteInternal(owner, label, operation, settings);
        }

        internal static ProgressDialogResult Execute(Window owner, string label, Func<object> operationWithResult)
        {
            return ExecuteInternal(owner, label, operationWithResult, null);
        }

        internal static ProgressDialogResult Execute(Window owner, string label, Func<object> operationWithResult, ProgressDialogSettings settings)
        {
            return ExecuteInternal(owner, label, operationWithResult, settings);
        }

        internal static void Execute(Window owner, string label, Action operation, Action<ProgressDialogResult> successOperation, Action<ProgressDialogResult> failureOperation = null, Action<ProgressDialogResult> cancelledOperation = null)
        {
            ProgressDialogResult result = ExecuteInternal(owner, label, operation, null);

            if (result.Cancelled && cancelledOperation != null)
                cancelledOperation(result);
            else if (result.OperationFailed && failureOperation != null)
                failureOperation(result);
            else
            {
                successOperation?.Invoke(result);
            }
        }

        internal static ProgressDialogResult ExecuteInternal(Window owner, string label, object operation, ProgressDialogSettings settings)
        {
            ProgressDialog dialog = new ProgressDialog(settings) { Owner = owner };

            if (!string.IsNullOrEmpty(label))
            {
                dialog.Label = label;
                dialog.Title = label;
            }

            return dialog.Execute(operation);
        }
    }
}
