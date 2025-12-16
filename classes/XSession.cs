using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace runnerDotNet
{
    public class XSession
    {
        static XSession session = new XSession();

        public static XSession Session
        {
            get { return session; }
        }
		
		public void Abandon()
		{
			HttpContext.Current.Session.Abandon();
		}

        public XVar this[object key]
        {
            set { HttpContext.Current.Session[key.ToString()] = value; }
            get { return XVar.Pack(HttpContext.Current.Session[key.ToString()]); }
        }
		
		public void InitAndSetArrayItem(XVar value, params XVar[] keys)
		{
			if(keys.Length == 0)
				this[keys[0]] = value;
			else
			{
				if (!KeyExists(keys[0]))
					this[keys[0]] = XVar.Array();
				this[keys[0]].SetArrayItemInternal(value, keys.Skip(1).ToArray());
			}
		}

        public bool KeyExists(XVar key)
        {
            return HttpContext.Current.Session[key.ToString()] != null;
        }

        public void Remove(XVar key)
        {
            HttpContext.Current.Session.Remove(key.ToString());
        }

        public IEnumerable<KeyValuePair<XVar, dynamic>> GetEnumerator()
        {
            foreach (string key in HttpContext.Current.Session)
                yield return new KeyValuePair<XVar, dynamic>(key,
                    XVar.Pack(HttpContext.Current.Session[key]));
        }
    }
}