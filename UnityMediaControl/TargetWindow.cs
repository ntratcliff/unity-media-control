namespace UnityMediaControl
{
    internal struct TargetWindow
    {
        public string Class { get; private set; }
        public string Title { get; private set; }

        public TargetWindow(string hWindClass, string hWindTitle)
        {
            Class = hWindClass;
            Title = hWindTitle;
        }
    }
}
