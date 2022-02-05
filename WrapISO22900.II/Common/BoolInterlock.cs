
using System.Threading;

namespace ISO22900.II
{
    public class BoolInterlock
    {
        private int state = States.Unlocked;

        public bool IsUnlocked
        {
            get { return state == States.Unlocked; }
        }

        public bool IsLocked
        {
            get { return state == States.Locked; }
        }

        public bool Enter()
        {
            //Set state to Locked, and return the original state
            return Interlocked.Exchange(ref state, States.Locked) == States.Unlocked;
        }
        public void Exit()
        {
            state = States.Unlocked;
        }
        //Can't be an Enum due to being passed by ref in the Exchange() method
        private static class States
        {
            public const int Unlocked = 0;
            public const int Locked = 1;
        }
    }
}
