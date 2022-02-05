#region License

// /*
// MIT License
// 
// Copyright (c) 2022 Joerg Frank
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion


//https://blog.stephencleary.com/2009/08/how-to-implement-idisposable-and.html
//https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

using System;
using System.IO;
using System.Threading;

namespace ISO22900.II
{
    public abstract class ManagedDisposable : IDisposable
    {
        /// <summary>
        /// To detect redundant calls
        /// </summary>
        private BoolInterlock DisposalInterlock { get; } = new BoolInterlock();

        internal delegate void DisposePrototype();
        /// <summary>
        /// Event that fires immediately before the own disposal occurs.
        /// </summary>
        internal event DisposePrototype Disposing;

        /// <summary>
        /// Returns true if Dispose() has been called 
        /// </summary>
        public bool IsDisposed
        {
            get { return DisposalInterlock.IsLocked; }
        }

        /// <summary>
        /// Checks if Dispose() has been called, and throws an ObjectDisposedxException() if it has
        /// </summary>
        protected void CheckDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().ToString());
        }
        /// <summary>
        /// Disposal implementation for unmanaged objects should go in this method.
        /// </summary>
        protected abstract void FreeUnmanagedResources();

        /// <summary>
        /// Disposal implementation for managed objects should go in this method.
        /// </summary>
        protected abstract void FreeManagedResources();

        protected void Dispose(bool disposing)
        {
            if (DisposalInterlock.Enter())
            {
                CallDisposeMethods(disposing);
            }
        }

        protected void CallDisposeMethods(bool disposing)
        {
            if (disposing)
            {
                Disposing?.Invoke();

                //will never be used again
                //it only fires once
                //here we just remove everything so that there are no cross references
                if ( Disposing != null )
                {
                    foreach ( var d in Disposing.GetInvocationList() )
                    {
                        Disposing -= (DisposePrototype)d;
                    }
                }

                FreeManagedResources();
            }
            FreeUnmanagedResources();
        }

        public void Dispose()
        {
            Dispose(true);

            //The only reason that SuppressFinalize is called after Dispose(true) is because this allows
            //the finalizer to run if the Dispose’s Dispose(true) fails (by throwing an exception). 
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     When the finalizer thread gets around to running, it runs all the destructors of the object.
        ///     Destructors will run in order from most derived to least derived.
        /// </summary>
        ~ManagedDisposable()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception exception)
            {
                //This is bad.
                //At least attempt to get a log message out if this happens.
                try
                {
                    var builder = new System.Text.StringBuilder();
                    builder.AppendLine($"{DateTime.Now} - Exception in type '{GetType()}'");
                    builder.Append(exception.StackTrace);
                    builder.Append(exception.Message);
                    var innerException = exception.InnerException;
                    while (innerException != null)
                    {
                        builder.Append(innerException.Message);
                        innerException = innerException.InnerException;
                    }

                    File.AppendAllText(@"FinalizerException.txt", builder.ToString());
                }
                catch { }   //Swallow any exceptions inside a finalizer
            }
        }
    }
}
