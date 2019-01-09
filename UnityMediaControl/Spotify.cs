using System;

namespace EditorMediaManager
{
    class Spotify
    {
        /// <summary>
        /// Name of the Spotify process
        /// </summary>
        private const string processName = "Spotify";

        /// <summary>
        /// Name of the window when Spotify is paused
        /// </summary>
        private const string pausedWindowName = "Spotify";

        private static Spotify instance;
        public static Spotify Instance
        {
            get
            {
                if (instance == null)
                {
                    // get a new instance 
                    instance = new Spotify();
                }

                return instance;
            }
        }

        public IntPtr WindowHandle { get; private set; }

        /// <summary>
        /// Whether or not the Spotify process is running
        /// </summary>
        public bool IsRunning
        {
            get { return WindowHandle != IntPtr.Zero; }
        }

        /// <summary>
        /// Whether or not the Spotify process is playing
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                if (!IsRunning) return false;

                if (!string.IsNullOrEmpty(Title))
                {
                    return !string.Equals(
                        Title,
                        pausedWindowName,
                        StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The Spotify window title. Will be null if Spotify is not running
        /// </summary>
        public string Title { get; private set; }

        public Spotify()
        {
            Refresh();
        }

        /// <summary>
        /// Refreshes cached values
        /// </summary>
        public void Refresh()
        {
            RefreshWindowHandle();
            Title = GetWindowTitle();
        }

        /// <summary>
        /// Get the title of the Spotify window
        /// </summary>
        /// <returns>The title of the Spotify window, or null if Spotify isn't running</returns>
        private string GetWindowTitle()
        {
            if (!IsRunning) return null;

            return User32Interop.GetWindowText(WindowHandle);
        }

        /// <summary>
        /// Attempts to get the window handle by process name
        /// </summary>
        private void RefreshWindowHandle()
        {
            WindowHandle = User32Interop.GetWindowByProcessName(processName);
        }
    }
}
