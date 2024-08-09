using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;


namespace WindowResizer.Utils
{
    /// <summary>
    /// Provides utility methods for window.
    /// </summary>
    public static class WindowUtil
    {
        /// <summary>
        /// Changes the position and dimensions of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="x">The new position of the left side of the window.</param>
        /// <param name="y">The new position of the top of the window.</param>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        /// <param name="doRepaint">Indicates whether the window is to be repainted.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.MoveWindow(IntPtr, int, int, int, int, bool)"/> is failed.</exception>
        public static void MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool doRepaint)
        {
            if (!NativeMethods.MoveWindow(hWnd, x, y, width, height, doRepaint))
            {
                ThrowLastWin32Exception(nameof(NativeMethods.MoveWindow) + " failed");
            }
        }

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
                ThrowLastWin32Exception(nameof(NativeMethods.SetWindowPos) + " failed");
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
                ThrowLastWin32Exception(nameof(NativeMethods.SetWindowPos) + " failed");
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
                ThrowLastWin32Exception(nameof(NativeMethods.ShowWindow) + " failed");
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
                ThrowLastWin32Exception(nameof(NativeMethods.ShowWindow) + " failed");
            }
        }

        /// <summary>
        /// Display scpecified window as fullscreen.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.ShowWindow(nint, CmdShow)"/> is failed.</exception>
        public static nuint SetWindow(IntPtr hWnd, nuint style)
        {
            var oldStyle = NativeMethods.SetWindowLongPtr(hWnd, (int)GwlIndice.Style, style);
            if (oldStyle == 0)
            {
                ThrowLastWin32Exception(nameof(NativeMethods.SetWindowLongPtr) + " failed");
            }
            return oldStyle;
        }

        public static void SetForegroundWindow(IntPtr hWnd)
        {
            if (!NativeMethods.SetForegroundWindow(hWnd))
            {
                ThrowLastWin32Exception(nameof(NativeMethods.SetForegroundWindow) + " failed");
            }
        }

        /// <summary>
        /// Display scpecified window as fullscreen.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.ShowWindow(nint, CmdShow)"/> is failed.</exception>
        public static nuint MakeFullscreen(IntPtr hWnd, out IntPtr hPrevMenu)
        {
            Restore(hWnd);

            hPrevMenu = GetMenu(hWnd);
            SetMenu(hWnd, IntPtr.Zero);

            try
            {
                var hMonitor = MonitorFromWindow(hWnd);
                var monitorInfo = GetMonitorInfo(hMonitor);
                var screenRect = monitorInfo.RectMonitor;

                var prevWindowStyle = SetWindow(hWnd, (uint)(WindowStyleFlags.Visible | WindowStyleFlags.Popup));
                MoveWindow(hWnd, screenRect.X, screenRect.Y, screenRect.Width, screenRect.Height, true);
                return prevWindowStyle;
            }
            catch
            {
                SetMenu(hWnd, hPrevMenu);
                throw;
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
                ThrowLastWin32Exception(nameof(NativeMethods.ShowWindow) + " failed");
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
                ThrowLastWin32Exception(nameof(NativeMethods.GetWindowRect) + " failed");
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
                ThrowLastWin32Exception(nameof(NativeMethods.GetClientRect) + " failed");
            }
            return clientRect;
        }

        /// <summary>
        /// Retrieves a handle to the menu assigned to the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose menu handle is to be retrieved.</param>
        /// <returns>Handle to the menu.</returns>
        public static IntPtr GetMenu(IntPtr hWnd)
        {
            return NativeMethods.GetMenu(hWnd);
        }

        /// <summary>
        /// Assigns a new menu to the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to which the menu is to be assigned.</param>
        /// <param name="hMenu">A handle to the new menu. If this parameter is NULL, the window's current menu is removed.</param>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetMenu(IntPtr)"/> is failed.</exception>
        public static void SetMenu(IntPtr hWnd, IntPtr hMenu)
        {
            if (!NativeMethods.SetMenu(hWnd, hMenu))
            {
                ThrowLastWin32Exception(nameof(NativeMethods.SetMenu) + " failed");
            }
        }

        /// <summary>
        /// Retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window of interest.</param>
        /// <param name="flag">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <returns>A handle to the monitor.</returns>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.MonitorFromWindow(IntPtr, MonitorDefaultFlags)"/> is failed.</exception>
        public static IntPtr MonitorFromWindow(IntPtr hWnd, MonitorDefaultFlags flag = MonitorDefaultFlags.ToNearest)
        {
            var hMonitor = NativeMethods.MonitorFromWindow(hWnd, flag);
            if (hMonitor == IntPtr.Zero)
            {
                ThrowLastWin32Exception(nameof(NativeMethods.MonitorFromWindow) + " failed");
            }
            return hMonitor;
        }

        /// <summary>
        /// Retrieves information about a display monitor.
        /// </summary>
        /// <param name="hMonitor">A handle to the display monitor of interest.</param>
        /// <returns>Monitor information.</returns>
        /// <exception cref="Win32Exception">Thrown when <see cref="NativeMethods.GetMonitorInfo(nint, ref MonitorInfo)"/> is failed.</exception>
        public static MonitorInfo GetMonitorInfo(IntPtr hMonitor)
        {
            var monitorInfo = MonitorInfo.Create();
            if (!NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo))
            {
                ThrowLastWin32Exception(nameof(NativeMethods.GetMonitorInfo) + " failed");
            }
            return monitorInfo;
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
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"/></para>
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

            /// <summary>
            /// Changes the position and dimensions of the specified window.
            /// For a top-level window, the position and dimensions are relative to the upper-left corner of the screen.
            /// For a child window, they are relative to the upper-left corner of the parent window's client area.
            /// </summary>
            /// <param name="hWnd">A handle to the window.</param>
            /// <param name="x">The new position of the left side of the window.</param>
            /// <param name="y">The new position of the top of the window.</param>
            /// <param name="width">The new width of the window.</param>
            /// <param name="height">The new height of the window.</param>
            /// <param name="doRepaint">
            /// Indicates whether the window is to be repainted.
            /// If this parameter is true, the window receives a message.
            /// If the parameter is false, no repainting of any kind occurs.
            /// This applies to the client area, the nonclient area (including the title bar and scroll bars),
            /// and any part of the parent window uncovered as a result of moving a child window.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is true.</para>
            /// <para>If the function fails, the return value is false.
            /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-movewindow"/></para>
            /// <para>If the <paramref name="doRepaint"/> parameter is true,
            /// the system sends the WM_PAINT message to the window procedure immediately after moving the window
            /// (that is, the MoveWindow function calls the UpdateWindow function).
            /// If <paramref name="doRepaint"/> is false, the application must explicitly invalidate
            /// or redraw any parts of the window and parent window that need redrawing.</para>
            /// <para><see cref="MoveWindow(IntPtr, int, int, int, int, bool)"/> sends the WM_WINDOWPOSCHANGING, WM_WINDOWPOSCHANGED, WM_MOVE, WM_SIZE,
            /// and WM_NCCALCSIZE messages to the window.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool doRepaint);

            /// <summary>
            /// Changes an attribute of the specified window. The function also sets a value at the specified offset in the extra window memory.
            /// </summary>
            /// <param name="hWnd">
            /// A handle to the window and, indirectly, the class to which the window belongs.
            /// The <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> function fails if the process that owns the window specified
            /// by the hWnd parameter is at a higher process privilege in the UIPI hierarchy than the process the calling thread resides in.</param>
            /// <param name="nIndex">
            /// The zero-based offset to the value to be set.
            /// Valid values are in the range zero through the number of bytes of extra window memory, minus the size of a LONG_PTR.
            /// To set any other value, specify one of the <see cref="GwlIndice"/> values.</param>
            /// <param name="dwNewLong">The replacement value.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is the previous value of the specified offset.</para>
            /// <para>If the function fails, the return value is zero. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// <para>If the previous value is zero and the function succeeds,
            /// the return value is zero, but the function does not clear the last error information.
            /// To determine success or failure, clear the last error information by calling SetLastError with 0, then call <see cref="SetWindowLongPtr(nint, int, nuint)"/>.
            /// Function failure will be indicated by a return value of zero and a <see cref="Marshal.GetLastWin32Error"/> result that is nonzero.</para>
            /// </returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptra"/></para>
            /// <para>Certain window data is cached, so changes you make using <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/>
            /// will not take effect until you call the <see cref="SetWindowPos(IntPtr, IntPtr, int, int, int, int, SwpFlags)"/> function.</para>
            /// <para>If you use <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> with the <see cref="GwlIndice.WndProc"/> index
            /// to replace the window procedure, the window procedure must conform to the guidelines specified
            /// in the description of the WindowProc callback function.
            /// </para>
            /// <para>If you use <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> with the <see cref="GwlIndice.MsgResult"/> index
            /// to set the return value for a message processed by a dialog box procedure,
            /// the dialog box procedure should return TRUE directly afterward.
            /// Otherwise, if you call any function that results in your dialog box procedure receiving a window message,
            /// the nested window message could overwrite the return value you set by using <see cref="GwlIndice.MsgResult"/>.</para>
            /// <para>Calling <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> with the <see cref="GwlIndice.WndProc"/> index
            /// creates a subclass of the window class used to create the window.
            /// An application can subclass a system class, but should not subclass a window class created by another process.
            /// The <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> function creates the window subclass by changing the window procedure
            /// associated with a particular window class, causing the system to call the new window procedure instead of the previous one.
            /// An application must pass any messages not processed by the new window procedure to the previous window procedure by calling CallWindowProc.
            /// This allows the application to create a chain of window procedures.</para>
            /// <para>Reserve extra window memory by specifying a nonzero value in the cbWndExtra member
            /// of the WNDCLASSEX structure used with the RegisterClassEx function.</para>
            /// <para>Do not call <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> with the <see cref="GwlIndice.HWndParent"/> index
            /// to change the parent of a child window.
            /// Instead, use the SetParent function.</para>
            /// <para>If the window has a class style of CS_CLASSDC or CS_PARENTDC, do not set the extended window styles WS_EX_COMPOSITED or WS_EX_LAYERED.</para>
            /// <para>Calling <see cref="SetWindowLongPtr(IntPtr, int, nuint)"/> to set the style on a progressbar will reset its position.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern nuint SetWindowLongPtr(IntPtr hWnd, int nIndex, nuint dwNewLong);

            /// <summary>
            /// Retrieves a handle to the menu assigned to the specified window.
            /// </summary>
            /// <param name="hWnd">A handle to the window whose menu handle is to be retrieved.</param>
            /// <returns>The return value is a handle to the menu.
            /// If the specified window has no menu, the return value is <see cref="IntPtr.Zero"/>.
            /// If the window is a child window, the return value is undefined.</returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmenu"/></para>
            /// <para><see cref="GetMenu(IntPtr)"/> does not work on floating menu bars.
            /// Floating menu bars are custom controls that mimic standard menus; they are not menus.
            /// To get the handle on a floating menu bar, use the Active Accessibility APIs.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetMenu(IntPtr hWnd);

            /// <summary>
            /// Assigns a new menu to the specified window.
            /// </summary>
            /// <param name="hWnd">A handle to the window to which the menu is to be assigned.</param>
            /// <param name="hMenu">A handle to the new menu. If this parameter is NULL, the window's current menu is removed.</param>
            /// <returns>
            /// <para>If the function succeeds, the return value is true.</para>
            /// <para>If the function fails, the return value is false.
            /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-setmenu"/></para>
            /// <para>The window is redrawn to reflect the menu change.
            /// A menu can be assigned to any window that is not a child window.</para>
            /// <para>The <see cref="SetMenu(IntPtr, IntPtr)"/> function replaces the previous menu, if any, but it does not destroy it.
            /// An application should call the DestroyMenu function to accomplish this task.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

            /// <summary>
            /// The <see cref="MonitorFromWindow(nint, MonitorDefaultFlags)"/> function retrieves a handle
            /// to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window.
            /// </summary>
            /// <param name="hWnd">A handle to the window of interest.</param>
            /// <param name="flag">
            /// <para>Determines the function's return value if the window does not intersect any display monitor.</para>
            /// <para>This parameter can be one of the <see cref="MonitorDefaultFlags"/> values.</para>
            /// </param>
            /// <returns>
            /// <para>If the window intersects one or more display monitor rectangles,
            /// the return value is an HMONITOR handle to the display monitor that has the largest area of intersection with the window.</para>
            /// <para>If the window does not intersect a display monitor, the return value depends on the value of <paramref name="flag"/>.</para>
            /// </returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow"/></para>
            /// <para>If the window is currently minimized, <see cref="MonitorFromWindow(nint, MonitorDefaultFlags)"/> uses
            /// the rectangle of the window before it was minimized.</para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr MonitorFromWindow(IntPtr hWnd, MonitorDefaultFlags flag);

            /// <summary>
            /// The <see cref="GetMonitorInfo(nint, ref MonitorInfo)"/> function retrieves information about a display monitor.
            /// </summary>
            /// <param name="hMonitor">A handle to the display monitor of interest.</param>
            /// <param name="pMonitorInfo">
            /// <para>A pointer to a <see cref="MonitorInfo"/> or MONITORINFOEX structure that receives information about the specified display monitor.</para>
            /// <para>You must set the cbSize member of the structure to sizeof(MONITORINFO) or sizeof(MONITORINFOEX)
            /// before calling the <see cref="GetMonitorInfo(nint, ref MonitorInfo)"/> function.
            /// Doing so lets the function determine the type of structure you are passing to it.</para>
            /// <para>The MONITORINFOEX structure is a superset of the <see cref="MonitorInfo"/> structure.
            /// It has one additional member: a string that contains a name for the display monitor.
            /// Most applications have no use for a display monitor name, and so can save some bytes by using a <see cref="MonitorInfo"/> structure.</para>
            /// </param>
            /// <returns>
            /// <para>If the function succeeds, the return value is true.</para>
            /// <para>If the function fails, the return value is false.</para>
            /// </returns>
            /// <remarks>
            /// <para><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfow"/></para>
            /// </remarks>
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo monitorInfo);
        }
    }
}
