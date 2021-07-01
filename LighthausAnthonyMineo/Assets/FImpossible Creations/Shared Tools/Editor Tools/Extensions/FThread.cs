using System.Collections;
using System.Threading;

namespace FIMSpace.FTex
{
    /// <summary>
    /// FM: Base class for asynchronous thread operations
    /// </summary>
    public class FThread
    {
        private bool done = false;
        private object handle = new object();
        private Thread fThread = null;

        public bool IsDone
        {
            get
            {
                bool temp;
                lock (handle) temp = done;
                return temp;
            }

            set
            {
                lock (handle) done = value;
            }
        }

        public virtual void Start()
        {
            fThread = new Thread(Run);
            fThread.Start();
        }

        public virtual void Abort()
        {
            fThread.Abort();
        }

        protected virtual void ThreadOperations() { }

        protected virtual void OnFinished() { }

        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Run()
        {
            ThreadOperations();
            IsDone = true;
        }
    }
}
