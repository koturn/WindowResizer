namespace WindowResizer.Utils
{
    /// <summary>
    /// wParam values of WM_SYSCOMMAND message.
    /// </summary>
    /// <remarks>
    /// <see cref="https://learn.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
    /// </remarks>
    public enum SysCommand : uint
    {
        /// <summary>
        /// Indicates whether the screen saver is secure.
        /// </summary>
        IsSecure = 0x00000001,
        /// <summary>
        /// Sizes the window.
        /// </summary>
        Size = 0xf000,
        /// <summary>
        /// Moves the window.
        /// </summary>
        Move = 0xf010,
        /// <summary>
        /// Minimizes the window.
        /// </summary>
        Minimize = 0xf020,
        /// <summary>
        /// Maximizes the window.
        /// </summary>
        Maximize = 0xf030,
        /// <summary>
        /// Moves to the next window.
        /// </summary>
        NextWindow = 0xf040,
        /// <summary>
        /// Moves to the previous window.
        /// </summary>
        PrevWindow = 0xf050,
        /// <summary>
        /// Closes the window.
        /// </summary>
        Close = 0xf060,
        /// <summary>
        /// Scrolls vertically.
        /// </summary>
        VScroll = 0xf070,
        /// <summary>
        /// Scrolls horizontally.
        /// </summary>
        HScroll = 0xf080,
        /// <summary>
        /// Retrieves the window menu as a result of a mouse click.
        /// </summary>
        MouseMenu = 0xf090,
        /// <summary>
        /// Retrieves the window menu as a result of a keystroke.
        /// For more information, see the Remarks section.
        /// </summary>
        KeyMenu = 0xf100,
        /// <summary>
        /// Restores the window to its normal position and size.
        /// </summary>
        Restore = 0xf120,
        /// <summary>
        /// Activates the Start menu.
        /// </summary>
        TaskList = 0xf130,
        /// <summary>
        /// Executes the screen saver application specified in the [boot] section of the System.ini file.
        /// </summary>
        ScreenSave = 0xf140,
        /// <summary>
        /// Activates the window associated with the application-specified hot key.
        /// The lParam parameter identifies the window to activate.
        /// </summary>
        HotKey = 0xf150,
        /// <summary>
        /// Selects the default item; the user double-clicked the window menu.
        /// </summary>
        Default = 0xf160,
        /// <summary>
        /// <para>
        /// Sets the state of the display.
        /// This command supports devices that have power-saving features, such as a battery-powered personal computer.
        /// </para>
        /// <para>
        /// The lParam parameter can have the following values:
        /// <list type="bullet">
        ///   <item>-1 (the display is powering on)</item>
        ///   <item>1 (the display is going to low power)</item>
        ///   <item>2 (the display is being shut off)</item>
        /// </list>
        /// </para>
        /// </summary>
        MonitorPower = 0xf170,
        /// <summary>
        /// Changes the cursor to a question mark with a pointer.
        /// If the user then clicks a control in the dialog box, the control receives a WM_HELP message.
        /// </summary>
        ContextHelp = 0xf180
    }
}
