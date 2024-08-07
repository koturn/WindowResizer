namespace WindowResizer.Utils
{
    /// <summary>
    /// Second argument values of <see cref="WindowUtil.NativeMethods.SetWindowLongPtr(System.IntPtr, int, nuint)"/>
    /// </summary>
    internal enum GwlIndice : int
    {
        /// <summary>
        /// Sets a new extended window style.
        /// </summary>
        ExStyle = -20,
        /// <summary>
        /// Sets a new application instance handle.
        /// </summary>
        HInsrance = -6,
        /// <summary>
        /// Retrieves a handle to the parent window, if there is one.
        /// </summary>
        HWndParent = -8,
        /// <summary>
        /// Sets a new identifier of the child window. The window cannot be a top-level window.
        /// </summary>
        Id = -12,
        /// <summary>
        /// Sets a new window style.
        /// </summary>
        Style = -16,
        /// <summary>
        /// Sets the user data associated with the window.
        /// This data is intended for use by the application that created the window.
        /// Its value is initially zero.
        /// </summary>
        UserData = -21,
        /// <summary>
        /// Sets a new address for the window procedure.
        /// </summary>
        WndProc = -4,
        /// <summary>
        /// Sets the new pointer to the dialog box procedure (for 32bit process).
        /// </summary>
        DlgProc32 = MsgResult + 4,
        /// <summary>
        /// Sets the new pointer to the dialog box procedure (for 64bit version).
        /// </summary>
        DlgProc64 = MsgResult + 8,
        /// <summary>
        /// Sets the return value of a message processed in the dialog box procedure.
        /// </summary>
        MsgResult = 0,
        /// <summary>
        /// Sets new extra information that is private to the application, such as handles or pointers (for 32bit process).
        /// </summary>
        User32 = DlgProc32 + 4,
        /// <summary>
        /// Sets new extra information that is private to the application, such as handles or pointers (for 64bit process).
        /// </summary>
        User64 = DlgProc64 + 8
    }
}
