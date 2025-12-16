using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace runnerDotNet
{
    /// <summary>
    /// Class for display admin_rights_list
    /// </summary>
    public class RunnerLdap : XClass
    {
        public XVar ldapServerAddress = "";
		public LdapConnection ldapconn = null;
        public XVar ldapbind = null;

		public XVar lastError;

        public RunnerLdap(XVar ldapServerAddress )
        {
            this.ldapServerAddress = ldapServerAddress;
        }

		public XVar runner_ldap_connect(XVar usernames, XVar aPassword, XVar followRefs)
        {
            bool result = false;

			foreach (KeyValuePair<XVar, dynamic> uname in usernames.GetEnumerator())
			{
				try
				{
					LdapDirectoryIdentifier directory = new LdapDirectoryIdentifier( CommonFunctions.ldap_getServerPort( this.ldapServerAddress ), true, false );
					var ldapConnection = new LdapConnection(directory);
					ldapConnection.Credential = new System.Net.NetworkCredential(uname.Value.ToString(), aPassword.ToString());
					ldapConnection.AuthType = AuthType.Basic;
					ldapConnection.SessionOptions.ProtocolVersion = 3;
					if( MVCFunctions.strtolower( MVCFunctions.substr( this.ldapServerAddress, 0, 8 ) ) == "ldaps://") {
						ldapConnection.SessionOptions.SecureSocketLayer = true;
                        ldapConnection.SessionOptions.VerifyServerCertificate = new System.DirectoryServices.Protocols.VerifyServerCertificateCallback(
                            (LdapConnection connection, System.Security.Cryptography.X509Certificates.X509Certificate certificate) => true
                        );
					}
					if( followRefs == true ) {
						ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.All;
					}
					ldapConnection.Bind();
					ldapconn = ldapConnection;
					result = true;
				}
				catch (Exception ex) 
				{
					lastError = ex.Message;
					continue;
				}

				break;
			}
            return result;
        }

        /// <summary>
        /// Search LDAP tree
        /// </summary>
        /// <param name="filter">string The search filter</param>
        /// <param name="attributes">array An array of the required attributes</param>
        /// <returns></returns>
        public XVar runner_ldap_getData(XVar filter, XVar baseDN, XVar attributes = null)
        {
			SearchRequest request = new SearchRequest(baseDN, filter, System.DirectoryServices.Protocols.SearchScope.Subtree);
			
			// without it SearchRequest to AD will be very very slow
			SearchOptionsControl SuppressReferrals = new SearchOptionsControl(SearchOption.DomainScope);
			request.Controls.Add(SuppressReferrals);
			
			if (MVCFunctions.count(attributes) > 0)
			{
				foreach (var attribute in attributes.GetEnumerator())
				{
					request.Attributes.Add(attribute.Value);
				}
			}

			try
			{
				var response = ldapconn.SendRequest(request);
				return runner_convert_entries((SearchResponse)response);
			}
			catch (Exception ex)
			{
				lastError = ex.Message;
				return false;
			}
        }

		private XVar runner_convert_entries(SearchResponse aData)
        {
            XVar resultArr = XVar.Array();
            if (aData.Entries.Count > 0)
            {
				for (int i = 0; i < aData.Entries.Count; i++)
                {
                    XVar result = XVar.Array();
					foreach (var resultItem in aData.Entries[i].Attributes)
                    {
                        DictionaryEntry property = (DictionaryEntry)resultItem;
						if (((DirectoryAttribute)property.Value).Count > 1 || property.Key.ToString() == "memberof")
                        {
                            XVar tmpResult = XVar.Array();
							for (int j = 0; j < ((DirectoryAttribute)property.Value).Count; j++)
                            {
								tmpResult.Add(new XVar(((DirectoryAttribute)property.Value)[j]));
                            }
                            result[property.Key] = tmpResult;
                        }
                        else if (property.Key.ToString() == "dn")
                        {
                            result[property.Key] = new XVar(property.Value);
                        }
                        else
                        {
							result[property.Key] = new XVar(((DirectoryAttribute)property.Value)[0]);
                        }
                    }
                    resultArr.Add(result);
                }
            }

			return resultArr;
        }


        public void runner_ldap_unbind()
        {
			if (ldapconn != null)
			{
				this.ldapconn.Dispose();
				ldapconn = null;
			}
	    }
	
        public XVar ldap_error()
        {
		    return lastError;
	    }

        public XVar getGroupSid(XVar userSid, XVar primaryGroupId)
        {
            XVar tgroup = MVCFunctions.substr(MVCFunctions.bin2hex(userSid), 0, -8);
            int pgInt = primaryGroupId.ToInt();
            string group = "";
            for (int i = 0; i < MVCFunctions.strlen(tgroup); i += 2)
            {
                group += "\\" + MVCFunctions.substr(tgroup, i, 2);
            }
            group += "\\" + (pgInt & 255).ToString("X2");
            group += "\\" + ((pgInt >> 8) & 255).ToString("X2");
            group += "\\" + ((pgInt >> 16) & 255).ToString("X2");
            group += "\\" + ((pgInt >> 24) & 255).ToString("X2");

            return group;
        }
    }
}