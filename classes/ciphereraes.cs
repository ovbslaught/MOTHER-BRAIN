using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Security.Cryptography;
using runnerDotNet;


namespace runnerDotNet
{
	public class RunnerCiphererAES : XClass
	{
		private byte[] key;
		private byte[] INITIALISATION_VECTOR;
		
		public RunnerCiphererAES( String  _key, String algorithm = ""  ) 
		{
			String iv = "A7EC0E8D9D35BFDA";
			INITIALISATION_VECTOR = new byte[iv.Length];
			System.Buffer.BlockCopy( iv.ToCharArray(), 0, INITIALISATION_VECTOR, 0, iv.Length );
			
			key = new byte[ _key.ToString().Length ];
			System.Buffer.BlockCopy( _key.ToString().ToCharArray(), 0, key, 0, _key.ToString().Length );
			
		}
		public XVar Encrypt( XVar source ) {
			if( source.ToString().Length == 0 )
				return source;
				
			byte[] encrypted;
			using (RijndaelManaged rijndael = new RijndaelManaged())
			{
				rijndael.Key = key;
				rijndael.IV = INITIALISATION_VECTOR;

				ICryptoTransform encryptor = rijndael.CreateEncryptor();
				using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write( source.ToString() );
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
			}
			return MVCFunctions.bin2hex( new XVar(encrypted) );
		}

		/**
		 * Decrypt
		 * Decrypt string, ecncrypted with AES algorythm
		 * @param {string} encrypted value
		 * @return {string} decrypted value
		 */
		public XVar Decrypt( XVar source ) {
			if( source.ToString().Length == 0 )
				return source;
			try
            {
	
				byte[] encrypted = (byte[]) MVCFunctions.hex2byte( source ).Value;
				using (RijndaelManaged rijndael = new RijndaelManaged())
				{
					rijndael.Key = key;
					rijndael.IV = INITIALISATION_VECTOR;

					ICryptoTransform decryptor = rijndael.CreateDecryptor();
					// Create the streams used for decryption.
					using (MemoryStream msDecrypt = new MemoryStream(encrypted))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{

								// Read the decrypted bytes from the decrypting stream
								// and place them in a string.
								return srDecrypt.ReadToEnd();
							}
						}
					}
					
				}
			} 
			catch (Exception ex ) 
			{
				return source;
			}
			
		}
	}

}