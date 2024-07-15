using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace WindowResizer.Utils
{
    public class WindowUtil
    {
        /// <summary>
        /// Set window size.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="width">Window width.</param>
        /// <param name="height">Window height.</param>
        /// <param name="doCenterResize">Resize based on center of window or not.</param>
        /// <param name="doActivate">Activate window on resize or not.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetWindowRect"/>
        /// or <see cref="NativeMethods.SetWindowPos"/> is failed.</exception>
        public static void SetWindowSize(IntPtr hWnd, int width, int height, bool doCenterResize = true, bool doActivate = true)
        {
            Restore(hWnd);

            var x = 0;
            var y = 0;
            var flags = SwpFlags.NoMove;
            if (doCenterResize)
            {
                var windowRect = GetWindowRect(hWnd);
                x = windowRect.Left + (windowRect.Width - width) / 2;
                y = windowRect.Top + (windowRect.Height - height) / 2;
                flags = SwpFlags.None;
            }

            if (!doActivate)
            {
                flags |= SwpFlags.NoActivate;
            }

            if (!NativeMethods.SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, flags))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetWindowPos failed");
            }
        }

        /// <summary>
        /// Set window size based on client size.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="width">Window width.</param>
        /// <param name="height">Window height.</param>
        /// <param name="doCenterResize">Resize based on center of window or not.</param>
        /// <param name="doActivate">Activate window on resize or not.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetWindowRect"/>,
        /// <see cref="NativeMethods.GetClientRect"/> or <see cref="NativeMethods.SetWindowPos"/> is failed.</exception>
        public static void SetClientSize(IntPtr hWnd, int width, int height, bool doCenterResize = true, bool doActivate = true)
        {
            Restore(hWnd);

            var windowRect = GetWindowRect(hWnd);
            var clientRect = GetClientRect(hWnd);
            var windowWidth = width + windowRect.Width - clientRect.Width;
            var windowHeight = height + windowRect.Height - clientRect.Height;

            var x = 0;
            var y = 0;
            var flags = SwpFlags.NoMove;
            if (doCenterResize)
            {
                x = windowRect.Left + (windowRect.Width - windowWidth) / 2;
                y = windowRect.Top + (windowRect.Height - windowHeight) / 2;
                flags = SwpFlags.None;
            }

            if (!doActivate)
            {
                flags |= SwpFlags.NoActivate;
            }

            if (!NativeMethods.SetWindowPos(hWnd, IntPtr.Zero, x, y, windowWidth, windowHeight, flags))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetWindowPos failed");
            }
        }

        /// <summary>
        /// Display scpecified window as a maximized window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.ShowWindow(nint, CmdShow)"/> is failed.</exception>
        public static void Maximize(IntPtr hWnd)
        {
            if (!NativeMethods.ShowWindow(hWnd, CmdShow.ShowMaximized))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ShowWindow failed");
            }
        }

        /// <summary>
        /// Display scpecified window as a minimized window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.ShowWindow(nint, CmdShow)"/> is failed.</exception>
        public static void Minimize(IntPtr hWnd)
        {
            if (!NativeMethods.ShowWindow(hWnd, CmdShow.ShowMinimized))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ShowWindow failed");
            }
        }

        /// <summary>
        /// Activate and restore scpecified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.ShowWindow(nint, CmdShow)"/> is failed.</exception>
        public static void Restore(IntPtr hWnd)
        {
            if (!NativeMethods.ShowWindow(hWnd, CmdShow.Restore))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ShowWindow failed");
            }
        }

        /// <summary>
        /// Get window rectangle.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <returns>Window rectangle.</returns>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetWindowRect"/> is failed.</exception>
        public static WindowRect GetWindowRect(IntPtr hWnd)
        {
            if (!NativeMethods.GetWindowRect(hWnd, out var windowRect))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetWindowRect failed");
            }
            return windowRect;
        }

        /// <summary>
        /// Get client rectangle.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <returns>Client rectangle.</returns>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetClientRect"/> is failed.</exception>
        public static WindowRect GetClientRect(IntPtr hWnd)
        {
            if (!NativeMethods.GetClientRect(hWnd, out var clientRect))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetClientRect failed");
            }
            return clientRect;
        }

        /// <summary>
        /// Provides some P/Invoke methods.
        /// </summary>
        internal static class NativeMethods
        {
            /// <summary>
            /// Changes the size, position, and Z order of a child, pop-up, or top-level window.
            /// These windows are ordered according to their appearance on the screen.
            /// The topmost window receives the highest rank and is the first window in the Z order.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order.
            /// This parameter must be a window handle or one of the following values.
            /// For more information about how this parameter is used, see the following Remarks section.</param>
            /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
            /// <param name="y">The new position of the top of the window, in client coordinates.</param>
            /// <param name="cx">The new width of the window, in pixels.</param>
            /// <param name="cy">The new height of the window, in pixels.</param>
            /// <param name="flags">The window sizing and positioning flags. This parameter can be a combination of the <see cref="SwpFlags"/> values.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is true.</para>
            /// <para>If the function fails, the return value is false.
            /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"/></para>
            /// <para>As part of the Vista re-architecture, all services were moved off the interactive desktop into Session 0.
            /// hwnd and window manager operations are only effective inside a session and cross-session attempts to manipulate the hwnd will fail.
            /// For more information, see The Windows Vista Developer Story: Application Compatibility Cookbook.</para>
            /// <para>If you have changed certain window data using SetWindowLong, you must call SetWindowPos for the changes to take effect.
            /// Use the following combination for uFlags: <see cref="SwpFlags.NoMove"/> | <see cref="SwpFlags.NoSize"/> | <see cref="SwpFlags.NoZOrder"/> | <see cref="SwpFlags.FrameChanged"/>.</para>
            /// <para>A window can be made a topmost window either by setting the <paramref name="hWndInsertAfter"/> parameter to HWND_TOPMOST and ensuring that the <see cref="SwpFlags.NoZOrder"/> flag is not set,
            /// or by setting a window's position in the Z order so that it is above any existing topmost windows.
            /// When a non-topmost window is made topmost, its owned windows are also made topmost.
            /// Its owners, however, are not changed.</para>
            /// <para>If neither the <see cref="SwpFlags.NoActivate"/> nor <see cref="SwpFlags.NoZOrder"/> flag is specified
            /// (that is, when the application requests that a window be simultaneously activated and its position in the Z order changed),
            /// the value specified in <paramref name="hWndInsertAfter"/> is used only in the following circumstances.
            /// <list type="bullet">
            ///   <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag is specified in <paramref name="hWndInsertAfter"/>.</item>
            ///   <item>The window identified by hWnd is not the active window.</item>
            /// </list>
            /// </para>
            /// <para>An application cannot activate an inactive window without also bringing it to the top of the Z order.
            /// Applications can change an activated window's position in the Z order without restrictions,
            /// or it can activate a window and then move it to the top of the topmost or non-topmost windows.</para>
            /// <para>If a topmost window is repositioned to the bottom (HWND_BOTTOM) of the Z order or after any non-topmost window, it is no longer topmost.
            /// When a topmost window is made non-topmost, its owners and its owned windows are also made non-topmost windows.</para>
            /// <para>A non-topmost window can own a topmost window, but the reverse cannot occur.
            /// Any window (for example, a dialog box) owned by a topmost window is itself made a topmost window,
            /// to ensure that all owned windows stay above their owner.</para>
            /// <para>If an application is not in the foreground, and should be in the foreground, it must call the SetForegroundWindow function.</para>
            /// <para>To use SetWindowPos to bring a window to the top, the process that owns the window must have SetForegroundWindow permission.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SwpFlags flags);

            /// <summary>
            /// Retrieves the dimensions of the bounding rectangle of the specified window.
            /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="rect">An output <see cref="WindowRect"/> structure that receives the screen coordinates
            /// of the upper-left and lower-right corners of the window.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is true.</para>
            /// <para>If the function fails, the return value is false.
            /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect"/></para>
            /// <para>In conformance with conventions for the <see cref="WindowRect"/> structure, the bottom-right coordinates of the returned rectangle are exclusive.
            /// In other words, the pixel at (right, bottom) lies immediately outside the rectangle.</para>
            /// <para><see cref="GetWindowRect"/> is virtualized for DPI.</para>
            /// <para>In Windows Vista and later, the Window Rect now includes the area occupied by the drop shadow.</para>
            /// <para>Calling <see cref="GetWindowRect"/> will have different behavior depending on whether the window has ever been shown or not.
            /// If the window has not been shown before, <see cref="GetWindowRect"/> will not include the area of the drop shadow.</para>
            /// <para>To get the window bounds excluding the drop shadow, use DwmGetWindowAttribute, specifying DWMWA_EXTENDED_FRAME_BOUNDS.
            /// Note that unlike the Window Rect, the DWM Extended Frame Bounds are not adjusted for DPI.
            /// Getting the extended frame bounds can only be done after the window has been shown at least once.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowRect(IntPtr hWnd, out WindowRect rect);

            /// <summary>
            /// Retrieves the coordinates of a window's client area.
            /// The client coordinates specify the upper-left and lower-right corners of the client area.
            /// Because client coordinates are relative to the upper-left corner of a window's client area,
            /// the coordinates of the upper-left corner are (0,0).
            /// </summary>
            /// <param name="hWnd">A handle to the window whose client coordinates are to be retrieved.</param>
            /// <param name="rect">An output <see cref="WindowRect"/> structure that receives the client coordinates.
            /// The left and top members are zero.
            /// The right and bottom members contain the width and height of the window.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is nonzero.</para>
            /// <para>If the function fails, the return value is zero.
            /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclientrect"/></para>
            /// <para>In conformance with conventions for the <see cref="WindowRect"/> structure,
            /// the bottom-right coordinates of the returned rectangle are exclusive.
            /// In other words, the pixel at (right, bottom) lies immediately outside the rectangle.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetClientRect(IntPtr hWnd, out WindowRect rect);

            /// <summary>
            /// Brings the thread that created the specified window into the foreground and activates the window.
            /// Keyboard input is directed to the window, and various visual cues are changed for the user.
            /// The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.
            /// </summary>
            /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
            /// <returns>
            /// <para>If the window was brought to the foreground, the return value is true.</para>
            /// <para>If the window was not brought to the foreground, the return value is false.</para>
            /// </returns>
            /// <remarks>
            /// <para><seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow"/></para>
            /// <para>The system restricts which processes can set the foreground window.
            /// A process can set the foreground window by calling <see cref="SetForegroundWindow"/> only if:
            ///<list type="bullet">
            ///   <item>
            ///     All of the following conditions are true:
            ///     <list type="bullet">
            ///       <item>The calling process belongs to a desktop application, not a UWP app or a Windows Store app designed for Windows 8 or 8.1.</item>
            ///       <item>The foreground process has not disabled calls to SetForegroundWindow by a previous call to the LockSetForegroundWindow function.</item>
            ///       <item>The foreground lock time-out has expired (see SPI_GETFOREGROUNDLOCKTIMEOUT in SystemParametersInfo).</item>
            ///       <item>No menus are active.</item>
            ///     </list>
            ///   </item>
            ///   <item>
            ///     Additionally, at least one of the following conditions is true:
            ///     <list type="bullet">
            ///       <item>The calling process is the foreground process.</item>
            ///       <item>The calling process was started by the foreground process.</item>
            ///       <item>There is currently no foreground window, and thus no foreground process.</item>
            ///       <item>The calling process received the last input event.</item>
            ///       <item>Either the foreground process or the calling process is being debugged.</item>
            ///     </list>
            ///   </item>
            /// </list>
            /// </para>
            /// <para>It is possible for a process to be denied the right to set the foreground window even if it meets these conditions.</para>
            /// <para>An application cannot force a window to the foreground while the user is working with another window.
            /// Instead, Windows flashes the taskbar button of the window to notify the user.</para>
            /// <para>A process that can set the foreground window can enable another process to set the foreground window by calling the AllowSetForegroundWindow function.
            /// The process specified by the dwProcessId parameter to AllowSetForegroundWindow loses the ability
            /// to set the foreground window the next time that either the user generates input,
            /// unless the input is directed at that process, or the next time a process calls AllowSetForegroundWindow,
            /// unless the same process is specified as in the previous call to AllowSetForegroundWindow.</para>
            /// <para>The foreground process can disable calls to SetForegroundWindow by calling the LockSetForegroundWindow function.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            /// <summary>
            /// Sets the specified window's show state.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="cmdShow">
            /// Controls how the window is to be shown.
            /// This parameter is ignored the first time an application calls <see cref="ShowWindow"/>, if the program that launched the application provides a STARTUPINFO structure.
            /// Otherwise, the first time <see cref="ShowWindow"/> is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter.
            /// </param>
            /// <returns>
            /// <para>If the window was previously visible, the return value is nonzero.</para>
            /// <para>If the window was previously hidden, the return value is zero.</para>
            /// </returns>
            /// <remarks>
            /// <para><see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"/></para>
            /// <para>To perform certain special effects when showing or hiding a window, use AnimateWindow.</para>
            /// <para>The first time an application calls <see cref="ShowWindow"/>, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter.
            /// Subsequent calls to <see cref="ShowWindow"/> must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.</para>
            /// <para>As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to <see cref="ShowWindow"/>
            /// if the program that launched the application specifies startup information in the structure.
            /// In this case, <see cref="ShowWindow"/> uses the information specified in the STARTUPINFO structure to show the window.
            /// On subsequent calls, the application must call <see cref="ShowWindow"/> with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application.
            /// This behavior is designed for the following situations:
            /// <list type="bullet">
            ///   <item>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.</item>
            ///   <item>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared,
            ///   and later call <see cref="ShowWindow"/> with the <see cref="CmdShow.Show"/> flag set to make it visible.</item>
            /// </list>
            /// </para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool ShowWindow(IntPtr hWnd, CmdShow cmdShow);
        }
    }
}
