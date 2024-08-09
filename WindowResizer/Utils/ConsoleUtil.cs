using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;


namespace WindowResizer.Utils
{
    public partial class ConsoleUtil
    {
        /// <summary>
        /// Allocate console or show associated console window.
        /// </summary>
        /// <param name="autoFlush"></param>
        public static void AllocOrShowConsole(bool autoFlush = true)
        {
            if (NativeMethods.AllocConsole())
            {
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = autoFlush
                });
            }
            else
            {
                ShowConsole();
            }
        }

        /// <summary>
        /// Allocates a new console for the calling process and setup console.
        /// </summary>
        /// <exception cref="Win32Exception">Thrown then <see cref="NativeMethods.AllocConsole"/> failed.</exception>
        public static void AllocConsole(bool autoFlush = true)
        {
            if (!NativeMethods.AllocConsole())
            {
                ThrowLastWin32Exception(nameof(NativeMethods.AllocConsole) + " failed");
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = autoFlush
            });
        }

        /// <summary>
        /// Get console window handle.
        /// </summary>
        /// <returns>Console window handle.</returns>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetConsoleWindow"/> failed.</exception>
        public static IntPtr GetConsoleWindow()
        {
            var hWnd = NativeMethods.GetConsoleWindow();
            if (hWnd == IntPtr.Zero)
            {
                ThrowLastWin32Exception(nameof(NativeMethods.GetConsoleWindow) + " failed");
            }
            return hWnd;
        }

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.FreeConsole"/> failed.</exception>
        public static void FreeConsole()
        {
            if (!NativeMethods.FreeConsole())
            {
                ThrowLastWin32Exception(nameof(NativeMethods.FreeConsole) + " failed");
            }
        }

        /// <summary>
        /// Disable close buttons on console window.
        /// </summary>
        /// <exception cref="Win32Exception">Thrown then <see cref="NativeMethods.GetSystemMenu(IntPtr, bool)"/>
        /// or <see cref="NativeMethods.RemoveMenu(IntPtr, uint, MenuFlags)"/> failed.</exception>
        public static void DisableCloseButton()
        {
            var hWnd = GetConsoleWindow();
            var hMenu = WindowUtil.GetSystemMenu(hWnd, false);
            WindowUtil.RemoveMenu(hMenu, SysCommand.Close);
        }

        /// <summary>
        /// Disable Ctrl-C and Ctrl-Break on console window.
        /// </summary>
        public static void DisableExitKeys()
        {
            Console.TreatControlCAsInput = true;
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        /// <summary>
        /// <para>Callback method for <see cref="Console.CancelKeyPress"/>.</para>
        /// <para>Ignore <see cref="ConsoleSpecialKey.ControlBreak"/>.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConsoleCancelEventArgs"/> object that contains the event data.</param>
        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Show associated console window.
        /// </summary>
        /// <returns>true if success. false if failed to get console window handle.</returns>
        public static void ShowConsole()
        {
            ShowConsole(CmdShow.Show);
        }

        /// <summary>
        /// Hide associated console window.
        /// </summary>
        /// <returns>true if success. false if failed to get console window handle.</returns>
        public static void HideConsole()
        {
            ShowConsole(CmdShow.Hide);
        }

        /// <summary>
        /// Sets the console window's show state.
        /// </summary>
        /// <param name="cmdShow">Controls how the console window is to be shown.</param>
        private static void ShowConsole(CmdShow cmdShow)
        {
            WindowUtil.ShowWindow(GetConsoleWindow(), cmdShow);
        }

        /// <summary>
        /// Throw <see cref="Win32Exception"/> associated with last Win32 error.
        /// </summary>
        /// <param name="message">A detailed description of the error.</param>
        /// <exception cref="Win32Exception">Always thrown.</exception>
        [DoesNotReturn]
        private static void ThrowLastWin32Exception(string message)
        {
            ThrowWin32Exception(Marshal.GetLastWin32Error(), message);
        }

        /// <summary>
        /// Throw <see cref="Win32Exception"/>.
        /// </summary>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        /// <param name="message">A detailed description of the error.</param>
        /// <exception cref="Win32Exception">Always thrown.</exception>
        [DoesNotReturn]
        private static void ThrowWin32Exception(int error, string message)
        {
            throw new Win32Exception(error, $"{message}; [0x{error:X08}] {Marshal.GetPInvokeErrorMessage(error)}");
        }

        /// <summary>
        /// Provides some native methods.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// Allocates a new console for the calling process.
            /// </summary>
            /// <returns>
            /// <para>If the function succeeds, the return value is nonzero.</para>
            /// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/console/allocconsole"/></para>
            /// <para>
            /// A process can be associated with only one console,
            /// so the AllocConsole function fails if the calling process already has a console.
            /// A process can use the <see cref="FreeConsole"/> function to detach itself from its current console,
            /// then it can call AllocConsole to create a new console or AttachConsole to attach to another console.
            /// </para>
            /// If the calling process creates a child process, the child inherits the new console.
            /// <para>
            /// </para>
            /// <see cref="AllocConsole"/> initializes standard input, standard output, and standard error handles for the new console.
            /// The standard input handle is a handle to the console's input buffer,
            /// and the standard output and standard error handles are handles to the console's screen buffer.
            /// To retrieve these handles, use the GetStdHandle function.
            /// <para>
            /// This function is primarily used by a graphical user interface (GUI) application to create a console window.
            /// GUI applications are initialized without a console.
            /// Console applications are initialized with a console, unless they are created as detached processes
            /// (by calling the CreateProcess function with the DETACHED_PROCESS flag).
            /// </para>
            /// </remarks>
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool AllocConsole();

            /// <summary>
            /// Detaches the calling process from its console.
            /// </summary>
            /// <returns>
            /// <para>If the function succeeds, the return value is nonzero.</para>
            /// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/console/freeconsole"/></para>
            /// <para>
            /// A process can be attached to at most one console.
            /// A process can use the FreeConsole function to detach itself from its console.
            /// If other processes share the console, the console is not destroyed, but the process that called FreeConsole cannot refer to it.
            /// A console is closed when the last process attached to it terminates or calls FreeConsole.
            /// After a process calls FreeConsole, it can call the AllocConsole function to create a new console or AttachConsole to attach to another console.
            /// If the calling process is not already attached to a console, the FreeConsole request still succeeds.
            /// </para>
            /// </remarks>
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool FreeConsole();

            /// <summary>
            /// Retrieves the window handle used by the console associated with the calling process.
            /// </summary>
            /// <returns>The return value is a handle to the window used by the console associated with the calling process
            /// or <see cref="IntPtr.Zero"/> if there is no such associated console.</returns>
            /// <remarks>
            /// <para><seealso cref="https://learn.microsoft.com/en-us/windows/console/getconsolewindow"/></para>
            /// <para>This API is not recommended and does not have a virtual terminal equivalent.
            /// This decision intentionally aligns the Windows platform with other operating systems.
            /// This state is only relevant to the local user, session, and privilege context.
            /// Applications remoting via cross-platform utilities and transports like SSH may not work as expected if using this API.</para>
            /// <para>
            /// For an application that is hosted inside a pseudoconsole session, this function returns a window handle for message queue purposes only.
            /// The associated window is not displayed locally as the pseudoconsole is serializing all actions to a stream for presentation on another terminal window elsewhere.
            /// </para>
            /// </remarks>
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetConsoleWindow();
        }
    }
}
