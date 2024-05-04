using System.Runtime.InteropServices;


namespace WindowResizer.Utils
{
    /// <summary>
    /// Structure which represents window rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowRect
    {
        /// <summary>
        /// x position of upper-left corner.
        /// </summary>
        public int Left { get; set; }
        /// <summary>
        /// y position of upper-left corner.
        /// </summary>
        public int Top { get; set; }
        /// <summary>
        /// x position of lower-right corner.
        /// </summary>
        public int Right { get; set; }
        /// <summary>
        /// y position of lower-right corner.
        /// </summary>
        public int Bottom { get; set; }
        /// <summary>
        /// Alias of <see cref="Left"/>.
        /// </summary>
        public int X => Left;
        /// <summary>
        /// Alias of <see cref="Top"/>.
        /// </summary>
        public int Y => Top;
        /// <summary>
        /// Width of rectangle.
        /// </summary>
        public int Width => Right - Left;
        /// <summary>
        /// Height of rectangle.
        /// </summary>
        public int Height => Bottom - Top;
    }
}
