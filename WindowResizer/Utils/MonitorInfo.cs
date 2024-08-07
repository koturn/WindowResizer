using System;
using System.Runtime.InteropServices;


namespace WindowResizer.Utils
{
    /// <summary>
    /// <para>The <see cref="MonitorInfo"/> structure contains information about a display monitor.</para>
    /// <para>The <see cref="WindowUtil.NativeMethods.GetMonitorInfo(IntPtr, IntPtr)"/> function stores
    /// information in a <see cref="MonitorInfo"/> structure or a MONITORINFOEX structure.</para>
    /// <para>The <see cref="MonitorInfo"/> structure is a subset of the MONITORINFOEX structure.
    /// The MONITORINFOEX structure adds a string member to contain a name for the display monitor.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInfo
    {
        /// <summary>
        /// <para>The size of the structure, in bytes.</para>
        /// <para>Set this member to sizeof ( MONITORINFO ) before calling the <see cref="WindowUtil.NativeMethods.GetMonitorInfo"/> function.
        /// Doing so lets the function determine the type of structure you are passing to it.</para>
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// A <see cref="WindowRect"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public WindowRect RectMonitor { get; set; }
        /// <summary>
        /// A <see cref="WindowRect"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public WindowRect RectWork { get; set; }
        /// <summary>
        /// A set of flags that represent attributes of the display monitor.
        /// </summary>
        public MonitorInfoFlags Flags { get; set; }

        /// <summary>
        /// Create a instance of <see cref="MonitorInfo"/>.
        /// </summary>
        /// <returns>An instance of <see cref="MonitorInfo"/>.</returns>
        public static MonitorInfo Create()
        {
            return new MonitorInfo()
            {
                Size = Marshal.SizeOf(typeof(MonitorInfo))
            };
        }
    }
}
