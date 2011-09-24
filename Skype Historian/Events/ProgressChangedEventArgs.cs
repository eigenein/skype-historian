using System;

namespace SkypeHistorian.Events
{
    public class ProgressChangedEventArgs : EventArgs
    {
        private readonly int value;

        private readonly int maximum;

        public ProgressChangedEventArgs(int value, int maximum)
        {
            this.value = value;
            this.maximum = maximum;
        }

        public int Value
        {
            get { return value; }
        }

        public int Maximum
        {
            get { return maximum; }
        }
    }
}
