namespace WindowResizer.Utils
{
    /// <summary>
    /// Second argument type of <see cref="WindowUtil.NativeMethods.MonitorFromWindow(System.IntPtr, MonitorDefaultFlags)"/>.
    /// </summary>
    public enum MonitorDefaultFlags : uint
    {
        /// <summary>
        /// Returns NULL (<see cref="System.IntPtr.Zero"/>).
        /// </summary>
        ToNull = 0x00000000,
        /// <summary>
        /// Returns a handle to the primary display monitor.
        /// </summary>
        ToNearest = 0x00000001,
        /// <summary>
        /// Returns a handle to the primary display monitor.
        /// </summary>
        ToPrimary = 0x00000002,
    }
}
