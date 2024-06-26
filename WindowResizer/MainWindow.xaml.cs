using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using WindowResizer.Utils;


namespace WindowResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowRect? _prevWindowRect;

        private Task<Process[]>? _initGetProcessTask;

        /// <summary>
        /// Initialize window components.
        /// </summary>
        public MainWindow()
        {
            _initGetProcessTask = Task.Run(() =>
            {
                return Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero).OrderBy(p => p.ProcessName).ToArray();
            });
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when <see cref="MainWindow"/> loaded.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_initGetProcessTask != null)
            {
                await RefreshProcessesAsync(_initGetProcessTask);
                _initGetProcessTask = null;
            }
        }

        private void ComboBoxProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var p = GetSelectedProcess();
                if (p == null)
                {
                    return;
                }
                var hWnd = p.MainWindowHandle;
                RefreshWindowSizeNud(hWnd);
                RefreshClientSizeNud(hWnd);

                _prevWindowRect = null;
                _buttonUndoResize.IsEnabled = false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    this,
                    ex.ToString(),
                    ex.GetType().ToString(),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            await RefreshProcessesAsync();
        }

        private void ButtonResize_Click(object sender, RoutedEventArgs e)
        {
            var p = GetSelectedProcess();
            if (p == null)
            {
                return;
            }
            var hWnd = p.MainWindowHandle;
            var doActivate = _checkBoxActivate.IsChecked.GetValueOrDefault();
            try
            {
                _prevWindowRect = WindowUtil.GetWindowRect(hWnd);

                if (_rbWindowBased.IsChecked.GetValueOrDefault())
                {
                    WindowUtil.SetWindowSize(hWnd, (int)_nudWindowWidth.Value, (int)_nudWindowHeight.Value, doActivate);
                }
                else
                {
                    WindowUtil.SetClientSize(hWnd, (int)_nudClientWidth.Value, (int)_nudClientHeight.Value, doActivate);
                }
                if (doActivate)
                {
                    WindowUtil.NativeMethods.SetForegroundWindow(hWnd);
                }

                RefreshWindowSizeNud(hWnd);
                RefreshClientSizeNud(hWnd);

                _buttonUndoResize.IsEnabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    this,
                    ex.ToString(),
                    ex.GetType().ToString(),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ButtonUndoResize_Click(object sender, RoutedEventArgs e)
        {
            if (!_prevWindowRect.HasValue)
            {
                return;
            }

            var p = GetSelectedProcess();
            if (p == null)
            {
                return;
            }
            var hWnd = p.MainWindowHandle;
            var doActivate = _checkBoxActivate.IsChecked.GetValueOrDefault();

            var rect = _prevWindowRect.Value;
            WindowUtil.SetWindowSize(hWnd, rect.Width, rect.Height, doActivate);

            RefreshWindowSizeNud(hWnd);
            RefreshClientSizeNud(hWnd);

            _prevWindowRect = null;
            _buttonUndoResize.IsEnabled = false;
        }

        private Process? GetSelectedProcess()
        {
            var p = (Process)_comboBoxProcess.SelectedItem;
            if (p == null)
            {
                System.Windows.MessageBox.Show(
                    this,
                    "No process is selected",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            p.Refresh();

            if (p.HasExited)
            {
                System.Windows.MessageBox.Show(
                    this,
                    $"Selected process has been exited",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            if (p.MainWindowHandle == IntPtr.Zero)
            {
                System.Windows.MessageBox.Show(
                    this,
                    "Selected process has no window handle",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return p;
        }

        private async Task RefreshProcessesAsync()
        {
            await RefreshProcessesAsync(Task.Run(() =>
            {
                return Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero).OrderBy(p => p.ProcessName).ToArray();
            }));
        }

        private async Task RefreshProcessesAsync(Task<Process[]> getProcessTask)
        {
            var comboBox = _comboBoxProcess;
            comboBox.IsEnabled = false;

            var prevSelectedProc = (Process)_comboBoxProcess.SelectedItem;

            var procs = await getProcessTask;

            comboBox.SelectionChanged -= ComboBoxProcess_SelectionChanged;
            var items = comboBox.Items;
            items.Clear();
            var index = -1;
            for (var i = 0; i < procs.Length; i++)
            {
                var p = procs[i];
                items.Add(p);
                if (index == -1 && prevSelectedProc != null
                    && p.Id == prevSelectedProc.Id && p.SessionId == prevSelectedProc.SessionId && p.ProcessName == prevSelectedProc.ProcessName)
                {
                    index = i;
                }
            }
            comboBox.SelectionChanged += ComboBoxProcess_SelectionChanged;
            if (procs.Length > 0)
            {
                comboBox.SelectedIndex = Math.Max(0, index);
            }

            comboBox.IsEnabled = true;
        }

        private void RefreshWindowSizeNud(IntPtr hWnd)
        {
            var windowRect = WindowUtil.GetWindowRect(hWnd);
            _nudWindowWidth.Value = windowRect.Width;
            _nudWindowHeight.Value = windowRect.Height;
        }

        private void RefreshClientSizeNud(IntPtr hWnd)
        {
            var clientRect = WindowUtil.GetClientRect(hWnd);
            _nudClientWidth.Value = clientRect.Width;
            _nudClientHeight.Value = clientRect.Height;
        }
    }
}
