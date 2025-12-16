using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace runnerDotNet
{
	public interface IXCloneable
    {
        dynamic Clone();
    }

    public class XVarConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value == null)
                return;

            var xv = value as XVar;
            serializer.Serialize(writer, xv.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType,object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    [JsonConverter(typeof(XVarConverter)), Serializable]
    public partial class XVar : IComparable<XVar>, IXCloneable
    {
        #region Properties

        private Type _type;

        object value;

		private enum BitwiseOperation {
			And,
			Or,
			Xor,
		};

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public object Value
        {
            get { return value; }
            set
            {
                if (value != null && value.GetType() == typeof(XVar))
                {
                    this.value = ((XVar)value).Value;
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public XVar this[string index, string fieldName]
        {
            get
            {
                return (XVar)this.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance).GetValue(this);
            }
            set
            {
                FieldInfo fieldInfo = this.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    if (fieldName == "xt" && fieldInfo.FieldType == typeof(XTempl))
                        fieldInfo.SetValue(this, value as XTempl);
                    else
                        fieldInfo.SetValue(this, value);
                }
            }
        }

		private IDictionary<XVar, XVar> InternalDictionary
        {
			get { return this.value as IDictionary<XVar, XVar>; }
        }


		static string _CorrectDecimalSeparator;
		public static string CorrectDecimalSeparator
		{
 			get
			{
				if(_CorrectDecimalSeparator == null)
					_CorrectDecimalSeparator = System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
				return _CorrectDecimalSeparator;
			}
		}

		static string _IncorrectDecimalSeparator;
		public static string IncorrectDecimalSeparator
		{
			get
			{
				if(_IncorrectDecimalSeparator == null)
					_IncorrectDecimalSeparator = System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator == "," ? "." : ",";
				return _IncorrectDecimalSeparator;
			}
		}

        #endregion

        #region Constructors

        public XVar()
        {
            _type = typeof(object);
        }

        public XVar(object val)
        {
            if (val == null)
            {
                return;
            }
            Type t = val.GetType();
			if (t == typeof(XVar))
			{
				this.value = (val as XVar).Value;
				_type = (val as XVar).Type;
			}
            else if (t == typeof(decimal))
            {
                this.value = decimal.ToDouble(((decimal)val));
                _type = typeof(double);
            }
            //if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            //{
            //    this.value = Convert.ChangeType(value, Nullable.GetUnderlyingType(t));
            //}
            else
            {
                this.value = val;
                _type = val.GetType();
            }
        }

        // number of values should be always even. using is something like:
        // var bar = new XVar("code", someobject, "name", someobject2, 3, someobject3);
        public XVar(params object[] values)
        {
            if (values == null)
                return;

            InitDictionary();

            for (int i = 0; i < values.Count(); i += 2)
            {
                ((Dictionary<XVar, XVar>)this.value)[XVar.Pack(values[i])] = (i + 1) >= values.Count() ? null : XVar.Pack(values[i + 1]);
            }
        }

        #endregion

        #region Operators

        public static bool operator ==(XVar x, XVar y)
        {
			bool xIsNull = x as Object == null || x.Value == DBNull.Value;
			bool yIsNull = y as Object == null || y.Value == DBNull.Value;

			if(xIsNull && yIsNull)
				return true;

			if (xIsNull || yIsNull)
			{
 				if((xIsNull && y.IsRunnerType()) || (yIsNull && x.IsRunnerType()))
					return false;
			}

            object p1, p2;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
            {
                return CSmartStr(p1) == CSmartStr(p2);
            }
            if (p1.GetType() == p2.GetType())
            {
                return p1.Equals(p2);
            }

			object outP1, outP2;
            TypeReduction(p1, p2, out outP1, out outP2);
			return outP1.Equals(outP2);
        }

        public static bool operator !=(XVar x, XVar y)
        {
			return !(x == y);
        }

        public static XVar operator +(XVar x, XVar y)
        {
            if (x == null) return y;
            if (y == null) return x;

            object p1, p2, result;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
            {
                result = CSmartStr(p1) + CSmartStr(p2);
            }
            else
            {
				object outP1, outP2;
				TypeReduction(p1, p2, out outP1, out outP2);

				Type p1Type = outP1.GetType();

                if (p1Type == typeof(string))
                {
					XVar v1 = null;
                    XVar v2 = null;
					if (ParseStringValues(outP1, outP2, ref v1, ref v2))
					{
						return v1 + v2;
					}
					else
					{
						result = 0;
						//result = (string)outP1 + (string)(outP2);
						//throw new ArgumentException("Do not use \"+\" to concatenate XVar strings, use MVCFunctions.Concat() instead.");
					}
                }
                else if (p1Type == typeof(char))
                {
					result = (char)outP1 + (char)outP2;
                }
                else if (p1Type == typeof(int))
                {
					result = (int)outP1 + (int)outP2;
                }
				else if (p1Type == typeof(Int64))
				{
					result = (Int64)outP1 + (Int64)outP2;
				}
                else if (p1Type == typeof(double))
                {
					result = (double)outP1 + (double)outP2;
                }
				else if (p1Type == typeof(bool))
				{
					result = (((bool)outP1) == true ? 1 : 0) + (((bool)outP2) == true ? 1 : 0);
				}
                else
                    throw new ArgumentException();
            }

            return new XVar(result);
        }

		public static XVar operator -(XVar x)
		{
			return XVar.Pack(0) - x;
		}

        public static XVar operator -(XVar x, XVar y)
        {
            if (x == null) return 0 - y;
            if (y == null) return x;

            object p1, p2, result;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
            {
                throw new ArgumentException();
            }
            else
            {
				object outP1, outP2;
				TypeReduction(p1, p2, out outP1, out outP2);

				if (outP1.GetType() == typeof(string))
                {
                    XVar v1 = null;
                    XVar v2 = null;
					if (ParseStringValues(outP1, outP2, ref v1, ref v2))
                    {
                        return v1 - v2;
                    }
                    else
                        throw new ArgumentException();
                }
				else if (outP1.GetType() == typeof(char))
                {
					result = (char)outP1 - (char)outP2;
                }
				else if (outP1.GetType() == typeof(int))
                {
					result = (int)outP1 - (int)outP2;
                }
				else if (outP1.GetType() == typeof(Int64))
				{
					result = (Int64)outP1 - (Int64)outP2;
				}
				else if (outP1.GetType() == typeof(double))
                {
					result = (double)outP1 - (double)outP2;
                }
				else if (outP1.GetType() == typeof(bool))
				{
					result = (((bool)outP1) == true ? 1 : 0) - (((bool)outP2) == true ? 1 : 0);
				}
                else
                    throw new ArgumentException();
            }

            return new XVar(result);
        }

        public static XVar operator ++(XVar x)
        {
            return x + 1;
        }

        public static XVar operator --(XVar x)
        {
            return x - 1;
        }

        public static XVar operator *(XVar x, XVar y)
        {
            if (x == null || y == null) return 0;

            object p1, p2, result;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
                throw new ArgumentException();
            else
            {
				object outP1, outP2;
				TypeReduction(p1, p2, out outP1, out outP2);

				Type p1Type = outP1.GetType();

                if (p1Type == typeof(string))
                {
                    XVar v1 = null;
                    XVar v2 = null;
					if (ParseStringValues(outP1, outP2, ref v1, ref v2))
                    {
                        return v1 * v2;
                    }
                    else
                        throw new ArgumentException();
                }
                else if (p1Type == typeof(int))
                {
					result = (int)outP1 * (int)outP2;
                }
				else if (p1Type == typeof(Int64))
				{
					result = (Int64)outP1 * (Int64)outP2;
				}
                else if (p1Type == typeof(double))
                {
					result = (double)outP1 * (double)outP2;
                }
                else
                    throw new ArgumentException();
            }

            return new XVar(result);
        }

        public static XVar operator /(XVar x, XVar y)
        {
            if (x == null || y == null) return 0;

            object p1, p2, result;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
            {
                throw new ArgumentException();
            }
            else
            {
				object outP1, outP2;
				TypeReduction(p1, p2, out outP1, out outP2);

                Type p1Type = p1.GetType();

                if (p1Type == typeof(string))
                {
                    XVar v1 = null;
                    XVar v2 = null;
					if (ParseStringValues(outP1, outP2, ref v1, ref v2))
                    {
                        return v1 / v2;
                    }
                    else
                        throw new ArgumentException();
                }
                else if (p1Type == typeof(int) || p1Type == typeof(Int64))
                {
					// parse int as double always. do not trust TypeReduction
					result = Convert.ToDouble(CSmartDbl(p1)) / Convert.ToDouble(CSmartDbl(p2));
                }
                else if (p1Type == typeof(double))
                {
					result = (double)outP1 / (double)outP2;
                }
                else
                    throw new ArgumentException();
            }

            return new XVar(result);
        }

        private static bool ParseStringValues(object p1, object p2, ref XVar v1, ref XVar v2)
        {
            bool firstCorrect = true;
			bool secondCorrect = true;

            int tIntVal;
            double tDoubleVal;

			var p1Str = new string(p1.ToString().TakeWhile(x => char.IsNumber(x) || x == '.').ToArray());
			if (int.TryParse(p1Str, out tIntVal))
            {
                v1 = tIntVal;
            }
			else if (double.TryParse(p1Str.Replace(IncorrectDecimalSeparator, CorrectDecimalSeparator), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out tDoubleVal))
			{
				v1 = tDoubleVal;
			}
			else
			{
				firstCorrect = false;
				v1 = XVar.Pack(0);
			}

			var p2Str = new string(p2.ToString().TakeWhile(x => char.IsNumber(x) || x == '.').ToArray());
			if (int.TryParse(p2Str, out tIntVal))
			{
				v2 = tIntVal;
			}
			else if (double.TryParse(p2Str.Replace(IncorrectDecimalSeparator, CorrectDecimalSeparator), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out tDoubleVal))
			{
				v2 = tDoubleVal;
			}
			else
			{
				secondCorrect = false;
				v2 = XVar.Pack(0);
			}

            return firstCorrect || secondCorrect;
        }

        public static XVar operator %(XVar x, XVar y)
        {
            if (x == null || y == null) return 0;

            object p1, p2, result;
            TryGetVals(x, y, out p1, out p2);
            if (p1 == null || p2 == null)
                throw new ArgumentException();
            else
            {
				object outP1, outP2;
				TypeReduction(p1, p2, out outP1, out outP2);

				Type p1Type = outP1.GetType();

                if (p1Type == typeof(string))
                {
                    XVar v1 = null;
                    XVar v2 = null;
					if (ParseStringValues(outP1, outP2, ref v1, ref v2))
                    {
                        return v1 % v2;
                    }
                    else
                        throw new ArgumentException();
                }
                else if (p1Type == typeof(int))
                {
					result = (int)outP1 % (int)outP2;
                }
				else if (p1Type == typeof(Int64))
				{
					result = (Int64)outP1 % (Int64)outP2;
				}
                else if (p1Type == typeof(double))
                {
					result = (double)outP1 % (double)outP2;
                }
                else
                    throw new ArgumentException();
            }

            return new XVar(result);
        }

        public static bool operator ==(XVar x, object y)
        {
            return x == new XVar(y);
        }

        public static bool operator !=(XVar x, object y)
        {
            return x != new XVar(y);
        }

        public static bool operator >=(XVar x, XVar y)
        {
            return x == y || x > y;
        }

        public static bool operator <=(XVar x, XVar y)
        {
            return x == y || x < y;
        }


        public static bool operator >(XVar x, XVar y)
        {
            if (x == null)
                return false;
            else if (y == null)
                return true;

            object p1, p2;
            TryGetVals(x, y, out p1, out p2);
			object outP1, outP2;
			TypeReduction(p1, p2, out outP1, out outP2);

			if (outP1 is int)
				return (int)outP1 > (int)outP2;

			if (outP1 is Int64)
				return (Int64)outP1 > (Int64)outP2;

			if (outP1 is double)
				return (double)outP1 > (double)outP2;

			if (outP1 is DateTime)
				return (DateTime)outP1 > (DateTime)outP2;

			return string.CompareOrdinal(outP1.ToString(), outP2.ToString()) > 0;
        }

        public static bool operator <(XVar x, XVar y)
        {
            if (x == null)
                return y != null;
            else if (y == null)
                return false;

            object p1, p2;
            TryGetVals(x, y, out p1, out p2);
			object outP1, outP2;
			TypeReduction(p1, p2, out outP1, out outP2);

			if (outP1 is int)
				return (int)outP1 < (int)outP2;

			if (outP1 is Int64)
				return (Int64)outP1 < (Int64)outP2;

			if (outP1 is double)
				return (double)outP1 < (double)outP2;

			if (outP1 is DateTime)
				return (DateTime)outP1 < (DateTime)outP2;

			return string.CompareOrdinal(outP1.ToString(), outP2.ToString()) < 0;
        }

		private static XVar PerformStringBitwiseOperation(XVar left, XVar right, BitwiseOperation op)
		{
			// simulate php behavior

			Encoding enc = System.Text.Encoding.UTF8;
	        Byte[] bytes1 = left.IsByteArray()? (Byte[])left.Value: enc.GetBytes(left.ToString());
			Byte[] bytes2 = right.IsByteArray()? (Byte[])right.Value: enc.GetBytes(right.ToString());
			Byte[] resBytes = new Byte[Math.Min(bytes1.Length, bytes2.Length)];

			switch(op) {
				case BitwiseOperation.And: {
					for(int i = 0; i < resBytes.Length; i++) resBytes[i] = (Byte)((int)bytes1[i] & (int)bytes2[i]);
				} break;
				case BitwiseOperation.Or: {
					for(int i = 0; i < resBytes.Length; i++) resBytes[i] = (Byte)((int)bytes1[i] | (int)bytes2[i]);
				} break;
				case BitwiseOperation.Xor: {
					for(int i = 0; i < resBytes.Length; i++) resBytes[i] = (Byte)((int)bytes1[i] ^ (int)bytes2[i]);
				} break;
			}
			return new XVar(enc.GetString(resBytes));
		}

		private static bool PrepareNumberOperands(XVar left, XVar right, out object outP1, out object outP2)
		{
			if(left == null)
				left = XVar.Pack(0);
			if(right == null)
				right = XVar.Pack(0);

			object p1, p2;
            TryGetVals(left, right, out p1, out p2);

			outP1 = null;
			outP2 = null;
			if (p1 == null || p2 == null)
                return false;

			TypeReduction(p1, p2, out outP1, out outP2);

			Type p1Type = outP1.GetType();
			Type p2Type = outP2.GetType();

			if (p1Type == typeof(string) && p2Type != typeof(string))
			{
				if (!CSmartLng(ref outP1, outP1.ToString().Length > 9))
					return false;
				p1Type = outP1.GetType();
			}

			if (p1Type != typeof(string) && p2Type == typeof(string))
			{
				if (!CSmartLng(ref outP2, outP2.ToString().Length > 9))
					return false;
				p2Type = outP2.GetType();
			}

			if(p1Type == typeof(double))
			{
				outP1 = (int)(double)outP1;
				p1Type = outP1.GetType();
			}

			if(p2Type == typeof(double))
			{
				outP2 = (int)(double)outP2;
				p2Type = outP2.GetType();
			}

			return true;
		}

		public static XVar BitwiseAnd(XVar left, XVar right)
		{
			object p1, p2;
			if(!PrepareNumberOperands(left, right, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();
			Type p2Type = p2.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(string) && p2Type == typeof(string))
				result = PerformStringBitwiseOperation(left, right, BitwiseOperation.And);
			else if (p1Type == typeof(Int64) || p2Type == typeof(Int64))
				result = ((Int64)p1 & (Int64)p2);
			else if (p1Type == typeof(int) || p2Type == typeof(int))
				result = ((int)p1 & (int)p2);
			else
				throw new ArgumentException();

            return result;
		}

		public static XVar BitwiseOr(XVar left, XVar right)
		{
			object p1, p2;
			if(!PrepareNumberOperands(left, right, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();
			Type p2Type = p2.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(string) && p2Type == typeof(string))
				result = PerformStringBitwiseOperation(left, right, BitwiseOperation.Or);
			else if (p1Type == typeof(Int64) || p2Type == typeof(Int64))
				result = ((Int64)p1 | (Int64)p2);
			else if (p1Type == typeof(int) || p2Type == typeof(int))
				result = ((int)p1 | (int)p2);
			else
				throw new ArgumentException();

            return result;
		}

		public static XVar BitwiseXor(XVar left, XVar right)
		{
			object p1, p2;
			if(!PrepareNumberOperands(left, right, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();
			Type p2Type = p2.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(string) && p2Type == typeof(string))
				result = PerformStringBitwiseOperation(left, right, BitwiseOperation.Xor);
			else if (p1Type == typeof(Int64) || p2Type == typeof(Int64))
				result = ((Int64)p1 ^ (Int64)p2);
			else if (p1Type == typeof(int) || p2Type == typeof(int))
				result = ((int)p1 ^ (int)p2);
			else
				throw new ArgumentException();

            return result;
		}
		public static XVar operator <<(XVar val, int count)
		{
			object p1, p2;
			if(!PrepareNumberOperands(val, null, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(Int64))
				result = ((Int64)p1 << count);
			else if (p1Type == typeof(int))
				result = ((int)p1 << count);
			else
				throw new ArgumentException();

            return result;
		}

		public static XVar operator >>(XVar val, int count)
		{
			object p1, p2;
			if(!PrepareNumberOperands(val, null, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(Int64))
				result = ((Int64)p1 >> count);
			else if (p1Type == typeof(int))
				result = ((int)p1 >> count);
			else
				throw new ArgumentException();

            return result;
		}

		public static XVar operator ~(XVar val)
		{
			object p1, p2;
			if(!PrepareNumberOperands(val, null, out p1, out p2))
				throw new ArgumentException();

			Type p1Type = p1.GetType();

			XVar result = new XVar(0);
			if (p1Type == typeof(Int64))
				result = ~((Int64)p1);
			else if (p1Type == typeof(int))
				result = ~((int)p1);
			else
				throw new ArgumentException();

            return result;
		}

		public static bool operator true(XVar val)
		{
			return ((bool)val == true);
		}

    	public static bool operator false(XVar val)
		{
			return ((bool)val == false);
		}

        #endregion

        #region Impicit casts

        public static implicit operator XVar(int val)
        {
            return new XVar(val);
        }

        public static implicit operator int(XVar val)
        {
			if (val.Type == typeof(int))
			{
				return (int)val.Value;
			}
			else
			{
				var res = val.Value;
				if (CSmartLng(ref res))
					return (int)res;
				else return (int)(double)res;
			}
        }

        public static implicit operator XVar(long val)
        {
            return new XVar(val);
        }

        public static implicit operator long(XVar val)
        {
            return Convert.ToInt64(val.Value);
        }

        public static implicit operator double(XVar val)
        {
			if (val.Type == typeof(double))
			{
				return (double)val.Value;
			}
            else return CSmartDbl(val.Value);
        }

        public static implicit operator XVar(double val)
        {
            return new XVar(val);
        }

        public static implicit operator XVar(string val)
        {
            return new XVar(val);
        }

        public static implicit operator string(XVar val)
        {
            if(val as object != null)
                return val.ToString();

            return "";
        }

        public static implicit operator XVar(bool val)
        {
            return new XVar(val);
        }

        public static implicit operator bool(XVar val)
        {
            if (val as object == null || val.value is DBNull)
            {
                return false;
            }

            if (val.IsRunnerType())
            {
                return true;
            }
            if (val.IsArray())
            {
                return val.Count() > 0;
            }

			if( val.IsByteArray() ) {
				return (val.Value as byte[]).Length > 0;
			} 
			
            if (val.Value == null)
            {
                return false;
            }

            if (val.Type == typeof(bool))
            {
                return (bool)val.Value;
            }
            else
            {
                if (val.IsNumeric())
                {
                    if (val.Type == typeof(int))
                    {
                        return (int)val.value != 0;
                    }
					else if (val.Type == typeof(Int64))
					{
						return (Int64)val.value != 0;
					}
                    else if (val.Type == typeof(double))
                    {
                        return (double)val.value != 0;
                    }
                    else
                    {
                        string stringVal = val.value.ToString();
                        if (val.Type == typeof(string) && stringVal != "")
                        {
                            int tempInt = 0;
                            double tempDouble = 0;
                            if (int.TryParse(stringVal, out tempInt) || double.TryParse(stringVal,  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out tempDouble))
                            {
                                return tempInt != 0 || tempDouble != 0;
                            }
                        }
                    }
                }

                if (val.IsString())
                {
                    return val.ToString() != "";
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion

        #region IEqualityComparer

		public static bool Equals(XVar x, XVar y)
		{
			bool xIsNull = x as Object == null || x.Value == DBNull.Value || (x.Value == null && !x.IsRunnerType());
			bool yIsNull = y as Object == null || y.Value == DBNull.Value || (y.Value == null && !y.IsRunnerType());

			if(xIsNull && yIsNull)
				return true;

			if(xIsNull || yIsNull)
				return false;

			// Int32 equals Int64
			if (x.Type != y.Type && !((x.Type == typeof(Int32) || x.Type == typeof(Int64)) && (y.Type == typeof(Int32) || y.Type == typeof(Int64))))
			return false;

			return x.Equals(y);
		}

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return !this.IsRunnerType() && this.value == null;
            }

            if (this.IsRunnerType())
            {
                if (o.GetType() != this.GetType())
                    return false;

                return ReferenceEquals(this, o);
            }
            else
            {
                if (o.GetType() != typeof(XVar))
                {
                    if (this.value != null)
                    {
                        return this.value.Equals(o);
                    }
                    else
                    {
                        return false;
                    }
                }

                XVar ox = o as XVar;

                if (this.value == null)
					return ox.value == null;

                // for arrays
				if (ox.Type == typeof(string) && this.Type == typeof(string))
					return (string)this.Value == ox.ToString();

                if (ox.Type == typeof(string) || this.Type == typeof(string))
                    return this.ToString() == ox.ToString();

				// for comparison on different types. should be (decimal)1.0m == (int)1
				var target = ox.Value;
				object outValue, outTarget;
				TypeReduction(value, target, out outValue, out outTarget);
				return outValue.Equals(outTarget);

                //return this == (XVar)o; // do not use, cuz of typeReduction
            }
        }

        /*public override bool Equals(object o)
        {
            if (o == null)
                return this.value == null;

            if (this.IsRunnerType())
            {
                if (o.GetType() != this.GetType())
                    return false;

                return ReferenceEquals(this, o);
            }
            else
            {
                if (o.GetType() != typeof(XVar))
                {
                    if (this.value != null)
                    {
                        return this.value.Equals(o);
                    }
                    else
                    {
                        return false;
                    }
                }

                return this.value.Equals(((XVar)o).value);
            }
        }*/

        public override int GetHashCode()
        {
			if (value == null)
				return 0;

			return CSmartStr(value).GetHashCode();
        }

        #endregion

        #region IEnumerable<KeyValuePair<XVar, XVar>>

        public IEnumerable<KeyValuePair<XVar, dynamic>> GetEnumerator()
        {

            if (this.value != null && this._type != typeof(XVar)) {
				if ((this.Type != typeof(Dictionary<XVar, XVar>)) && (this.Type != typeof(XSettingsMap)))
                {
                    yield break;
                }

			}

			InitDictionary();
            foreach (var keyValuePair in this.InternalDictionary)
                yield return new KeyValuePair<XVar, dynamic>(keyValuePair.Key, keyValuePair.Value);
        }

        #endregion

        #region IComparable

        public int CompareTo(XVar other)
        {
            return this < other ? -1 : this > other ? 1 : 0;
        }

        #endregion

        #region Dictionary composition

        public void InitDictionary()
        {
            if (this.value == null || this._type == typeof(XVar))
            {
                this.value = new Dictionary<XVar, XVar>();
                _type = typeof(Dictionary<XVar, XVar>);
            }
            else
            {
				if ((this.Type != typeof(Dictionary<XVar, XVar>)) && (this.Type != typeof(XSettingsMap)))
                {
                    throw new ArrayTypeMismatchException("Variable already has non array value");
                }
            }
        }

        public XVar this[object key]
        {
            set { this.SetArrayItem(key, value); }
            get { return this.GetArrayItem(key); }
        }

        public XVar GetArrayItem<T>(T key)
        {
            XVar xKey = XVar.Pack(key);
            if (this.value != null && !this.IsArray())
            {
                if (xKey.IsNumeric() && xKey > -1 && xKey < this.value.ToString().Length)
                {
                    return this.value.ToString()[xKey].ToString();
                }
                else
                {
                    return "";
                }
            }

            if (this.value as Object == null)
				InitDictionary();

            XVar val;
            if (InternalDictionary.TryGetValue(xKey, out val))
                return val;

            return new XVar(null);
        }

        public void SetArrayItem<K, V>(K key, V value)
        {
            InitDictionary();
            InternalDictionary[XVar.Pack(key)] = XVar.Pack(value);
        }

        public void SetArrayItemInternal<K, V>(V value, K[] keys)
        {

			// lots of code. but its faster than recursion

			switch(keys.Count<K>())
			{
				case 1:
					{
						if ((object)keys[0] == null)
							this.Add(value);
						else this[keys[0]] = XVar.Pack(value);
						break;
					}
				case 2:
					{
						if (this[keys[0]] == null)
							this[keys[0]] = XVar.Array();
						if ((object)keys[1] == null)
							this[keys[0]].Add(value);
						else this[keys[0]][keys[1]] = XVar.Pack(value);
						break;
					}
				case 3:
					{
						if (this[keys[0]] == null)
							this[keys[0]] = XVar.Array();
						if (this[keys[0]][keys[1]] == null)
							this[keys[0]][keys[1]] = XVar.Array();
						if ((object)keys[2] == null)
							this[keys[0]][keys[1]].Add(value);
						else this[keys[0]][keys[1]][keys[2]] = XVar.Pack(value);
						break;
					}
				case 4:
					{
						if (this[keys[0]] == null)
							this[keys[0]] = XVar.Array();
						if (this[keys[0]][keys[1]] == null)
							this[keys[0]][keys[1]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]] == null)
							this[keys[0]][keys[1]][keys[2]] = XVar.Array();
						if ((object)keys[3] == null)
							this[keys[0]][keys[1]][keys[2]].Add(value);
						else this[keys[0]][keys[1]][keys[2]][keys[3]] = XVar.Pack(value);
						break;
					}
				case 5:
					{
						if (this[keys[0]] == null)
							this[keys[0]] = XVar.Array();
						if (this[keys[0]][keys[1]] == null)
							this[keys[0]][keys[1]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]] == null)
							this[keys[0]][keys[1]][keys[2]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]][keys[3]] == null)
							this[keys[0]][keys[1]][keys[2]][keys[3]] = XVar.Array();
						if ((object)keys[4] == null)
							this[keys[0]][keys[1]][keys[2]][keys[3]].Add(value);
						else this[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]] = XVar.Pack(value);
						break;
					}
				case 6:
					{
						if (this[keys[0]] == null)
							this[keys[0]] = XVar.Array();
						if (this[keys[0]][keys[1]] == null)
							this[keys[0]][keys[1]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]] == null)
							this[keys[0]][keys[1]][keys[2]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]][keys[3]] == null)
							this[keys[0]][keys[1]][keys[2]][keys[3]] = XVar.Array();
						if (this[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]] == null)
							this[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]] = XVar.Array();
						if ((object)keys[5] == null)
							this[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]].Add(value);
						else this[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]][keys[5]] = XVar.Pack(value);
						break;
					}
				default:
					throw new ArgumentOutOfRangeException("Large initialization found " + keys.Count().ToString());
			}
        }

		#region generics and overrides (for better performance of InitAndSetArrayItem)

		public XVar InitAndSetArrayItem<K1, K2, K3, K4>(object value, K1 k1, K2 k2, K3 k3, K4 k4, object k5)
		{
			return InitAndSetArrayItem<K1, K2, K3, K4, object, object>(value, k1, k2, k3, k4, k5);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3>(object value, K1 k1, K2 k2, K3 k3, object k4)
		{
			return InitAndSetArrayItem<K1, K2, K3, object, object>(value, k1, k2, k3, k4);
		}

		public XVar InitAndSetArrayItem<K1, K2>(object value, K1 k1, K2 k2, object k3)
		{
			return InitAndSetArrayItem<K1, K2, object, object>(value, k1, k2, k3);
		}

		public XVar InitAndSetArrayItem<K1>(object value, K1 k1, object k2)
		{
			return InitAndSetArrayItem<K1, object, object>(value, k1, k2);
		}

		public XVar InitAndSetArrayItem(object value, object k1)
		{
			return InitAndSetArrayItem<object, object>(value, k1);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, K4, K5>(object value, K1 k1, K2 k2, K3 k3, K4 k4, K5 k5)
		{
			return InitAndSetArrayItem<K1, K2, K3, K4, K5, object>(value, k1, k2, k3, k4, k5);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, K4>(object value, K1 k1, K2 k2, K3 k3, K4 k4)
		{
			return InitAndSetArrayItem<K1, K2, K3, K4, object>(value, k1, k2, k3, k4);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3>(object value, K1 k1, K2 k2, K3 k3)
		{
			return InitAndSetArrayItem<K1, K2, K3, object>(value, k1, k2, k3);
		}

		public XVar InitAndSetArrayItem<K1, K2>(object value, K1 k1, K2 k2)
		{
			return InitAndSetArrayItem<K1, K2, object>(value, k1, k2);
		}

		public XVar InitAndSetArrayItem<K1>(object value, K1 k1)
		{
			return InitAndSetArrayItem<K1, object>(value, k1);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, K4, K5, K6, V>(V value, K1 k1, K2 k2, K3 k3, K4 k4, K5 k5, K6 k6)
		{
			InitDictionary();
			this.SetArrayItemInternal(value, new object[] { k1, k2, k3, k4, k5, k6 });
			return XVar.Pack(value);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, K4, K5, V>(V value, K1 k1, K2 k2, K3 k3, K4 k4, K5 k5)
		{
			InitDictionary();
			this.SetArrayItemInternal(value, new object[] { k1, k2, k3, k4, k5 });
			return XVar.Pack(value);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, K4, V>(V value, K1 k1, K2 k2, K3 k3, K4 k4)
		{
			InitDictionary();
			this.SetArrayItemInternal(value, new object[] { k1, k2, k3, k4 });
			return XVar.Pack(value);
		}

		public XVar InitAndSetArrayItem<K1, K2, K3, V>(V value, K1 k1, K2 k2, K3 k3)
		{
			InitDictionary();
			this.SetArrayItemInternal(value, new object[] { k1, k2, k3});
			return XVar.Pack(value);
		}

		public XVar InitAndSetArrayItem<K1, K2, V>(V value, K1 k1, K2 k2)
		{
			InitDictionary();
			this.SetArrayItemInternal(value, new object[]{k1, k2});
			return XVar.Pack(value);
		}

        public XVar InitAndSetArrayItem<K1, V>(V value, K1 k1)
        {
            InitDictionary();
            this.SetArrayItemInternal(value, new object[]{ k1 });
            return XVar.Pack(value);
        }

		#endregion

		public void Remove(XVar val)
        {
            InitDictionary();
            if (InternalDictionary.ContainsKey(val))
                InternalDictionary.Remove(val);
        }

		public XVar Pop()
        {
            InitDictionary();
			int count = InternalDictionary.Count();
			if( count == 0 ) {
				return null;
			}
			int key  = count - 1;
			if( InternalDictionary.ContainsKey( key ) ) {
				XVar lastVal = InternalDictionary[ key ] ;
				InternalDictionary.Remove( key );
				return lastVal;
			}
			return null;
        }


        public bool KeyExists(XVar key)
        {
            if( key as Object == null || key.Value == null)
				return false;
			InitDictionary();
            return InternalDictionary.ContainsKey(key);
        }

        public bool ValueExists(XVar val)
        {
            InitDictionary();
			if (InternalDictionary is Dictionary<XVar, XVar>)
				return ((Dictionary<XVar, XVar>)InternalDictionary).ContainsValue(val);
			else
			{
				foreach(KeyValuePair<XVar, XVar> pair in InternalDictionary)
				{
					if (pair.Value == val)
						return true;
				}
			}
			return false;
        }

        public XVar ArrayKeys(XVar searchKey = null)
        {
            InitDictionary();
            XVar result = XVar.Array();
            foreach (var pair in InternalDictionary)
			{
				if(searchKey as object == null || searchKey == pair.Value)
					result.Add(pair.Key);
			}
            return result;
        }

        public XVar Count()
        {
            InitDictionary();
            return InternalDictionary.Count();
        }

        public List<KeyValuePair<XVar, XVar>> ToList()
        {
            InitDictionary();
            return InternalDictionary.ToList();
        }

        public void RemoveAll()
        {
            InitDictionary();
            InternalDictionary.Clear();
        }

        public void Add<V>(V value)
        {
            InitDictionary();

			int count = InternalDictionary.Count;
			while (InternalDictionary.ContainsKey(count))
				count++;

			InternalDictionary.Add(count, XVar.Pack(value));
        }

        public XVar Distinct()
        {
            InitDictionary();
            XVar result = XVar.Array();
            foreach (var item in InternalDictionary)
            {
				if(!result.ValueExists(item.Value))
					result.SetArrayItem(item.Key, item.Value);
            }
            return result;
        }

        public XVar ArraySlice(XVar firstIndex, XVar length)
        {
			// read the comment for ArraySplice

            InitDictionary();
            XVar result = XVar.Array();

			int count = Count();

			if (firstIndex >= 0)
				foreach (var item in InternalDictionary.OrderBy(x => x.Key).Skip((int)firstIndex).Take((int)(length >= 0 ? length : count - firstIndex + length)))
					result.Add(item.Value);
			else
			{
				foreach (var item in InternalDictionary.OrderBy(x => x.Key).Skip((int)(count + firstIndex)).Take((int)(length >= 0 ? length : - firstIndex + length)))
					result.Add(item.Value);
			}

            return result;
        }

        public XVar ArraySplice(XVar offset, XVar length)
        {
            // from man:

            //If offset is positive then the start of removed portion is at that offset from the beginning of the input array.
            //If offset is negative then it starts that far from the end of the input array.

            //If length is specified and is positive, then that many elements will be removed.
            //If length is specified and is negative then the end of the removed portion will be that many elements from the end of the array.

            InitDictionary();

            XVar extracteds = XVar.Array();
            XVar remaideds = XVar.Array();

            int index = 0;
            int len = length.ToInt();

            foreach (var pair in InternalDictionary.OrderBy(x => x.Key))
            {
                if (offset >= 0 && index >= offset)
                {
                    if (len > 0 && index < offset + len)
                        extracteds[pair.Key] = pair.Value;
                    else if (len < 0 && index >= InternalDictionary.Count + len)
                        extracteds[pair.Key] = pair.Value;
                    else
                        remaideds[pair.Key] = pair.Value;
                }
                else if (offset < 0 && index >= InternalDictionary.Count + offset)
                {
                    if (len > 0 && index < InternalDictionary.Count + offset + len)
                        extracteds[pair.Key] = pair.Value;
                    else if (len < 0 && index < InternalDictionary.Count + len)
                        extracteds[pair.Key] = pair.Value;
                    else
                        remaideds[pair.Key] = pair.Value;
                }
                else remaideds[pair.Key] = pair.Value;

                index++;
            }

            this.Value = remaideds.Value;

			RebuildKeys(0);

            return extracteds;
        }

        public XVar Intersect(XVar intersection)
        {
			if (intersection == null)
				return this;

            InitDictionary();
            XVar result = XVar.Array();
			foreach (KeyValuePair<XVar, dynamic> pair in GetEnumerator())
				if (intersection.ValueExists(pair.Value))
					result.SetArrayItem<XVar, XVar>(pair.Key, pair.Value);

            return result;
        }

        public XVar ArraySearch(XVar needle)
        {
            InitDictionary();
            var findedItem = InternalDictionary.FirstOrDefault(item => item.Value == needle.Value);
            if (findedItem as object != null && findedItem.Key != null)
                return findedItem.Key;

            return false;
        }

        public XVar ArrayClone()
        {
            InitDictionary();
			try
			{
				return new XVar(new Dictionary<XVar, XVar>(InternalDictionary));
			}
			catch (System.Exception ex)
			{
				int zzz = 0;
			}

			return null;
        }

		public XVar Sort()
        {
            InitDictionary();
            XVar result = XVar.Array();
            foreach (var item in InternalDictionary.OrderBy(item => item.Value))
                result.SetArrayItem<XVar, XVar>(item.Key, item.Value);
            return result;
        }

        public XVar KSort()
        {
            InitDictionary();
            XVar result = XVar.Array();
            foreach (var item in InternalDictionary.OrderBy(item => item.Key))
                result.SetArrayItem<XVar, XVar>(item.Key, item.Value);
            return result;
        }

		public delegate XVar Comparer(XVar x, XVar b);

		public void USort(XVar parameters)
		{
			XVar obj;
			string className, methodName;
			if(parameters == null)
				return;

			if (parameters.IsArray())
			{
				if (parameters.Count() < 2)
					return;

				obj = parameters[0];
				methodName = parameters[1];
			}
			else
			{
				obj = new XVar( "CommonFunctions" );
				methodName = parameters;
			}

			InitDictionary();

			Comparer d = null;
			if( obj.IsString() ) {
				className = obj.ToString();
				Assembly assem = Assembly.GetExecutingAssembly();
				Type tClass = assem.GetType("runnerDotNet." + className);
				if(tClass == null)
					throw new ArgumentException("Sorting callback not found");

				MethodInfo mi = tClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
				if (mi == null)
				{
					tClass = assem.GetType("runnerDotNet.MVCFunctions");
					mi = tClass.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
				}

				if (mi == null)
					throw new ArgumentException("Sorting callback not found");

				try
				{
					d = (Comparer)Delegate.CreateDelegate(typeof(Comparer), mi);
				}
				catch
				{
					throw new ArgumentException("Sorting callback not found");
				}
			} else {
				//d = (Comparer)Delegate.CreateDelegate(typeof(Comparer), className, methodName );
			}

			XVar result = XVar.Array();
			int index = 0;
			var list = InternalDictionary.Values.ToList();
			if( obj.IsString() ) {
				list.Sort((x,y)=>d.Invoke(x,y));
			} else {
				list.Sort((x,y)=>obj.Invoke(methodName, x,y));
			}

			InternalDictionary.Clear();
			foreach (var item in list)
				SetArrayItem<XVar, XVar>(index++, item);
		}

        public object[] ToArray()
        {
            InitDictionary();
            ArrayList result = new ArrayList();
            foreach (var item in this.InternalDictionary)
                result.Add(item.Value);

            return result.ToArray();
        }

        public void RebuildKeys(int startIndex)
        {
            Dictionary<XVar, XVar> result = new Dictionary<XVar, XVar>();

            Dictionary<int, XVar> orderedResult = new Dictionary<int, XVar>();

            foreach (KeyValuePair<XVar, XVar> pair in InternalDictionary)
            {
                int tmpRes;
                if ((pair.Key).Value is int)
                {
                    orderedResult[new XVar((int)(pair.Key).Value)] = pair.Value;
                }
                else if ((pair.Key).Value is string && Int32.TryParse((string)(pair.Key).Value, out tmpRes))
                {
                    orderedResult[tmpRes] = pair.Value;
                }
                else
                    result[pair.Key] = pair.Value;
            }

            int index = 0;
            foreach (KeyValuePair<int, XVar> pair in orderedResult.OrderBy(x => x.Key))
            {
                result[startIndex + index++] = pair.Value;
            }

            this.value = result;
        }

        public XVar AnyKey()
        {
            return InternalDictionary.First().Key;
        }

        #endregion

        #region String functions

        public XVar IndexOf(XVar needle, XVar startIndex = null)
        {
            if (startIndex as object == null)
                startIndex = 0;
			else if (startIndex < 0) startIndex = 0;

            int index = this.ToString().IndexOf(needle.ToString(), startIndex.ToInt());
            if (index >= 0)
            {
                return index;
            }

            return false;
        }

        public XVar CaseInsensitiveIndexOf(XVar needle, XVar startIndex = null)
        {
            if (startIndex as object == null)
                startIndex = 0;
			else if (startIndex < 0) startIndex = 0;

            int index = this.ToString().IndexOf(needle.ToString(), startIndex.ToInt(), StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return index;
            }

            return false;
        }

        public XVar LastIndexOf(XVar needle, XVar startIndex = null)
        {
            if (this.ToString().Length > 0)
            {
                if (startIndex as object == null)
                    startIndex = this.ToString().Length - 1;
				else if (startIndex < 0) startIndex = 0;

                int index = this.ToString().LastIndexOf(needle.ToString(), startIndex.ToInt());
                if (index >= 0)
                {
                    return index;
                }
            }
            return false;
        }

        public XVar Length()
        {
            int result = 0;
            if (this.IsByteArray())
            {
                result = (this.Value as byte[]).Length;
            }
            else
            {
                result = this.ToString().Length;
            }
            return result;
        }

        public XVar ToUpper()
        {
            return this.ToString().ToUpperInvariant();
        }

        public XVar ToLower()
        {
            return this.ToString().ToLowerInvariant();
        }

        public XVar nl2br()
        {
            return this.ToString().Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
        }

        public XVar Substring(XVar startPosition, XVar length = null)
        {
            string strValue = this.ToString();

            if (startPosition < 0)
            {
                startPosition = strValue.Length + startPosition;
            }
            if (startPosition > -1 && startPosition < strValue.Length)
            {
                if (length as object != null && length < 0)
                    length = strValue.Length - startPosition + length;

                if (length as object != null && startPosition + length < strValue.Length)
                {
                    return strValue.Substring(startPosition.ToInt(), length.ToInt());
                }
                else
                {
                    return strValue.Substring(startPosition.ToInt());
                }
            }
            return "";
        }

        public XVar Replace(XVar oldValue, XVar newValue, bool caseInsensitive = false)
        {
            if (this.Value == null)
            {
                return null;
            }
            String result = this.ToString();
			StringComparison compare = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if (oldValue.IsArray())
            {
				int index = 0;
                foreach (var item in oldValue.GetEnumerator())
                {
					var replaceStr = newValue.IsArray() ? newValue[index].ToString() : newValue.ToString();
					result = ReplaceStr(result, item.Value.ToString(), replaceStr, compare);
					index++;
                }
            }
            else
                result = ReplaceStr(result, oldValue.ToString(), newValue.ToString(), compare);

            return result.ToString();
        }

        public string ReplaceStr(String str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public XVar Trim(XVar trimChars = null)
        {
            return this.ToString().Trim(trimChars ? trimChars.ToString().ToCharArray() : null);
        }

        public XVar Reverse()
        {
            if (this.IsArray())
            {
                XVar result = new XVar();
                foreach (var item in this.InternalDictionary.Reverse())
                {
                    result.SetArrayItem<XVar, XVar>(item.Key, item.Value);
                }
                return result;
            }
            else
                return new string(this.ToString().Reverse().ToArray());
        }

        public XVar Split(XVar delim)
        {
            var strList = value.ToString().Split(new string[] { delim.ToString() }, StringSplitOptions.None);
            var result = new XVar();
            for (int i = 0; i < strList.Length; i++)
                result[i] = strList[i];
            return result;
        }

		public XVar SplitByLen(XVar len = null)
		{
			if(len as object == null)
				len = 1;

			String str = value.ToString();
			var result = XVar.Array();
			Enumerable.Range(0, str.Length / len).ToList()
				.ForEach(i => result.Add(str.Substring(i * len, len)));

			return result;
		}

        #endregion

        #region IS functions

		public virtual bool IsRunnerType()
        {
			return false;
        }

		public bool IsNumericType(object o)
		{
			switch (Type.GetTypeCode(o.GetType()))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

        public bool IsNumeric()
        {
            bool result = false;
			if (this.value != null && IsNumericType(this.value))
            {
                result = true;
            }
            else
            {
                if (this.Type == typeof(string) && this.value.ToString() != "")
                {
                    int tempInt = 0;
                    double tempDouble = 0;
                    result = int.TryParse(this.value.ToString(), out tempInt) || double.TryParse(this.value.ToString(),  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out tempDouble);
                }
            }

            return result;
        }

        public bool IsByteArray()
        {
            return this.value != null && this.Type == typeof(byte[]);
        }

        public bool IsString()
        {
            return this.value != null && this.Type == typeof(string);
        }

        public bool IsArray()
        {
			return this.value != null && ((this.Type == typeof(Dictionary<XVar, XVar>) || this.Value is Dictionary<XVar, XVar> ) || (this.Type == typeof(XSettingsMap) && (this.Value as XSettingsMap).IsArray()));
        }

        public bool IsProperArray()
        {
            if (this.Type == typeof(Dictionary<XVar, XVar>)
				|| this.Value is Dictionary<XVar, XVar>
				|| (this.Type == typeof(XSettingsMap) && this.Value is XSettingsMap && (this.Value as XSettingsMap).IsArray()))
            {
                var keys = this.InternalDictionary.Keys
                    .Where(key => key.Type == typeof(int))
                    .OrderBy(key => key.Value)
                    .Select(key => key.Value as int?).ToList();
                if (keys.Count != this.InternalDictionary.Count || keys.Count > 0 && keys[0] != 0)
                {
                    return false;
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    if (i > 0 && keys[i] != keys[i - 1] + 1)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool IsObject()
        {
            return this.IsRunnerType() ||
                this.value != null &&
                    !(this.IsEmpty() ||
                    this.IsNumeric() ||
                    this.IsString() ||
                    this.IsArray());
        }

        public bool IsEmpty()
        {
            if (this.IsRunnerType())
            {
                return false;
            }

            if (this.value == null || this == false || this == "")
            {
                return true;
            }

            if (this.Type == typeof(Dictionary<XVar, XVar>))
            {
                return this.Count() == 0;
            }

            return false;
        }

        #endregion

        public static XVar Array()
        {
            return new XVar(new Dictionary<XVar, XVar>());
        }

        public static XVar Array( params object[] parameters )
        {
            XVar ret = new XVar(new Dictionary<XVar, XVar>());
            for (int i = 0; i < parameters.Length; i++) {
				ret[i] = new XVar(parameters[i]);
			}
			return ret;
        }

        public static XVar Pack(object obj)
        {
            if (obj == null) // very fast
            {
                return new XVar(null);
            }

			if(obj is XVar)
				return obj as XVar; // fast

            if (typeof(XVar).IsInstanceOfType(obj)) // slow
            {
                return (XVar)obj;
            }

            return new XVar(obj);
        }

      	public System.Text.RegularExpressions.RegexOptions PrepareRegexPattern(ref string patternStr)
		{
			var options = System.Text.RegularExpressions.RegexOptions.None;
			if (patternStr.IndexOf('/') >= 0)
			{
				patternStr = patternStr.Substring(patternStr.IndexOf('/') + 1);
				var suffix = patternStr.Substring(patternStr.LastIndexOf('/') + 1);
				patternStr = patternStr.Substring(0, patternStr.LastIndexOf('/'));

				if (suffix.Contains('i'))
					options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
				if (suffix.Contains('m'))
					options |= System.Text.RegularExpressions.RegexOptions.Multiline;
				if (suffix.Contains('x'))
					options |= System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace;
			}
			return options;
		}

      	public XVar PregMatch(XVar pattern, XVar matchesResult = null)
		{
			var patternStr = pattern.ToString();
			var options = PrepareRegexPattern(ref patternStr);

			if (matchesResult as object == null)
				return System.Text.RegularExpressions.Regex.IsMatch(this.ToString(), patternStr, options);

			var match = System.Text.RegularExpressions.Regex.Match(this.ToString(), patternStr, options);
			matchesResult.Value = null; // may already contain string or something
			matchesResult.InitDictionary();
			matchesResult.Add(match.ToString());
			for (int i = 1; i < match.Groups.Count; i++)
			{
				matchesResult[i] = match.Groups[i].Value;
			}
			return match.Success;
		}

		public XVar PregMatchAll(XVar pattern, XVar matchesResult)
		{
			var patternStr = pattern.ToString();
			var options = PrepareRegexPattern(ref patternStr);

			var matches = System.Text.RegularExpressions.Regex.Matches(this.ToString(), patternStr, options);
			matchesResult.Value = null; // may already contain string or something
			matchesResult.InitDictionary();
			matchesResult.Add(XVar.Array());

			int index = 0;
			foreach (System.Text.RegularExpressions.Match match in matches)
			{
				matchesResult[0].Add(match.ToString());
				for (int i = 1; i < match.Groups.Count; i++)
				{
					if (i > index)
						matchesResult[++index] = XVar.Array();

					matchesResult[i].Add(match.Groups[i].Value);
				}
			}
			return matches.Count;
		}

		public XVar PregFindMatches(XVar pattern, XVar matchesResult)
		{
			var patternStr = pattern.ToString();
			var options = PrepareRegexPattern(ref patternStr);

			var matches = System.Text.RegularExpressions.Regex.Matches(this.ToString(), patternStr, options);
			matchesResult.Value = null; // may already contain string or something
			matchesResult.InitDictionary();

			int index = 0;
			foreach (System.Text.RegularExpressions.Match match in matches)
			{
				XVar m = XVar.Array();
				m["match"] = match.ToString();
				m["submatches"] = XVar.Array();
				m["offset"] = new XVar( match.Index );

				for (int i = 1; i < match.Groups.Count; i++)
				{
					m["submatches"].Add( new XVar(match.Groups[i].Value) );
				}
				matchesResult.Add( m );
			}
			return matches.Count;
		}


		public XVar PregReplace(XVar pattern, XVar replacement, XVar str, XVar limit = null)
		{
			var patternStr = pattern.ToString();
			var options = PrepareRegexPattern(ref patternStr);

			if(limit as object == null)
				return System.Text.RegularExpressions.Regex.Replace(str.ToString(), patternStr, replacement.ToString(), options);

			int count = 0;
			var result = System.Text.RegularExpressions.Regex.Replace(str.ToString(), patternStr,
				new System.Text.RegularExpressions.MatchEvaluator(m =>
				{
					if (count < limit)
					{
						count++;
						return replacement.ToString();
					}
					else
						return m.Value;
				}), options);

			return result;
		}

		public XVar PregSplit(XVar pattern, XVar str)
		{
			var patternStr = pattern.ToString();
			var options = PrepareRegexPattern(ref patternStr);

			// php-like pattern is wrapped into ()
			// exclude ( ) so split delimenters will be excluded from result array
			if (patternStr.First() == '(' && patternStr.Last() == ')')
				patternStr = patternStr.Substring(1, patternStr.Length - 2);

			var strings = System.Text.RegularExpressions.Regex.Split(str.ToString(), patternStr, options);
			XVar result = XVar.Array();
			foreach(var st in strings)
			{
				result.Add(st);
			}
			return result;
		}

        // Override the ToString method to convert XVar to a string:
        public override string ToString()
        {
            if (this.value is Dictionary<XVar, XVar>)
            {
				return ArrayToString();
            }
            else return CSmartStr(value);
        }

		public string ArrayToString()
		{
			Dictionary<XVar, XVar> thisAsDictionary = (Dictionary<XVar, XVar>)this.value;
			var result = new StringBuilder();
			foreach (var key in ((Dictionary<XVar, XVar>)this.value).Keys)
			{
				if (thisAsDictionary[key] != null)
					result.AppendFormat("{0}-{1},", key.ToString(), thisAsDictionary[key].ToString());
			}
			return result.ToString();
		}

        public int ToInt()
        {
            if (value is int)
            {
                return (int)value;
            }
            if (value is double)
            {
                return (int)(double)value;
            }
            else
            {
                double result = 0;
                if (double.TryParse(value.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result))
                {
                    return (int)result;
                }
                return 0;
            }
        }

        public XVar Invoke(XVar method, params object[] parameters)
        {
            object invokeTarget = this.value != null ? this.value : this;
            var methodInfo = invokeTarget.GetType().GetMethod(method.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var parametersInfo = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null || parameters[i].GetType() != typeof(XVar) && !parameters[i].GetType().IsSubclassOf(typeof(XVar)))
                    parameters[i] = new XVar(parameters[i]);
            }
            if (parameters == null || parameters.Length != parametersInfo.Length)
            {
                object[] newParameters = new object[parametersInfo.Length];
                for (int i = 0; i < parametersInfo.Length; i++)
                {
                    newParameters[i] = parameters.Length > i ? parameters[i] : null;
                }
                return (XVar)methodInfo.Invoke(invokeTarget, newParameters);
            }
            else
                return (XVar)methodInfo.Invoke(invokeTarget, parameters);
        }

        public XVar GetFieldValue(XVar name)
        {
            return (XVar)this.GetType().GetField(name.ToString()).GetValue(this);
        }

        public void SetFieldValue(XVar name, XVar value)
        {
            this.GetType().GetField(name.ToString()).SetValue(this, value);
        }

        #region Type conversions

        private static void TryGetVals(XVar x, XVar y, out object p1, out object p2)
        {
            if (x as object != null)
            {
                p1 = x.value;
            }
            else
            {
                p1 = null;
            }
            if (y as object != null)
            {
                p2 = y.value;
            }
            else
            {
                p2 = null;
            }
        }

        private static int CSimpleInt(object p1)
        {
            if (p1.GetType() == typeof(int))
            {
                return (int)p1;
            }
            if (p1.GetType() == typeof(byte))
            {
                return (int)(byte)p1;
            }
            return (int)p1;
        }

		private static void TypeReduction(object p1, object p2, out object outP1, out object outP2)
		{
			outP1 = p1;
			outP2 = p2;

			var p1Type = Type.GetTypeCode(outP1.GetType());
			var p2Type = Type.GetTypeCode(outP2.GetType());

			SimplifyTypes(ref outP1, ref p1Type, p2Type);
			SimplifyTypes(ref outP2, ref p2Type, p1Type);

			if (p1Type == p2Type)
				return;

			if (Reduction(ref outP1, ref outP2, p1Type, p2Type) == Int32.MaxValue)
			{
				outP1 = CSmartStr(outP1);
				outP2 = CSmartStr(outP2);
			}
		}

		static private void SimplifyTypes(ref object redObj, ref TypeCode refType, TypeCode otherType)
		{
			switch (refType)
			{
				case TypeCode.DBNull:
					if (otherType == TypeCode.String)
					{
						redObj = "";
						refType = TypeCode.String;
					}
					else
					{
						redObj = 0;
						refType = TypeCode.Int32;
					}
					break;
				case TypeCode.Single:
				case TypeCode.Decimal:
					redObj = Convert.ToDouble(redObj);
					refType = TypeCode.Double;
					break;
				case TypeCode.UInt32:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					redObj = Convert.ToInt32(redObj);
					refType = TypeCode.Int32;
					break;
			}
		}

		static private int Reduction(ref object redObj1, ref object redObj2, TypeCode tObj1, TypeCode tObj2, int level = 0)
		{
			int redLevel = Int32.MaxValue;
			switch (tObj1)
			{
			 case TypeCode.Boolean:
					{
						if ((level > 1 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 1)) > 1)
							&& tObj2 != TypeCode.Boolean)
						{
							redObj2 = (bool)new XVar(redObj2);
						}
						else return redLevel;
						return 1;
					}
				case TypeCode.Char:
					{
						if ((level > 2 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 2)) > 2)
							&& tObj2 != TypeCode.Char)
						{
							redObj2 = Convert.ToChar(redObj2);
						}
						else return redLevel;
						return 2;
					}
				case TypeCode.Double:
					{
						if(tObj2 == TypeCode.Double)
							redObj2 = Convert.ToDouble(redObj2);
						else if (level > 3 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 3)) > 3)
							redObj2 = CSmartDbl(redObj2);
						else return redLevel;
						return 3;
					}
				case TypeCode.Int64:
					{
						if (tObj2 == TypeCode.Int64)					// if same type
							redObj2 = Convert.ToInt64(redObj2);			// just be shure
																		// if type-reduction-priority level is lower then first - try to reduct other variable
						else if (level > 4 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 4)) > 4)
						{												// it looks like other's variable level is higher - try to use current algorithm
							if (!CSmartLng(ref redObj2, true))			// CSmartLng false means it looks like double
							{
								double dblVal2 = (double)redObj2;
								if (dblVal2 - (int)dblVal2 == 0)		// its double without decimal part
								{
									redObj2 = (Int64)dblVal2;			// so its int - convert both variables to int
									redObj1 = Convert.ToInt64(redObj1);
								}
								else redObj1 = (double)(Int64)(redObj1);// its double with decimal part - convert other variable to double
							}
							else redObj1 = Convert.ToInt64(redObj1);	// CSmartLng true means it looks like int - so convert other variable to int
						}
						else return redLevel;
						return 4;
					}
				case TypeCode.Int32:
					{
						if (tObj2 == TypeCode.Int32)
							redObj2 = Convert.ToInt32(redObj2);
						else if (level > 5 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 5)) > 5)
						{
							if (!CSmartLng(ref redObj2))
							{
								double dblVal2 = (double)redObj2;
								if (dblVal2 > Int32.MaxValue && dblVal2 - (int)dblVal2 == 0)
								{
									redObj2 = (Int64)dblVal2;
									redObj1 = (Int64)(Int32)redObj1;
								}
								else redObj1 = (double)(int)(redObj1);
							}
							else redObj1 = CSimpleInt(redObj1);
						}
						else return redLevel;
						return 5;
					}
				case TypeCode.DateTime:
					{
						if (level > 6 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 6)) > 6)
						{
							if (tObj1 == TypeCode.DateTime)
								redObj1 = ((DateTime)redObj1).ToString("yyyy-MM-dd HH:mm:ss");
							if (tObj2 == TypeCode.DateTime)
								redObj2 = ((DateTime)redObj2).ToString("yyyy-MM-dd HH:mm:ss");
						}
						else return redLevel;
						return 6;
					}
				case TypeCode.String:
					{
						if (level > 7 || (redLevel = Reduction(ref redObj2, ref redObj1, tObj2, tObj1, 7)) > 7)
							redObj2 = redObj2.ToString();
						else return redLevel;
						return 7;
					}
				}

			return Int32.MaxValue;
		}
        private static double CSmartDbl(object value)
        {
            Type valType = value.GetType();
            if (valType == typeof(Single))
				return (double)(Single)value;
			else if (valType == typeof(decimal))
				return (double)(decimal)value;
			else if (valType == typeof(double))
				return (double)value;

            if (valType == typeof(bool))
                if ((bool)value)
                    return 1;

            if (valType == typeof(DateTime))
            {
                // ask Leasha about this
                //if(((DateTime)value).Year == 1899) then strValue=date() & " " & strValue
                return (new DateTime(1970, 1, 1) - (DateTime)value).Days;
            }

			//    On Error Resume Next
            double result = 0;
			double.TryParse(value.ToString().Replace(IncorrectDecimalSeparator, CorrectDecimalSeparator),  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }

        private static bool CSmartLng(ref object value, bool int64 = false)
        {
			// return true if succeeded to parse int, or if parsing failed
			// return false if succeeded to parse double

			if (value == null)
			{
				value = 0;
				return true;
			}

            Type valType = value.GetType();
            if (valType == typeof(bool))
				if ((bool)value)
				{
					value = 1;
					return true;
				}

			var valStr = value.ToString();

			if (valStr.Length > 0)
			{
				if (int64)
				{
					Int64 intResult;
					if (Int64.TryParse(valStr, out intResult))
					{
						value = intResult;
						return true;
					}
				}
				else
				{
					int intResult;
					if (Int32.TryParse(valStr, out intResult))
					{
						value = intResult;
						return true;
					}
				}

				bool boolResult;
				if (bool.TryParse(valStr, out boolResult))
				{
					value = boolResult ? 1 : 0;
					return true;
				}

				double dblResult = 0;
				if (double.TryParse(valStr.Replace(IncorrectDecimalSeparator, CorrectDecimalSeparator),  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out dblResult))
				{
					value = dblResult;
					return false;
				}

				// try last time with great assumptions
				valStr = new string(valStr.Trim().TakeWhile(x => Char.IsDigit(x)).ToArray());
				int intResult2;
				if (Int32.TryParse(valStr, out intResult2))
				{
					value = intResult2;
					return true;
				}
			}

            value = 0;
			return true;
        }

        private static string CSmartStr(object value)
        {
            if (value == null)
                return "";
            Type valType = value.GetType();
            if (valType == typeof(string))
                return value.ToString();
            if (valType == typeof(double)  )
                return ((double)value).ToString( System.Globalization.CultureInfo.InvariantCulture );
            if ( valType == typeof(float))
                return ((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (valType == typeof(decimal)  )
                return ((decimal)value).ToString( System.Globalization.CultureInfo.InvariantCulture );
            if (valType == typeof(bool))
                if ((bool)value)
                    return "1";
                else
                    return "";
			if (valType == typeof(DateTime))
				return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");

            return value.ToString();
        }

        #endregion

		public dynamic Clone()
        {
            if (IsArray())
                return ArrayClone();

            if (IsNumeric())
                return new XVar(Value);

            if (IsString())
                return new XVar(ToString());

            return this;
        }

		public static dynamic Clone(dynamic x)
		{
			if (x as object == null)
				return null;

			if(x as IXCloneable != null)
				return x.Clone();

			return x;
		}
    }

	/// <summary>
	/// SettingsMap wrapper
	/// </summary>
	public class XSettingsMap : IDictionary<XVar, XVar>
	{
		SettingsMap _m;
		public XSettingsMap(SettingsMap m)
		{
			_m = m;
		}

		public XVar GetValue()
		{
			return _m._x;
		}

		public bool IsArray()
		{
			return _m.IsArray();
		}

		public int Count
		{
			get { return _m.Count; }
		}

		public ICollection<XVar> Keys
		{
			get
			{
				if(_m.IsPlainArray)
					return _m.Keys.Select(x => XVar.Pack(int.Parse(x))).ToArray();
				else return _m.Keys.Select(x => XVar.Pack(x)).ToArray();
			}
		}
		public ICollection<XVar> Values
		{
			get
			{
				return _m.Values.Select(x => XVar.Pack(x)).ToArray();
			}
		}

		public XVar this[XVar key]
		{
			get
			{
				return _m[key.ToString()];
			}
			set
			{
				if( key.Type == typeof(int) && key == 0 && _m.Count == 0 ) {
					_m.Add( value );
				} else {
					_m[key.ToString()] = value;
				}
			}
		}

		public void Add(XVar key, XVar value)
		{
			if( key.Type == typeof(int) && key == 0 && _m.Count == 0 ) {
				_m.Add( value );
			} else {
				_m[key.ToString()] = value;	
			}
		}

		public void Add(KeyValuePair<XVar, XVar> pair)
		{
			_m[pair.Key.ToString()] = pair.Value;
		}

		public void Clear()
		{
			_m.Clear();
		}

		public bool ContainsKey(XVar key)
		{
			return _m.ContainsKey(key.ToString());
		}

		public bool Remove(XVar key)
		{
			return _m.Remove(key.ToString());
		}

		public bool Remove(KeyValuePair<XVar, XVar> pair)
		{
			return _m.Remove(pair.Key.ToString());
		}

		public void CopyTo(KeyValuePair<XVar, XVar>[] pairs, int count)
		{
			int pos = 0;
			foreach (KeyValuePair<string, SettingsMap> pair in _m)
			{
				pairs[pos++] = new KeyValuePair<XVar, XVar>(pair.Key, pair.Value);

				if(pos == count)
					break;
			}
		}

		public bool Contains(KeyValuePair<XVar, XVar> pair)
		{
			return _m.ContainsKey(pair.Key.ToString());
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool TryGetValue(XVar key, out XVar value)
		{
			value = null;
			SettingsMap r1 = null; ;
			if (_m.TryGetValue(key.ToString(), out r1))
			{
				value = r1;
				return true;
			}
			else return false;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _m.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<XVar, XVar>> GetEnumerator()
		{
			foreach (KeyValuePair<string, SettingsMap> pair in _m)
			{
				if (_m.IsPlainArray)
					yield return new KeyValuePair<XVar, XVar>(int.Parse(pair.Key), pair.Value);
				else yield return new KeyValuePair<XVar, XVar>(pair.Key, pair.Value);
			}
		}

		public override string ToString()
		{
			return _m.ToString();
		}
	}

	/// <summary>
	/// Simplified table settings container
	/// </summary>
	public class SettingsMap : Dictionary<String, SettingsMap>, IXCloneable
	{
		public XVar _x;
		bool _isArray = false; 		// is array by definition
		bool _isPlainArray = false; // is plain array by usage (not a hashmap, just an ordinary plain array)
		public bool IsArray() 		// is array by usage
		{
			return _x as object == null && (_isArray || _isPlainArray || Count > 0);
		}

		public bool IsPlainArray { get { return _isPlainArray; } }

		public SettingsMap() { }

		public SettingsMap(bool isArray)
		{
			_isArray = isArray;
		}

		public SettingsMap this[int key]
		{
			get {
                SettingsMap ret = null;
                base.TryGetValue(key.ToString(), out ret);
                return ret;
			}
			set { base[key.ToString()] = value; }
		}

		public SettingsMap this[XVar key]
		{
			get {
                SettingsMap ret = null;
                base.TryGetValue(key.ToString(), out ret);
                return ret;
			}
			set { base[key.ToString()] = value; }
		}

		public static implicit operator SettingsMap(string val)
		{
			var res = new SettingsMap();
			res._x = val;
			return res;
		}

		public static implicit operator SettingsMap(double val)
		{
			var res = new SettingsMap();
			res._x = val;
			return res;
		}

		public static implicit operator SettingsMap(int val)
		{
			var res = new SettingsMap();
			res._x = val;
			return res;
		}

		public static implicit operator SettingsMap(bool val)
		{
			var res = new SettingsMap();
			res._x = val;
			return res;
		}

		public static implicit operator SettingsMap(XVar val)
		{
			var res = new SettingsMap();

			if (val.IsArray())
			{
 				foreach (var pair in val.GetEnumerator())
					res[pair.Key] = pair.Value;
			}
			else res._x = val;

			return res;
		}

		public static SettingsMap GetArray()
		{
			return new SettingsMap(true);
		}

		public static int count(SettingsMap p)
		{
			if (p.Count > 0)
				return p.Count;

			if (p._x != null && p._x.IsArray())
				return p._x.Count();

			return 0;
		}

		public void Add<T>(T x)
		{
			if (!_isPlainArray)
				_isPlainArray = true;

			this[this.Count.ToString()] = XVar.Pack(x);
		}

		public static implicit operator string(SettingsMap m)
		{
			if (m == null)
				return null;

			if(m._x != null)
				return m._x.ToString();

			return "";
		}

		public static implicit operator XVar(SettingsMap m)
		{
			if (m == null)
				return null;

			if (m._x as object != null)
			{
				if (m._x.Type == typeof(SettingsMap)) // repack nested SettingsMap
					return new XVar(new XSettingsMap(m._x.Value as SettingsMap));
				return m._x;
			}
			else return new XVar(new XSettingsMap(m));
		}

		public dynamic Clone()
		{
			return (XVar)this;
		}

		public override string ToString()
		{
			if (_x as object != null)
				return _x.ToString();
			else if (this.Count == 0 && _isArray)
				return "[]";
			else if (IsPlainArray)
				return string.Join(", ", this.Values);
			else return string.Join(", ", this.ToList().Select(x => string.Format("{0} => {1}", x.Key, x.Value)));
		}
	}

#if (!TEST)

	public partial class ProjectSettings: IXCloneable
	{
		public dynamic Clone()
		{
			return this;
		}
	}

	public partial class XTempl: IXCloneable
	{
		public dynamic Clone()
		{
			return this;
		}
	}

	public partial class SQLQuery: IXCloneable
	{
		public dynamic Clone()
		{
			return this;
		}
	}

#endif

}