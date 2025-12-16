using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using runnerDotNet;
using LumenWorks.Framework.IO.Csv;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography.Xml;


namespace runnerDotNet
{
    public static partial class Constants
    {
        public const int STR_PAD_LEFT = 0;
        public const int STR_PAD_RIGHT = 1;
        public const int STR_PAD_BOTH = 2;
    };
    public static class HtmlExtensions
    {
        public static void RenderSharedView(this HtmlHelper helper, string view)
        {
            if (helper == null) // RunnerRazor render
                return;

            // renders only assigned views
            var xt = (XTempl)helper.ViewData["xt"];
            var viewName = xt.getVar(view);

            if (viewName && File.Exists(MVCFunctions.getabspath("/Views/Shared/" + viewName.ToString() + ".cshtml")))
            {
                System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(helper, view);
            }
        }
    }
    public class ErrorHandler : XClass
    {
        public XVar errorstack = new XVar();

        public XVar handle_mail_error(XVar _param_errno, XVar _param_errstr, XVar _param_errfile, XVar _param_errline)
        {
            XVar errno = _param_errno.Clone();
            XVar errstr = _param_errstr.Clone();
            XVar errfile = _param_errfile.Clone();
            XVar errline = _param_errline.Clone();
            if (errstr.IndexOf("It is not safe to rely on the system's timezone settings.") != 0)
            {
                return null;
            }
            this.errorstack[MVCFunctions.count(this.errorstack)] = new XVar("number", errno, "description", errstr, "file", errfile, "line", errline);
            return null;
        }

        public XVar getErrorMessage()
        {
            XVar msg;
            msg = "";
            foreach (KeyValuePair<XVar, dynamic> err in this.errorstack.GetEnumerator())
            {
                if (msg)
                {
                    msg = MVCFunctions.Concat(msg, "\r\n");
                }
                msg = MVCFunctions.Concat(msg, err.Value["description"]);
            }
            return msg;
        }
    }

    static class FormatReplaccer
    {
        static int formatCount;
        static Regex regex;

        static FormatReplaccer()
        {
            string pattern = @"%(?<Number>\d+)?(?<DotNumber>\.\d+)?(?<Type>d|f|s)";
            regex = new Regex(pattern, RegexOptions.Compiled);
        }

        public static string FormatCStyleStrings(string input)
        {
            formatCount = 0; // reset count

            string result = regex.Replace(input, FormatReplacement);
            return result;
        }

        private static string FormatReplacement(Match m)
        {
            string number = m.Groups["Number"].Value;
            string type = m.Groups["Type"].Value;
            string dotnumber = m.Groups["DotNumber"].Value;

            string fmt;

            if (type == "d")
            {
                if (number.Length >= 2 && number[0] == '0')
                    number = number.TrimStart('0');
                fmt = type + number;
            }
            else if (type == "f")
                fmt = type + dotnumber.TrimStart('.');
            else
                fmt = type + number;


            return String.Concat("{", formatCount++, ":", fmt, "}");
        }
    }

    public class StringPadder : ICustomFormatter
    {
        public string Format(string format, object arg,
             IFormatProvider formatProvider)
        {
            if (arg is XVar)
                arg = (arg as XVar).Value;

            switch (format[0])
            {
                case 'd':
                    if (!(arg is int))
                        return Int32.Parse(arg.ToString()).ToString(format);
                    break;
                case 'f':
                    if (!(arg is double) && !(arg is Single))
                        return Double.Parse(arg.ToString()).ToString(format);
                    break;
                case 's':
                    return arg.ToString();
            }

            return string.Format("{0:" + format + "}", arg);
        }
    }

    public class StringPadderFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return new StringPadder();

            return null;
        }
        public static readonly IFormatProvider Default =
           new StringPadderFormatProvider();
    }

    public class FakeView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new InvalidOperationException();
        }
    }

    public class SecureWebClient : WebClient
    {
        protected string certPath;
        public SecureWebClient(dynamic certPath = null)
        {
            this.certPath = certPath as Object == null ? "" : certPath.ToString();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            if (!String.IsNullOrEmpty(this.certPath) && MVCFunctions.file_exists(this.certPath))
            {
                X509Certificate myCert = X509Certificate.CreateFromCertFile(this.certPath);
                request.ClientCertificates.Add(myCert);
            }
            return request;
        }
    }

    #region Dummies classes
/*

    /// <summary>
    /// Dummy class for events. Must be implemented somehow
    /// </summary>
    public class eventsBase : XClass
    {
        public XVar events = new XVar();
        public XVar captchas = new XVar();
        public XVar maps = new XVar();

        public eventsBase()
        {
        }

        public XVar exists(XVar eventName, XVar table = null)
        {
            return false;
        }

        public XVar existsMap(XVar page)
        {
            return this.maps.KeyExists(page);
        }

        public XVar existsCAPTCHA(XVar page)
        {
            return this.captchas.KeyExists(page);
        }
    }
*/
    public class RunnerRedirectException : Exception
    {
        public RunnerRedirectException(XVar URL) : base(URL.ToString())
        {
        }
    }

    public class RunnerInlineOutputException : Exception
    {
    }

    #endregion

    public static class MVCFunctions
    {
        [ThreadStatic] static Random _randomizer = null;

        static Random randomizer
        {
            get
            {
                if (_randomizer == null)
                {
                    _randomizer = new Random();
                }
                return _randomizer;
            }
        }

        private static readonly DateTime PHP_NULL_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static XVar bin2hex(XVar binVal)
        {
            if (binVal == null)
                return null;

            StringBuilder result = new StringBuilder();
            byte[] byteArray = binVal.IsByteArray() ? (byte[])binVal.Value : Encoding.Default.GetBytes(binVal.ToString());
            for (int i = 0; i < byteArray.Length; i++)
            {
                result.Append(byteArray[i].ToString("X2").ToLower());
            }
            return result.ToString();
        }

        public static XVar mb_convert_encoding(XVar str, XVar toEncoding, XVar fromEncoding)
        {
            Encoding encTo = Encoding.GetEncoding(toEncoding.ToString());
            Encoding encFrom = Encoding.GetEncoding(fromEncoding.ToString());
            return encTo.GetString(encFrom.GetBytes(str.ToString()));
        }

        public static bool IsNumeric(XVar val)
        {
            if (val as object != null)
                return val.IsNumeric();
            else return false;
        }

        public static XVar min(XVar a, XVar b)
        {
            return a < b ? a : b;
        }

        public static XVar max(XVar a, XVar b)
        {
            return a > b ? a : b;
        }

        public static XVar abs(XVar val)
        {
            return val < 0 ? 0 - val : val;
        }

        public static void Echo(XVar text)
        {
            if (text as object != null) {
                //MVCFunctions.EchoToOutput( text );
				GlobalVars.BufferStack.Peek().Append(text.ToString());
			}
        }

        public static void EchoToOutput(XVar text)
        {
            // used in events user code.
            GlobalVars.IsOutputDone = true;
            if (text as object != null)
                HttpContext.Current.Response.Write(text.ToString());
        }

        public static void print_r(XVar arg)
        {
            EchoToOutput(runner_json_encode(arg));
        }

        public static void Exit()
        {
            // used in events user code.
            // stops script execution and commits echo buffer to response

            MVCFunctions.ob_flush();
            //			HttpContext.Current.Response.Write(GlobalVars.Buffer.ToString());
            HttpContext.Current.Response.End();
            throw new RunnerInlineOutputException();
        }

        public static void ob_start()
        {
            GlobalVars.BufferStack.Push(new StringBuilder());
        }

        public static XVar ob_get_contents()
        {
            return GlobalVars.BufferStack.Peek().ToString();
        }

        public static void ob_flush()
        {
            HttpContext.Current.Response.Write(GlobalVars.BufferStack.Pop().ToString());
            if (GlobalVars.BufferStack.Count == 0)
                ob_start();
        }

        public static void ob_end_clean()
        {
            GlobalVars.BufferStack.Pop();
            if (GlobalVars.BufferStack.Count == 0)
                ob_start();
        }

        public static void clearstatcache()
        {
        }

        public static XVar strcasecmp(XVar str1, XVar str2)
        {
            return str1.ToString().CompareTo(str2.ToString());
        }

        public static XVar strcmp(XVar str1, XVar str2)
        {
            return strcasecmp(str1, str2);
        }

        public static XVar is_string(XVar var)
        {
            return var != null && var.IsString();
        }

        public static XVar is_array(XVar var)
        {
            return var != null && var.IsArray();
        }

        public static XVar is_object(XVar var)
        {
            return var != null && var.IsObject();
        }

        public static XVar is_a(XVar var, XVar className)
        {
            return var != null &&
                var.IsObject() &&
                (var.GetType().Name == className.ToString() ||
                var.Value != null && var.Value.GetType().Name == className.ToString());
        }

        public static XVar is_float(XVar var)
        {
            TypeCode valType = var != null ? Type.GetTypeCode(var.Value.GetType()) : TypeCode.Empty;
            return valType == TypeCode.Decimal || valType == TypeCode.Double || valType == TypeCode.Single;
        }

        public static XVar serialize(XVar obj)
        {
            return obj;
        }

        public static XVar unserialize(XVar obj)
        {
            return obj;
        }

        public static XVar rand(XVar minValue = null, XVar maxValue = null)
        {
            XVar result;
            if (minValue as object == null)
            {
                result = randomizer.NextDouble();
            }
            else
            {
                if (maxValue as object == null)
                    result = randomizer.Next(minValue, int.MaxValue);
                else
                    result = randomizer.Next(minValue, (maxValue >= int.MaxValue) ? int.MaxValue : (int)maxValue + 1);
            }
            return result;
        }

        public static XVar trim(XVar value, XVar trimChars = null)
        {
            if (value != null)
                return value.Trim(trimChars);

            return "";
        }

        public static XVar is_dir(XVar path)
        {
            return Directory.Exists(path);
        }

        public static XVar append_to_file(XVar filename, XVar str)
        {
            FileStream stream = File.Open(filename, FileMode.Append);
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            stream.Write(enc.GetBytes(str), 0, enc.GetByteCount(str));
            stream.Close();
            return null;
        }

        public static XVar rename(XVar from, XVar to)
        {
            try
            {
                var fromPath = from.ToString().Replace('/', '\\');
                var toPath = to.ToString().Replace('/', '\\');
                if (File.Exists(toPath))
                {
                    System.IO.File.Copy(fromPath, toPath, true);
                    System.IO.File.Delete(fromPath);
                }
                else
                {
                    System.IO.File.Move(fromPath, toPath);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public static XVar fopen(XVar filename, XVar mode)
        {
            FileMode fileMode = FileMode.OpenOrCreate;
            switch (mode.ToString())
            {
                case "a":
                    fileMode = FileMode.Append;
                    break;
                case "r":
                    fileMode = FileMode.Open;
                    break;
            }
            return new XVar(File.Open(filename, fileMode));
        }

        public static XVar fgetcsv(XVar handle, XVar length = null, XVar delimiter = null, XVar enclosure = null, XVar escape = null)
        {
            if (handle == null)
            {
                return null;
            }
            XVar result = XVar.Array();
            FileStream fs = handle.Value as FileStream;
            //fs.Read
            //Encoding.Default.GetChars(

            return result;
        }

        public static void fputs(XVar fileStream, XVar content)
        {
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            ((FileStream)fileStream.Value).Write(enc.GetBytes(content), 0, enc.GetByteCount(content));
        }

        public static void fclose(XVar fileStream)
        {
            ((FileStream)fileStream.Value).Close();
        }

        public static XVar file_exists(XVar filename)
        {
            var invalidChars = Path.GetInvalidPathChars();
            if (filename.ToString().Any(x => invalidChars.Contains(x)))
                return false;

            if (isAbsolutePath(filename))
                return (File.Exists(filename));
            else return File.Exists(getabspath(filename));
        }

        public static XVar filesize(XVar filename)
        {
            if (File.Exists(filename))
                return new FileInfo(filename).Length;
            else return new FileInfo(getabspath(filename)).Length;
        }

        public static XVar filemtime(dynamic filename)
        {
            if (File.Exists(filename))
            {
                return XVar.Pack(new FileInfo(filename).LastWriteTime);
            }
            return XVar.Pack(new FileInfo(getabspath(filename)).LastWriteTime);
        }

        public static XVar unlink(XVar filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static XVar copy(XVar source, XVar destination)
        {
            try
            {
                File.Copy(source, destination);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static XVar remoteAddr()
        {
            if (HttpContext.Current.Request.UserHostAddress == "::1")
                return "127.0.0.1";
            else return HttpContext.Current.Request.UserHostAddress;
        }

        public static XVar session_id()
        {
            return HttpContext.Current.Session.SessionID;
        }

        public static XVar array_pop(XVar arr)
        {
            return arr.Pop();
        }

        public static XVar array_intersect(params XVar[] arrays)
        {
            XVar result = new XVar();
            if (arrays != null && arrays.Count() > 1)
            {
                result = arrays[0];
                for (int i = 1; i < arrays.Count(); i++)
                {
                    result = result.Intersect(arrays[i]);
                }
            }
            return result.Distinct();
        }

        public static XVar array_merge(params XVar[] arrays)
        {
            XVar result = new XVar();
            if (arrays != null && arrays.Count() > 1)
            {
                for (int i = 0; i < arrays.Count(); i++)
                {
                    if (!arrays[i].IsArray())
                        continue;
                    foreach (var item in arrays[i].GetEnumerator())
                        if (item.Key.IsNumeric() && result.KeyExists(item.Key))
                            result.Add(item.Value);
                        else result.SetArrayItem<XVar, XVar>(item.Key, item.Value);
                }
            }
            return result;
        }

        public static XVar count(XVar arr)
        {
            if (arr as object == null)
                return 0;

            if (arr.IsArray())
                return arr.Count();

            if (arr.Value is string)
                return (arr.Value as string).Length;

            if (arr.Type == typeof(XSettingsMap))
                return ((XSettingsMap)(arr.Value)).Count;

            if (arr.Value is IEnumerable)
                throw new NotImplementedException("Method count for " + arr.Value.GetType() + " is not implemented");

            return 0;
        }

        public static XVar strlen(string val)
        {
            if (val != null)
                return val.Length;

            return 0;
        }

        public static XVar strlen(XVar str)
        {
		    return str.Length();
        }


        public static XVar strlen(XVar str, XVar startPosition, XVar length = null)
        {
            if (str != null)
                return str.Substring(startPosition, length);

            return "";
        }

        public static XVar array_search(XVar needle, XVar haystack)
        {
            if ( needle != null && haystack != null )
                return haystack.ArraySearch(needle);

            return false;
        }

        public static void sort(ref dynamic arr, XVar sortFlags = null)
        {
            if (arr as object != null)
            {
                arr = arr.Sort();
            }
        }

        public static void ksort(ref dynamic arr, XVar sortFlags = null)
        {
            if (arr as object != null)
            {
                arr = arr.KSort();
            }
        }

        public static XVar array_shift(XVar arr) // returns first element and removes it from array
        {
            if (arr == null || !arr.IsArray() || arr.Count() == 0)
                return null; // maybe XVar(null)?

            arr.RebuildKeys(-1);
            if (arr.KeyExists(-1))
            {
                XVar resultValue = arr[-1];
                arr.Remove(-1);
                return resultValue;
            }
            else // string-key array
            {
                XVar resultKey = arr.AnyKey();
                XVar resultValue = arr[resultKey];
                arr.Remove(resultKey);
                return resultValue;
            }
        }

        public static XVar array_unshift(XVar arr, params XVar[] prependVars)
        {
            // warning: php changes indexes of whole array. here indexes will be only shifted

            if (arr as object == null)
                throw new ArgumentException("array_unshift argument cannot be null");

            if (prependVars == null)
                return arr.Count();

            arr.InitDictionary();
            arr.RebuildKeys(prependVars.Count());

            XVar arrayBefore = arr.ArrayClone();

            arr.RemoveAll();

            int index = 0;
            foreach (XVar val in prependVars)
                arr.InitAndSetArrayItem(val, index++);

            foreach (KeyValuePair<XVar, dynamic> pair in arrayBefore.GetEnumerator())
                arr.InitAndSetArrayItem(pair.Value, pair.Key);

            return arr.Count();
        }

        public static XVar array_splice(XVar input, XVar offset, XVar length = null)
        {
            if (input == null || !input.IsArray())
                input = XVar.Array();

            if (length as object == null)
                length = input.Count();

            return input.ArraySplice(offset, length);
        }

        public static XVar array_diff(XVar array1, params XVar[] arrays)
        {
            if (array1 == null || !array1.IsArray() || arrays.Count() == 0)
                return array1;

            var result = XVar.Array();
            foreach (var element in array1.GetEnumerator())
            {
                bool hasEl = false;
                foreach (var array in arrays)
                {
                    if (array.ValueExists(element.Value))
                    {
                        hasEl = true;
                        break;
                    }
                }

                if (!hasEl)
                    result[element.Key] = element.Value;
            }

            return result;
        }

        public static string ExtractMessage(dynamic exception)
        {
            if (exception != null)
            {
                if (exception is Exception)
                    return exception.Message + "\n" + ExtractMessage(exception.InnerException);
                else
                    return exception.ToString();
            }
            return "";
        }

        public static XVar extract_error_info(dynamic exception, dynamic context = null)
        {
            if (context == null)
                context = HttpContext.Current;

            XVar errinfo = XVar.Array();
            errinfo.InitAndSetArrayItem(new XVar(GlobalVars.strLastSQL), "sqlStr");

            errinfo.InitAndSetArrayItem(new XVar(ExtractMessage(exception)), "errstr");

            int errCode = (exception is HttpException) ? ((HttpException)exception).GetHttpCode() : ((int)HttpStatusCode.InternalServerError);
            errinfo.InitAndSetArrayItem(errCode, "errno");

            errinfo.InitAndSetArrayItem(new XVar(context.Request.Url), "url");

            XVar debugRows = XVar.Array();
            StackTrace st = new StackTrace(exception, true);
            StackFrame frame = st.GetFrame(0);
            errinfo.InitAndSetArrayItem(frame.GetFileLineNumber(), "errline");
            errinfo.InitAndSetArrayItem(frame.GetFileName(), "errfile");

            string pathToRoot = HttpContext.Current.Server.MapPath("/");
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                XVar row = XVar.Array();

                string fileName = sf.GetFileName() != null ? sf.GetFileName() : "[External code]";
                int idx = fileName.IndexOf(pathToRoot, StringComparison.CurrentCultureIgnoreCase);
                if (idx >= 0)
                {
                    fileName = fileName.Substring(0, idx) + fileName.Substring(idx + pathToRoot.Length);
                }
                row.InitAndSetArrayItem(fileName, "file");

                row.InitAndSetArrayItem(sf.GetFileLineNumber(), "line");

                dynamic method = sf.GetMethod();
                string methodName = (method.DeclaringType != null ? (method.DeclaringType + ".") : "") + method.Name;
                row.InitAndSetArrayItem(method.DeclaringType != null ? method.DeclaringType : "", "class");
                row.InitAndSetArrayItem(method.DeclaringType != null ? "." : "", "type");
                row.InitAndSetArrayItem(method.Name, "function");
                debugRows.Add(row);
            }
            errinfo.InitAndSetArrayItem(debugRows, "debugRows");
            return errinfo;
        }

        public static XVar runner_error_handler(Exception exception, object sender = null)
        {
            if (!ProjectSettings.getProjectValue( "detailedError" ))
            {
                MVCFunctions.Echo( ProjectSettings.getProjectValue( "customErrorMsg" ) );
                MVCFunctions.Exit();
            }
            dynamic errinfo = MVCFunctions.extract_error_info(exception.InnerException != null ? exception.InnerException : exception, ((HttpApplication)sender).Context);
            CommonFunctions.runner_show_error(errinfo);
            return new XVar(true);
        }

        public static XVar empty_error_handler(Exception exception, object sender = null)
        {
            return new XVar(true);
        }

        public static XVar array_reverse(XVar arr)
        {
            return arr.Reverse();
        }

        public static XVar explode(XVar separator, XVar value, XVar limit = null)
        {
            #region default values
            if (limit as Object == null) limit = new XVar(Int32.MaxValue);
            #endregion

            if (value as object == null)
                return XVar.Array();

            XVar result = new XVar();
            string[] splitted = value.ToString().Split(new string[] { separator }, limit.ToInt(), StringSplitOptions.None);
            foreach (string item in splitted)
                result.Add(item);

            return result;
        }

        public static XVar implode(XVar array)
        {
            return implode("", array);
        }

        public static XVar implode(XVar glue, XVar array)
        {
            if (!array.IsArray())
                return array.ToString();

            StringBuilder result = new StringBuilder();

            bool first = true;

            foreach (KeyValuePair<XVar, dynamic> element in array.GetEnumerator())
            {
                result.Append((first ? "" : glue.ToString()) + element.Value.ToString());
                first = false;
            }

            return result.ToString();
        }

        public static XVar join(XVar glue, XVar array)
        {
            return implode(glue, array);
        }

        public static XVar IsJSONAccepted()
        {
            return true;
        }

        public static XVar GetRemoteUser()
        {
            return HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
        }

        public static XVar GetHttpRange()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_RANGE"];
        }

        public static XVar GetServerPort()
        {
            return HttpContext.Current.Request.Url.Port;
        }

        public static XVar GetServerName()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        public static XVar GetScriptName()
        {
            return HttpContext.Current.Request.Url.AbsolutePath;
        }

        public static XVar GetQueryString()
        {
            return HttpContext.Current.Request.Url.Query.TrimStart('?');
        }

        public static XVar array_unique(XVar array)
        {
            return array.Distinct();
        }

        public static XVar time()
        {
            return Math.Round((DateTime.UtcNow - PHP_NULL_TIME).TotalSeconds, 0);
        }

        public static DateTime fromPHPTime(XVar phpTime)
        {
            return PHP_NULL_TIME.AddSeconds(phpTime);
        }

        public static XVar mktime(XVar hour, XVar minute, XVar second, XVar month, XVar day, XVar year)
        {
            // -31st of April should be last day of Febrary, etc

            int dayNum = (int)day;
            int monthNum = (int)month;
            int yearNum = (int)year;

            yearNum += (monthNum - 1) / 12; // negative month are not supported. only overflowed
            monthNum = (monthNum - 1) % 12 + 1;

            // negative and overflowed days are supported
            int maxDays = DateTime.DaysInMonth(yearNum, monthNum);
            int deltaDays = dayNum > maxDays ? dayNum - maxDays : (dayNum < 0 ? dayNum : 0);
            dayNum = dayNum > maxDays ? maxDays : (dayNum < 1 ? 1 : dayNum);

            var resultDate = new DateTime(yearNum, monthNum, dayNum, hour, minute, second);
            if (deltaDays != 0)
                resultDate = resultDate.AddDays(deltaDays);

            return (resultDate - PHP_NULL_TIME).TotalSeconds;
        }

        public static XVar localtime(XVar time, XVar assoc = null)
        {
            if (assoc as object == null)
                assoc = false;

            XVar res = XVar.Array();
            var dt = PHP_NULL_TIME.AddSeconds(time);
            if (assoc)
            {
                res["tm_sec"] = dt.Second;
                res["tm_min"] = dt.Minute;
                res["tm_hour"] = dt.Hour;
                res["tm_mday"] = dt.Day;
                res["tm_mon"] = dt.Month;
                res["tm_year"] = dt.Year;
                res["tm_wday"] = (int)dt.DayOfWeek;
                res["tm_yday"] = dt.DayOfYear;
                res["tm_isdst"] = dt.IsDaylightSavingTime() ? 1 : 0;
            }
            else
            {
                res[0] = dt.Second;
                res[1] = dt.Minute;
                res[2] = dt.Hour;
                res[3] = dt.Day;
                res[4] = dt.Month;
                res[5] = dt.Year;
                res[6] = (int)dt.DayOfWeek;
                res[7] = dt.DayOfYear;
                res[8] = dt.IsDaylightSavingTime() ? 1 : 0;
            }

            return res;
        }

        public static XVar getYMDdate(XVar unixTimeStamp)
        {
            return PHP_NULL_TIME.AddSeconds(unixTimeStamp).ToString("yyyy-M-d");
        }

        public static XVar getHISdate(XVar unixTimeStamp)
        {
            return PHP_NULL_TIME.AddSeconds(unixTimeStamp).ToString("HH:mm:ss");
        }

        public static bool COOKIEKeyExists(XVar key)
        {
            return HttpContext.Current.Request.Cookies[key.ToString()] != null;
        }

        public static bool REQUESTKeyExists(XVar key)
        {
            return MVCFunctions.POSTKeyExists(key) || MVCFunctions.GETKeyExists(key);
        }

        public static bool POSTKeyExists(XVar key)
        {
            return HttpContext.Current.Request.Unvalidated().Form[key.ToString()] != null;
        }

        public static bool GETKeyExists(XVar key)
        {
            return HttpContext.Current.Request.Unvalidated().QueryString[key.ToString()] != null;
        }

        // actually it is EnumerateRequest
        public static IEnumerable<KeyValuePair<XVar, dynamic>> EnumeratePOST(String key)
        {
            var post = HttpContext.Current.Request.Unvalidated().Form;
            var get = HttpContext.Current.Request.Unvalidated().QueryString;

            if (post[key] != null)
                yield return new KeyValuePair<XVar, dynamic>(post[key], new XVar(post[key]));
            else if (post[key + "[]"] != null)
            {
                String val = post[key + "[]"];
                int idx = 0;
                foreach (String value in val.Split(','))
                {
                    yield return new KeyValuePair<XVar, dynamic>(idx++, new XVar(value));
                }
            }
            else if (get[key] != null)
                yield return new KeyValuePair<XVar, dynamic>(get[key], new XVar(get[key]));
            else if (get[key + "[]"] != null)
            {
                String val = get[key + "[]"];
                int idx = 0;
                foreach (String value in val.Split(','))
                {
                    yield return new KeyValuePair<XVar, dynamic>(idx++, new XVar(value));
                }
            }
        }

        public static IEnumerable<KeyValuePair<XVar, dynamic>> EnumeratePOST()
        {
            foreach (string key in HttpContext.Current.Request.Unvalidated().Form)
                yield return new KeyValuePair<XVar, dynamic>(key,
                    new XVar(HttpContext.Current.Request.Unvalidated().Form[key]));
        }

        public static IEnumerable<KeyValuePair<XVar, dynamic>> EnumerateGET()
        {
            foreach (string key in HttpContext.Current.Request.Unvalidated().QueryString)
                yield return new KeyValuePair<XVar, dynamic>(key,
                    new XVar(HttpContext.Current.Request.Unvalidated().QueryString[key]));
        }

        public static XVar urldecode(XVar encoded)
        {
            return HttpContext.Current.Server.UrlDecode(encoded.ToString());
        }

        public static XVar urlencode(XVar decoded)
        {
            return HttpContext.Current.Server.UrlEncode(decoded.ToString());
        }

        public static XVar RawUrlEncode(XVar rawText)
        {
            if (rawText == null)
                return "";
            return Uri.EscapeDataString(rawText.ToString());
        }

        public static XVar RawUrlDecode(XVar encodedText)
        {
            return HttpUtility.UrlDecode(encodedText.ToString(), Encoding.GetEncoding((int)GlobalVars.cCodePage));
        }

        public static XVar GetLocalLink(XVar table, XVar action = null, XVar getParams = null)
        {
            string tableName = table.ToString();
            if (tableName == "assetmanager")
                return "assetmanager.aspx";

            string actionName = action as object != null ? action.ToString() : "";
            if (actionName == "")
                tableName = tableName.Replace("-", "_");
            else
                actionName = actionName.Replace("-", "_");

            string url = tableName;

            if (string.IsNullOrEmpty(url))
                return url;

            if (actionName != "")
                url += "/" + actionName;
            if (getParams as object != null && getParams != "")
            {
                if (getParams.ToString().First() != '?') url += "?";
                url += getParams.ToString();
            }

            return url;
        }


        public static XVar GetTableLink(XVar table, XVar action = null, XVar getParams = null)
        {
            return CommonFunctions.GetTableLink(table, action, getParams);
        }

        public static XVar GetWebRootPath()
        {
            var res = HttpContext.Current.Request.ApplicationPath;
            if (!res.EndsWith("/"))
                res += "/";

            return new XVar(res);
        }

        public static XVar GetCaptchaPath()
        {
            return MVCFunctions.GetWebRootPath().ToString() + "securitycode";
        }

        public static XVar GetCaptchaSwfPath()
        {
            return MVCFunctions.GetWebRootPath().ToString() + "securitycode.swf";
        }
/*
        public static XVar GetTemplateName(XVar table, XVar templateName)
        {
            XVar result = "";

            if (templateName == "")
            {
                return Constants.GLOBAL_PAGES_SHORT + "/" + table.ToString() + ".cshtml";
            }

            result = templateName.ToString() + ".cshtml";

            if (table != "")
            {
                result = table.ToString() + "/" + result.ToString();
            }

            return result;
        }
*/
        public static void HeaderRedirect(XVar header)
        {
            string url = header.ToString().Trim();
            //				throw new RunnerRedirectException(url[0] == '/' || Uri.IsWellFormedUriString(header, UriKind.Absolute) ? url : (projectPath().ToString() + GetTableLink(header).ToString() ));
            if (url == "")
            {
                return;
            }
            bool absoluteUrl = url.Substring(0, 1) == "/"
                || url.Length >= 7 && url.Substring(0, 7).ToLower() == "http://"
                || url.Length >= 8 && url.Substring(0, 8).ToLower() == "https://";

            if (!absoluteUrl)
            {
                url = projectPath().ToString() + url;
            }
            throw new RunnerRedirectException(url);
        }

        public static void HeaderRedirect(XVar table, XVar action = null, XVar getParams = null)
        {
            throw new RunnerRedirectException(projectPath().ToString() + GetTableLink(table, action, getParams).ToString());
        }

        public static XVar addHeader(dynamic _param_header)
        {
            MVCFunctions.Header(_param_header);
            return null;
        }

        public static void Header(XVar header)
        {
            string headerName = "";
            string headerValue = "";
            int colonPosition = header.IndexOf(":");
            if (colonPosition < 1)
            {
                headerName = header;
                int spacePosition = header.IndexOf(" ");
                if (spacePosition > 0)
                {
                    int secondspace = header.ToString().Substring(spacePosition + 1).IndexOf(" ");
                    if (secondspace > 0)
                    {
                        HttpContext.Current.Response.StatusCode = Int32.Parse(header.ToString().Substring(spacePosition + 1, secondspace));
                    }
                }
                return;
            }
            else
            {
                headerName = header.Substring(0, colonPosition).ToString();
                headerValue = header.Substring(colonPosition + 1).ToString();
            }
            HeaderInternal(headerName, headerValue);
        }

        public static void Header(XVar headerName, XVar headerValue)
        {
            HeaderInternal(headerName, headerValue);
        }

        private static void HeaderInternal(String headerName, String headerValue)
        {
            headerName = headerName.Trim();
            headerValue = headerValue.Trim();

            if (headerName == "Content-Type")
                HttpContext.Current.Response.ContentType = headerValue;

            HttpContext.Current.Response.AddHeader(headerName, headerValue);
        }

        internal static XVar Content(XVar content)
        {
            //var contentResult = new ContentResult();
            //contentResult.Content = content.ToString();
            return content.ToString();
        }

        public static XVar GetBuferContentAndClearBufer()
        {
            var str = GlobalVars.BufferStack.Pop().ToString();
            if (GlobalVars.BufferStack.Count == 0)
                ob_start();
            return str;
        }

        public static XVar Concat(params object[] strings)
        {
            if (strings == null)
                return "";

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < strings.Count(); i++)
                result.Append(strings[i]);

            return result.ToString();
        }

        public static XVar htmlspecialchars(XVar text)
        {
            String ret = text.ToString().Replace("&", "&amp;")
                .Replace(">", "&gt;")
                .Replace("<", "&lt;")
                .Replace("\"", "&quot;");
            return new XVar(ret);
        }

        public static object BuildPlainDictionary(XVar xObj)
        {
            if (xObj != null)
            {
                if (xObj.IsArray())
                {
                    bool isProperArray = xObj.IsProperArray();
                    dynamic result;
                    if (isProperArray)
                    {
                        result = new List<object>();
                    }
                    else
                    {
                        result = new Dictionary<object, object>();
                    }

                    foreach (var item in xObj.GetEnumerator())
                    {
                        dynamic key = item.Key, val;
                        if (item.Value.IsArray())
                        {
                            val = BuildPlainDictionary(item.Value);
                        }
                        else
                        {
                            if (item.Value != null && item.Value.Value != null && item.Value.Value.GetType() == typeof(DateTime))
                            {
                                val = item.Value.ToString();
                            }
                            else
                            {
                                val = item.Value.Value;
                            }
                        }

                        if (val is XSettingsMap)
                            val = (val as XSettingsMap).GetValue();

                        if (isProperArray)
                        {
                            result.Add(val);
                        }
                        else
                        {
                            result[key] = val;
                        }

                    }

                    return result;
                }
                else
                    return xObj.Value;
            }

            return null;
        }
        
		/**
		 * Old event/BT code compatibility
		*/
		public static XVar my_json_encode(XVar objectToEncode) {
			return MVCFunctions.runner_json_encode( objectToEncode );
		}
		/**
		 * Old event/BT code compatibility
		*/
        public static XVar my_json_decode(XVar objectToDecode) {
			return MVCFunctions.runner_json_decode( objectToDecode );
		}


        public static XVar runner_json_encode(XVar objectToEncode)
        {
			return Newtonsoft.Json.JsonConvert.SerializeObject(BuildPlainDictionary(objectToEncode), Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
                }
			)
            // </script> must be repalaced with <\/script>
			.Replace( "/", "\\/" );
        }

        public static XVar runner_json_decode(XVar objectToDecode)
        {
            XVar result = new XVar();
            if (objectToDecode != null)
            {
                try
                {
                    result = MVCFunctions.decodeJSONNode(JsonConvert.DeserializeObject(objectToDecode.ToString()));
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        private static XVar decodeJSONNode(object Node)
        {
            XVar result;
            if (Node.GetType() == typeof(JArray))
            {
                result = XVar.Array();
                foreach (var element in Node as JArray)
                {
                    result.Add(decodeJSONNode(element));
                }
            }
            else if (Node.GetType() == typeof(JObject))
            {
                result = new XVar();
                foreach (var element in Node as JObject)
                {
                    result[element.Key] = decodeJSONNode(element.Value);
                }
            }
            else if (Node.GetType() == typeof(JValue))
            {
                result = XVar.Pack((Node as JValue).Value);
            }
            else
            {
                result = XVar.Pack(Node);
            }
            return result;
        }


        public static XVar runner_mail(XVar var_params)
        {
            //	use System.Net.Mail instead of CDOSys
			return MVCFunctions.runner_mail_net(var_params);

        }

        // runner_mail that uses System.Net.Mail #11608
        public static XVar runner_mail_net(XVar var_params)
        {
            bool isMailed = false;
            string smtpResponse = "";
            try
            {
                bool isHtml = false;
                string charset = var_params.KeyExists("charset") ? var_params["charset"].ToString() : "",
                    body = var_params.KeyExists("body") ? var_params["body"].ToString() : "",
                    to = var_params.KeyExists("to") ? var_params["to"].ToString() : "",
                    bcc = var_params.KeyExists("bcc") ? var_params["bcc"].ToString() : "",
                    cc = var_params.KeyExists("cc") ? var_params["cc"].ToString() : "",
                    replyTo = var_params.KeyExists("replyTo") ? var_params["replyTo"].ToString() : "",
                    from = var_params.KeyExists("from") ? var_params["from"].ToString() : "",
                    priority = var_params.KeyExists("priority") ? var_params["priority"].ToString() : "",
                    subject = var_params.KeyExists("subject") ? var_params["subject"].ToString() : "",
                    header = "";

                if (from == "")
                    from = ProjectSettings.getSecurityValue( "emailSettings", "fromEmail" );

                if (body == "")
                {
                    body = var_params.KeyExists("htmlbody") ? var_params["htmlbody"].ToString() : "";
                    if (charset == "")
                    {
                        charset = "utf-8";
                    }
                    isHtml = true;
                }

                MailMessage message = new MailMessage();
                foreach (var addr in to.Split(',').Select(x => x.Trim()))
                    message.To.Add(new MailAddress(addr));
                message.Body = body;
                message.IsBodyHtml = isHtml;
                message.Subject = subject;
                if (charset != "")
                {
                    message.BodyEncoding = Encoding.GetEncoding(charset);
                    message.SubjectEncoding = Encoding.GetEncoding(charset);
                }

                if (from != "")
                {
                    if (from.IndexOf("<") == -1)
                        from = "<" + from + ">";
                    message.From = new MailAddress(from);
                }
                if (cc != "")
                {
                    message.CC.Add(cc);
                }
                if (bcc != "")
                {
                    header += MVCFunctions.Concat("Bcc: ", bcc, "\r\n");
                    message.Bcc.Add(bcc);
                }
                if (priority != "")
                {
                    switch (priority.ToLower())
                    {
                        case "low":
                            message.Priority = MailPriority.Low;
                            break;
                        case "normal":
                            message.Priority = MailPriority.Normal;
                            break;
                        case "high":
                            message.Priority = MailPriority.High;
                            break;
                    }
                }
                if (replyTo != "")
                {
                    if (replyTo.IndexOf("<") == -1)
                        replyTo = "<" + replyTo + ">";
                    message.ReplyToList.Add(replyTo);
                }

                if (var_params.KeyExists("attachments") && var_params["attachments"].Count() > 0)
                {
                    foreach (var attachment in var_params["attachments"].GetEnumerator())
                    {
                        System.Net.Mail.Attachment Attachment = new System.Net.Mail.Attachment(attachment.Value["path"]);
                        if (attachment.Value.KeyExists("name"))
                        {
                            Attachment.Name = attachment.Value["name"];
                        }
                        message.Attachments.Add(Attachment);
                    }
                }

                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768 /*| 12288*/ );
                var smtp = new SmtpClient();

                string strSMTPUser = ProjectSettings.getSecurityValue( "emailSettings", "SMTPUser" );
                string strSMTPPassword = ProjectSettings.getSecurityValue( "emailSettings", "SMTPPassword" );

                if (strSMTPUser != "" || var_params.KeyExists("username"))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(
                        var_params.KeyExists("username") ? var_params["username"].ToString() : strSMTPUser,
                        var_params.KeyExists("password") ? var_params["password"].ToString() : strSMTPPassword
                        );
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }

                if ( ProjectSettings.getSecurityValue( "emailSettings", "SMTPServer" ) != "" || var_params.KeyExists("host"))
                {
                    if (var_params.KeyExists("host"))
                        smtp.Host = var_params["host"];
                    else
                        smtp.Host = ProjectSettings.getSecurityValue( "emailSettings", "SMTPServer" );
                }
                else
                {
                    smtp.Host = "localhost";
                }

                if ( ProjectSettings.getSecurityValue( "emailSettings", "SMTPPort" ) != "" || var_params.KeyExists("port"))
                {
                    if (var_params.KeyExists("port"))
                        smtp.Port = var_params["port"];
                    else
                        smtp.Port = int.Parse( ProjectSettings.getSecurityValue( "emailSettings", "SMTPPort" ) );
                }
				if ( ProjectSettings.getSecurityValue( "emailSettings", "securityProtocol" ) == 1 || ProjectSettings.getSecurityValue( "emailSettings", "securityProtocol" ) == 2 )
                {
                    smtp.EnableSsl = true;
                }

                smtp.Send(message);
                isMailed = true;
            }
            catch (Exception ex)
            {
                smtpResponse = new XVar(ex.Message).nl2br().ToString();
            }
            return new XVar("mailed", isMailed, "success", isMailed, "errors", smtpResponse, "message", smtpResponse);
        }

        public static XVar getabspath(XVar path)
        {
            if (path == null)
            {
                return new XVar();
            }
            string pathToMap = path.ToString();
            if (pathToMap.IndexOf("/") == 0)
            {
                pathToMap = pathToMap.Substring(1);
            }

            if (pathToMap.StartsWith(HttpContext.Current.Request.PhysicalApplicationPath))
                return pathToMap;
            else return HttpContext.Current.Request.PhysicalApplicationPath + pathToMap;
        }

        public static XVar getabsurl(dynamic _param_uri)
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public static XVar myfile_exists(XVar filename)
        {
            return file_exists(filename);
        }

        public static XVar try_create_new_file(XVar filename)
        {
            var invalidChars = Path.GetInvalidPathChars();
            if (filename.ToString().Any(x => invalidChars.Contains(x)))
                return false;
            var _filename = filename;
            if (!isAbsolutePath(filename))
                _filename = getabspath(filename);
            try
            {
                FileStream stream = File.Open(_filename, FileMode.CreateNew);
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static XVar myfile_get_contents(XVar file, XVar mode = null, int length = 0)
        {
            if (file.Value is HttpPostedFile)
            {
                return MVCFunctions.myfile_get_contents((HttpPostedFile)file.Value, mode, length);
            }

            try
            {
                if (mode as object == null)
                    mode = "rb";

                if (mode.ToString().Contains("b"))
                    return new XVar(File.ReadAllBytes(file));
                else
                    return File.ReadAllText(file, Encoding.GetEncoding((int)GlobalVars.cCodePage));
            }
            catch (Exception)
            {
                return new XVar("");
            }
        }

        public static XVar myfile_get_contents(HttpPostedFile file, XVar mode = null, int length = 0)
        {
            if (mode as object == null)
                mode = "rb";

            if (mode.ToString().Contains("b"))
            {
                byte[] buffer = new byte[file.InputStream.Length];
                file.InputStream.Position = 0;
                file.InputStream.Read(buffer, 0, (int)file.InputStream.Length);
                file.InputStream.Position = 0;
                return new XVar(buffer);
            }
            else
            {
                StringBuilder contents = new StringBuilder();

                if (file.ContentLength > 0)
                {
                    byte[] buffer = new byte[1024];
                    Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
                    while (file.InputStream.Read(buffer, 0, buffer.Length) > 0)
                    {
                        contents.Append(enc.GetString(buffer));
                    }
                    file.InputStream.Position = 0;
                }
                return contents.ToString();
            }
        }

        public static XVar myurl_get_contents(XVar url)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768 /*| 12288*/ );
            /*
			System.Net.ServicePointManager.ServerCertificateValidationCallback =
            delegate (
                object s,
                System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors
            ) {
                return true;
            };
			*/
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "PHPRunner 10");
            return client.DownloadString(url.ToString());
        }

        public static XVar myurl_get_contents_binary(XVar url)
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768 /*| 12288*/ );
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "PHPRunner 10");
            client.Headers.Add("Referer", "PHPRunner 10");

            byte[] content = client.DownloadData(url.ToString());
            return XVar.Pack(content);
        }


        public static void printfile(XVar _param_filename)
        {
            string filename = _param_filename.ToString();
            if (!file_exists(filename))
                return;

            HttpContext.Current.Response.WriteFile(filename);
        }

        public static void printfileByRange(XVar _param_filename, XVar startPos, XVar endPos)
        {
            string filename = _param_filename.ToString();
            int length = endPos - startPos + 1;

            if (!file_exists(filename))
                return;
            HttpContext.Current.Response.WriteFile(filename, startPos, length);
        }

        public static XVar mysprintf(XVar _param_format, XVar _param_params)
        {
            string format = _param_format.ToString();
            return String.Format(StringPadderFormatProvider.Default, FormatReplaccer.FormatCStyleStrings(format),
                _param_params.ToList().Select(x => x.Value).ToArray());
        }

        public static XVar now()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static XVar refine(XVar _param_str)
        {
            return _param_str.ToString();
        }

        public static XVar SupposeImageType(XVar _param_file)
        {
            if (_param_file.IsByteArray())
            {
                return MVCFunctions._SupposeImageType((byte[])_param_file.Value);
            }

            return MVCFunctions._SupposeImageType(_param_file.ToString());
        }

        public static XVar _SupposeImageType(string file)
        {
            if (file.Length > 1 && file[0] == 'B' && file[1] == 'M')
            {
                return "image/bmp";
            }
            if (file.Length > 2 && file[0] == 'G' && file[1] == 'I' && file[2] == 'F')
            {
                return "image/gif";
            }
            if (file.Length > 3 && (file[0]) == 255 && (file[1]) == 216 && (file[2]) == 255)
            {
                return "image/jpeg";
            }
            if (file.Length > 11 && (file[0]) == 'R' && (file[1]) == 'I' && (file[2]) == 'F' && (file[3]) == 'F' && (file[8]) == 'W' && (file[9]) == 'E' && (file[10]) == 'B' && (file[11]) == 'P')
            {
                return "image/webp";
            }

            if (file.Length > 8 && (file[0]) == 137 && (file[1]) == 80 && (file[2]) == 78 && (file[3]) == 71 && (file[4]) == 13 && (file[5]) == 10 && (file[6]) == 26 && (file[7]) == 10)
            {
                return "image/png";
            }
            return null;
        }

        public static XVar _SupposeImageType(byte[] file)
        {
            if (file.Length > 1 && file[0] == 'B' && file[1] == 'M')
            {
                return "image/bmp";
            }
            if (file.Length > 2 && file[0] == 'G' && file[1] == 'I' && file[2] == 'F')
            {
                return "image/gif";
            }
            if (file.Length > 3 && (file[0]) == 255 && (file[1]) == 216 && (file[2]) == 255)
            {
                return "image/jpeg";
            }
            if (file.Length > 11 && (file[0]) == 'R' && (file[1]) == 'I' && (file[2]) == 'F' && (file[3]) == 'F' && (file[8]) == 'W' && (file[9]) == 'E' && (file[10]) == 'B' && (file[11]) == 'P')
            {
                return "image/webp";
            }
            if (file.Length > 8 && (file[0]) == 137 && (file[1]) == 80 && (file[2]) == 78 && (file[3]) == 71 && (file[4]) == 13 && (file[5]) == 10 && (file[6]) == 26 && (file[7]) == 10)
            {
                return "image/png";
            }
            return null;
        }


        public static XVar prepare_file(XVar _param_value, XVar _param_field, XVar _param_controltype, XVar _param_postfilename, XVar _param_id)
        {
            XVar value = _param_value.Clone();
            string field = _param_field.ToString();
            string controltype = _param_controltype.ToString();
            string postfilename = _param_postfilename.ToString();
            string id = _param_id.ToString();
            XVar filename, ret;
            filename = "";
            HttpPostedFile file = HttpContext.Current.Request.Files["value_" + MVCFunctions.GoodFieldName(field).ToString() + "_" + id];
            if (file == null)
            {
                return false;
            }
            if (postfilename.Trim() != "")
            {
                filename = postfilename.Trim();
            }
            else
            {
                filename = file.FileName;
            }
            if (controltype.Substring(4, 1) == "1")
            {
                return new XVar("filename", "", "value", "");
            }
            if (controltype.Substring(4, 1) == "0")
            {
                return false;
            }
            BinaryReader br = new BinaryReader(file.InputStream);
            byte[] result = new byte[file.InputStream.Length];
            for (int i = 0; i < file.InputStream.Length; i++)
            {
                result[i] = br.ReadByte();
            }
            br.Close();
            ret = new XVar(result);
            if (ret == null || result.Length == 0)
            {
                return false;
            }
            return new XVar("filename", filename, "value", ret);
        }

        public static XVar prepare_upload(XVar _param_field, XVar _param_controltype, XVar _param_postfilename, XVar _param_value, XVar _param_table, XVar _param_id, XVar _pageObject)
        {
            // used only in old projects and business templates

            var pageObject = XVar.UnPackRunnerPage(_pageObject);

            string field = _param_field.ToString();
            string controltype = _param_controltype.ToString();
            string postfilename = _param_postfilename.ToString();
            string id = _param_id.ToString();
            string table = _param_table.ToString();

            XVar value = _param_value.Clone();

            XVar abs = pageObject.pSet.isAbsolute(field);
            XVar sbstr1 = controltype[6];

            HttpPostedFile file = HttpContext.Current.Request.Files["value_" + MVCFunctions.GoodFieldName(field).ToString() + "_" + id];
            if (file == null || value == "")
            {
                if (sbstr1 != '1')
                {
                    return false;
                }
            }
            if (sbstr1 == '1')
            {
                if (postfilename.Length != 0)
                {
                    pageObject.filesToDelete[MVCFunctions.count(pageObject.filesToDelete)] = new DeleteFile(postfilename, pageObject.pSet.getUploadFolder(field), abs);
                    if (pageObject.pSet.getCreateThumbnail(field))
                    {
                        pageObject.filesToDelete[MVCFunctions.count(pageObject.filesToDelete)] = new DeleteFile(MVCFunctions.Concat(pageObject.pSet.getStrThumbnail(field), postfilename),
                            pageObject.pSet.getUploadFolder(field), abs);
                    }
                }
                return "";
            }
            if (sbstr1 == '0')
            {
                return false;
            }
            if (file.ContentLength != 0)
            {
                if (!pageObject.pSet.getResizeOnUpload(field))
                {
                    // possible hdd memory leak.
                    var tempFileName = Path.Combine(MVCFunctions.getabspath("temp"), System.Guid.NewGuid() + ".tmp");
                    file.SaveAs(tempFileName);
                    pageObject.filesToMove[MVCFunctions.count(pageObject.filesToMove)] = new MoveFile(new XVar(tempFileName), value, pageObject.pSet.getUploadFolder(field), abs);
                }
                else
                {
                    XVar contents, ext, thumb;
                    contents = MVCFunctions.myfile_get_contents(file);
                    ext = CommonFunctions.CheckImageExtension(file.FileName);
                    thumb = MVCFunctions.CreateThumbnail(contents, pageObject.pSet.getNewImageSize(field), ext);
                    pageObject.filesToSave[MVCFunctions.count(pageObject.filesToSave)] = new SaveFile(thumb, value, pageObject.pSet.getUploadFolder(field), abs);
                }
            }
            return value;
        }

        public static XVar FieldSubmitted(XVar field)
        {
            return HttpContext.Current.Request.Unvalidated().Form["type_" + MVCFunctions.GoodFieldName(field).ToString()] != null
                || HttpContext.Current.Request.Unvalidated().Form["value_" + MVCFunctions.GoodFieldName(field).ToString()] != null
                || HttpContext.Current.Request.Unvalidated().Form["value_" + MVCFunctions.GoodFieldName(field).ToString() + "[]"] != null
                || HttpContext.Current.Request.Files["value_" + MVCFunctions.GoodFieldName(field).ToString()] != null;
        }

        public static XVar GetUploadedFileContents(XVar name)
        {
            return MVCFunctions.myfile_get_contents(HttpContext.Current.Request.Files[name.ToString()]);
        }

        public static XVar GetUploadedFileName(XVar name)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[name.ToString()];
            if (file == null)
            {
                return false;
            }

            return file.FileName;
        }

        public static XVar PrepareBlobs(ref dynamic values, ref dynamic blobfields, XVar _pageObject)
        {
            var pageObject = XVar.UnPackRunnerPage(_pageObject);
            Connection conn = pageObject.connection;

            XVar blobs = XVar.Array();

            if (conn.dbType == Constants.nDATABASE_Oracle || conn.dbType == Constants.nDATABASE_SQLite3)
            {
                XVar idx = 1;
                foreach (KeyValuePair<XVar, dynamic> bfield in blobfields.GetEnumerator())
                {
                    blobs[pageObject.getTableField(bfield.Value)] = values[bfield.Value];
                    if (conn.dbType == Constants.nDATABASE_Oracle)
                    {
                        values[bfield.Value] = MVCFunctions.Concat(":bnd", idx);
                        idx++;
                    }
                    else if (conn.dbType == Constants.nDATABASE_SQLite3)
                    {
                        values[bfield.Value] = "";
                    }
                    else
                        values[bfield.Value] = "?";
                }
            }
            else
                blobfields = XVar.Array();

            return blobs;
        }

        public static XVar ExecuteUpdate(dynamic pageObj, XVar _param_strSQL, XVar blobs)
        {
            string strSQL = _param_strSQL.ToString();
            XVar blobfields, blobidarray, ekey, errFunction, errhandler = null, idx, numblobs, stml, stmt;
            XVar blobTypes = XVar.Array();

            try
            {
                pageObj.connection.execWithBlobProcessing(strSQL, blobs, blobTypes);
            }
            catch (Exception ex)
            {
                pageObj.setDatabaseError(ex.Message);
                return false;
            }
            return true;
        }

        public static void runner_move_uploaded_file(XVar _param_source, XVar _param_dest)
        {
            string source = _param_source.ToString();
            string dest = _param_dest.ToString();
            if (File.Exists(dest))
                File.Delete(dest);
            File.Move(source, dest);
        }

        public static void runner_save_file(XVar filename, XVar contents)
        {
            if (contents.Value is byte[])
                File.WriteAllBytes(filename.ToString(), contents.Value as byte[]);
            else
                File.WriteAllText(filename.ToString(), contents.ToString());
        }

        public static void runner_delete_file(XVar file)
        {
            if (file)
            {

                System.Threading.Thread.Sleep(300);
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                    else if (File.Exists(getabspath(file)))
                        File.Delete(getabspath(file));
                }
                catch (Exception ex)
                {
                    return;
                }

            }
        }

        public static void import_error_handler(Exception ex)
        {
            // ??
        }

        public static XVar GetCurrentYear()
        {
            return DateTime.Now.Year;
        }
        public static void sortMembers(XVar arr)
        {
            List<KeyValuePair<XVar, XVar>> tempList = arr.ToList();
            tempList.Sort(delegate (KeyValuePair<XVar, XVar> first, KeyValuePair<XVar, XVar> second)
            {
                XVar a = first.Value, b = second.Value;
                XVar gcount, i;
                gcount = MVCFunctions.count(a["usergroup_boxes"]["data"]);
                i = 0;
                for (; i < gcount; i++)
                {
                    if (a["usergroup_boxes"]["data"][i]["usergroup_box"] == null && GlobalVars.sortgroup == -1)
                    {
                        break;
                    }
                    if (a["usergroup_boxes"]["data"][i]["usergroup_box"] != null && a["usergroup_boxes"]["data"][i]["usergroup_box"]["data"][0]["group"] == GlobalVars.sortgroup)
                    {
                        break;
                    }
                }
                if (i == gcount ||
                    a["usergroup_boxes"]["data"][i]["usergroup_box"] == null &&
                    b["usergroup_boxes"]["data"][i]["usergroup_box"] == null ||
                    a["usergroup_boxes"]["data"][i]["usergroup_box"] != null &&
                    b["usergroup_boxes"]["data"][i]["usergroup_box"] != null &&
                    a["usergroup_boxes"]["data"][i]["usergroup_box"]["data"][0]["checked"] == b["usergroup_boxes"]["data"][i]["usergroup_box"]["data"][0]["checked"])
                {
                    if (a["user"] == b["user"])
                    {
                        return 0;
                    }
                    if (b["user"] < a["user"])
                    {
                        return 1;
                    }
                    return -1;
                }
                if (GlobalVars.sortorder == "a" && a["usergroup_boxes"]["data"][i]["usergroup_box"] != null && a["usergroup_boxes"]["data"][i]["usergroup_box"]["data"][0]["checked"] == "")
                {
                    return 1;
                }
                if (GlobalVars.sortorder == "d" && b["usergroup_boxes"]["data"][i]["usergroup_box"] != null && b["usergroup_boxes"]["data"][i]["usergroup_box"]["data"][0]["checked"] == "")
                {
                    return 1;
                }
                return -1;
            });
            arr.RemoveAll();
            foreach (var item in tempList)
                arr.SetArrayItem(item.Key, item.Value);
        }

        public static XVar POSTSize()
        {
            return HttpContext.Current.Request.Unvalidated().Form.Count;
        }

        public static XVar GETSize()
        {
            return HttpContext.Current.Request.Unvalidated().QueryString.Count;
        }

        public static XVar UploadFilesCount()
        {
            return HttpContext.Current.Request.Files.Count;
        }

        public static bool SERVERKeyExists(XVar key)
        {
            return HttpContext.Current.Request.ServerVariables[key.ToString()] != null;
        }

        public static XVar GetServerVariable(XVar name)
        {
            return HttpContext.Current.Request.ServerVariables[name.ToString()];
        }

        // actually it is RequestValue
        public static XVar postvalue(XVar _param_name)
        {
            string name = _param_name.ToString();
            XVar value;
            if (is_array(GlobalVars.jsonDataFromRequest) && GlobalVars.jsonDataFromRequest[name] != null)
            {
                value = GlobalVars.jsonDataFromRequest[name];
            }
            else if (HttpContext.Current.Request.Unvalidated().Form[name] != null)
            {
                value = HttpContext.Current.Request.Unvalidated().Form[name];
            }
            else
            {
                if (HttpContext.Current.Request.Unvalidated().Form[name + "[]"] != null)
                {
                    value = XVar.Array();
                    var res = HttpContext.Current.Request.Unvalidated().Form.GetValues(name + "[]");
                    foreach (var str in res)
                        value.Add(str);
                }
                else
                {
                    if (HttpContext.Current.Request.Unvalidated().QueryString[name] != null)
                    {
                        value = HttpContext.Current.Request.Unvalidated().QueryString[name];
                    }
                    else
                    {
                        if (HttpContext.Current.Request.Unvalidated().QueryString[name + "[]"] != null)
                        {
                            value = XVar.Array();
                            var res = HttpContext.Current.Request.Unvalidated().QueryString.GetValues(name + "[]");
                            foreach (var str in res)
                                value.Add(str);
                        }
                        else
                        {
                            // slowest part. try to find multidimensional array
                            var keys = HttpContext.Current.Request.Unvalidated().Form.Keys.Cast<string>().Where(x => x.StartsWith(name + "[")).ToArray();
                            if (keys.Count() > 0)
                                return ParsePostArray(name, keys);

                            return "";
                        }
                    }
                }
            }

            return value;
        }
/*
        public static XVar getCustomMapIcon(XVar field, XVar table, XVar data)
        {
            return CustomExpressionProvider.Instance.getCustomMapIcon(field, "", table, data);
        }
*/		

/*
        public static XVar getMapCustomIcon(XVar eventObj, XVar funcName, XVar data)
        {
            var methodInfo = eventObj.GetType().GetMethod( funcName.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			object[] parameters = new object[2];
			parameters[0] = new XVar("");
			parameters[1] = data;
			methodInfo.Invoke(eventObj, parameters );
			return new XVar(parameters[0]);
        }
        public static XVar getDashMapCustomLocationIcon(XVar dashName, XVar dashElementName, XVar data)
        {
            return CustomExpressionProvider.Instance.getDashMapCustomLocationIcon(dashName, dashElementName, data);
        }
*/

        public static XVar ParsePostArray(string name, string[] keys)
        {
            // parse values like "somearray[0][ID]", "somearray[1][]" into XVar.Array
            XVar res = XVar.Array();
            foreach (var key in keys)
            {
                // warning: ] or [ in key name is not supported
                var val = HttpContext.Current.Request.Unvalidated().Form[key];
                var subKeys = key.Substring(name.Length).TrimStart('[').TrimEnd(']').Split(new string[] { "][" }, StringSplitOptions.None)
                    .Select(x => x == "" ? null : new XVar(x)).ToArray();

                // max 3 dimensions. add more if needed
                if (subKeys.Length == 1)
                    res.InitAndSetArrayItem(val, subKeys[0]);
                else if (subKeys.Length == 2)
                    res.InitAndSetArrayItem(val, subKeys[0], subKeys[1]);
                else if (subKeys.Length == 3)
                    res.InitAndSetArrayItem(val, subKeys[0], subKeys[1], subKeys[2]);
            }

            return res;
        }

        public static XVar ModifyRequest(XVar key, XVar value)
        {
            // do not use, unless you have to
            // arrays not supported
            // works only with GET

            var nameValues = HttpUtility.ParseQueryString(HttpContext.Current.Request.Unvalidated().QueryString.ToString());
            nameValues.Set(key, value);
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            string updatedQueryString = "?" + nameValues.ToString();
            HttpContext.Current.RewritePath(url + updatedQueryString);

            return null;
        }

/*        
		public static XVar CustomExpression(XVar value, XVar data, XVar field, XVar ptype, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetCustomExpression(value, data, field, ptype, table);
        }

        public static XVar fileCustomExpression(XVar file, XVar data, XVar field, XVar ptype, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetFileCustomExpression(file, data, field, ptype, table);
        }

        public static XVar GetLWWhere(XVar field, XVar ptype, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetLWWhere(field, ptype, table);
        }

        public static XVar GetDefaultValue(XVar field, XVar ptype, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetDefaultValue(field, ptype, table);
        }

        public static XVar GetAutoUpdateValue(XVar field, XVar ptype, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetAutoUpdateValue(field, ptype, table);
        }

        public static XVar GetUploadFolderExpression(XVar field, XVar file, XVar table = null)
        {
            return CustomExpressionProvider.Instance.GetUploadFolderExpression(field, file, table);
        }

        public static XVar getIntervalLimitsExpressions(XVar table, XVar field, XVar idx, XVar isLowerBound)
        {
            return CustomExpressionProvider.Instance.GetIntervalLimitsExprs(table, field, idx, isLowerBound);
        }
*/		

        public static XVar mdeleteIndex(XVar _param_i)
        {
            XVar i = _param_i.Clone();
            return i - 1;
        }

        public static XVar parse_backtrace(XVar _param_errfFile, XVar _param_errLine, XVar _param_splitAsArray = null)
        {
            // not used in .net
            throw new NotImplementedException();
        }

        public static XVar runner_error_handler(XVar _param_errno, XVar _param_errstr, XVar _param_errfile, XVar _param_errline)
        {
            // not used in .net
            throw new NotImplementedException();
        }

        public static XVar no_output_done()
        {
            return !GlobalVars.IsOutputDone;
        }

        public static XVar format_currency(XVar _param_val)
        {
            throw new NotImplementedException();
            XVar val = _param_val.Clone();
            //return CommonFunctions.str_format_currency(val);
        }
        public static XVar format_number(XVar _param_val, XVar _param_valDigits = null)
        {
            throw new NotImplementedException();
            XVar val = _param_val.Clone();
            XVar valDigits = _param_valDigits.Clone();
            //return CommonFunctions.str_format_number(val, valDigits);
        }
        public static XVar format_datetime(XVar _param_time)
        {
            throw new NotImplementedException();
            XVar time = _param_time.Clone();
            //return CommonFunctions.str_format_datetime(time);
        }
        public static XVar format_time(XVar _param_time)
        {
            throw new NotImplementedException();
            XVar time = _param_time.Clone();
            //return CommonFunctions.str_format_time(time);
        }

        public static XVar secondsPassedFrom(XVar _param_startMoment)
        {
            DateTime startMoment = DateTime.Parse(_param_startMoment.ToString(), System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None); // date only in en-US style
            return (DateTime.Now - startMoment).TotalSeconds;
        }

        public static XVar xtempl_call_func(XVar _param_func, XVar var_params)
        {
            throw new NotImplementedException();

            /*XVar func = _param_func.Clone();
			if(func.FunctionExists() != 0)
			{
				CommonFunctions.dynamic(var_params);
			}
			return null;*/
        }

        public static void echoBinary(XVar output, XVar _param_bufferSize = null)
        {
            string strOutput = output.IsByteArray() ? "" : output.ToString();
            int bufferSize = _param_bufferSize as object != null ? _param_bufferSize.ToInt() : 8192;
            int totalChars = output.IsByteArray() ? (output.Value as byte[]).Length : strOutput.Length;

            for (int i = 0; i <= totalChars; i += bufferSize)
            {
                int size = i + bufferSize < totalChars ? bufferSize : totalChars - i;
                if (output.IsByteArray())
                {
                    byte[] subBuff = new byte[size];
                    Buffer.BlockCopy((output.Value as byte[]), i, subBuff, 0, size);
                    HttpContext.Current.Response.BinaryWrite(subBuff);
                }
                else
                {
                    byte[] subBuff = Encoding.Default.GetBytes(strOutput.Substring(i, size));
                    HttpContext.Current.Response.BinaryWrite(subBuff);
                }
            }
        }

        public static XVar echoBinaryPartial(XVar _param_outputString, XVar _param_startPos, XVar _param_endPos, XVar _param_bufferSize = null)
        {
            int startPos = _param_startPos,
                endPos = _param_endPos,
                bufferSize = _param_bufferSize as object != null ? _param_bufferSize.ToInt() : 8192,
                length = endPos - startPos + 1;
            if (length < bufferSize)
            {
                bufferSize = length;
            }
            for (int i = startPos; i <= endPos && bufferSize > 0;)
            {
                HttpContext.Current.Response.OutputStream.Write(_param_outputString.Value as byte[], i, bufferSize);
                i += bufferSize;
                if (length < i + bufferSize)
                {
                    bufferSize = length - i;
                }
            }
            return null;
        }

        public static void setObjectProperty(XVar obj, XVar key, XVar value)
        {
            obj["field", key] = value;
        }

        public static void returnError404()
        {
            HttpContext.Current.Response.StatusCode = 404;
        }

        public static void execute_events(XVar var_params)
        {
            throw new NotImplementedException();
            /*if(var_params["custom1"].FunctionExists() != 0)
			{
			}*/
        }

        public static XVar GetMySQLLastInsertID(XVar connection)
        {
            return XVar.UnPackConnection(connection).getInsertedId();
        }

        /*
                public static XVar DoUpdateRecord(dynamic pageObj)
                {
                    return CommonFunctions.DoUpdateRecordSQL(pageObj);
                }
        */

        public static XVar DoInsertRecord(dynamic table, ref dynamic avalues, ref dynamic blobfields, dynamic pageObject)
        {
            return CommonFunctions.DoInsertRecordSQL(table, ref avalues, ref blobfields, pageObject);
        }

        public static XVar xtempl_include_header(XVar _param_xt, XVar _param_fname, XVar _param_param)
        {
            // not used

            XTempl xt = (XTempl)_param_xt;
            XVar fname = _param_fname.Clone();
            XVar param = _param_param.Clone();
            xt.assign_function(fname, "xt_include", new XVar("file", param));
            return null;
        }

        public static XVar db_query_safe(XVar _param_qstring, ref dynamic errstring, XVar _param_conn)
        {
            XVar qstring = _param_qstring.Clone();
            Connection conn = _param_conn == null ? CommonFunctions.getDefaultConnection() : XVar.UnPackConnection(_param_conn);
            XVar ret = null;

            try
            {
                ret = conn.query(qstring).getQueryHandle();
            }
            catch (Exception ex)
            {
                errstring = new XVar("<br>" + ex.Message);
                return false;
            }

            return ret;
        }

        [Obsolete("Not used anymore", true)]
        public static void binPrint(XVar value, XVar _param_size)
        {
        }

        public static XVar GoodFieldName(XVar _param_field)
        {
            string field = "";
            StringBuilder result = new StringBuilder();

            if (_param_field == null)
            {
                field = "";
            }
            /*			else if(GlobalVars.cCharset == "utf-8")
                        {
                            field = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(_param_field));
                        }
            */
            else
            {
                field = _param_field;
            }
            for (int i = 0; i < field.Length; i++)
            {
                char t = field[i];
                if ((t < 'a' || 'z' < t) && (t < 'A' || 'Z' < t) && (t < '0' || '9' < t))
                {
                    result.Append("_");
                }
                else
                {
                    result.Append(t);
                }
            }
            return result.ToString();
        }

        public static XVar xt_getvar(XVar xt, XVar _param_name)
        {
            XVar name = _param_name.Clone();
            for (int i = MVCFunctions.count(((XTempl)xt).xt_stack) - 1; i >= 0; i--)
            {
                if (((XTempl)xt).xt_stack[i].KeyExists(name))
                {
                    return ((XTempl)xt).xt_stack[i][name];
                }
            }
            if (!((XTempl)xt).testingFlag)
            {
                return false;
            }
            if (GlobalVars.testingLinks.KeyExists(name))
            {
                return MVCFunctions.Concat("func=\"", GlobalVars.testingLinks[name], "\"");
            }
            else
            {
                return false;
            }
        }

        public static XVar xt_mlang_message(XVar pparams)
        {
            if (pparams == null)
                pparams = new XVar();

            if (pparams.IsArray())
                pparams = pparams["custom1"];

            return CommonFunctions.mlang_message(pparams);
        }

        /*		public static void xt_process_template(XVar xt, XVar str, dynamic viewBag = null)
                {
                    XTempl templ = XVar.UnPackXTempl(xt);

        //			String bufferBkf = GlobalVars.Buffer.ToString();
        //			GlobalVars.Buffer.Clear();
                    string rendered = RunnerRazor.RenderRazorString<XTempl>(templ, str.ToString(), viewBag);
        //          MVCFunctions.Echo(bufferBkf + rendered);
                    MVCFunctions.Echo(rendered);
                }
        */
        public static XVar parse_addr_list(XVar _param_to)
        {
            throw new NotImplementedException();

            XVar var_to = _param_to.Clone();
            XVar addr_arr = new XVar();

            string cleanTo = Regex.Replace(_param_to, "^[\\s*,]+|[\\s*,]+$", "");

            cleanTo = Regex.Replace(cleanTo, "\\s+,", ",");

            string[] arr = Regex.Split(cleanTo, "(,(?=([^\"]*(\"[^\"]*\")?)*$))(?![^\\(]*(\\),|\\)$))");

            Regex emailRegex = new Regex("(([A-Za-z\\d_\\-\\.+]+@[A-Za-z\\d_\\-\\.]+\\.[A-Za-z\\d_\\-]+)(?=([^\"]*(\"[^\"]*\")?)*$))(?![^\\(]*(\\),|\\)$))");

            foreach (string match in arr)
            {
                string trimmedMatch = match.Trim();
                if (trimmedMatch != "")
                {

                    var matches = emailRegex.Matches(trimmedMatch);
                    if (matches.Count > 0 && matches[0].Groups[1].Value != "")
                    {
                        string name = emailRegex.Replace(trimmedMatch, "");
                        name = Regex.Replace(name, "\"|<>$", "");
                        addr_arr.Add(new XVar("addr", matches[0].Groups[1].Value, "name", name));
                    }
                }
            }
            return addr_arr;
        }

        public static XVar substr(XVar str, XVar startPos, XVar length = null)
        {
            if (str == null)
                return new XVar();
            return str.Substring(startPos, length);
        }

        public static XVar substr_replace(XVar str, XVar replacement, XVar start, XVar length = null)
        {
            if (str == null)
            {
                return replacement;
            }

            XVar replacementLength;
            if (length as object == null || length == 0)
            {
                replacementLength = str.Length() - start;
            }
            else
            {
                if (length > 0)
                {
                    replacementLength = length;
                }
                else
                {
                    replacementLength = str.Length() - start + length;
                }
            }
            return MVCFunctions.Concat(str.Substring(0, start), replacement, str.Substring(start + replacementLength));
        }

        public static XVar utf8_substr(XVar str, XVar from, XVar len)
        {
            return substr(str, from, len);
        }

        public static XVar getFileNameFromURL()
        {
            // actually returns controler and action

            var controller = ((MvcHandler)HttpContext.Current.Handler).RequestContext.RouteData.GetRequiredString("controller");
            var action = ((MvcHandler)HttpContext.Current.Handler).RequestContext.RouteData.GetRequiredString("action");

            if (controller != "Global")
                return controller + "/" + action;
            else return action;
        }

        public static XVar strlen_bin(XVar str)
        {
            return str.Length();
        }

        public static XVar db_stripslashesbinaryAccess(XVar str)
        {
            if (str.IsArray())
            {
                str = MVCFunctions.implode("", str);
            }
            XVar pos = str.IndexOf(".Picture");
            if (pos <= 0 || pos > 300)
            {
                return str;
            }
            XVar pos1 = str.IndexOf("BM", pos);
            if (pos1 == false || 300 < pos1)
            {
                return str;
            }
            return str.Substring(pos1);
        }

        public static void SendContentLength(XVar len)
        {
            MVCFunctions.Header("Content-Length: " + len.ToString());
        }

        public static XVar DecodeUTF8(XVar str)
        {
            if (str == null)
                str = "";

            return str;
        }

        public static XVar escapeEntities(XVar str)
        {
            return str;
        }

        public static void n_printDebug()
        {
        }

        public static XVar in_arrayi(XVar needle, XVar haystack)
        {
            foreach (KeyValuePair<XVar, dynamic> value in haystack.GetEnumerator())
            {
                if (value.Value.ToLower() == needle.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public static void f_printDebug()
        {
        }

        public static void _mvcDebug()
        {
        }

        public static XVar hex2bin(XVar source)
        {
            int decNumber = Convert.ToInt16(source.ToString(), 16);
            return Convert.ToString(decNumber, 2);
        }

        public static XVar hex2byte(XVar source)
        {
            string hex = source.ToString();
            return new XVar(Enumerable.Range(0, hex.ToString().Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray());
        }

        public static XVar toPHPTime(XVar datevalue)
        {
            return ((DateTime)datevalue.Value - PHP_NULL_TIME).TotalSeconds;
        }
        /*
			Rotates dst image as src Exif[Orientation] if possible.
		*/
        public static bool exifRotateImage(Image src, Image dst)
        {
            const int exifOrientationID = 0x112; //274
            bool isRotated = src.PropertyIdList.Contains(exifOrientationID);
            if (isRotated)
            {
                var prop = src.GetPropertyItem(exifOrientationID);
                int val = BitConverter.ToUInt16(prop.Value, 0);
                var rot = RotateFlipType.RotateNoneFlipNone;

                if (val == 3 || val == 4)
                    rot = RotateFlipType.Rotate180FlipNone;
                else if (val == 5 || val == 6)
                    rot = RotateFlipType.Rotate90FlipNone;
                else if (val == 7 || val == 8)
                    rot = RotateFlipType.Rotate270FlipNone;

                if (val == 2 || val == 4 || val == 5 || val == 7)
                    rot |= RotateFlipType.RotateNoneFlipX;

                if (rot != RotateFlipType.RotateNoneFlipNone)
                    dst.RotateFlip(rot);

            }
            return isRotated;
        }

       	public static Image ImageFromFile(string fileName)
        {
            try
            {
                return Image.FromFile(fileName);               
            }
            catch (Exception e) { }

            return null;
        }

        public static Image ImageFromStream(Stream s)
        {
            try
            {
   
                
                return Image.FromStream(s);
            }
            catch (Exception e) { }

            return null;
        }

        public static Image ImageFromBytes(byte[] val)
        {
            try
            {
                MemoryStream ms = new MemoryStream(val);
   
                return Image.FromStream(ms);                
            }
            catch (Exception e) { }

            return null;
        }

        public static byte[] BytesFromImage(Image img, string ext = "jpg")
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                
    
                dynamic format = GetImageFormatByExt(ext);
                img.Save(ms, format);                
                return ms.ToArray();
            }
            catch (Exception e) { }

            return null;
        }
		
        public static XVar CreateThumbnail(XVar value, XVar size, XVar ext)
        {
            if (value == null || value.Value == null || (value.Value as byte[]).Length == 0)
            {
                return value;
            }

            Image originalImage = MVCFunctions.ImageFromBytes(value.Value as byte[]);
            if (originalImage == null)
            {
                return value;
            }

            XVar width_old = originalImage.Size.Width;
            XVar height_old = originalImage.Size.Height;
            if (size < width_old || size < height_old)
            {
                XVar final_height, final_width;
                if (height_old <= width_old)
                {
                    final_height = height_old * size / width_old;
                    final_width = size;
                }
                else
                {
                    final_width = width_old * size / height_old;
                    final_height = size;
                }

                Bitmap resizedImage = new Bitmap((int)final_width, (int)final_height, PixelFormat.Format32bppPArgb);
                ResizeImage(width_old, height_old, originalImage, final_width, final_height, resizedImage);

                bool success = exifRotateImage(originalImage, resizedImage);

                string fileExt = ext.ToString().TrimStart('.').ToLower();
                return new XVar(MVCFunctions.BytesFromImage(resizedImage, fileExt == "" ? "jpg" : fileExt));
            }
            return value;
        }

        private static ImageFormat GetImageFormatByExt(string fileExt)
        {
            ImageFormat imageFormat;
            switch (fileExt)
            {
                case "jpg":
                case "jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                case "png":
                    imageFormat = ImageFormat.Png;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                default:
                    throw new FormatException("Unsupported image format");
            }
            return imageFormat;
        }

        private static void ResizeImage(XVar width_old, XVar height_old, Image originalImage, XVar final_width, XVar final_height, Bitmap resizedImage)
        {
            resizedImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(resizedImage);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(originalImage,
                new Rectangle(0, 0, (int)final_width, (int)final_height),
                new Rectangle(0, 0, (int)width_old, (int)height_old),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
        }

        public static XVar imageCreateThumb(XVar new_width, XVar new_height, XVar img_width, XVar img_height, XVar file_path, XVar options, XVar new_file_path, XVar uploadedFile)
        {
            bool result = true;
            Image originalImage = MVCFunctions.ImageFromStream((uploadedFile["file"].Value as HttpPostedFile).InputStream);
			if(originalImage == null)
			{
				return false;
			}

            double sourceWidth = originalImage.Width,
                sourceHeight = originalImage.Height,
                destHeight = sourceHeight,
                destWidth = sourceWidth;

            if (sourceWidth > new_width)
            {
                destWidth = new_width;
                destHeight = sourceHeight * new_width / sourceWidth;
            }

            if (destHeight > new_height)
            {
                destHeight = new_height;
                destWidth = sourceWidth * new_height / sourceHeight;
            }

            if (destWidth > 0 && destHeight > 0)
            {
                using (Bitmap resizedImage = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb))
                {
                    ResizeImage(sourceWidth, sourceHeight, originalImage, destWidth, destHeight, resizedImage);

                    bool success = exifRotateImage(originalImage, resizedImage);

                    byte[] buffer = MVCFunctions.BytesFromImage(resizedImage, file_path.Substring(file_path.LastIndexOf(".") + 1).ToLower().ToString());
                    if (buffer == null)
                    {
                        return false;
                    }
                    File.WriteAllBytes(new_file_path.ToString(), buffer);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public static XVar uploadFiles(XVar option)
        {
            XVar result = XVar.Array();
            HttpPostedFile uploadedFile = HttpContext.Current.Request.Files[option.ToString() + "[]"];
            XVar uploadedFileData = XVar.Array();
            uploadedFileData["name"] = uploadedFile.FileName;
            uploadedFileData["tmp_name"] = uploadedFile.FileName;
            uploadedFileData["size"] = uploadedFile.ContentLength;
            uploadedFileData["type"] = uploadedFile.ContentType;
            uploadedFileData["error"] = "";
            uploadedFileData["file"] = uploadedFile;
            result.Add(uploadedFileData);
            return result;
        }

        private static string upcount_name_callback(Match match)
        {
            if (match.Value == "")
                return "";

            int index = 0;
            string ext = "";
            if (match.Groups[1].Value != "")
                int.TryParse(match.Groups[1].Value, out index);
            if (match.Groups[2].Value != "")
                ext = match.Groups[2].Value;

            return " (" + (index + 1).ToString() + ")" + ext;
        }

        public static XVar upcount_name(XVar name)
        {
            return Regex.Replace(name, @"(?:(?: \(([\d]+)\))?(\.[^.]+))?$", new MatchEvaluator(upcount_name_callback));
        }

        public static XVar trim_file_name(XVar name, XVar _param_type, XVar _param_index, XVar _param_obj)
        {
            XVar var_type = _param_type.Clone();
            XVar index = _param_index.Clone();
            XVar obj = _param_obj.Clone();
            FileInfo fileInfo = new FileInfo(name);
            string file_name = fileInfo.Name;
            if (file_name.IndexOf(".") < 0)
            {
                Match match = Regex.Match(_param_type, "^image\\/(gif|jpe?g|png)");
                if (match.Groups[1].Value != "")
                    file_name += "." + match.Groups[1].Value;
            }
            while (XSession.Session[MVCFunctions.Concat("mupload_", obj["field", "formStamp"])].KeyExists(file_name))
            {
                file_name = MVCFunctions.upcount_name(file_name);
            }
            return file_name;
        }

        public static XVar basename(XVar path, XVar extension = null)
        {
            string result = Path.GetFileName(path);
            if (extension as object != null)
            {
                if (result.LastIndexOf(extension.ToString()) == result.Length - extension.Length())
                    result = result.Substring(0, result.LastIndexOf(extension.ToString()));
            }

            return result;
        }

        public static XVar pathinfo(XVar path)
        {
            XVar result = XVar.Array();

            try
            {
                FileInfo fileInfo = new FileInfo(path);

                result.SetArrayItem("basename", fileInfo.Name);
                result.SetArrayItem("filename", fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length));

                if (fileInfo.Extension.IndexOf(".") == 0)
                    result["extension"] = fileInfo.Extension.Substring(1);
                else
                    result["extension"] = fileInfo.Extension;
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    // try uri
                    Uri pathUri = new Uri(path);
                    String _path = String.Format("{0}{1}{2}{3}", pathUri.Scheme, Uri.SchemeDelimiter, pathUri.Authority, pathUri.AbsolutePath);
                    String _base = Path.GetFileName(pathUri.LocalPath);
                    String _ext = Path.GetExtension(_path);

                    result.SetArrayItem("basename", _base);
                    result.SetArrayItem("extension", _ext.TrimStart('.'));
                    result.SetArrayItem("filename", _base.Substring(0, _base.Length - _ext.Length));
                }
            }

            return result;
        }

        public static void upload_File(XVar uploaded_file, XVar destination)
        {
            //throw new Exception("Need to check security");

            var dstStr = destination.ToString().Replace('/', '\\');

            var dir = System.IO.Path.GetDirectoryName(dstStr);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            (uploaded_file["file"].Value as HttpPostedFile).SaveAs(dstStr);
        }

        public static XVar GDExist()
        {
            return true;
        }

        public static XVar makeSurePathExists(XVar abspath)
        {
            if (!Directory.Exists(abspath))
            {
                return Directory.CreateDirectory(abspath).Exists;
            }

            return true;
        }

        public static XVar mkdir(XVar abspath, XVar notUsed1 = null, XVar notUsed2 = null)
        {
            return makeSurePathExists(abspath);
        }

        public static EditControl createControlClass(XVar className, XVar field, XVar pageObject, XVar id, XVar connection)
        {
            Assembly assembly = typeof(XVar).Assembly;
            Type type = assembly.GetType("runnerDotNet." + className.ToString());
            return (EditControl)Activator.CreateInstance(type, new Object[] { field, pageObject, id, connection });
        }

        public static ViewControl createViewControlClass(XVar className, XVar field, XVar container, XVar pageObject)
        {
            Assembly assembly = typeof(XVar).Assembly;
            Type type = assembly.GetType("runnerDotNet." + className.ToString());
            return (ViewControl)Activator.CreateInstance(type, new Object[] { field, container, pageObject });
        }

        public static XVar getQueryString()
        {
            if (HttpContext.Current.Request.QueryString.Count > 0)
                return MVCFunctions.urldecode(HttpContext.Current.Request.QueryString.ToString());
            else
                return "";
        }

        public static XVar cross_sort_arr_y(XVar _param_a, XVar _param_b)
        {
            throw new NotImplementedException();
        }
        public static void SortForCrossTable(XVar sort_y)
        {
            XVar group_sort_y = GlobalVars.group_sort_y;

            List<KeyValuePair<XVar, XVar>> tempList = sort_y.ToList();
            tempList.Sort(delegate (KeyValuePair<XVar, XVar> first, KeyValuePair<XVar, XVar> second)
            {
                XVar a = first.Value, b = second.Value;
                if (group_sort_y[a] == group_sort_y[b])
                {
                    return 0;
                }
                if (group_sort_y[a] < group_sort_y[b])
                {
                    return -1;
                }
                return 1;
            });

            sort_y.RemoveAll();
            foreach (var item in tempList)
                sort_y.SetArrayItem(item.Key, item.Value);
        }

        #region RunnerRazor

        public static string RenderViewToString<T>(ControllerContext ControllerContext, string viewName, T model)
        {
            ControllerContext.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ControllerContext.Controller.ViewData,
                    ControllerContext.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderSectionToString<T>(HttpContext context, string controller, string action, string sectionName, T model) where T : XTempl
        {
            return MVCFunctions.RenderSectionToString<T>(context.Request.PhysicalApplicationPath + "Views/" + controller + "/" + action + ".cshtml", sectionName, model);
        }

        public static string RenderSectionToString<T>(string path, string sectionName, T model) where T : XTempl
        {
            if (sectionName != string.Empty)
            {
                StreamReader sr = new StreamReader(HttpContext.Current.Request.PhysicalApplicationPath + "Views/Shared/" + path + ".cshtml", Encoding.GetEncoding((int)GlobalVars.cCodePage));
                string templateString = sr.ReadToEnd();
                sr.Close();

                return RenderSectionToStringFromString<T>(templateString, sectionName, model); // complex cases
            }

            // simple cases

            System.IO.TextWriter writer = new System.IO.StringWriter();
            var html = new HtmlHelper(new ViewContext(model.currentViewContext.Controller.ControllerContext, new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), writer), new System.Web.Mvc.ViewPage());

            System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(html, path, model);

            return writer.ToString();
        }

        public static string RenderSectionToStringFromString<T>(string templateString, string sectionName, T model)
        {
            if (templateString == null)
                return "";
            if (sectionName != "")
            {
                int startPosition = templateString.IndexOf("@foreach (string _" + MVCFunctions.GoodFieldName(sectionName).ToString());
                if (startPosition < 0)
                    return "";
                int endPosition = 0;
                int bracesCount = 0;
                for (int i = startPosition + 1; i < templateString.Length; i++)
                {
                    if (templateString[i] == '}')
                    {
                        if (bracesCount == 1)
                        {
                            endPosition = i;
                            break;
                        }
                        bracesCount--;
                    }
                    else
                        if (templateString[i] == '{')
                        bracesCount++;
                }
                if (startPosition > endPosition)
                    return "";
                templateString = templateString.Substring(startPosition, endPosition - startPosition + 1);
            }
            return RunnerRazor.RenderRazorString<T>(model, templateString);
        }

        #endregion


        #region XTemple functions

        public static object AssignMethod(XVar methodName, object methodOwner, XVar _params)
        {
            dynamic parameters = (_params ?? new XVar()).Clone();
            return new XVar(new XTempl.xtMethodDelegate(delegate (XVar fparams)
            {
                XVar funcParameters = MVCFunctions.array_merge(fparams, parameters);
                MethodInfo method = methodOwner.GetType().GetMethods().Where(m => m.Name == methodName.ToString()).First();
                return method.Invoke(methodOwner,
                    method.GetParameters().Length > 0 ?
                    (new object[] { funcParameters })
                    : null) as XVar;
            }));
        }

        public static object AssignFunction(XVar functionName, XVar _param_parameters, XVar functionOwner = null)
        {
            dynamic parameters = (_param_parameters ?? new XVar()).Clone();

            return XVar.Pack(new XTempl.xtFuncDelegate(delegate (XVar _params)
            {
                XVar funcParameters = MVCFunctions.array_merge(_params, parameters);

                MethodInfo function = null;

                if (functionOwner as object != null)
                {
                    function = functionOwner.GetType().GetMethod(functionName.ToString(), BindingFlags.Instance | BindingFlags.Public);
                }

                if (function == null)
                {
                    function = typeof(MVCFunctions).GetMethod(functionName.ToString(), BindingFlags.Static | BindingFlags.Public);
                    if (function == null)
                    {
                        function = typeof(CommonFunctions).GetMethod(functionName.ToString(), BindingFlags.Static | BindingFlags.Public);
                    }
                    if (function == null)
                    {
                        return null;
                    }
                }

                return function.Invoke(functionOwner,
                    function.GetParameters().Length > 0 ? new object[] { funcParameters } : null
                    ) as XVar;
            }));
        }

        public static XVar xt_include(XVar parameters)
        {
            if (parameters != null && parameters["file"] != null)
            {
                XVar fileName = getabspath(parameters["file"]);
                if (file_exists(fileName))
                {
                    return File.ReadAllText(fileName.ToString(), Encoding.GetEncoding((int)GlobalVars.cCodePage));
                }
            }

            return new XVar();
        }

/*
        public static XVar xt_buildeditcontrol(XVar var_params)
        {
            XVar additionalCtrlParams, extraParams, field, fieldNum, id, mode, validate;

            dynamic pageObj = var_params["pageObj"];


            field = var_params["field"];
            if (var_params["mode"] == "edit")
            {
                mode = Constants.MODE_EDIT;
            }
            else
            {
                if (var_params["mode"] == "add")
                {
                    mode = Constants.MODE_ADD;
                }
                else
                {
                    if (var_params["mode"] == "inline_edit")
                    {
                        mode = Constants.MODE_INLINE_EDIT;
                    }
                    else
                    {
                        if (var_params["mode"] == "inline_add")
                        {
                            mode = Constants.MODE_INLINE_ADD;
                        }
                        else
                        {
                            mode = Constants.MODE_SEARCH;
                        }
                    }
                }
            }
            fieldNum = 0;
            if (var_params["fieldNum"])
            {
                fieldNum = var_params["fieldNum"];
            }
            id = "";
            if (!var_params["id"].Equals(""))
            {
                id = var_params["id"];
            }
            validate = new XVar();
            if (MVCFunctions.count(var_params["validate"]) != 0)
            {
                validate = var_params["validate"];
            }
            additionalCtrlParams = new XVar();
            if (MVCFunctions.count(var_params["additionalCtrlParams"]) != 0)
            {
                additionalCtrlParams = var_params["additionalCtrlParams"];
            }
            extraParams = new XVar();
            if (MVCFunctions.count(var_params["extraParams"]) != 0)
            {
                extraParams = var_params["extraParams"];
            }

            // Should use global $gSettings, $data; somehow
            XVar data = new XVar();
            if (var_params["data"])
            {
                data = var_params["data"];
            }
            else
            {
                data = pageObj.getFieldControlsData();
            }

            MVCFunctions.ob_start();
            pageObj.getControl(field, id, extraParams).buildControl(var_params["value"], mode, fieldNum, validate, additionalCtrlParams, data);
            string controlHTML = MVCFunctions.ob_get_contents();
            MVCFunctions.ob_end_clean();
            return controlHTML;
        }
*/
        #endregion

        public static XVar GetRootPathForResources(dynamic filePath)
        {
            return filePath;
        }

        public static XVar httpDateString(dynamic _param_value)
        {
            return (PHP_NULL_TIME + TimeSpan.FromSeconds(_param_value.ToInt())).ToUniversalTime().ToString("r");
        }

        public static void SetCookie(XVar name, XVar value, XVar expire = null, XVar path = null, XVar domain = null, XVar secure = null, XVar httponly = null, XVar sameSiteStrict = null)
        {
            HttpCookie cookie = new HttpCookie(name.ToString(), value.ToString());

            if (expire as object != null)
                cookie.Expires = PHP_NULL_TIME + TimeSpan.FromSeconds(expire.ToInt());

            if (path as object != null && path.ToString() != string.Empty)
                cookie.Path = path.ToString();

            if (domain as object != null && domain.ToString() != string.Empty)
                cookie.Domain = domain.ToString();

            if (secure as object != null && secure == true)
                cookie.Secure = true;

            if (httponly as object != null && httponly == true)
                cookie.HttpOnly = true;

            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Request.Cookies.Set(cookie); // in the case if GetCookie is used just after SetCookie
        }

        public static XVar GetCookie(XVar name)
        {
            // mvc bug. HttpContext.Current.Request.Cookies indexed by case insensitive name.

            int keyIndex = -1;
            int curIndex = 0;
            foreach (var key in HttpContext.Current.Request.Cookies.Keys)
            {
                if (key.ToString() == name.ToString())
                {
                    keyIndex = curIndex;
                    break;
                }
                curIndex++;
            }

            if (keyIndex == -1)
                return XVar.Pack(null);

            var cookie = HttpContext.Current.Request.Cookies[keyIndex];
            if (cookie != null)
                return XVar.Pack(cookie.Value);
            return XVar.Pack(null);
        }

        public static void RemoveCookie(XVar name)
        {
            HttpContext.Current.Request.Cookies.Remove(name);
            //			HttpContext.Current.Response.Cookies.Remove(name);
        }

        public static XVar str_replace(XVar oldValue, XVar newValue, XVar str)
        {
            if (str == null)
                return "";

            return str.Replace(oldValue, newValue);
        }

        public static XVar str_ireplace(XVar oldValue, XVar newValue, XVar str)
        {
            if (str == null)
                return "";

            return str.Replace(oldValue, newValue, true);
        }

        public static XVar str_pad(XVar input, XVar pad_length, XVar pad_string = null, XVar pad_type = null)
        {
            if (pad_string as object == null)
                pad_string = new XVar(" ");

            if (pad_type as object == null)
                pad_type = Constants.STR_PAD_RIGHT;

            string strInput = input.ToString();

            if (pad_length.ToInt() <= strInput.Length)
                return input;

            string leftPadding = "";
            if (pad_type.ToInt() == Constants.STR_PAD_LEFT || pad_type.ToInt() == Constants.STR_PAD_BOTH)
            {
                int lpadLen = pad_length.ToInt() - strInput.Length;
                if (pad_type.ToInt() == Constants.STR_PAD_BOTH)
                    lpadLen /= 2;

                leftPadding = new System.Text.StringBuilder().Insert(0, pad_string.ToString(), lpadLen / pad_string.Length() + 1).ToString();
                leftPadding = leftPadding.Substring(0, lpadLen);
            }

            string rightPadding = "";
            if (pad_type.ToInt() == Constants.STR_PAD_RIGHT || pad_type.ToInt() == Constants.STR_PAD_BOTH)
            {
                int rpadLen = pad_length.ToInt() - strInput.Length;
                if (pad_type.ToInt() == Constants.STR_PAD_BOTH)
                    rpadLen = rpadLen - rpadLen / 2;

                rightPadding = new System.Text.StringBuilder().Insert(0, pad_string.ToString(), rpadLen / pad_string.Length() + 1).ToString();
                rightPadding = rightPadding.Substring(0, rpadLen);
            }

            return leftPadding + strInput + rightPadding;
        }

        public static XVar nl2br(XVar str)
        {
            if (str == null)
                return "";
            return str.nl2br();
        }

        public static XVar in_array(XVar val, XVar array)
        {
            if (array == null)
                return false;
            return array.ValueExists(val);
        }

        public static XVar array_keys(XVar array, XVar searchKey = null)
        {
            if (array == null)
                return XVar.Array();
            return array.ArrayKeys(searchKey);
        }

        public static XVar array_slice(XVar array, XVar firstIndex, XVar length = null)
        {
            if (array == null)
                return XVar.Array();

            if (length as object == null)
                length = array.Count();

            return array.ArraySlice(firstIndex, length);
        }

        public static XVar intval(XVar val)
        {
            if (val == null)
                return 0;
            return val.ToInt();
        }

        public static XVar split(XVar array)
        {
            throw new ApplicationException("Function \"split\" is deprecated");
        }

        public static XVar str_split(XVar str, XVar length = null)
        {
            if (str == null)
                return XVar.Array();

            return str.SplitByLen(length);
        }

        public static XVar strpos(XVar str, XVar needle, XVar startIndex = null)
        {
            if (str == null)
                return false;
            return str.IndexOf(needle, startIndex);
        }

        public static XVar strrpos(XVar str, XVar needle, XVar startIndex = null)
        {
            if (str == null)
                return false;
            return str.LastIndexOf(needle, startIndex);
        }

        public static XVar function_exists(XVar funcName)
        {
            throw new NotImplementedException();
        }

        public static XVar stripos(XVar str, XVar needle, XVar startIndex = null)
        {
            if (str == null)
                return false;
            return str.CaseInsensitiveIndexOf(needle, startIndex);
        }

        public static XVar preg_match(XVar pattern, XVar str, XVar matches = null)
        {
            if (str == null || pattern == null)
                return false;

            return str.PregMatch(pattern, matches);
        }

        public static XVar preg_match_all(XVar pattern, XVar str, XVar matches = null)
        {
            if (str == null || pattern == null)
                return false;

            if (matches as object == null)
                matches = XVar.Array();

            return str.PregMatchAll(pattern, matches);
        }

        public static XVar findMatches(dynamic _param_pattern, dynamic _param_str)
        {
            XVar matches = XVar.Array();
            _param_str.PregFindMatches(_param_pattern, matches);
            return matches;
        }


        public static XVar preg_replace(XVar pattern, XVar replacement, XVar str, XVar limit = null)
        {
            if (str == null || pattern == null)
                return str;

            if (replacement == null)
                replacement = "";

            return str.PregReplace(pattern, replacement, str, limit);
        }

        public static XVar preg_split(XVar pattern, XVar str, XVar limit = null, XVar flags = null)
        {
            if (str == null || pattern == null)
                return str;

            // flag PREG_SPLIT_DELIM_CAPTURE is not needed in c#
            // limit is not used

            return str.PregSplit(pattern, str);
        }

        public static XVar preg_quote(dynamic str, dynamic delimeter = null)
        {
            string curStr = str == null ? "" : str.ToString();

            var result = Regex.Escape(str);
            if (delimeter as object != null && delimeter != "")
                result = result.Replace(delimeter.ToString(), "\\" + delimeter.ToString());

            return result;
        }

        public static XVar strrev(XVar str)
        {
            if (str == null)
                return "";
            return str.Reverse();
        }

        public static XVar strtolower(XVar str)
        {
            if (str == null)
                return "";
            return str.ToLower();
        }

        public static XVar strtoupper(XVar str)
        {
            if (str == null)
                return "";
            return str.ToUpper();
        }

        public static XVar GetPageURLWithGetParams()
        {
            string pageURL = HttpContext.Current.Request.RawUrl;
            if (pageURL.IndexOf("?") > 0)
            {
                pageURL += "&pdf=1";
            }
            else
            {
                pageURL += "?pdf=1";
            }
            return pageURL;
        }

        public static XVar array_change_key_case(XVar array, int caseType)
        {
            if (array == null)
            {
                return XVar.Array();
            }
            XVar result = XVar.Array();
            foreach (var item in array.GetEnumerator())
            {
                XVar key = caseType == Constants.CASE_LOWER ? item.Key.ToLower() : item.Key.ToUpper();
                result[key] = item.Value;
            }
            return result;
        }

        public static XVar base64_encode(XVar plainText)
        {
            byte[] toEncodeAsBytes;

            if (plainText.IsByteArray())
                toEncodeAsBytes = (byte[])plainText.Value;
            else
                toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static XVar base64_decode(XVar encodedData)
        {
            // php base64_decode returns false on failure
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static XVar md5(XVar plainText, XVar raw_output = null)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = plainText.IsByteArray() ? (byte[])plainText.Value : ASCIIEncoding.Default.GetBytes(plainText);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            if (raw_output)
            {
                return new XVar(encodedBytes);
            }
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }

        public static XVar ord(XVar character)
        {
            if (character == null)
            {
                return null;
            }
            return new XVar(Convert.ToInt32(Convert.ToChar(character.ToString())));
        }

        public static XVar chr(XVar characterCode)
        {
            if (characterCode == null)
                characterCode = 0;

            return new XVar(Convert.ToChar(characterCode.ToInt()).ToString());
        }

        public static XVar errhandler_db_query_safe(dynamic _param_errno, dynamic _param_errstr, dynamic _param_errfile, dynamic _param_errline)
        {
            // not used in .net
            throw new NotImplementedException();
        }


        public static XVar isAbsolutePath(dynamic path)
        {
            if (path == null)
                return false;

            return Path.IsPathRooted(path.ToString());
        }

        public static XVar json_mb_encode_numericentity(dynamic item, dynamic _param_key)
        {
            // not used in .net
            throw new NotImplementedException();
        }

        public static XVar makeFloat(dynamic value)
        {
            if (value == null)
                return 0;

            double d = 0;
            Double.TryParse(value.ToString().Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d);

            return new XVar(d);
        }

        public static XVar runner_htmlspecialchars(dynamic str)
        {
            if (str == null)
                return new XVar("");
            return htmlspecialchars(str);
        }

        public static XVar runner_strlen(dynamic str)
        {
            if (str == null)
                return new XVar(0);

            return str.ToString().Length;
        }

        public static XVar runner_strpos(dynamic haystack, dynamic needle, dynamic offset = null)
        {
            if (offset as object == null)
                offset = 0;

            if (needle == null || haystack == null)
                return new XVar(-1);

            string str = haystack.ToString();
            int ret;
            if (offset >= 0)
            {
                ret = str.IndexOf(needle.ToString(), offset);
            }
            else
            {
                ret = str.IndexOf(needle.ToString(), str.Length + offset, -offset);
            }
            if (ret == -1)
                return false;
            return ret;
        }

        public static XVar runner_strrpos(dynamic haystack, dynamic needle, dynamic offset = null)
        {
            if (offset as object == null)
                offset = 0;

            if (needle == null || haystack == null)
                return new XVar(-1);

            string str = haystack.ToString();
            int ret;
            if (offset >= 0)
                ret = str.LastIndexOf(needle.ToString(), str.Length - 1, str.Length - offset);
            else
                ret = str.LastIndexOf(needle.ToString(), str.Length - 1, -offset);
            if (ret == -1)
                return false;
            return ret;
        }

        public static XVar runner_substr(dynamic str, dynamic start, dynamic length = null)
        {
            if (str == null || start == null)
                return new XVar("");

            return str.Substring(start, length);
        }

        public static XVar runner_convert_encoding(dynamic _param_str, dynamic _param_to_encoding, dynamic _param_from_encoding)
        {
            Encoding target = Encoding.GetEncoding((string)_param_to_encoding);
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes((string)_param_str);
            byte[] isoBytes = Encoding.Convert(utf8, target, utfBytes);
            string msg = target.GetString(isoBytes);
            return (XVar)msg;
        }

        public static XVar runner_encode_numeric_entity(dynamic _param_str, dynamic _param_convmap, dynamic _param_encoding)
        {
            // not used in .net
            throw new NotImplementedException();
        }

        public static XVar runner_decode_numeric_entity(dynamic _param_str, dynamic _param_convmap, dynamic _param_encoding)
        {
            // not used in .net
            throw new NotImplementedException();
        }

        public static XVar hasNonAsciiSymbols(dynamic str)
        {
            string curStr = str == null ? "" : str.ToString();

            for (int i = 0; i < curStr.Length; i++)
            {
                if (curStr[i] > 127)
                    return true;
            }

            return false;
        }

        public static XVar runner_json_encode_unescaped_unicode(dynamic value)
        {
            return runner_json_encode(value);
        }

        public static XVar strtotime(dynamic timeStr)
        {
            if (timeStr == null)
                return 0;

            DateTime result;
            if (string.Compare(timeStr, "now", true) == 0)
                result = DateTime.Now;
            else DateTime.TryParse(timeStr, System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out result); // date only in en-US style

            return new XVar(toPHPTime(new XVar(result)));
        }

        public static XVar number_format(XVar number, XVar decimals = null, XVar dec_point = null, XVar thousands_sep = null)
        {
            if (decimals as object == null)
                decimals = 0;
            if (dec_point as object == null)
                dec_point = ".";
            if (thousands_sep as object == null)
                thousands_sep = ",";

            var format = string.Format("{{0:n{0}}}", decimals);
            Object[] args = new Object[1];
            args[0] = (double)number;
            string result = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, args);

            if (dec_point != "." || thousands_sep != ",")
            {
                result = result.Replace(',', 'T');
                result = result.Replace('.', 'D');
                result = result.Replace("D", dec_point);
                result = result.Replace("T", thousands_sep);
            }
            return result;
        }

        public static XVar extension_loaded(XVar extension)
        {
            return false;
        }

        public static XVar usort(XVar arr, XVar parameters)
        {
            if (arr == null)
                return null;

            arr.USort(parameters);
            return null;
        }

        public static XVar runner_serialize_array(XVar obj)
        {
            return runner_json_encode(obj);
        }

        public static XVar runner_unserialize_array(XVar obj)
        {
            return runner_json_decode(obj);
        }

        public static XVar getHTMLEntityData(dynamic encodedEntity)
        {
            var oldStr = encodedEntity.ToString();
            var str = WebUtility.HtmlDecode(oldStr);
            if (oldStr == str)
                return new XVar("isHTMLEntity", false, "entityLength", 0);
            else return new XVar("isHTMLEntity", true, "entityLength", runner_strlen(str));
        }

        public static XVar runner_html_entity_decode(dynamic _param_str)
        {
            var oldStr = _param_str.ToString();
            var str = WebUtility.HtmlDecode(oldStr.Replace("&nbsp;", " "));
            return str;
        }

        public static XVar date(XVar format, XVar phpTime = null)
        {
            var dt = phpTime as object == null ? DateTime.Now : fromPHPTime(phpTime);

            var res = "";
            foreach (var c in format.ToString())
                res += ConvertFormat(c, dt);

            return res;
        }

        public static XVar gmdate(XVar format, XVar timestamp = null)
        {
            throw new NotImplementedException();
        }

        public static XVar iso8601date(dynamic _param_timestamp)
        {
            return MVCFunctions.fromPHPTime(_param_timestamp).ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static XVar iso8601date_timestamp(dynamic _param_timestamp)
        {
            return MVCFunctions.fromPHPTime(_param_timestamp).ToString("yyyyMMddTHHmmssZ");
        }

        public static XVar cal_to_jd(XVar calendar, XVar month, XVar day, XVar year)
        {
            DateTime dt = new DateTime();
            if (calendar.Equals(0))
                dt = new DateTime((int)year, (int)month, (int)day, new System.Globalization.GregorianCalendar());
            else if (calendar.Equals(1))
                dt = new DateTime((int)year, (int)month, (int)day, new System.Globalization.JulianCalendar());
            else
                dt = new DateTime((int)year, (int)month, (int)day);

            return new XVar(Math.Ceiling(dt.ToOADate() + 2415018.5));
        }

        public static XVar cal_from_jd(XVar jd, XVar calendar)
        {
            DateTime dt = DateTime.FromOADate(jd - 2415018.5);

            if (calendar.Equals(0) || calendar.Equals(1))
            {
                System.Globalization.Calendar calObj = calendar.Equals(1) ? (System.Globalization.Calendar)new System.Globalization.JulianCalendar()
                    : (System.Globalization.Calendar)new System.Globalization.GregorianCalendar();

                dt = new DateTime(calObj.GetYear(dt), calObj.GetMonth(dt), calObj.GetDayOfMonth(dt));
            }

            var arrDate = XVar.Array();
            arrDate["date"] = dt.ToShortDateString();
            arrDate["month"] = dt.Month;
            arrDate["day"] = dt.Day;
            arrDate["year"] = dt.Year;
            arrDate["abbrevmonth"] = dt.ToString("MMM");
            arrDate["monthname"] = dt.ToString("MMMM");

            if (calendar.Equals(1)) // julian has enumeration starting with 1
            {
                dt = dt.AddDays(-1);
            }

            arrDate["dow"] = (int)dt.DayOfWeek;
            arrDate["abbrevdayname"] = dt.ToString("ddd");
            arrDate["dayname"] = dt.DayOfWeek.ToString();


            return arrDate;
        }

        static string ConvertFormat(char c, DateTime dt)
        {
            switch (c)
            {
                case 'd':
                    return dt.ToString("dd");
                case 'D':
                    return dt.ToString("ddd");
                case 'j':
                    return dt.ToString("%d");
                case 'l':
                    return dt.ToString("dddd");
                case 'N':
                    return ((int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek).ToString();
                case 'S':
                    return (dt.Day == 1 || dt.Day == 21 || dt.Day == 31) ? "st" : (dt.Day == 2 || dt.Day == 22) ? "nd" : (dt.Day == 3 || dt.Day == 23) ? "rd" : "th";
                case 'w':
                    return ((int)dt.DayOfWeek).ToString();
                case 'z':
                    return dt.DayOfYear.ToString();
                case 'W': // annoying ISO 8601 standard
                    return System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(
                         dt.DayOfWeek >= DayOfWeek.Monday && dt.DayOfWeek <= DayOfWeek.Wednesday ? dt.AddDays(3) : dt,
                         System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();
                case 'F':
                    return dt.ToString("MMMM");
                case 'm':
                    return dt.ToString("MM");
                case 'M':
                    return dt.ToString("MMM");
                case 'n':
                    return dt.ToString("%M");
                case 't':
                    return dt.Day.ToString();
                case 'L':
                    return System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.IsLeapYear(dt.Year) ? "1" : "0";
                case 'o':
                    return dt.ToString("yyyy");
                case 'Y':
                    return dt.ToString("yyyy");
                case 'y':
                    return dt.ToString("yy");
                case 'a':
                    return dt.ToString("tt").ToLower();
                case 'A':
                    return dt.ToString("tt");
                case 'B':
                    return (dt.ToUniversalTime().AddHours(1).TimeOfDay.TotalMilliseconds / 86400d).ToString();
                case 'g':
                    return dt.ToString("%h");
                case 'G':
                    return dt.ToString("%H");
                case 'h':
                    return dt.ToString("hh");
                case 'H':
                    return dt.ToString("HH");
                case 'i':
                    return dt.ToString("mm");
                case 's':
                    return dt.ToString("ss");
                case 'u':
                    return dt.ToString("fffff");
                case 'e':
                    return TimeZone.CurrentTimeZone.StandardName;
                case 'I':
                    return TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt) ? "1" : "0";
                case 'O':
                    var offset = TimeZone.CurrentTimeZone.GetUtcOffset(dt);
                    return string.Format("{2}{0:00}{1:00}", Math.Abs(offset.TotalHours), offset.TotalMinutes, offset.TotalHours < 0 ? "-" : "+");
                case 'P':
                    var offset1 = TimeZone.CurrentTimeZone.GetUtcOffset(dt);
                    return string.Format("{2}{0:00}:{1:00}", Math.Abs(offset1.TotalHours), offset1.TotalMinutes, offset1.TotalHours < 0 ? "-" : "+");
                case 'T':
                    return string.Join("", TimeZone.CurrentTimeZone.StandardName.Split(' ').Select(x => x[0].ToString().ToUpper()).ToArray());
                case 'Z':
                    return TimeZone.CurrentTimeZone.GetUtcOffset(dt).TotalSeconds.ToString();
                case 'c':
                    return dt.ToUniversalTime().ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                case 'r':
                    return dt.ToString("r");
                case 'U':
                    return toPHPTime(new XVar(dt)).ToString();
                default:
                    return c.ToString();
            }
        }

        public static XVar file_get_contents(XVar file)
        {
            return myfile_get_contents(file);
        }

        public static XVar sprintf(XVar format, params XVar[] args)
        {
            var array = XVar.Array();
            foreach (var arg in args.ToList())
                array.Add(arg);

            return mysprintf(format, array);
        }

        public static XVar feof(XVar file)
        {
            if (file == null || file.Type != typeof(FileStream))
                return true;

            var stream = file.Value as FileStream;
            return stream.Position >= stream.Length;
        }

        public static XVar fread(XVar file, XVar amount)
        {
            if (file == null || file.Type != typeof(FileStream))
                return true;

            if (amount == 0)
                return null;

            var stream = file.Value as FileStream;

            var res = new byte[amount];
            stream.Read(res, (int)stream.Position, amount);

            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            return enc.GetString(res);
        }

        public static XVar dirname(XVar file)
        {
            return System.IO.Path.GetDirectoryName(file);
        }

        public static XVar strval(XVar value)
        {
            return value.ToString();
        }

        public static XVar GetFile(XVar name)
        {
            // TODO: REFACTOR
            XVar result = XVar.Array();
            HttpPostedFile uploadedFile = HttpContext.Current.Request.Files[name.ToString()];
            XVar uploadedFileData = XVar.Array();
            uploadedFileData["name"] = uploadedFile.FileName;
            uploadedFileData["tmp_name"] = uploadedFile.FileName;
            uploadedFileData["size"] = uploadedFile.ContentLength;
            uploadedFileData["type"] = uploadedFile.ContentType;
            uploadedFileData["error"] = "";
            uploadedFileData["file"] = uploadedFile;
            return uploadedFileData;
        }

        public static XVar move_uploaded_file(XVar fileName, XVar targetName)
        {
            if (file_exists(fileName))
                return copy(fileName, targetName);
            else return false;
        }

        public static XVar runner_getimagesize(XVar file_name, XVar uploadedFile)
        {
            XVar result = new XVar(0, null, 1, null);
            if (uploadedFile)
            {
                try
                {
                    using (Image originalImage = MVCFunctions.ImageFromStream((uploadedFile["file"].Value as HttpPostedFile).InputStream))
                    {
                        result[0] = originalImage.Width;
                        result[1] = originalImage.Height;
                    }
                }
                catch (Exception ex)
                {
                    result[0] = 0;
                    result[1] = 0;
                }
            }

            return result;
        }

        public static XVar strstr(XVar haystack, XVar needle, XVar before_needle = null)
        {
            if (haystack == null)
                return new XVar(false);
            if (needle == null)
                return haystack;

            XVar pos = strpos(haystack, needle);
            if (pos != false)
            {
                if (before_needle as object != null && before_needle == true)
                    return substr(haystack, 0, pos);
                else return substr(haystack, pos);
            }
            else return pos;
        }

        public static XVar stristr(XVar haystack, XVar needle, XVar before_needle = null)
        {
            if (haystack == null)
                return new XVar(false);
            if (needle == null)
                return haystack;

            XVar pos = stripos(haystack, needle);
            if (pos != false)
            {
                if (before_needle as object != null && before_needle == true)
                    return substr(haystack, 0, pos);
                else return substr(haystack, pos);
            }
            else return pos;
        }

        public static XVar substr_count(XVar haystack, XVar needle)
        {
            if (haystack == null || needle == null)
                return 0;

            var hStr = haystack.ToString();
            var nStr = needle.ToString();

            return (hStr.Length - hStr.Replace(nStr, "").Length) / nStr.Length;
        }

        public static XVar ucfirst(XVar str)
        {
            if (str == null)
                return str;

            string s = str.ToString();

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static XVar trigger_error(XVar str, XVar code = null)
        {
            throw new ApplicationException(str);
        }

        public static XVar crc32(XVar str)
        {
            if (str as object == null)
                return 0;

            return str.ToString().GetHashCode();
        }

        public static XVar pack(XVar format, params XVar[] args)
        {
            if (format == "v")
                return args[0];
            else if (format == "V")
                return args[0];
            else return 0;
        }

        public static XVar array_merge_assoc(params XVar[] args)
        {
            return array_merge(args);
        }

        public static XVar Ciphcoding()
        {
            /*
			var res = CommonFunctions.Ciphcoding();
            if (res != "")
            {
                Echo(res);
                Exit();
            }
			*/
            return null;
        }

        public static XVar formatNumberForHTML5(dynamic _param_value)
        {
            return _param_value;
        }



        public static XVar useMySQLiLib()
        {
            return false;
        }

        public static XVar isSqlsrvExtLoaded()
        {
            return true;
        }

        public static XVar useMSSQLWinConnect()
        {
            return true;
        }

        /*
                public static XVar DoInsertRecordOnAdd(dynamic pageObject)
                {
                    return CommonFunctions.DoInsertRecordSQLOnAdd( pageObject );
                }
        */

        public static XVar runner_copy_file(dynamic _param_source, dynamic _param_dest)
        {
            if (File.Exists(_param_source))
            {
                File.Copy(_param_source, _param_dest, true);
            }
            return true;
        }
        public static XVar tempnam(dynamic _param_dir, dynamic _param_prefix)
        {
            string fileName = string.Empty;
            string dir = _param_dir == "" ? Path.GetTempPath() : _param_dir;
            try
            {
                for (int i = 0; i < 10; ++i) // max 10 attempts
                {
                    string name = _param_prefix + Path.GetRandomFileName();
                    fileName = Path.Combine(dir, name);

                    if (!File.Exists(fileName))
                        break;
                }

                using (new FileStream(fileName, FileMode.CreateNew)) { }
            }
            catch (Exception ex)
            {
                return false;
            }
            return fileName;
        }

        public static XVar cutBOM(dynamic _param_line, string bom = "")
        {
            string str = (string)_param_line;
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

            if (str.StartsWith(_byteOrderMarkUtf8))
            {
                str = str.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return (XVar)str;
        }

        public static XVar printBOM()
        {
            HttpContext.Current.Response.Write("\xfeff");
            return true;
        }

        /**
		 * save text for import
		 */
        public static XVar runner_save_textfile(dynamic _param_fileName, dynamic _param_txtData)
        {
            var dir = System.IO.Path.GetDirectoryName(_param_fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            StreamWriter outfile = new StreamWriter((string)_param_fileName, false, enc);

            outfile.Write((string)_param_txtData);
            outfile.Close();
            return true;
        }

        public static XVar deleteTemporaryFilesFromDirTMP()
        {
            throw new NotImplementedException();
        }

        static SortedDictionary<char, string> _PHPCSFormats;

        public static XVar runner_date_format(dynamic _param_param, dynamic _param_date = null)
        {
            DateTime param_date = DateTime.Now;
            try
            {
                if (_param_date != null && !String.IsNullOrEmpty(_param_date.ToString()))
                    param_date = fromPHPTime(_param_date);
            }
            catch
            {
                param_date = DateTime.Now;
            }

            if (_PHPCSFormats == null)
            {
                _PHPCSFormats = new SortedDictionary<char, string>();
                _PHPCSFormats['d'] = "dd";
                _PHPCSFormats['D'] = "ddd";
                _PHPCSFormats['j'] = "%d";
                _PHPCSFormats['l'] = "dddd";
                _PHPCSFormats['F'] = "MMMM";
                _PHPCSFormats['m'] = "MM";
                _PHPCSFormats['M'] = "MMM";
                _PHPCSFormats['n'] = "%M";
                _PHPCSFormats['o'] = "yyyy";
                _PHPCSFormats['Y'] = "yyyy";
                _PHPCSFormats['y'] = "yy";
                _PHPCSFormats['a'] = "tt";
                _PHPCSFormats['A'] = "tt";
                _PHPCSFormats['g'] = "%h";
                _PHPCSFormats['G'] = "%H";
                _PHPCSFormats['h'] = "hh";
                _PHPCSFormats['H'] = "HH";
                _PHPCSFormats['i'] = "mm";
                _PHPCSFormats['s'] = "ss";
            }
            StringBuilder CSFormat = new StringBuilder();
            foreach (char c in _param_param.ToString())
            {
                string subst;
                if (_PHPCSFormats.TryGetValue(c, out subst))
                    CSFormat.Append(subst);
                else
                    CSFormat.Append(c);
            }
            return param_date.ToString(CSFormat.ToString());
        }

        public static XVar runner_set_page_timeout(dynamic _param_seconds)
        {
            return null;
        }

        public static XVar getPasswordHash(XVar plainText)
        {
            return (XVar)BCrypt.Net.BCrypt.HashPassword(plainText.ToString());
        }

        public static bool passwordVerify(XVar pass, XVar hash)
        {
            string strHash = hash.ToString();
            if (strHash.StartsWith("$2y$"))
            {
                strHash = "$2a$" + strHash.Substring(4);
            }
            bool verifyOK = false;
            try
            {
                verifyOK = BCrypt.Net.BCrypt.Verify(pass.ToString(), strHash);
            }
            catch (Exception)
            {
                verifyOK = false;
            }
            return verifyOK;
        }


        public delegate XVar dashSnippetDelegate(ref XVar header);

        public static XVar runner_print_r(dynamic _param_value, dynamic _param_return = null)
        {
            throw new NotImplementedException();
        }

        public static XVar getArrayWithoutObjects(dynamic _param_arr, dynamic print_r_depth)
        {
            throw new NotImplementedException();
        }

        public static XVar getRawLWWhere(dynamic _param_field, dynamic _param_ptype, dynamic _param_table = null)
        {
            throw new NotImplementedException();
        }

        public static XVar runner_sms(dynamic _param_number, dynamic _param_message, dynamic _param_parameters = null)
        {
            return CommonFunctions.runner_sms(_param_number, _param_message, _param_parameters);
        }


        public static XVar runner_post_request(dynamic _param_url, dynamic _param_parameters, dynamic _param_headers = null, dynamic _param_certPath = null)
        {

            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768);
            XVar result = XVar.Array();
            result.InitAndSetArrayItem(false, "error");
            result.InitAndSetArrayItem(null, "content");

            SecureWebClient wc = new SecureWebClient(_param_certPath);
            var URI = new Uri(_param_url.ToString());

            foreach (KeyValuePair<XVar, dynamic> value in _param_headers.GetEnumerator())
            {
                wc.Headers[value.Key.ToString()] = value.Value.ToString();
            }

            NameValueCollection data = new NameValueCollection();
            foreach (KeyValuePair<XVar, dynamic> value in _param_parameters.GetEnumerator())
            {
                data[value.Key.ToString()] = value.Value.ToString();
            }

            try
            {
                string content = Encoding.UTF8.GetString(wc.UploadValues(URI, "POST", data));
                result.InitAndSetArrayItem(content, "content");
            }
            catch (WebException e)
            {
                result.InitAndSetArrayItem(e.Message, "error");
            }
            catch (ArgumentNullException e)
            {
                result.InitAndSetArrayItem(e.Message, "error");
            }
            return result;
        }

        public static XVar runner_http_request_old(dynamic _param_url, dynamic _param_parameters = null, dynamic method = null, dynamic _param_headers = null, dynamic _param_certPath = null)
        {

            if (method as Object == null)
            {
                method = new XVar("GET");
            }
            if (_param_parameters as Object == null)
            {
                _param_parameters = XVar.Array();
            }
            if (_param_headers as Object == null)
            {
                _param_headers = XVar.Array();
            }
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768);
            XVar result = XVar.Array();
            result.InitAndSetArrayItem(false, "error");
            result.InitAndSetArrayItem(null, "content");

            SecureWebClient wc = new SecureWebClient(_param_certPath);
            var URI = new Uri(_param_url.ToString());

            foreach (KeyValuePair<XVar, dynamic> value in _param_headers.GetEnumerator())
            {
                wc.Headers[value.Key.ToString()] = value.Value.ToString();
            }

            NameValueCollection data = new NameValueCollection();
            foreach (KeyValuePair<XVar, dynamic> value in _param_parameters.GetEnumerator())
            {
                data[value.Key.ToString()] = value.Value.ToString();
            }

            try
            {
                if (method == "POST")
                {
                    string content = Encoding.UTF8.GetString(wc.UploadValues(URI, "POST", data));
                    result["content"] = content;
                }
                else
                {
                    XVar parameters = XVar.Array();
                    foreach (KeyValuePair<XVar, dynamic> value in _param_parameters.GetEnumerator())
                    {
                        parameters.InitAndSetArrayItem(value.Key.ToString() + "=" + MVCFunctions.urlencode(value.Value.ToString()).ToString(), null);
                    }
                    String url = _param_url.ToString();
                    if (MVCFunctions.count(parameters) > 0)
                    {
                        if (url.Contains("?"))
                            url += "&";
                        else
                            url += "?";
                        url += MVCFunctions.implode("&", parameters).ToString();
                    }
                    URI = new Uri(url);
                    string content = Encoding.UTF8.GetString(wc.DownloadData(URI));
                    result["content"] = content;
                }
            }
            catch (WebException e)
            {
                result.InitAndSetArrayItem(e.Message, "error");
            }
            catch (ArgumentNullException e)
            {
                result.InitAndSetArrayItem(e.Message, "error");
            }
            return result;
        }


        private static Stream decodedResponseStream(Stream st, string contentEncodingValue)
        {
            string[] encodings = contentEncodingValue.Split(',');
            foreach (string alg in encodings)
            {
                switch (alg.Trim())
                {
                    case "gzip":
                        st = new GZipStream(st, CompressionMode.Decompress);
                        break;
                    case "deflate":
                        st = new DeflateStream(st, CompressionMode.Decompress);
                        break;
                    case "br":
                        throw new Exception("Content-Encoding: br - not supported");
                        //responseStream = new BrotliStream(responseStream, CompressionMode.Decompress);
                        break;
                    case "identity":
                        //do nothing
                        break;
                    case "compress":
                        throw new Exception("Content-Encoding: compress - not supported");
                }
            }
            return st;
        }

        public static XVar runner_http_request(dynamic _param_url, dynamic _body = null, dynamic method = null, dynamic _param_headers = null, dynamic _param_certPath = null)
        {

            if (method as Object == null)
            {
                method = new XVar("GET");
            }
            if (_body as Object == null)
            {
                _body = new XVar("");
            }
            if (_param_headers as Object == null)
            {
                _param_headers = XVar.Array();
            }
            if (_param_certPath as Object == null)
            {
                _param_certPath = new XVar("");
            }
            string url = _param_url.ToString();

            XVar body = new XVar(_body);

            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(3072 | 768);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString();
            foreach (KeyValuePair<XVar, dynamic> value in _param_headers.GetEnumerator())
            {
                if (value.Key.ToString().Equals("content-type", StringComparison.OrdinalIgnoreCase))
                {
                    request.ContentType = value.Value.ToString();
                }
                else if (value.Key.ToString().Equals("accept", StringComparison.OrdinalIgnoreCase))
                {
                    request.Accept = value.Value.ToString();
                }
                else if (value.Key.ToString().Equals("user-agent", StringComparison.OrdinalIgnoreCase))
                {
                    request.UserAgent = value.Value.ToString();
                }
                else if (value.Key.ToString().Equals("content-length", StringComparison.OrdinalIgnoreCase))
                {
                    request.ContentLength = value.Value.ToInt();
                }
                else if (value.Key.ToString().Equals("host", StringComparison.OrdinalIgnoreCase))
                {
                    request.Host = value.Value.ToString();
                }
                else if (value.Key.ToString().Equals("referer", StringComparison.OrdinalIgnoreCase))
                {
                    request.Referer = value.Value.ToString();
                }
                else if (value.Key.ToString().Equals("transfer-encoding", StringComparison.OrdinalIgnoreCase))
                {
                    request.TransferEncoding = value.Value.ToString();
                }
                else
                {
                    request.Headers[value.Key.ToString()] = value.Value.ToString();
                }
            }
            if (body != "")
            {
                byte[] data = body.IsByteArray() ? (byte[])body.Value : Encoding.ASCII.GetBytes(body.ToString());
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }
            XVar result = XVar.Array();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                processHttpResponse(response, result);
                return result;
            }
            catch (WebException e)
            {
                result["error"] = new XVar(e.Message);
                HttpWebResponse response = (HttpWebResponse)e.Response;
                if (response != null)
                {
                    processHttpResponse(response, result);
                }
                return result;
            }
            catch (Exception e)
            {
                result["error"] = new XVar(e.Message);
                return result;
            }


        }

        /**
		 *	Fill in resilt["content"] "header" and "responseCode"
		 */
        static void processHttpResponse(HttpWebResponse response, XVar result)
        {
            result["responseCode"] = new XVar((int)response.StatusCode);

            Stream responseStream = response.GetResponseStream();
			if (response.ContentEncoding != null)
            {
                responseStream = decodedResponseStream(responseStream, response.ContentEncoding);
            }

			Encoding encoding;
			try {
				encoding = Encoding.GetEncoding(response.CharacterSet);
			} catch (ArgumentException ex) {
				encoding = Encoding.Default;
			}
            
			
			StreamReader streamReader = new StreamReader(responseStream, encoding );
            result["content"] = new XVar(streamReader.ReadToEnd());
            XVar headerStrings = XVar.Array();
            for (int i = 0; i < response.Headers.Count; ++i)
            {
                headerStrings.InitAndSetArrayItem(response.Headers.Keys[i] + ":" + response.Headers[i], null);
            }
            result["header"] = MVCFunctions.implode("\r\n", headerStrings);
        }



        public static void importTableSettings(dynamic tableName)
        {
            if (!GlobalVars.runnerTableSettings.KeyExists(tableName)) {
                typeof(RunnerSettings).GetMethod( "table_" + (string)CommonFunctions.GetTableURL(tableName) ).Invoke( null, null );
                
                //  call after table init event
                MVCFunctions.createEventClass( tableName );
            }
        }

        public static void importTableOps(dynamic tableName)
        {
            dynamic t = Type.GetType(MVCFunctions.Concat("runnerDotNet.", (string)CommonFunctions.GetTableURL(tableName), "_Ops"));
            if (t != null)
            {
                t.GetMethod("Apply", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, null);
            }
        }

        public static void importTableList()
        {
			RunnerSettings.dbtables();
        }

        public static void importTableInfo(dynamic varname)
        {
            typeof(RunnerSettings).GetMethod( "dbtable_" + varname.ToString() ).Invoke( null, null );
        }

        public static void importPageOptions(dynamic table, dynamic page)
        {
            if (page == null)
                return;
            if (!GlobalVars.page_options.KeyExists(table))
            {
                GlobalVars.page_options[table] = XVar.Array();
                GlobalVars.pd_pages[table] = XVar.Array();
            }
            if (GlobalVars.page_options[table].KeyExists(page))
            {
                return;
            }

            XVar className;
            if ((string)table == "<global>")
                className = "___global___";
            else
                className = CommonFunctions.GetTableURL(table);
            Type t = Type.GetType(MVCFunctions.Concat("runnerDotNet.Options_", (string)className, "_", (string)page));

            if (t != null)
            {
                GlobalVars.page_options[table][page] = (XVar)t.GetMethod("options", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, null);
                GlobalVars.pd_pages[table][page] = (XVar)t.GetMethod("page", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, null);
            }
        }

        public static void loadMaps(dynamic _param_pSet_packed)
        {

            #region packeted values
            ProjectSettings _param_pSet = XVar.UnPackProjectSettings(_param_pSet_packed);
            #endregion

            dynamic maps = _param_pSet.maps();
            foreach (KeyValuePair<XVar, dynamic> map in maps.GetEnumerator())
            {
                GlobalVars.globalEvents.Invoke("event_" + MVCFunctions.GoodFieldName(map.Value.ToString()).ToString());
            }
        }

        public static XVar cloneArray(dynamic _param_arr)
        {
            return MVCFunctions.runner_json_decode(MVCFunctions.runner_json_encode(_param_arr));
        }

        public static XVar getPageStrings(dynamic _param_tableName)
        {
            throw new NotImplementedException();
        }

        public static XVar simplify_file_name(XVar filename)
        {
            return filename;
        }

        public static XVar base64_encode_binary(dynamic _param_data)
        {
            return base64_encode(_param_data);
        }

        public static XVar base64_decode_binary(dynamic encodedData)
        {
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                return XVar.Pack(encodedDataAsBytes);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static XVar base64_bin2str(dynamic _param_data)
        {
            return base64_encode(_param_data);
        }

        public static XVar base64_str2bin(dynamic _param_str)
        {
            return new XVar(System.Convert.FromBase64String(_param_str));
        }

        public static XVar myfile_get_contents_binary(dynamic _param_filename)
        {
            return myfile_get_contents(_param_filename);
        }

        public static XVar projectPath()
        {
            string path = System.Web.HttpUtility.UrlPathEncode(HttpContext.Current.Request.ApplicationPath);
            if (path.Length == 0 || path[path.Length - 1] != '/')
                path += "/";
            return path;
        }

        public static XVar hash(XVar alg, XVar data, XVar raw_output = null)
        {
            throw new NotImplementedException();
        }

        public static XVar hash_hmac_sha256(XVar data, XVar key, XVar raw_output = null)
        {
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            Byte[] keyBytes = key.IsByteArray() ? (Byte[])key.Value : enc.GetBytes(key.ToString());
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                hmacsha256.ComputeHash(data.IsByteArray() ? (Byte[])data.Value : enc.GetBytes(data.ToString()));
                //                return raw_output ? new XVar(System.Text.Encoding.UTF8.GetString( hmacsha256.Hash ) ) : bin2hex(new XVar(hmacsha256.Hash));
                return raw_output ? new XVar(hmacsha256.Hash) : bin2hex(new XVar(hmacsha256.Hash));
            }
            return false;
        }

        public static XVar hash_sha256(XVar data)
        {
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            using (SHA256 sha256 = SHA256.Create())
            {
                sha256.ComputeHash(data.IsByteArray() ? (Byte[])data.Value : enc.GetBytes(data.ToString()));
                return bin2hex(new XVar(sha256.Hash));
            }
            return false;
        }


        public static XVar hash_hmac_sha1(XVar data, XVar key, XVar raw_output = null)
        {
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            Byte[] keyBytes = key.IsByteArray() ? (Byte[])key.Value : enc.GetBytes(key.ToString());
            using (var hmacsha1 = new HMACSHA1(keyBytes))
            {
                hmacsha1.ComputeHash(data.IsByteArray() ? (Byte[])data.Value : enc.GetBytes(data.ToString()));
                return raw_output ? new XVar(System.Text.Encoding.UTF8.GetString(hmacsha1.Hash)) : bin2hex(new XVar(hmacsha1.Hash));
            }
            return false;
        }

        public static XVar fbGetAccessToken(dynamic _param_fbObj)
        {
            throw new NotImplementedException();
        }

        public static XVar fbCreateObject(dynamic _param_appId, dynamic _param_appSecret)
        {
            throw new NotImplementedException();
        }

        public static XVar fbGetUserInfo(dynamic _param_fbObj, string srToken = "")
        {
            throw new NotImplementedException();
        }

        public static XVar fbGetSignedRequest(dynamic _param_fbObj)
        {
            throw new NotImplementedException();
        }

        public static XVar fbDestroySession(dynamic _param_fbObj)
        {
            throw new NotImplementedException();
        }

        public static XVar getImageDimensions(dynamic _param_image)
        {
            dynamic res = XVar.Array();
            try
            {
                using (Image sourceImage = MVCFunctions.ImageFromBytes(_param_image.Value))
                {
                    res.InitAndSetArrayItem(sourceImage.Width, "width");
                    res.InitAndSetArrayItem(sourceImage.Height, "height");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return res;
        }

        public static XVar getHttpHeader(dynamic _param_name)
        {
            XVar envName = MVCFunctions.Concat("HTTP_", MVCFunctions.strtoupper(_param_name));
            if (MVCFunctions.SERVERKeyExists(envName))
            {
                return MVCFunctions.GetServerVariable(envName);
            }

            return XVar.Pack(HttpContext.Current.Request.Headers.Get(_param_name.ToString()));
        }

        public static XVar weeknumber(dynamic _param_time)
        {
            throw new NotImplementedException();
        }

        public static XVar debugVar(dynamic _param_v, dynamic _param_text = null)
        {
            throw new NotImplementedException();
        }

        public static XVar showError(dynamic _param_message)
        {
            throw new Exception(_param_message);
        }

        public static XVar parseQueryString(dynamic _param_str)
        {
            if (_param_str as Object == null)
            {
                _param_str = new XVar("");
            }
            NameValueCollection qscoll = HttpUtility.ParseQueryString(_param_str);
            XVar ret = XVar.Array();
            foreach (String s in qscoll.AllKeys)
            {
                ret[s] = qscoll[s];
            }
            return ret;
        }

        public static XVar convertToPem(dynamic _param_jwk)
        {
            throw new NotImplementedException();
        }

        public static byte[] asn1ShrinkModulus(byte[] modulus)
        {
            //nearest left power of 2
            int x = modulus.Length;
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            x = x - (x >> 1);

            var len = x;
            var result = new byte[len];
            var offset = modulus.Length - len;
            for (int i = 0; i < len; i++)
            {
                result[i] = modulus[i + offset];
            }
            return result;
        }

        public static XVar verifyOpenIdToken(dynamic _param_jwt, dynamic _param_jwk)
        {
            string[] parts = ((string)_param_jwt).Split('.');

            var rsa = RSA.Create();
            var modulusBytes = base64UrlDecodeToBytes(_param_jwk["n"]);
            rsa.ImportParameters(new RSAParameters
            {
                Exponent = base64UrlDecodeToBytes(_param_jwk["e"]),
                //#16561
                Modulus = asn1ShrinkModulus(modulusBytes),
            });

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");


            if (!rsaDeformatter.VerifySignature(hash, base64UrlDecodeToBytes(parts[2])))
            {
                return false;
            }

            XVar jwtParsed = XVar.Clone(Security.parseJWT((XVar)(_param_jwt)));
            return jwtParsed;
        }

        public static byte[] base64UrlDecodeToBytes(string str)
        {
            var _str = str;

            _str = _str.Replace('-', '+');
            _str = _str.Replace('_', '/');

            while (_str.Length % 4 != 0)
            {
                _str += "=";
            }

            return Convert.FromBase64String(_str); ;
        }

        public static XVar runner_base32_encode(XVar plainText)
        {
            StringBuilder encodedData = new StringBuilder();
            char[] charTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=".ToCharArray();
            byte[] bytes = plainText.IsByteArray() ? (byte[])plainText.Value : System.Text.Encoding.UTF8.GetBytes(plainText);
            int bitsLen = bytes.Length * 8;
            for (int bitIndex = 0; bitIndex < bitsLen; bitIndex += 5)
            {
                int byteIndex = bitIndex / 8;
                int bitOffset = bitIndex % 8;

                int fiveBits = 0;
                if (bitOffset <= 3)
                {
                    fiveBits = (bytes[byteIndex] >> (8 - 5 - bitOffset)) & 0x1f;
                }
                else
                {
                    int mask = (1 << (8 - bitOffset)) - 1;
                    fiveBits = (bytes[byteIndex] & mask) << (bitOffset - 3);
                    if (byteIndex < bytes.Length - 1)
                    {
                        fiveBits += bytes[byteIndex + 1] >> (8 - bitOffset + 3);
                    }
                }
                encodedData.Append(charTable[fiveBits]);
            }
            encodedData.Append(charTable[32], 8 - encodedData.Length % 8);
            return encodedData.ToString();
        }

        public static XVar runner_base32_decode(XVar encodedData)
        {
            char[] charTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567=".ToCharArray();
            string encoded = encodedData.ToString().TrimEnd(charTable[32]);
            byte[] decodedBytes = new byte[encoded.Length * 5 / 8];

            for (int i = 0; i < encoded.Length; ++i)
            {
                int fiveBits = Array.IndexOf(charTable, encoded[i]);
                if (fiveBits < 0)
                    continue;

                int byteIndex = i * 5 / 8;
                int bitOffset = i * 5 % 8;

                if (bitOffset <= 3)
                {
                    decodedBytes[byteIndex] += (byte)(fiveBits << (3 - bitOffset));
                }
                else
                {
                    decodedBytes[byteIndex] += (byte)(fiveBits >> (bitOffset - 3));
                    if (byteIndex < decodedBytes.Length - 1)
                    {
                        int mask = (1 << (bitOffset - 3)) - 1;
                        decodedBytes[byteIndex + 1] = (byte)((fiveBits & mask) << (8 - bitOffset + 3));
                    }
                }
            }

            return new XVar(decodedBytes);
            // return System.Text.Encoding.UTF8.GetString(decodedBytes);
        }

        public static XVar calculateTotpCode(XVar secret)
        {
            int codeLength = 6;
            int timeSlice = (int)(MVCFunctions.time() / 30);
            XVar secretkey = runner_base32_decode(secret);

            // Pack time into binary string(8-byte)
            byte[] data = new byte[8];
            data[0] = data[1] = data[2] = data[3] = 0;
            data[4] = (byte)(timeSlice >> 0x18);
            data[5] = (byte)(timeSlice >> 0x10);
            data[6] = (byte)(timeSlice >> 8);
            data[7] = (byte)timeSlice;

            // Hash it with users secret key(get hex output)
            XVar hm = hash_hmac_sha1(XVar.Pack(data), secretkey, XVar.Pack(false));

            // Use last nipple(half byte) of result as index/offset
            int offset = Convert.ToInt32(hm.Substring(hm.Length() - 1), 16);

            // grab 4 bytes of the result
            int value = 0;
            value |= (Convert.ToInt32(hm.Substring(offset * 2 + 6, 2), 16) << 0x0);
            value |= (Convert.ToInt32(hm.Substring(offset * 2 + 4, 2), 16) << 0x8);
            value |= (Convert.ToInt32(hm.Substring(offset * 2 + 2, 2), 16) << 0x10);
            value |= (Convert.ToInt32(hm.Substring(offset * 2 + 0, 2), 16) << 0x18);

            // Only 31 bits
            value = value & 0x7FFFFFFF;

            int modulo = (int)Math.Pow((double)10, codeLength);
            return MVCFunctions.str_pad(value % modulo, codeLength, "0", Constants.STR_PAD_LEFT);
        }

        public static XVar empty_error_handler()
        {
            throw new NotImplementedException();
        }

        public static XVar cutBOM(dynamic _param_line, dynamic _param_bom = null)
        {
            throw new NotImplementedException();
        }

        public static XVar parseCSVLineNew(XVar line, XVar delimiter)
        {
            var linestr = line.ToString();

            CsvReader csvRdr = new CsvReader(new StringReader(linestr), false, delimiter.ToString()[0]);
            try
            {
                if (csvRdr.ReadNextRecord())
                {
                    XVar row = XVar.Array();
                    for (int i = 0; i < csvRdr.FieldCount; i++)
                    {
                        row.Add(csvRdr[i]);
                    }
                    return row;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public static XVar getImportFileData(XVar fileName)
        {
            var uploadedFile = HttpContext.Current.Request.Files[fileName.ToString()];

            XVar uploadedFileData = XVar.Array();
            uploadedFileData["name"] = uploadedFile.FileName;
            uploadedFileData["tmp_name"] = uploadedFile.FileName;
            uploadedFileData["size"] = uploadedFile.ContentLength;
            uploadedFileData["type"] = uploadedFile.ContentType;
            uploadedFileData["error"] = "";
            uploadedFileData["file"] = uploadedFile;
            return uploadedFileData;
        }


        public static XVar getImportFileExtension(XVar fname)
        {
            return System.IO.Path.GetExtension(getTempImportFileName(fname)).TrimStart('.');
        }

        public static XVar getTempImportFileName(XVar fname)
        {
            return HttpContext.Current.Request.Files[fname.ToString()].FileName;
        }

        public static void deleteImportTempFile(XVar filePath)
        {
            var filePathStr = filePath.ToString().Replace('/', '\\');
            if (System.IO.File.Exists(filePathStr))
            {
                System.IO.File.Delete(filePathStr);
            }
        }

        public static XVar getFileNamesFromDir(XVar dirPath)
        {
            XVar names = XVar.Array();

            var dirPathStr = dirPath.ToString().Replace('/', '\\');

            if (!System.IO.Directory.Exists(dirPathStr))
                return names;

            foreach (var file in System.IO.Directory.EnumerateFiles(dirPathStr))
                names.Add(file);

            return names;
        }



        public static XVar CSVFileToText(dynamic _param_fileName, dynamic _param_preview = null, dynamic _param_fileEncoding = null)
        {
            Encoding enc = Encoding.GetEncoding((int)GlobalVars.cCodePage);
            Encoding fileEncoding;
            string text;

            if (_param_fileEncoding as Object != null)
            {
                fileEncoding = Encoding.GetEncoding((int)_param_fileEncoding);
            }
            else
            {
                fileEncoding = enc;
            }

            StreamReader streamReader = new StreamReader(_param_fileName, fileEncoding);

            if (XVar.Pack(_param_preview))
            {
                int fileLength = (int)(new FileInfo(_param_fileName).Length);
                char[] charBuffer = new char[6400];

                if ((int)(enc.GetBytes(new string(charBuffer))).Length < fileLength)
                {
                    streamReader.Read(charBuffer, 0, charBuffer.Length);
                    text = new string(charBuffer);
                }
                else
                {
                    text = streamReader.ReadToEnd();
                }
            }
            else
            {
                text = streamReader.ReadToEnd();
            }

            byte[] textBuffer = Encoding.Convert(streamReader.CurrentEncoding, enc, streamReader.CurrentEncoding.GetBytes(text));
            return XVar.Clone(enc.GetString(textBuffer));
        }

        public static XVar exifRotateImage(dynamic _param_image, dynamic _param_file_path)
        {
            throw new NotImplementedException();
        }

        public static XVar extract_error_info(dynamic _param_errno, dynamic _param_errstr, dynamic _param_errfile, dynamic _param_errline)
        {
            throw new NotImplementedException();
        }

        public static XVar runner_print_r_plain(dynamic _param_value, dynamic _param_return = null)
        {
            throw new NotImplementedException();
        }

        public static XVar regenerateSessionId()
        {
            return null;
        }


        public static XVar curl_headers_to_array(dynamic _param_headersData)
        {
            throw new NotImplementedException();
        }

        public static XVar try_decode_content(dynamic _param_data, dynamic _param_contentEncodingValue)
        {
            throw new NotImplementedException();
        }

/*
        public static XVar importSecuritySettings()
        {
            if (!GlobalVars.globalSettings["security"])
            {
                GlobalVars.globalSettings["security"] = SecuritySettings_class.settings();
            }
            return null;
        }
*/
        public static XVar verifySamlResponse(dynamic _param_rawResponse, dynamic _param_publicKey, dynamic _param_privateKey)
        {
            X509Certificate2 certReceived = new X509Certificate2(Encoding.ASCII.GetBytes(_param_publicKey));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(_param_rawResponse);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsManager.AddNamespace("saml2p", "urn:oasis:names:tc:SAML:2.0:protocol");
            nsManager.AddNamespace("saml2", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsManager.AddNamespace("xenc", "http://www.w3.org/2001/04/xmlenc#");

            XmlNode responseSignatureNode = xmlDoc.SelectSingleNode("//ds:Signature", nsManager);
            XmlNode certElement = xmlDoc.SelectSingleNode("//ds:X509Certificate", nsManager);

            if (!checkXmlSignature(xmlDoc, responseSignatureNode, certReceived, certElement))
            {
                return false;
            }

            XVar payload = XVar.Array();

            XmlNode encAssertion = xmlDoc.SelectSingleNode("/saml2p:Response/saml2:EncryptedAssertion", nsManager);
            if (encAssertion != null)
            {
                RSACryptoServiceProvider privateKey;
                try
                {
                    privateKey = getPrivateRSA((string)_param_privateKey);
                }
                catch (Exception ex)
                {
                    return false;
                }

                XmlNode encKeyNode = xmlDoc.GetElementsByTagName("xenc:EncryptedKey")[0];
                EncryptedKey encKey = new EncryptedKey();
                encKey.LoadXml((XmlElement)encKeyNode);

                XmlNode encDataNode = xmlDoc.GetElementsByTagName("xenc:EncryptedData")[0];
                EncryptedData encData = new EncryptedData();
                encData.LoadXml((XmlElement)encDataNode);

                // Create a new EncryptedXml object.
                EncryptedXml exml = new EncryptedXml();

                //EncryptedXml.XmlEncRSAOAEPUrl ?
                bool foaep = encKey.EncryptionMethod.KeyAlgorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";
                byte[] aesKey = privateKey.Decrypt(encKey.CipherData.CipherValue, foaep);

                byte[] aesIV = aesKey.Take(16).ToArray();
                AesManaged aes = new AesManaged();
                aes.Key = aesKey;
                aes.IV = aesIV;

                // Decrypt the element using the symmetric key.
                string rawAssertion = Encoding.UTF8.GetString(exml.DecryptData(encData, aes));

                XmlDocument tempDoc = new XmlDocument();
                tempDoc.LoadXml(rawAssertion);
                XmlNode tempNode = xmlDoc.ImportNode(tempDoc.DocumentElement, true);
                xmlDoc.DocumentElement.AppendChild(tempNode);
            }

            XmlNode assertion = xmlDoc.SelectSingleNode("/saml2p:Response/saml2:Assertion", nsManager);
            if (assertion != null)
            {
                XmlNode nameId = xmlDoc.SelectSingleNode("/saml2p:Response/saml2:Assertion/saml2:Subject/saml2:NameID", nsManager);
                if (nameId != null)
                {
                    payload["id"] = nameId.InnerText;
                }

                XmlNodeList attributes = xmlDoc.SelectNodes("/saml2p:Response/saml2:Assertion/saml2:AttributeStatement/saml2:Attribute", nsManager);
                foreach (XmlNode attribute in attributes)
                {
                    XVar values = XVar.Array();

                    foreach (XmlNode attrValue in attribute.ChildNodes)
                    {
                        values.Add(attrValue.InnerText);
                    }

                    if (values)
                    {
                        payload[attribute.Attributes["Name"].Value] = implode(", ", values);
                    }
                }

                XmlNode assertionSignature = xmlDoc.SelectSingleNode("/saml2p:Response/saml2:Assertion/ds:Signature", nsManager);
                if (assertionSignature == null)
                {
                    return payload;
                }

                XmlDocument doc = new XmlDocument();
                var parentNode = doc.ImportNode(assertion, true);
                doc.AppendChild(parentNode);
                XmlElement sigNode = (XmlElement)parentNode.SelectSingleNode("*[local-name()='Signature']");

                XmlNode certElement1 = xmlDoc.SelectSingleNode("/saml2p:Response/saml2:Assertion/ds:Signature//ds:X509Certificate", nsManager);


                if (!checkXmlSignature(doc, sigNode, certReceived, certElement1))
                {
                    return false;
                }

                return payload;

            }

            return false;
        }

        public static Boolean checkXmlSignature(XmlDocument xmlDoc, XmlNode signatureNode,
            X509Certificate2 certReceived, XmlNode certNode)
        {
            if (signatureNode == null)
            {
                return true;
            }
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.LoadXml((XmlElement)signatureNode);

            Boolean check = signedXml.CheckSignature(certReceived, true);

            X509Certificate2 certGiven = new X509Certificate2(Convert.FromBase64String(certNode.InnerText));
            Boolean check2 = signedXml.CheckSignature(certGiven, true);

            return check && check2 && certReceived.GetPublicKeyString() == certGiven.GetPublicKeyString();
        }

        public static RSACryptoServiceProvider getPrivateRSA(string private_key)
        {
            try
            {
                //  rsa xml string case
                RSACryptoServiceProvider privateKey = new RSACryptoServiceProvider();
                privateKey.FromXmlString(private_key);

                return privateKey;
            }
            catch (Exception ex)
            {

            }

            // hardcoded pfx certficate name and password
            string password = "";
            string certName = "samlcert.pfx";
            X509Certificate2 clientCertificate = new X509Certificate2(getabspath(certName), password);
            return (RSACryptoServiceProvider)clientCertificate.PrivateKey;
        }

       
        public static XVar createEventClass( dynamic _param_table )
        {
			if( GlobalVars._eventClasses == null ) {
                GlobalVars._eventClasses = XVar.Array();
            }
            string className = "eventclass_" + (string)CommonFunctions.GetTableURL(_param_table);
            if( GlobalVars._eventClasses.KeyExists( className ) ) {
                return GlobalVars._eventClasses[ className ];
            }
            Assembly assembly = typeof(XVar).Assembly;
            Type type = assembly.GetType("runnerDotNet." + className.ToString());
            //return (EventsAggregatorBase)Activator.CreateInstance(type, new Object[] { });
            GlobalVars._eventClasses[ className ] = (TableEventsBase)Activator.CreateInstance(type, new Object[] { });
            return GlobalVars._eventClasses[ className ];
			
        }
        public static Assembly loadVBEvents()
        {
            return null;
        }

        public static XVar addSlashes(dynamic _param_str)
        {
            #region pass-by-value parameters
            dynamic str = XVar.Clone(_param_str);
            #endregion

            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            try
            {
                str = (XVar)Regex.Replace(str, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
            }
            return (XVar)str;
        }

        public static XVar stripSlashes(dynamic _param_str)
        {
            #region pass-by-value parameters
            dynamic str = XVar.Clone(_param_str);
            #endregion

            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            try
            {
                str = (XVar)Regex.Replace(str, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
            }
            catch (Exception Ex)
            {
                // handle any exception here
            }
            return (XVar)str;
        }


        public static XVar runner_file_get_contents(dynamic _param_filename, dynamic _param_mode = null, dynamic _param_length = null)
        {
            throw new NotImplementedException();
        }

        public static XVar savePrivateData(dynamic id, dynamic data)
        {
            XVar file_name = MVCFunctions.strtolower(id);
            XVar file_path = MVCFunctions.getabspath("templates_c/" + file_name.ToString() + ".json");
            MVCFunctions.runner_save_file(file_path, MVCFunctions.runner_json_encode(data));
            return null;
        }

        public static XVar loadPrivateData(dynamic id)
        {
            XVar file_name = MVCFunctions.strtolower(id);
            XVar file_path = MVCFunctions.getabspath("templates_c/" + file_name.ToString() + ".json");
            XVar data = MVCFunctions.myfile_get_contents(file_path, "");
            return MVCFunctions.runner_json_decode(data);
        }

        public static XVar windowsOS()
        {
            return true;
        }

        public static XVar importFontSettings()
        {
/*
            if (!GlobalVars.globalSettings["fonts"])
            {
                GlobalVars.globalSettings["fonts"] = FontSettings_class.settings();
            }
*/			
            return null;
        }

        public static XVar setCookieDirectly(dynamic _param_name, dynamic _param_value)
        {
            MVCFunctions.SetCookie(_param_name, _param_value);
            return null;
        }

        public static XVar loadMenu( string menu )
        {
            if( !CommonFunctions.menuExists( menu ) ) {
				return null;
			}
			typeof(RunnerSettings).GetMethod( "menu_" + menu ).Invoke( null, null );
			return GlobalVars.runnerMenus[ menu ];
        }

        public static XVar storeJSONDataFromRequest()
        {
            XVar contentType = MVCFunctions.getHttpHeader("Content-Type");
            if (!contentType || MVCFunctions.strtolower(contentType) != "application/json")
                return false;

            try
            {
                using (StreamReader sr = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    string jsonString = sr.ReadToEnd();
                    GlobalVars.jsonDataFromRequest = MVCFunctions.runner_json_decode(jsonString);
                }
            }
            catch (Exception e) 
            {
                return false;
            }
            return true;
        }

		public static XVar dateParseFromDBFormat(dynamic _param_datetime, dynamic _param_format)
		{
			throw new NotImplementedException();
		}

		public static XVar timestampToDbDate(dynamic _param_unixTimeStamp)
		{
			throw new NotImplementedException();
		}

		public static XVar runner_handle_exception(dynamic _param_e)
		{
			throw new NotImplementedException();
		}

		public static XVar runner_handle_shutdown()
		{
			throw new NotImplementedException();
		}

		public static XVar extract_error_info(dynamic _param_errno, dynamic _param_errstr, dynamic _param_errfile, dynamic _param_errline, dynamic _param_backtrace)
		{
			throw new NotImplementedException();
		}

		public static XVar _loadTablePages()
		{
			RunnerSettings.pages();
			return null;
		}

		public static XVar loadLanguageMessages(XVar filename)
		{
			typeof( CommonFunctions ).GetMethod( "LoadLanguage_" + MVCFunctions.GoodFieldName( filename.ToString() ).ToString() ).Invoke( null, null );
			return null;
		}

		public static XVar startSession()
		{
			throw new NotImplementedException();
		}

		public static XVar init_json_library()
		{
			throw new NotImplementedException();
		}

		public static XVar printStack()
		{
			throw new NotImplementedException();
		}

		public static XVar importExcelLibrary()
		{
			// do nothing
			return null;
		}

		public static XVar runnerGetConnectionInfo(dynamic _param_id)
		{
			return GlobalVars.runnerDatabases[ _param_id ];
		}

		public static XVar getMapCustomIcon(dynamic _param_eventObj, dynamic _param_funcName, dynamic _param_data)
		{
			throw new NotImplementedException();
		}

		public static XVar db2time(XVar _param_str)
		{
			#region pass-by-value parameters
			XVar str = _param_str.Clone();
			#endregion

			int day, hour, minute, month, second, year;
            bool havedate = false, 
                havetime = false,
                isdst = DateTime.Now.IsDaylightSavingTime();

            if (str.Type == typeof(DateTime))
            {
                year = ((DateTime)str.Value).Year;
                month = ((DateTime)str.Value).Month;
                day = ((DateTime)str.Value).Day;
                hour = ((DateTime)str.Value).Hour;
                minute = ((DateTime)str.Value).Minute;
                second = ((DateTime)str.Value).Second;
                havedate = true;
                havetime = hour > 0 || minute > 0 || second > 0;
            }
            else
            {
                if (str.IsNumeric())
                {
                    XVar len;
                    string pattern;
                    havedate = true;
                    len = str.Length();
                    if (10 <= len)
                    {
                        havetime = true;
                    }
                    switch (len.ToInt())
                    {
                        case 14:
                            pattern = "(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})";
                            break;
                        case 12:
                            pattern = "(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})";
                            break;
                        case 10:
                            pattern = "(\\d{4})(\\d{2})(\\d{2})(\\d{2})";
                            break;
                        case 8:
                            pattern = "(\\d{4})(\\d{2})(\\d{2})";
                            break;
                        case 18:
                            pattern = "(\\d{4})(\\d{2})(\\d{2})";
                            break;
                        case 6:
                            pattern = "(\\d{2})(\\d{2})(\\d{2})";
                            break;
                        case 4:
                            pattern = "(\\d{2})(\\d{2})";
                            break;
                        case 2:
                            pattern = "(\\d{2})";
                            break;
                        default:
                            return new XVar();
                    }
                    Match parsed = Regex.Match(str, pattern);
                    if (parsed.Groups[1].Value != "")
                    {
                        year = int.Parse(parsed.Groups[1].Value);
                        month = parsed.Groups.Count > 2 ? int.Parse(parsed.Groups[2].Value) : 1;
                        day = parsed.Groups.Count > 3 ? int.Parse(parsed.Groups[3].Value) : 1;
                        hour = parsed.Groups.Count > 4 ? int.Parse(parsed.Groups[4].Value) : 0;
                        minute = parsed.Groups.Count > 5 ? int.Parse(parsed.Groups[5].Value) : 0;
                        second = parsed.Groups.Count > 6 ? int.Parse(parsed.Groups[6].Value) : 0;
                    }
                    else
                    {
                        return new XVar();
                    }
                }
                else
                {
                    if (str.IsString())
                    {
                        if (Regex.IsMatch(str, "(\\d{4})-(\\d{1,2})-(\\d{1,2}) (\\d{1,2}):(\\d{1,2}):(\\d{1,2})"))
                        {
                            Match parsed = Regex.Match(str, "(\\d{4})-(\\d{1,2})-(\\d{1,2}) (\\d{1,2}):(\\d{1,2}):(\\d{1,2})");
                            year = int.Parse(parsed.Groups[1].Value);
                            month = int.Parse(parsed.Groups[2].Value);
                            day = int.Parse(parsed.Groups[3].Value);
                            hour = int.Parse(parsed.Groups[4].Value);
                            minute = int.Parse(parsed.Groups[5].Value);
                            second = int.Parse(parsed.Groups[6].Value);
                            havedate = true;
                            havetime = true;
                        }
                        else
                        {
                            if (Regex.IsMatch(str, "(\\d{4})-(\\d{1,2})-(\\d{1,2})"))
                            {
                                Match parsed = Regex.Match(str, "(\\d{4})-(\\d{1,2})-(\\d{1,2})");
                                year = int.Parse(parsed.Groups[1].Value);
                                month = int.Parse(parsed.Groups[2].Value);
                                day = int.Parse(parsed.Groups[3].Value);
                                hour = 0;
                                minute = 0;
                                second = 0;
                                havedate = true;
                            }
                            else
                            {
                                if (Regex.IsMatch(str, "(\\d{2})-(\\d{1,2})-(\\d{1,2})"))
                                {
                                    Match parsed = Regex.Match(str, "(\\d{2})-(\\d{1,2})-(\\d{1,2})");
                                    year = DateTime.Now.Year;
                                    month = DateTime.Now.Month;
                                    day = DateTime.Now.Day;
                                    hour = int.Parse(parsed.Groups[1].Value);
                                    minute = int.Parse(parsed.Groups[2].Value);
                                    second = int.Parse(parsed.Groups[3].Value);
                                    havetime = true;
                                }
                                else
                                {
                                    return new XVar();
                                }
                            }
                        }
                    }
                    else
                    {
                        return new XVar();
                    }
                }
            }
			if(!havetime)
			{
				hour = 0;
				minute = 0;
				second = 0;
			}
			if(!havedate)
			{
				year = DateTime.Now.Year;
				month = DateTime.Now.Month;
				day = DateTime.Now.Day;
			}
			return new XVar(0, year, 1, month, 2, day, 3, hour, 4, minute, 5, second);
		}
		public static XVar callDashboardSnippet(string funcName, ref dynamic icon, ref dynamic header)
		{
			dynamic snippetData = XVar.Array();
			MVCFunctions.ob_start();
			object[] args = new object[] { header, icon };
			GlobalVars.globalEvents.Invoke(funcName, args );
			header = args[0];
			icon = args[1];
			XVar ret = MVCFunctions.ob_get_contents();
			MVCFunctions.ob_end_clean();
			return ret;
		}

    }

    public partial class CommonFunctions
    {
		
		public static XVar hexdec(XVar _param_str)
		{
			return new XVar( int.Parse(_param_str.ToString(), System.Globalization.NumberStyles.HexNumber) );
		}

		public static XVar db2time(XVar _param_str)
		{
			return MVCFunctions.db2time( _param_str );
		}
        
		public static XVar session_get_cookie_params()
        {
            return null;
        }
        public static XVar mysqli_real_escape_string(dynamic conn, XVar str)
        {
            return MVCFunctions.str_replace(new XVar("'"), new XVar("''"), str);
        }

        public static XVar func_get_args(params dynamic[] args)
        {
            dynamic res = XVar.Array();
            foreach (dynamic val in args)
            {
                if (val.GetType() == typeof(object[]))
                {
                    dynamic r1 = CommonFunctions.func_get_args(val);
                    foreach (dynamic val1 in r1.GetEnumerator())
                    {
                        res.Add(val1.Value);
                    }
                }
                else
                {
                    res.Add((XVar)val);
                }
            }
            return res;
        }
        public static void http_response_code(XVar code)
        {
            HttpContext.Current.Response.StatusCode = code;
        }
		public static XVar reset( XVar var ) {
			return var;
		}

		public static XVar date_default_timezone_get() {
			return null;
		}
		public static XVar date_default_timezone_set( XVar x ) {
			return null;
		}
		public static XVar pg_escape_string( XVar x ) {
			return null;
		}
		public static XVar pg_escape_bytea( XVar x ) {
			return null;
		}
		public static XVar pg_unescape_bytea( XVar x ) {
			return null;
		}
		/**
		 * Old event/BT code compatibility
		*/
		public static XVar runner_json_encode(XVar objectToEncode) {
			return MVCFunctions.runner_json_encode( objectToEncode );
		}
		/**
		 * Old event/BT code compatibility
		*/
        public static XVar runner_json_decode(XVar objectToDecode) {
			return MVCFunctions.runner_json_decode( objectToDecode );
		}

    }

    public static partial class tDAL 
    {
		public static XVar CustomQuery(dynamic _param_dalSQL) {
			return CommonFunctions.CustomQuery( _param_dalSQL );
		}	
		
		public static XVar DBLookup(dynamic _param_dalSQL) {
			return CommonFunctions.DBLookup( _param_dalSQL );
		}	
	}

}