/// If file changed, please make appropriate changes in DesEncrypt project
/// or wizard function (dynamic permissions -> add new user) will no longer be correct

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace runnerDotNet
{
    public class RunnerCiphererDES : XClass
    {
        /// <summary>
        /// Encryption key (16 or 24 characters)
        /// </summary>
        private string padString = "bhw98yfEg8gFori4Cv235FD3";
        private string m_key = "";
        private string key
        {
            get { return this.m_key; }
            set
            {
                if (value.Length < 16)
                {
                    this.m_key = value + padString.Substring(0, 16 - value.Length);
                }
                else if (value.Length > 16 && value.Length < 24)
                {
                    this.m_key = value + padString.Substring(0, 24 - value.Length);
                }
                else if (value.Length > 24)
                {
                    this.m_key = value.Substring(0, 24);
                }
                else
                {
                    this.m_key = value;
                }
            }
        }
        private static string INITIALISATION_VECTOR = "d27b358d";

        public RunnerCiphererDES(XVar key)
        {
            this.key = key;
        }

        /// <summary>
        /// Encrypt string with 3DES algorythm
        /// </summary>
        /// <param name="source">plain value</param>
        /// <returns>encrypted value</returns>
        public XVar Encrypt(XVar source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return source;
            }
            byte[] key = Encoding.ASCII.GetBytes(this.key); //24characters        
            byte[] plainText = Encoding.UTF8.GetBytes(source);
            TripleDES des = TripleDES.Create();
            des.Key = key;
            des.Mode = CipherMode.CBC;
            des.IV = Encoding.ASCII.GetBytes(RunnerCiphererDES.INITIALISATION_VECTOR);
            ICryptoTransform ic = des.CreateEncryptor();
            byte[] encodedText = ic.TransformFinalBlock(plainText, 0, plainText.Length);

            return MVCFunctions.bin2hex(new XVar(encodedText));
        }

        /// <summary>
        /// Decrypt string, ecncrypted with 3DES algorythm
        /// </summary>
        /// <param name="source">encrypted value</param>
        /// <returns>decrypted value</returns>
        public XVar Decrypt(XVar source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return source;
            }
            try
            {
                byte[] key = Encoding.ASCII.GetBytes(this.key); //24characters        
                byte[] encodedText = (byte[])MVCFunctions.hex2byte(source).Value;
                TripleDES des = TripleDES.Create();
                des.Key = key;
                des.Mode = CipherMode.CBC;
                des.IV = Encoding.ASCII.GetBytes(RunnerCiphererDES.INITIALISATION_VECTOR);
                ICryptoTransform ic = des.CreateDecryptor();
                byte[] decodedText = ic.TransformFinalBlock(encodedText, 0, encodedText.Length);
                return UTF8Encoding.UTF8.GetString(decodedText);
            }
            catch (Exception ex)
            {
                return source;
            }
        }

    }
}