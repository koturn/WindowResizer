using System;


namespace WindowResizer.Utils
{
    /// <summary>
    /// Third argument values of <see cref="ConsoleUtil.NativeMethods.RemoveMenu(IntPtr, uint, MenuFlags)"/>
    /// </summary>
    internal enum MenuFlags : uint
    {
        /// <summary>
        /// Indicates that uPosition gives the identifier of the menu item.
        /// If neither the <see cref="ByCommand"/> nor <see cref="ByPosition"/> flag is specified,
        /// the  <see cref="ByCommand"/> flag is the default flag.
        /// </summary>
        ByCommand = 0x00000000,
        /// <summary>
        /// Indicates that uPosition gives the zero-based relative position of the menu item.
        /// </summary>
        ByPosition = 0x00000400
    }
}
