using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace runnerDotNet
{
    [Serializable]
	public class XClass : XVar, IDisposable
    {
		protected bool disposed = false;

		public virtual XVar __destruct()
		{
			// Free any managed objects here.
			return null;
		}

		public void Dispose()
		{ 
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return; 

			if (disposing) {
				// Free any other managed objects here.
				//
				__destruct();
			}

			// Free any unmanaged objects here.
			//
			disposed = true;
		}
  
		~XClass()
		{
			Dispose(false);
		}

		public override bool IsRunnerType()
		{
			return true;
		}
    }
}