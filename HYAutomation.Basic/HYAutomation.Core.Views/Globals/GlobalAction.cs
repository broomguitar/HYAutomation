namespace HYAutomation.Core.Views
{
    public class GlobalAction
    {
        private static readonly object _lockObj = new object();
        private static GlobalAction _instance;
        public static GlobalAction Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new GlobalAction();
                        }
                    }
                }
                return _instance;
            }
        }
        private GlobalAction() { }
    }
}
