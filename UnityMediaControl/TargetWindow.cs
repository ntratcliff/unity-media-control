namespace UnityMediaControl
{
    internal struct TargetWindow
    {
        public string Class { get; private set; }
        public string Title { get; private set; }
        public int Handle; // used for keeping track of windows if name changes

        public TargetWindow(string hWindClass, string hWindTitle)
        {
            Class = hWindClass;
            Title = hWindTitle;
            Handle = User32Interop.HWND_NONE;
        }

        public override string ToString()
        {
            return Title + "[" + Class + "]" + "(" + Handle + ")";
        }
    }
}
