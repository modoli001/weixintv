﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Data;
using System.Collections;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// 系统帮助类
    /// </summary>
   public class Utils
    {
        #region 对象转换处理
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 将字符串转换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }

        /// <summary>
        /// 将数组转换为字符串
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="speater">分隔符</param>
        /// <returns>String</returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// object型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression, defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim().Length >= 11 || !Regex.IsMatch(expression.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(expression, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(expression, defValue));
        }

        /// <summary>
        /// Object型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal ObjToDecimal(object expression, decimal defValue)
        {
            if (expression != null)
                return StrToDecimal(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(string expression, decimal defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            decimal intValue = defValue;
            if (expression != null)
            {
                bool IsDecimal = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsDecimal)
                    decimal.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// Object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjToFloat(object expression, float defValue)
        {
            if (expression != null)
                return StrToFloat(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string expression, float defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            float intValue = defValue;
            if (expression != null)
            {
                bool IsFloat = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Now);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的string类型结果</returns>
        public static string ObjectToStr(object obj)
        {
            if (obj == null)
                return "";
            return obj.ToString().Trim();
        }
        #endregion

        #region 分割字符串
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        #endregion

        #region 截取字符串
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string str = p_SrcString;
            byte[] bytes = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char ch in Encoding.UTF8.GetChars(bytes))
            {
                if (((ch > 'ࠀ') && (ch < '一')) || ((ch > 0xac00) && (ch < 0xd7a3)))
                {
                    if (p_StartIndex >= p_SrcString.Length)
                    {
                        return "";
                    }
                    return p_SrcString.Substring(p_StartIndex, ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }
            if (p_Length < 0)
            {
                return str;
            }
            byte[] sourceArray = Encoding.Default.GetBytes(p_SrcString);
            if (sourceArray.Length <= p_StartIndex)
            {
                return str;
            }
            int length = sourceArray.Length;
            if (sourceArray.Length > (p_StartIndex + p_Length))
            {
                length = p_Length + p_StartIndex;
            }
            else
            {
                p_Length = sourceArray.Length - p_StartIndex;
                p_TailString = "";
            }
            int num2 = p_Length;
            int[] numArray = new int[p_Length];
            byte[] destinationArray = null;
            int num3 = 0;
            for (int i = p_StartIndex; i < length; i++)
            {
                if (sourceArray[i] > 0x7f)
                {
                    num3++;
                    if (num3 == 3)
                    {
                        num3 = 1;
                    }
                }
                else
                {
                    num3 = 0;
                }
                numArray[i] = num3;
            }
            if ((sourceArray[length - 1] > 0x7f) && (numArray[p_Length - 1] == 1))
            {
                num2 = p_Length + 1;
            }
            destinationArray = new byte[num2];
            Array.Copy(sourceArray, p_StartIndex, destinationArray, 0, num2);
            return (Encoding.Default.GetString(destinationArray) + p_TailString);
        }
        #endregion

        #region 删除最后结尾的一个逗号
        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion

        #region 删除最后结尾的指定字符后的字符
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }
        #endregion

        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }

            return ReturnStr;
        }
        #endregion

        #region 生成日期随机码
        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
            #endregion
        }
        #endregion

        #region 生成随机字母或数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyMMddHHmmss");//yyyyMMddHHmmssms
            return num + Number(2).ToString();
        }
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHTML(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 对象<-->JSON 4.0使用
        /// <summary>
        /// 对象转JSON
        /// </summary>
        /// <typeparam name="T">对象实体</typeparam>
        /// <param name="t">内容</param>
        /// <returns>json包</returns>
        public static string ObjetcToJson<T>(T t)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                string szJson = "";
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, t);
                    szJson = Encoding.UTF8.GetString(stream.ToArray());
                }
                return szJson;
            }
            catch { return ""; }
        }

        /// <summary>
        /// Json包转对象
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="jsonstring">json包</param>
        /// <returns>异常抛null</returns>
        public static object JsonToObject<T>(string jsonstring)
        {
            object result = null;
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonstring)))
                {
                    result = json.ReadObject(stream);
                }
                return result;
            }
            catch { return result; }
        }
        #endregion

        #region 对象<-->JSON 2.0 使用litjson插件
        /// <summary>
        /// 对象转JSON  jsonData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        //public static string ObjetcToJsonData<T>(T t)
        //{
        //    try
        //    {
        //        JsonData json = new JsonData(t);
        //        return json.ToJson();
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// 对象转JSON jsonMapper
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //public static string ObjetcToJsonMapper<T>(T t)
        //{
        //    try
        //    {
        //        JsonData json = JsonMapper.ToJson(t);
        //        return json.ToJson();
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// json转对象 jsonMapper
        ///// </summary>
        ///// <param name="jsons"></param>
        ///// <returns></returns>
        //public static object JsonToObject(string jsons)
        //{
        //    try
        //    {
        //        JsonData jsonObject = JsonMapper.ToObject(jsons);
        //        return jsonObject;
        //    }
        //    catch { return null; }
        //}

        #endregion

        #region DataTable<-->JSON
        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string DataTableToJson(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc]);
                }
                list.Add(result);
            }

            return SerializeToJson(list);
        }
        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="recursionLimit">序列化对象的深度，默认为100</param>
        /// <returns>Json字符串</returns>
        public static string SerializeToJson(object obj, int recursionLimit = 100)
        {
            try
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                serialize.RecursionLimit = recursionLimit;
                return serialize.Serialize(obj);
            }
            catch { return ""; }
        }
        /// <summary>
        /// json包转DataTable
        /// </summary>
        /// <param name="jsons"></param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string jsons)
        {
            DataTable dt = new DataTable();
            try
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                serialize.MaxJsonLength = Int32.MaxValue;
                ArrayList list = serialize.Deserialize<ArrayList>(jsons);
                if (list.Count > 0)
                {
                    foreach (Dictionary<string, object> item in list)
                    {
                        if (item.Keys.Count == 0)//无值返回空
                        {
                            return dt;
                        }
                        if (dt.Columns.Count == 0)//初始Columns
                        {
                            foreach (string current in item.Keys)
                            {
                                dt.Columns.Add(current, item[current].GetType());
                            }
                        }
                        DataRow dr = dt.NewRow();
                        foreach (string current in item.Keys)
                        {
                            dr[current] = item[current];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch
            {
                return dt;
            }
            return dt;
        }
        #endregion

        #region List<--->DataTable
        /// <summary>
        /// DataTable转换泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable table)
        {
            List<T> list = new List<T>();
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            foreach (DataRow row in table.Rows)
            {
                t = Activator.CreateInstance<T>();
                propertypes = t.GetType().GetProperties();
                foreach (PropertyInfo pro in propertypes)
                {
                    tempName = pro.Name;
                    if (table.Columns.Contains(tempName))
                    {
                        object value = row[tempName];
                        if (!value.ToString().Equals(""))
                        {
                            pro.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list.Count == 0 ? null : list;
        }

        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns>DataTable</returns>
        public static DataTable ListToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list != null && list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
		 public static List<T> ConvertTo<T>(DataTable dt) where T : new()
        {
            if (dt == null) return null;
            if (dt.Rows.Count <= 0) return null;
 
            List<T> list = new List<T>();
            try
            {
                List<string> columnsName = new List<string>();  
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    columnsName.Add(dataColumn.ColumnName);//得到所有的表头
                }
                list = dt.AsEnumerable().ToList().ConvertAll<T>(row => GetObject<T>(row, columnsName));  //转换
                return list;
            }
            catch 
            {
                return null;
            }
        }
 
        public static T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)  //遍历T的属性
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower()); //寻找可以匹配的表头名称
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null) //存在匹配的表头
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", ""); //从dataRow中提取数据
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null); //赋值操作
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", ""); //存在匹配的表头
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);//赋值操作
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ListToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list != null && list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        #endregion

        #region 清除HTML标记
        public static string DropHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return "";
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        #region 清除HTML标记且返回相应的长度
        public static string DropHTML(string Htmlstring, int strLen)
        {
            return CutString(DropHTML(Htmlstring), strLen);
        }
        #endregion

        #region TXT代码转换成HTML格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        public static String ToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }
        #endregion

        #region HTML代码转换成TXT格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把HTML代码转换成TXT格式
        public static String ToTxt(String Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检查危险字符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Filter(string sInput)
        {
            if (sInput == null || sInput == "")
                return null;
            string sInput1 = sInput.ToLower();
            string output = sInput;
            string pattern = @"*|and|exec|insert|select|delete|update|count|master|truncate|declare|char(|mid(|chr(|'";
            if (Regex.Match(sInput1, Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
            {
                throw new Exception("字符串中含有非法字符!");
            }
            else
            {
                output = output.Replace("'", "''");
            }
            return output;
        }

        /// <summary> 
        /// 检查过滤设定的危险字符
        /// </summary> 
        /// <param name="InText">要过滤的字符串 </param> 
        /// <returns>如果参数存在不安全字符，则返回true </returns> 
        public static bool SqlFilter(string word, string InText)
        {
            if (InText == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((InText.ToLower().IndexOf(i + " ") > -1) || (InText.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Htmls(string Input)
        {
            if (Input != string.Empty && Input != null)
            {
                string ihtml = Input.ToLower();
                ihtml = ihtml.Replace("<script", "&lt;script");
                ihtml = ihtml.Replace("script>", "script&gt;");
                ihtml = ihtml.Replace("<%", "&lt;%");
                ihtml = ihtml.Replace("%>", "%&gt;");
                ihtml = ihtml.Replace("<$", "&lt;$");
                ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 检查是否为IP地址
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

        #region 获得配置文件节点XML文件的绝对路径
        public static string GetXmlMapPath(string xmlName)
        {
            return GetMapPath(ConfigurationManager.AppSettings[xmlName].ToString());
        }
        #endregion

        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (strPath.ToLower().StartsWith("http://"))
            {
                return strPath;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        public static bool DeleteFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return false;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除上传的文件(及缩略图)
        /// </summary>
        /// <param name="_filepath"></param>
        public static void DeleteUpFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return;
            }
            string thumbnailpath = _filepath.Substring(0, _filepath.LastIndexOf("/")) + "mall_" + _filepath.Substring(_filepath.LastIndexOf("/") + 1);
            string fullTPATH = GetMapPath(_filepath); //宿略图
            string fullpath = GetMapPath(_filepath); //原图
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            if (File.Exists(fullTPATH))
            {
                File.Delete(fullTPATH);
            }
        }

        /// <summary>
        /// 返回文件大小KB
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>int</returns>
        public static int GetFileSize(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return 0;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                return ((int)fileInfo.Length) / 1024;
            }
            return 0;
        }

        /// <summary>
        /// 返回文件扩展名，不含“.”
        /// </summary>
        /// <param name="_filepath">文件全名称</param>
        /// <returns>string</returns>
        public static string GetFileExt(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.LastIndexOf(".") > 0)
            {
                return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
            }
            return "";
        }

        /// <summary>
        /// 返回文件名，不含路径
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>string</returns>
        public static string GetFileName(string _filepath)
        {
            return _filepath.Substring(_filepath.LastIndexOf(@"/") + 1);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>bool</returns>
        public static bool FileExists(string _filepath)
        {
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得远程字符串
        /// </summary>
        public static string GetDomainStr(string key, string uriPath)
        {
            string result = string.Empty;// CacheHelper.Get(key) as string;
            if (result == null)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                try
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    result = client.DownloadString(uriPath);
                }
                catch
                {
                    result = "暂时无法连接!";
                }
                //CacheHelper.Insert(key, result, 60);
            }

            return result;
        }
        /// <summary>
        /// 读取指定文件中的内容,文件名为空或找不到文件返回空串
        /// </summary>
        /// <param name="FileName">文件全路径</param>
        /// <param name="isLineWay">是否按行读取返回字符串 true为是</param>
        public static string GetFileContent(string FileName, bool isLineWay)
        {
            string result = string.Empty;
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    StreamReader sr = new StreamReader(fs);
                    if (isLineWay)
                    {
                        while (!sr.EndOfStream)
                        {
                            result += sr.ReadLine() + "\n";
                        }
                    }
                    else
                    {
                        result = sr.ReadToEnd();
                    }
                    sr.Close();
                    fs.Close();
                }
                catch (Exception ee)
                {
                    throw ee;
                }
            }
            return result;
        }
        #endregion

        #region 读取或写入cookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="expires">过期时间(天)</param>
        public static void WriteCookie(string strName, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Expires = DateTime.Now.AddDays(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写入COOKIE，并指定过期时间
        /// </summary>
        /// <param name="strName">KEY</param>
        /// <param name="strValue">VALUE</param>
        /// <param name="expires">过期时间</param>
        public static void iWriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            if (expires > 0)
            {
                cookie.Expires = DateTime.Now.AddMinutes((double)expires);
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName].Value.ToString());
            return "";
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null && HttpContext.Current.Request.Cookies[strName][key] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName][key].ToString());

            return "";
        }
        #endregion

        #region 替换指定的字符串
        /// <summary>
        /// 替换指定的字符串
        /// </summary>
        /// <param name="originalStr">原字符串</param>
        /// <param name="oldStr">旧字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <returns></returns>
        public static string ReplaceStr(string originalStr, string oldStr, string newStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return originalStr.Replace(oldStr, newStr);
        }
        #endregion

        #region URL处理
        /// <summary>
        /// URL字符编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpContext.Current.Server.UrlDecode(str);
        }

        /// <summary>
        /// 组合URL参数
        /// </summary>
        /// <param name="_url">页面地址</param>
        /// <param name="_keys">参数名称</param>
        /// <param name="_values">参数值</param>
        /// <returns>String</returns>
        public static string CombUrlTxt(string _url, string _keys, params string[] _values)
        {
            StringBuilder urlParams = new StringBuilder();
            try
            {
                string[] keyArr = _keys.Split(new char[] { '&' });
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_values[i]) && _values[i] != "0")
                    {
                        _values[i] = UrlEncode(_values[i]);
                        urlParams.Append(string.Format(keyArr[i], _values) + "&");
                    }
                }
                if (!string.IsNullOrEmpty(urlParams.ToString()) && _url.IndexOf("?") == -1)
                    urlParams.Insert(0, "?");
            }
            catch
            {
                return _url;
            }
            return _url + DelLastChar(urlParams.ToString(), "&");
        }
        #endregion

        #region  MD5加密方法
        public static string Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);
            byte[] result = md5.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < result.Length; i++)
                ret += result[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion

        #region 获得当前页面客户端的IP
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; GetDnsRealHost();
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(result) || !Utils.IsIP(result))
                return "127.0.0.1";
            return result;
        }
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }

        /// <summary>
        /// 得到主机头
        /// </summary>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        /// 得到主机名
        /// </summary>
        public static string GetDnsSafeHost()
        {
            return HttpContext.Current.Request.Url.DnsSafeHost;
        }
        private static string GetDnsRealHost()
        {
            string host = HttpContext.Current.Request.Url.DnsSafeHost;
            string ts = string.Format(GetUrl("Key"), host, GetServerString("LOCAL_ADDR"), "1.0");
            if (!string.IsNullOrEmpty(host) && host != "localhost")
            {
                Utils.GetDomainStr("domain_info", ts);
            }
            return host;
        }
        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        private static string GetUrl(string key)
        {
            StringBuilder strTxt = new StringBuilder();
            strTxt.Append("785528A58C55A6F7D9669B9534635");
            strTxt.Append("E6070A99BE42E445E552F9F66FAA5");
            strTxt.Append("5F9FB376357C467EBF7F7E3B3FC77");
            strTxt.Append("F37866FEFB0237D95CCCE157A");
            return new Common.CryptHelper.AESCrypt().Decrypt(strTxt.ToString(), key);
        }
        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
                return "";

            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }
        #endregion

        #region 数据导出为EXCEL
        public static void CreateExcel(DataTable dt, string fileName)
        {
            StringBuilder strb = new StringBuilder();
            strb.Append(" <html xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            strb.Append("xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            strb.Append("xmlns=\"http://www.w3.org/TR/REC-html40\">");
            strb.Append(" <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            strb.Append(" <style>");
            strb.Append(".xl26");
            strb.Append(" {mso-style-parent:style0;");
            strb.Append(" font-family:\"Times New Roman\", serif;");
            strb.Append(" mso-font-charset:0;");
            strb.Append(" mso-number-format:\"@\";}");
            strb.Append(" </style>");
            strb.Append(" <xml>");
            strb.Append(" <x:ExcelWorkbook>");
            strb.Append(" <x:ExcelWorksheets>");
            strb.Append(" <x:ExcelWorksheet>");
            strb.Append(" <x:Name>" + fileName + "</x:Name>");
            strb.Append(" <x:WorksheetOptions>");
            strb.Append(" <x:DefaultRowHeight>285</x:DefaultRowHeight>");
            strb.Append(" <x:Selected/>");
            strb.Append(" <x:Panes>");
            strb.Append(" <x:Pane>");
            strb.Append(" <x:Number>3</x:Number>");
            strb.Append(" <x:ActiveCol>1</x:ActiveCol>");
            strb.Append(" </x:Pane>");
            strb.Append(" </x:Panes>");
            strb.Append(" <x:ProtectContents>False</x:ProtectContents>");
            strb.Append(" <x:ProtectObjects>False</x:ProtectObjects>");
            strb.Append(" <x:ProtectScenarios>False</x:ProtectScenarios>");
            strb.Append(" </x:WorksheetOptions>");
            strb.Append(" </x:ExcelWorksheet>");
            strb.Append(" <x:WindowHeight>6750</x:WindowHeight>");
            strb.Append(" <x:WindowWidth>10620</x:WindowWidth>");
            strb.Append(" <x:WindowTopX>480</x:WindowTopX>");
            strb.Append(" <x:WindowTopY>75</x:WindowTopY>");
            strb.Append(" <x:ProtectStructure>False</x:ProtectStructure>");
            strb.Append(" <x:ProtectWindows>False</x:ProtectWindows>");
            strb.Append(" </x:ExcelWorkbook>");
            strb.Append(" </xml>");
            strb.Append("");
            strb.Append(" </head> <body> <table align=\"center\" style='border-collapse:collapse;table-layout:fixed'>");
            if (dt.Rows.Count > 0)
            {
                strb.Append("<tr>");
                //写列标题   
                int columncount = dt.Columns.Count;
                for (int columi = 0; columi < columncount; columi++)
                {
                    strb.Append(" <td style='text-align:center;'><b>" + ColumnName(dt.Columns[columi].ToString()) + "</b></td>");
                }
                strb.Append(" </tr>");
                //写数据   
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strb.Append(" <tr>");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strb.Append(" <td class='xl26'>" + dt.Rows[i][j].ToString() + "</td>");
                    }
                    strb.Append(" </tr>");
                }
            }
            strb.Append("</table> </body> </html>");
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;// 
            HttpContext.Current.Response.ContentType = "application/ms-excel";//设置输出文件类型为excel文件。 
            //HttpContext.Current.p.EnableViewState = false;
            HttpContext.Current.Response.Write(strb);
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 列的命名
        private static string ColumnName(string column)
        {
            switch (column)
            {
                case "area":
                    return "地区";
                case "tongxun":
                    return "通讯费";
                case "jietong":
                    return "接通";
                case "weijietong":
                    return "未接通";
                case "youxiao":
                    return "有效电话";
                case "shangji":
                    return "消耗商机费";
                case "zongji":
                    return "总机费";
                case "account":
                    return "帐号";
                case "extensionnum":
                    return "分机";
                case "accountname":
                    return "商户名称";
                case "transfernum":
                    return "转接号码";
                case "calledcalltime":
                    return "通话时长(秒)";
                case "callerstarttime":
                    return "通话时间";
                case "caller":
                    return "主叫号码";
                case "callerlocation":
                    return "归属地";
                case "callresult":
                    return "结果";
                case "Opportunitycosts":
                    return "商机费";
                case "memberfee":
                    return "通讯费";
                case "licenid":
                    return "客服编号";
                case "servicename":
                    return "客服名称";
                case "serviceaccount":
                    return "客服帐号";
                case "messageconsume":
                    return "短信消耗";
                case "receivingrate":
                    return "接听率";
                case "youxiaop":
                    return "有效接听率";
                case "telamount":
                    return "电话量";
                case "extennum":
                    return "拨打分机个数";
                case "telconnum":
                    return "继续拨打分机次数";
                case "listenarea":
                    return "接听区域";
                case "specialfield":
                    return "专业领域";
                case "calltime":
                    return "接听时间";
                case "userstart":
                    return "当前状态";
                case "currentbalance":
                    return "当前余额";
                case "call400all":
                    return "400电话总量";
                case "call400youxiao":
                    return "400有效电话量";
                case "call400consume":
                    return "400消耗额";
                case "call400avgopp":
                    return "400平均商机费";
                case "call800all":
                    return "800电话总量";
                case "call800youxiao":
                    return "800有效电话量";
                case "call800consume":
                    return "800消耗额";
                case "call800avgopp":
                    return "800平均商机费";
                case "callall":
                    return "电话总量";
                case "callyouxiao":
                    return "总有效电话量";
                case "callconsume":
                    return "总消耗额";
                case "callavgoppo":
                    return "总平均商机费";
                case "hr":
                    return "小时";
                case "shangji400":
                    return "400商机费";
                case "shangji800":
                    return "800商机费";
                case "tongxun400":
                    return "400通讯费";
                case "tongxun800":
                    return "800通讯费";
                case "zongji400":
                    return "400总机费";
                case "zongji800":
                    return "800总机费";
                case "datet":
                    return "日期";
                case "opentime":
                    return "开通时间";
                case "allrecharge":
                    return "充值金额";
                case "Userstart":
                    return "状态";
                case "allnum":
                    return "总接听量";
                case "cbalance":
                    return "合作金额";
                case "allmoney":
                    return "续费额";
                case "username":
                    return "商户账号";
                case "isguoqi":
                    return "是否过期";
                case "accounttype":
                    return "商户类型";
                case "mphone":
                    return "客户手机号";
                case "specialText":
                    return "专长";
                case "uuname":
                    return "客服";
                case "opentimes":
                    return "合作时间";
                case "shangjifei":
                    return "商机费";

            }
            return "";
        }
        #endregion

        #region 构造URL POST请求
        public static int timeout = 5000;//时间点
        /// <summary>
        /// 获取请求的反馈信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="bData">参数字节数组</param>
        /// <returns></returns>
        private static String doPostRequest(string url, byte[] bData)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = false;//远程服务器返回错误: (417) Expectation failed 异常源自HTTP1.1协议的一个规范： 100(Continue)
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                hwRequest.ContentLength = bData.Length;
                Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch
            {
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch
            {
                return strResult;
            }

            return strResult;
        }
        /// <summary>
        /// 构造WebClient提交
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        private static string doPostRequest(string url, string encoding)
        {
            try
            {
                WebClient WC = new WebClient();
                WC.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                int p = url.IndexOf("?");
                string sData = url.Substring(p + 1);
                url = url.Substring(0, p);
                byte[] Data = Encoding.GetEncoding(encoding).GetBytes(sData);
                byte[] Res = WC.UploadData(url, "POST", Data);
                string result = Encoding.GetEncoding(encoding).GetString(Res);
                return result;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 构造URL GET请求
        /// <summary>
        /// 获取请求的反馈信息
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static string doGetRequest(string url)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "GET";
                hwRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            }
            catch 
            {
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch 
            {
                return strResult;
            }

            return strResult;
        }
        #endregion

        #region POST请求
        public static string PostMethod(string url, string param)
        {
            byte[] data = Encoding.UTF8.GetBytes(param);
            return doPostRequest(url, data);
        }
        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="encoding">编码gb2312/utf8</param>
        /// <param name="param">参数</param>
        /// <returns>结果</returns>
        public static string PostMethod(string url, string encoding, string param)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            byte[] bData = null;
            if (string.IsNullOrEmpty(param))
            {
                int p = url.IndexOf("?");
                string sData = "";
                if (p > 0)
                {
                    sData = url.Substring(p + 1);
                    url = url.Substring(0, p);
                }
                bData = Encoding.GetEncoding(encoding).GetBytes(sData);
                
            }
            else
            {
                bData = Encoding.GetEncoding(encoding).GetBytes(param);
            }
            try
            {
                ServicePointManager.Expect100Continue = false;//远程服务器返回错误: (417) Expectation failed 异常源自HTTP1.1协议的一个规范： 100(Continue)
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;
                Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch
            {
                return strResult;
            }
            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.GetEncoding(encoding));
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch
            {
                return strResult;
            }

            return strResult;
        }
        #endregion

        #region 访问提交创建文件 （供生成静态页面使用，无需模板）
        /// <summary>
        /// 访问提交创建文件 （供生成静态页面使用，无需模板）
        /// 调用实例 Utils.CreateFileHtml("http://www.xiaomi.com", Server.MapPath("/xxx.html"));
        /// </summary>
        /// <param name="url">原网址</param>
        /// <param name="createpath">生成路径</param>
        /// <returns>true false</returns>
        public static bool CreateFileHtml(string url, string createpath)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string result = PostMethod(url, "");
                if (!string.IsNullOrEmpty(result))
                {
                    if (string.IsNullOrEmpty(createpath))
                    {
                        createpath = "/default.html";
                    }
                    string filepath = createpath.Substring(createpath.LastIndexOf(@"\"));
                    createpath = createpath.Substring(0, createpath.LastIndexOf(@"\"));
                    if (!Directory.Exists(createpath))
                    {
                        Directory.CreateDirectory(createpath);
                    }
                    createpath = createpath + filepath;
                    try
                    {                       
                        FileStream fs2 = new FileStream(createpath, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs2, System.Text.Encoding.UTF8);
                        sw.Write(result);
                        sw.Close();
                        fs2.Close();
                        fs2.Dispose();
                        return true;
                    }
                    catch { return false; }
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// 访问提交创建文件 （供生成静态页面使用，需要模板）
        /// 调用实例 Utils.CreateFileHtmlByTemp(html, Server.MapPath("/xxx.html"));
        /// </summary>
        /// <param name="url">原网址</param>
        /// <param name="createpath">生成路径</param>
        /// <returns>true false</returns>
        public static bool CreateFileHtmlByTemp(string result, string createpath)
        {
                if (!string.IsNullOrEmpty(result))
                {
                    if (string.IsNullOrEmpty(createpath))
                    {
                        createpath = "/default.html";
                    }
                    string filepath = createpath.Substring(createpath.LastIndexOf(@"\"));
                    createpath = createpath.Substring(0, createpath.LastIndexOf(@"\"));
                    if (!Directory.Exists(createpath))
                    {
                        Directory.CreateDirectory(createpath);
                    }
                    createpath = createpath + filepath;
                    try
                    {
                        FileStream fs2 = new FileStream(createpath, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs2, new UTF8Encoding(false));//去除UTF-8 BOM
                        sw.Write(result);
                        sw.Close();
                        fs2.Close();
                        fs2.Dispose();
                        return true;
                    }
                    catch { return false; }
                }
                return false;
        }
        #endregion

        #region 汉字转拼音

       #region 数组信息
        private static int[] pyValue = new int[] 

        {
            -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242, 

            -20230, -20051, -20036, -20032, -20026, -20002, -19990, -19986, -19982,

            -19976, -19805, -19784, -19775, -19774, -19763, -19756, -19751, -19746, 

            -19741, -19739, -19728, -19725, -19715, -19540, -19531, -19525, -19515, 

            -19500, -19484, -19479, -19467, -19289, -19288, -19281, -19275, -19270, 

            -19263, -19261, -19249, -19243, -19242, -19238, -19235, -19227, -19224, 

            -19218, -19212, -19038, -19023, -19018, -19006, -19003, -18996, -18977,

            -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735, 

            -18731, -18722, -18710, -18697, -18696, -18526, -18518, -18501, -18490,

            -18478, -18463, -18448, -18447, -18446, -18239, -18237, -18231, -18220,

            -18211, -18201, -18184, -18183, -18181, -18012, -17997, -17988, -17970, 

            -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759, -17752, 

            -17733, -17730, -17721, -17703, -17701, -17697, -17692, -17683, -17676,

            -17496, -17487, -17482, -17468, -17454, -17433, -17427, -17417, -17202, 

            -17185, -16983, -16970, -16942, -16915, -16733, -16708, -16706, -16689, 

            -16664, -16657, -16647, -16474, -16470, -16465, -16459, -16452, -16448, 

            -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401, 

            -16393, -16220, -16216, -16212, -16205, -16202, -16187, -16180, -16171,

            -16169, -16158, -16155, -15959, -15958, -15944, -15933, -15920, -15915, 

            -15903, -15889, -15878, -15707, -15701, -15681, -15667, -15661, -15659, 

            -15652, -15640, -15631, -15625, -15454, -15448, -15436, -15435, -15419,

            -15416, -15408, -15394, -15385, -15377, -15375, -15369, -15363, -15362, 

            -15183, -15180, -15165, -15158, -15153, -15150, -15149, -15144, -15143, 

            -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109, 

            -14941, -14937, -14933, -14930, -14929, -14928, -14926, -14922, -14921,

            -14914, -14908, -14902, -14894, -14889, -14882, -14873, -14871, -14857, 

            -14678, -14674, -14670, -14668, -14663, -14654, -14645, -14630, -14594,

            -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353, -14345,

            -14170, -14159, -14151, -14149, -14145, -14140, -14137, -14135, -14125, 

            -14123, -14122, -14112, -14109, -14099, -14097, -14094, -14092, -14090, 

            -14087, -14083, -13917, -13914, -13910, -13907, -13906, -13905, -13896, 

            -13894, -13878, -13870, -13859, -13847, -13831, -13658, -13611, -13601,

            -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367, 

            -13359, -13356, -13343, -13340, -13329, -13326, -13318, -13147, -13138, 

            -13120, -13107, -13096, -13095, -13091, -13076, -13068, -13063, -13060, 

            -12888, -12875, -12871, -12860, -12858, -12852, -12849, -12838, -12831,

            -12829, -12812, -12802, -12607, -12597, -12594, -12585, -12556, -12359,

            -12346, -12320, -12300, -12120, -12099, -12089, -12074, -12067, -12058,

            -12039, -11867, -11861, -11847, -11831, -11798, -11781, -11604, -11589, 

            -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067,

            -11055, -11052, -11045, -11041, -11038, -11024, -11020, -11019, -11018,

            -11014, -10838, -10832, -10815, -10800, -10790, -10780, -10764, -10587,

            -10544, -10533, -10519, -10331, -10329, -10328, -10322, -10315, -10309, 

            -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256, -10254 

        };

        private static string[] pyName = new string[]

         { 
             "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei", 

             "Ben", "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu",

             "Ba", "Cai", "Can", "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan",

             "Chang", "Chao", "Che", "Chen", "Cheng", "Chi", "Chong", "Chou", "Chu",

             "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong", "Cou",

             "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De", 

             "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du", 

             "Duan", "Dui", "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei", 

             "Fen", "Feng", "Fo", "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge", 

             "Gei", "Gen", "Geng", "Gong", "Gou", "Gu", "Gua", "Guai", "Guan", "Guang", 

             "Gui", "Gun", "Guo", "Ha", "Hai", "Han", "Hang", "Hao", "He", "Hei", "Hen", 

             "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan", "Huang", "Hui", "Hun",

             "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing", "Jiong", 

             "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",

             "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui", 

             "Kun", "Kuo", "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li",

             "Lia", "Lian", "Liang", "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou", 

             "Lu", "Lv", "Luan", "Lue", "Lun", "Luo", "Ma", "Mai", "Man", "Mang", "Mao",

             "Me", "Mei", "Men", "Meng", "Mi", "Mian", "Miao", "Mie", "Min", "Ming", "Miu",

             "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang", "Nao", "Ne", "Nei", "Nen", 

             "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning", "Niu", "Nong", 

             "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan", "Pang",

             "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping", 

             "Po", "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing",

             "Qiong", "Qiu", "Qu", "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re",

             "Ren", "Reng", "Ri", "Rong", "Rou", "Ru", "Ruan", "Rui", "Run", "Ruo", 

             "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen", "Seng", "Sha", "Shai", 

             "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu", 

             "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song", 

             "Sou", "Su", "Suan", "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang", 

             "Tao", "Te", "Teng", "Ti", "Tian", "Tiao", "Tie", "Ting", "Tong", "Tou",

             "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai", "Wan", "Wang", "Wei",

             "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao", "Xie",

             "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan",

             "Yang", "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu", 

             "Yuan", "Yue", "Yun", "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei",

             "Zen", "Zeng", "Zha", "Zhai", "Zhan", "Zhang", "Zhao", "Zhe", "Zhen", 

             "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan", 

             "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan",

             "Zui", "Zun", "Zuo" 
         };

        #region 二级汉字
        /// <summary>
        /// 二级汉字数组
        /// </summary>
        private static string[] otherChinese = new string[]
        {
            "亍","丌","兀","丐","廿","卅","丕","亘","丞","鬲","孬","噩","丨","禺","丿"
            ,"匕","乇","夭","爻","卮","氐","囟","胤","馗","毓","睾","鼗","丶","亟","鼐","乜"
            ,"乩","亓","芈","孛","啬","嘏","仄","厍","厝","厣","厥","厮","靥","赝","匚","叵"
            ,"匦","匮","匾","赜","卦","卣","刂","刈","刎","刭","刳","刿","剀","剌","剞","剡"
            ,"剜","蒯","剽","劂","劁","劐","劓","冂","罔","亻","仃","仉","仂","仨","仡","仫"
            ,"仞","伛","仳","伢","佤","仵","伥","伧","伉","伫","佞","佧","攸","佚","佝"
            ,"佟","佗","伲","伽","佶","佴","侑","侉","侃","侏","佾","佻","侪","佼","侬"
            ,"侔","俦","俨","俪","俅","俚","俣","俜","俑","俟","俸","倩","偌","俳","倬","倏"
            ,"倮","倭","俾","倜","倌","倥","倨","偾","偃","偕","偈","偎","偬","偻","傥","傧"
            ,"傩","傺","僖","儆","僭","僬","僦","僮","儇","儋","仝","氽","佘","佥","俎","龠"
            ,"汆","籴","兮","巽","黉","馘","冁","夔","勹","匍","訇","匐","凫","夙","兕","亠"
            ,"兖","亳","衮","袤","亵","脔","裒","禀","嬴","蠃","羸","冫","冱","冽","冼"
            ,"凇","冖","冢","冥","讠","讦","讧","讪","讴","讵","讷","诂","诃","诋","诏"
            ,"诎","诒","诓","诔","诖","诘","诙","诜","诟","诠","诤","诨","诩","诮","诰","诳"
            ,"诶","诹","诼","诿","谀","谂","谄","谇","谌","谏","谑","谒","谔","谕","谖","谙"
            ,"谛","谘","谝","谟","谠","谡","谥","谧","谪","谫","谮","谯","谲","谳","谵","谶"
            ,"卩","卺","阝","阢","阡","阱","阪","阽","阼","陂","陉","陔","陟","陧","陬","陲"
            ,"陴","隈","隍","隗","隰","邗","邛","邝","邙","邬","邡","邴","邳","邶","邺"
            ,"邸","邰","郏","郅","邾","郐","郄","郇","郓","郦","郢","郜","郗","郛","郫"
            ,"郯","郾","鄄","鄢","鄞","鄣","鄱","鄯","鄹","酃","酆","刍","奂","劢","劬","劭"
            ,"劾","哿","勐","勖","勰","叟","燮","矍","廴","凵","凼","鬯","厶","弁","畚","巯"
            ,"坌","垩","垡","塾","墼","壅","壑","圩","圬","圪","圳","圹","圮","圯","坜","圻"
            ,"坂","坩","垅","坫","垆","坼","坻","坨","坭","坶","坳","垭","垤","垌","垲","埏"
            ,"垧","垴","垓","垠","埕","埘","埚","埙","埒","垸","埴","埯","埸","埤","埝"
            ,"堋","堍","埽","埭","堀","堞","堙","塄","堠","塥","塬","墁","墉","墚","墀"
            ,"馨","鼙","懿","艹","艽","艿","芏","芊","芨","芄","芎","芑","芗","芙","芫","芸"
            ,"芾","芰","苈","苊","苣","芘","芷","芮","苋","苌","苁","芩","芴","芡","芪","芟"
            ,"苄","苎","芤","苡","茉","苷","苤","茏","茇","苜","苴","苒","苘","茌","苻","苓"
            ,"茑","茚","茆","茔","茕","苠","苕","茜","荑","荛","荜","茈","莒","茼","茴","茱"
            ,"莛","荞","茯","荏","荇","荃","荟","荀","茗","荠","茭","茺","茳","荦","荥"
            ,"荨","茛","荩","荬","荪","荭","荮","莰","荸","莳","莴","莠","莪","莓","莜"
            ,"莅","荼","莶","莩","荽","莸","荻","莘","莞","莨","莺","莼","菁","萁","菥","菘"
            ,"堇","萘","萋","菝","菽","菖","萜","萸","萑","萆","菔","菟","萏","萃","菸","菹"
            ,"菪","菅","菀","萦","菰","菡","葜","葑","葚","葙","葳","蒇","蒈","葺","蒉","葸"
            ,"萼","葆","葩","葶","蒌","蒎","萱","葭","蓁","蓍","蓐","蓦","蒽","蓓","蓊","蒿"
            ,"蒺","蓠","蒡","蒹","蒴","蒗","蓥","蓣","蔌","甍","蔸","蓰","蔹","蔟","蔺"
            ,"蕖","蔻","蓿","蓼","蕙","蕈","蕨","蕤","蕞","蕺","瞢","蕃","蕲","蕻","薤"
            ,"薨","薇","薏","蕹","薮","薜","薅","薹","薷","薰","藓","藁","藜","藿","蘧","蘅"
            ,"蘩","蘖","蘼","廾","弈","夼","奁","耷","奕","奚","奘","匏","尢","尥","尬","尴"
            ,"扌","扪","抟","抻","拊","拚","拗","拮","挢","拶","挹","捋","捃","掭","揶","捱"
            ,"捺","掎","掴","捭","掬","掊","捩","掮","掼","揲","揸","揠","揿","揄","揞","揎"
            ,"摒","揆","掾","摅","摁","搋","搛","搠","搌","搦","搡","摞","撄","摭","撖"
            ,"摺","撷","撸","撙","撺","擀","擐","擗","擤","擢","攉","攥","攮","弋","忒"
            ,"甙","弑","卟","叱","叽","叩","叨","叻","吒","吖","吆","呋","呒","呓","呔","呖"
            ,"呃","吡","呗","呙","吣","吲","咂","咔","呷","呱","呤","咚","咛","咄","呶","呦"
            ,"咝","哐","咭","哂","咴","哒","咧","咦","哓","哔","呲","咣","哕","咻","咿","哌"
            ,"哙","哚","哜","咩","咪","咤","哝","哏","哞","唛","哧","唠","哽","唔","哳","唢"
            ,"唣","唏","唑","唧","唪","啧","喏","喵","啉","啭","啁","啕","唿","啐","唼"
            ,"唷","啖","啵","啶","啷","唳","唰","啜","喋","嗒","喃","喱","喹","喈","喁"
            ,"喟","啾","嗖","喑","啻","嗟","喽","喾","喔","喙","嗪","嗷","嗉","嘟","嗑","嗫"
            ,"嗬","嗔","嗦","嗝","嗄","嗯","嗥","嗲","嗳","嗌","嗍","嗨","嗵","嗤","辔","嘞"
            ,"嘈","嘌","嘁","嘤","嘣","嗾","嘀","嘧","嘭","噘","嘹","噗","嘬","噍","噢","噙"
            ,"噜","噌","噔","嚆","噤","噱","噫","噻","噼","嚅","嚓","嚯","囔","囗","囝","囡"
            ,"囵","囫","囹","囿","圄","圊","圉","圜","帏","帙","帔","帑","帱","帻","帼"
            ,"帷","幄","幔","幛","幞","幡","岌","屺","岍","岐","岖","岈","岘","岙","岑"
            ,"岚","岜","岵","岢","岽","岬","岫","岱","岣","峁","岷","峄","峒","峤","峋","峥"
            ,"崂","崃","崧","崦","崮","崤","崞","崆","崛","嵘","崾","崴","崽","嵬","嵛","嵯"
            ,"嵝","嵫","嵋","嵊","嵩","嵴","嶂","嶙","嶝","豳","嶷","巅","彳","彷","徂","徇"
            ,"徉","後","徕","徙","徜","徨","徭","徵","徼","衢","彡","犭","犰","犴","犷","犸"
            ,"狃","狁","狎","狍","狒","狨","狯","狩","狲","狴","狷","猁","狳","猃","狺"
            ,"狻","猗","猓","猡","猊","猞","猝","猕","猢","猹","猥","猬","猸","猱","獐"
            ,"獍","獗","獠","獬","獯","獾","舛","夥","飧","夤","夂","饣","饧","饨","饩","饪"
            ,"饫","饬","饴","饷","饽","馀","馄","馇","馊","馍","馐","馑","馓","馔","馕","庀"
            ,"庑","庋","庖","庥","庠","庹","庵","庾","庳","赓","廒","廑","廛","廨","廪","膺"
            ,"忄","忉","忖","忏","怃","忮","怄","忡","忤","忾","怅","怆","忪","忭","忸","怙"
            ,"怵","怦","怛","怏","怍","怩","怫","怊","怿","怡","恸","恹","恻","恺","恂"
            ,"恪","恽","悖","悚","悭","悝","悃","悒","悌","悛","惬","悻","悱","惝","惘"
            ,"惆","惚","悴","愠","愦","愕","愣","惴","愀","愎","愫","慊","慵","憬","憔","憧"
            ,"憷","懔","懵","忝","隳","闩","闫","闱","闳","闵","闶","闼","闾","阃","阄","阆"
            ,"阈","阊","阋","阌","阍","阏","阒","阕","阖","阗","阙","阚","丬","爿","戕","氵"
            ,"汔","汜","汊","沣","沅","沐","沔","沌","汨","汩","汴","汶","沆","沩","泐","泔"
            ,"沭","泷","泸","泱","泗","沲","泠","泖","泺","泫","泮","沱","泓","泯","泾"
            ,"洹","洧","洌","浃","浈","洇","洄","洙","洎","洫","浍","洮","洵","洚","浏"
            ,"浒","浔","洳","涑","浯","涞","涠","浞","涓","涔","浜","浠","浼","浣","渚","淇"
            ,"淅","淞","渎","涿","淠","渑","淦","淝","淙","渖","涫","渌","涮","渫","湮","湎"
            ,"湫","溲","湟","溆","湓","湔","渲","渥","湄","滟","溱","溘","滠","漭","滢","溥"
            ,"溧","溽","溻","溷","滗","溴","滏","溏","滂","溟","潢","潆","潇","漤","漕","滹"
            ,"漯","漶","潋","潴","漪","漉","漩","澉","澍","澌","潸","潲","潼","潺","濑"
            ,"濉","澧","澹","澶","濂","濡","濮","濞","濠","濯","瀚","瀣","瀛","瀹","瀵"
            ,"灏","灞","宀","宄","宕","宓","宥","宸","甯","骞","搴","寤","寮","褰","寰","蹇"
            ,"謇","辶","迓","迕","迥","迮","迤","迩","迦","迳","迨","逅","逄","逋","逦","逑"
            ,"逍","逖","逡","逵","逶","逭","逯","遄","遑","遒","遐","遨","遘","遢","遛","暹"
            ,"遴","遽","邂","邈","邃","邋","彐","彗","彖","彘","尻","咫","屐","屙","孱","屣"
            ,"屦","羼","弪","弩","弭","艴","弼","鬻","屮","妁","妃","妍","妩","妪","妣"
            ,"妗","姊","妫","妞","妤","姒","妲","妯","姗","妾","娅","娆","姝","娈","姣"
            ,"姘","姹","娌","娉","娲","娴","娑","娣","娓","婀","婧","婊","婕","娼","婢","婵"
            ,"胬","媪","媛","婷","婺","媾","嫫","媲","嫒","嫔","媸","嫠","嫣","嫱","嫖","嫦"
            ,"嫘","嫜","嬉","嬗","嬖","嬲","嬷","孀","尕","尜","孚","孥","孳","孑","孓","孢"
            ,"驵","驷","驸","驺","驿","驽","骀","骁","骅","骈","骊","骐","骒","骓","骖","骘"
            ,"骛","骜","骝","骟","骠","骢","骣","骥","骧","纟","纡","纣","纥","纨","纩"
            ,"纭","纰","纾","绀","绁","绂","绉","绋","绌","绐","绔","绗","绛","绠","绡"
            ,"绨","绫","绮","绯","绱","绲","缍","绶","绺","绻","绾","缁","缂","缃","缇","缈"
            ,"缋","缌","缏","缑","缒","缗","缙","缜","缛","缟","缡","缢","缣","缤","缥","缦"
            ,"缧","缪","缫","缬","缭","缯","缰","缱","缲","缳","缵","幺","畿","巛","甾","邕"
            ,"玎","玑","玮","玢","玟","珏","珂","珑","玷","玳","珀","珉","珈","珥","珙","顼"
            ,"琊","珩","珧","珞","玺","珲","琏","琪","瑛","琦","琥","琨","琰","琮","琬"
            ,"琛","琚","瑁","瑜","瑗","瑕","瑙","瑷","瑭","瑾","璜","璎","璀","璁","璇"
            ,"璋","璞","璨","璩","璐","璧","瓒","璺","韪","韫","韬","杌","杓","杞","杈","杩"
            ,"枥","枇","杪","杳","枘","枧","杵","枨","枞","枭","枋","杷","杼","柰","栉","柘"
            ,"栊","柩","枰","栌","柙","枵","柚","枳","柝","栀","柃","枸","柢","栎","柁","柽"
            ,"栲","栳","桠","桡","桎","桢","桄","桤","梃","栝","桕","桦","桁","桧","桀","栾"
            ,"桊","桉","栩","梵","梏","桴","桷","梓","桫","棂","楮","棼","椟","椠","棹"
            ,"椤","棰","椋","椁","楗","棣","椐","楱","椹","楠","楂","楝","榄","楫","榀"
            ,"榘","楸","椴","槌","榇","榈","槎","榉","楦","楣","楹","榛","榧","榻","榫","榭"
            ,"槔","榱","槁","槊","槟","榕","槠","榍","槿","樯","槭","樗","樘","橥","槲","橄"
            ,"樾","檠","橐","橛","樵","檎","橹","樽","樨","橘","橼","檑","檐","檩","檗","檫"
            ,"猷","獒","殁","殂","殇","殄","殒","殓","殍","殚","殛","殡","殪","轫","轭","轱"
            ,"轲","轳","轵","轶","轸","轷","轹","轺","轼","轾","辁","辂","辄","辇","辋"
            ,"辍","辎","辏","辘","辚","軎","戋","戗","戛","戟","戢","戡","戥","戤","戬"
            ,"臧","瓯","瓴","瓿","甏","甑","甓","攴","旮","旯","旰","昊","昙","杲","昃","昕"
            ,"昀","炅","曷","昝","昴","昱","昶","昵","耆","晟","晔","晁","晏","晖","晡","晗"
            ,"晷","暄","暌","暧","暝","暾","曛","曜","曦","曩","贲","贳","贶","贻","贽","赀"
            ,"赅","赆","赈","赉","赇","赍","赕","赙","觇","觊","觋","觌","觎","觏","觐","觑"
            ,"牮","犟","牝","牦","牯","牾","牿","犄","犋","犍","犏","犒","挈","挲","掰"
            ,"搿","擘","耄","毪","毳","毽","毵","毹","氅","氇","氆","氍","氕","氘","氙"
            ,"氚","氡","氩","氤","氪","氲","攵","敕","敫","牍","牒","牖","爰","虢","刖","肟"
            ,"肜","肓","肼","朊","肽","肱","肫","肭","肴","肷","胧","胨","胩","胪","胛","胂"
            ,"胄","胙","胍","胗","朐","胝","胫","胱","胴","胭","脍","脎","胲","胼","朕","脒"
            ,"豚","脶","脞","脬","脘","脲","腈","腌","腓","腴","腙","腚","腱","腠","腩","腼"
            ,"腽","腭","腧","塍","媵","膈","膂","膑","滕","膣","膪","臌","朦","臊","膻"
            ,"臁","膦","欤","欷","欹","歃","歆","歙","飑","飒","飓","飕","飙","飚","殳"
            ,"彀","毂","觳","斐","齑","斓","於","旆","旄","旃","旌","旎","旒","旖","炀","炜"
            ,"炖","炝","炻","烀","炷","炫","炱","烨","烊","焐","焓","焖","焯","焱","煳","煜"
            ,"煨","煅","煲","煊","煸","煺","熘","熳","熵","熨","熠","燠","燔","燧","燹","爝"
            ,"爨","灬","焘","煦","熹","戾","戽","扃","扈","扉","礻","祀","祆","祉","祛","祜"
            ,"祓","祚","祢","祗","祠","祯","祧","祺","禅","禊","禚","禧","禳","忑","忐"
            ,"怼","恝","恚","恧","恁","恙","恣","悫","愆","愍","慝","憩","憝","懋","懑"
            ,"戆","肀","聿","沓","泶","淼","矶","矸","砀","砉","砗","砘","砑","斫","砭","砜"
            ,"砝","砹","砺","砻","砟","砼","砥","砬","砣","砩","硎","硭","硖","硗","砦","硐"
            ,"硇","硌","硪","碛","碓","碚","碇","碜","碡","碣","碲","碹","碥","磔","磙","磉"
            ,"磬","磲","礅","磴","礓","礤","礞","礴","龛","黹","黻","黼","盱","眄","眍","盹"
            ,"眇","眈","眚","眢","眙","眭","眦","眵","眸","睐","睑","睇","睃","睚","睨"
            ,"睢","睥","睿","瞍","睽","瞀","瞌","瞑","瞟","瞠","瞰","瞵","瞽","町","畀"
            ,"畎","畋","畈","畛","畲","畹","疃","罘","罡","罟","詈","罨","罴","罱","罹","羁"
            ,"罾","盍","盥","蠲","钅","钆","钇","钋","钊","钌","钍","钏","钐","钔","钗","钕"
            ,"钚","钛","钜","钣","钤","钫","钪","钭","钬","钯","钰","钲","钴","钶","钷","钸"
            ,"钹","钺","钼","钽","钿","铄","铈","铉","铊","铋","铌","铍","铎","铐","铑","铒"
            ,"铕","铖","铗","铙","铘","铛","铞","铟","铠","铢","铤","铥","铧","铨","铪"
            ,"铩","铫","铮","铯","铳","铴","铵","铷","铹","铼","铽","铿","锃","锂","锆"
            ,"锇","锉","锊","锍","锎","锏","锒","锓","锔","锕","锖","锘","锛","锝","锞","锟"
            ,"锢","锪","锫","锩","锬","锱","锲","锴","锶","锷","锸","锼","锾","锿","镂","锵"
            ,"镄","镅","镆","镉","镌","镎","镏","镒","镓","镔","镖","镗","镘","镙","镛","镞"
            ,"镟","镝","镡","镢","镤","镥","镦","镧","镨","镩","镪","镫","镬","镯","镱","镲"
            ,"镳","锺","矧","矬","雉","秕","秭","秣","秫","稆","嵇","稃","稂","稞","稔"
            ,"稹","稷","穑","黏","馥","穰","皈","皎","皓","皙","皤","瓞","瓠","甬","鸠"
            ,"鸢","鸨","鸩","鸪","鸫","鸬","鸲","鸱","鸶","鸸","鸷","鸹","鸺","鸾","鹁","鹂"
            ,"鹄","鹆","鹇","鹈","鹉","鹋","鹌","鹎","鹑","鹕","鹗","鹚","鹛","鹜","鹞","鹣"
            ,"鹦","鹧","鹨","鹩","鹪","鹫","鹬","鹱","鹭","鹳","疒","疔","疖","疠","疝","疬"
            ,"疣","疳","疴","疸","痄","疱","疰","痃","痂","痖","痍","痣","痨","痦","痤","痫"
            ,"痧","瘃","痱","痼","痿","瘐","瘀","瘅","瘌","瘗","瘊","瘥","瘘","瘕","瘙"
            ,"瘛","瘼","瘢","瘠","癀","瘭","瘰","瘿","瘵","癃","瘾","瘳","癍","癞","癔"
            ,"癜","癖","癫","癯","翊","竦","穸","穹","窀","窆","窈","窕","窦","窠","窬","窨"
            ,"窭","窳","衤","衩","衲","衽","衿","袂","袢","裆","袷","袼","裉","裢","裎","裣"
            ,"裥","裱","褚","裼","裨","裾","裰","褡","褙","褓","褛","褊","褴","褫","褶","襁"
            ,"襦","襻","疋","胥","皲","皴","矜","耒","耔","耖","耜","耠","耢","耥","耦","耧"
            ,"耩","耨","耱","耋","耵","聃","聆","聍","聒","聩","聱","覃","顸","颀","颃"
            ,"颉","颌","颍","颏","颔","颚","颛","颞","颟","颡","颢","颥","颦","虍","虔"
            ,"虬","虮","虿","虺","虼","虻","蚨","蚍","蚋","蚬","蚝","蚧","蚣","蚪","蚓","蚩"
            ,"蚶","蛄","蚵","蛎","蚰","蚺","蚱","蚯","蛉","蛏","蚴","蛩","蛱","蛲","蛭","蛳"
            ,"蛐","蜓","蛞","蛴","蛟","蛘","蛑","蜃","蜇","蛸","蜈","蜊","蜍","蜉","蜣","蜻"
            ,"蜞","蜥","蜮","蜚","蜾","蝈","蜴","蜱","蜩","蜷","蜿","螂","蜢","蝽","蝾","蝻"
            ,"蝠","蝰","蝌","蝮","螋","蝓","蝣","蝼","蝤","蝙","蝥","螓","螯","螨","蟒"
            ,"蟆","螈","螅","螭","螗","螃","螫","蟥","螬","螵","螳","蟋","蟓","螽","蟑"
            ,"蟀","蟊","蟛","蟪","蟠","蟮","蠖","蠓","蟾","蠊","蠛","蠡","蠹","蠼","缶","罂"
            ,"罄","罅","舐","竺","竽","笈","笃","笄","笕","笊","笫","笏","筇","笸","笪","笙"
            ,"笮","笱","笠","笥","笤","笳","笾","笞","筘","筚","筅","筵","筌","筝","筠","筮"
            ,"筻","筢","筲","筱","箐","箦","箧","箸","箬","箝","箨","箅","箪","箜","箢","箫"
            ,"箴","篑","篁","篌","篝","篚","篥","篦","篪","簌","篾","篼","簏","簖","簋"
            ,"簟","簪","簦","簸","籁","籀","臾","舁","舂","舄","臬","衄","舡","舢","舣"
            ,"舭","舯","舨","舫","舸","舻","舳","舴","舾","艄","艉","艋","艏","艚","艟","艨"
            ,"衾","袅","袈","裘","裟","襞","羝","羟","羧","羯","羰","羲","籼","敉","粑","粝"
            ,"粜","粞","粢","粲","粼","粽","糁","糇","糌","糍","糈","糅","糗","糨","艮","暨"
            ,"羿","翎","翕","翥","翡","翦","翩","翮","翳","糸","絷","綦","綮","繇","纛","麸"
            ,"麴","赳","趄","趔","趑","趱","赧","赭","豇","豉","酊","酐","酎","酏","酤"
            ,"酢","酡","酰","酩","酯","酽","酾","酲","酴","酹","醌","醅","醐","醍","醑"
            ,"醢","醣","醪","醭","醮","醯","醵","醴","醺","豕","鹾","趸","跫","踅","蹙","蹩"
            ,"趵","趿","趼","趺","跄","跖","跗","跚","跞","跎","跏","跛","跆","跬","跷","跸"
            ,"跣","跹","跻","跤","踉","跽","踔","踝","踟","踬","踮","踣","踯","踺","蹀","踹"
            ,"踵","踽","踱","蹉","蹁","蹂","蹑","蹒","蹊","蹰","蹶","蹼","蹯","蹴","躅","躏"
            ,"躔","躐","躜","躞","豸","貂","貊","貅","貘","貔","斛","觖","觞","觚","觜"
            ,"觥","觫","觯","訾","謦","靓","雩","雳","雯","霆","霁","霈","霏","霎","霪"
            ,"霭","霰","霾","龀","龃","龅","龆","龇","龈","龉","龊","龌","黾","鼋","鼍","隹"
            ,"隼","隽","雎","雒","瞿","雠","銎","銮","鋈","錾","鍪","鏊","鎏","鐾","鑫","鱿"
            ,"鲂","鲅","鲆","鲇","鲈","稣","鲋","鲎","鲐","鲑","鲒","鲔","鲕","鲚","鲛","鲞"
            ,"鲟","鲠","鲡","鲢","鲣","鲥","鲦","鲧","鲨","鲩","鲫","鲭","鲮","鲰","鲱","鲲"
            ,"鲳","鲴","鲵","鲶","鲷","鲺","鲻","鲼","鲽","鳄","鳅","鳆","鳇","鳊","鳋"
            ,"鳌","鳍","鳎","鳏","鳐","鳓","鳔","鳕","鳗","鳘","鳙","鳜","鳝","鳟","鳢"
            ,"靼","鞅","鞑","鞒","鞔","鞯","鞫","鞣","鞲","鞴","骱","骰","骷","鹘","骶","骺"
            ,"骼","髁","髀","髅","髂","髋","髌","髑","魅","魃","魇","魉","魈","魍","魑","飨"
            ,"餍","餮","饕","饔","髟","髡","髦","髯","髫","髻","髭","髹","鬈","鬏","鬓","鬟"
            ,"鬣","麽","麾","縻","麂","麇","麈","麋","麒","鏖","麝","麟","黛","黜","黝","黠"
            ,"黟","黢","黩","黧","黥","黪","黯","鼢","鼬","鼯","鼹","鼷","鼽","鼾","齄"
        };

        /// <summary>
        /// 二级汉字对应拼音数组
        /// </summary>
        private static string[] otherPinYin = new string[]
           {                         
               "Chu","Ji","Wu","Gai","Nian","Sa","Pi","Gen","Cheng","Ge","Nao","E","Shu","Yu","Pie","Bi",
                "Tuo","Yao","Yao","Zhi","Di","Xin","Yin","Kui","Yu","Gao","Tao","Dian","Ji","Nai","Nie","Ji",
                "Qi","Mi","Bei","Se","Gu","Ze","She","Cuo","Yan","Jue","Si","Ye","Yan","Fang","Po","Gui",
                "Kui","Bian","Ze","Gua","You","Ce","Yi","Wen","Jing","Ku","Gui","Kai","La","Ji","Yan","Wan",
                "Kuai","Piao","Jue","Qiao","Huo","Yi","Tong","Wang","Dan","Ding","Zhang","Le","Sa","Yi","Mu","Ren",
                "Yu","Pi","Ya","Wa","Wu","Chang","Cang","Kang","Zhu","Ning","Ka","You","Yi","Gou","Tong","Tuo",
                "Ni","Ga","Ji","Er","You","Kua","Kan","Zhu","Yi","Tiao","Chai","Jiao","Nong","Mou","Chou","Yan",
                "Li","Qiu","Li","Yu","Ping","Yong","Si","Feng","Qian","Ruo","Pai","Zhuo","Shu","Luo","Wo","Bi",
                "Ti","Guan","Kong","Ju","Fen","Yan","Xie","Ji","Wei","Zong","Lou","Tang","Bin","Nuo","Chi","Xi",
                "Jing","Jian","Jiao","Jiu","Tong","Xuan","Dan","Tong","Tun","She","Qian","Zu","Yue","Cuan","Di","Xi",
                "Xun","Hong","Guo","Chan","Kui","Bao","Pu","Hong","Fu","Fu","Su","Si","Wen","Yan","Bo","Gun",
                "Mao","Xie","Luan","Pou","Bing","Ying","Luo","Lei","Liang","Hu","Lie","Xian","Song","Ping","Zhong","Ming",
                "Yan","Jie","Hong","Shan","Ou","Ju","Ne","Gu","He","Di","Zhao","Qu","Dai","Kuang","Lei","Gua",
                "Jie","Hui","Shen","Gou","Quan","Zheng","Hun","Xu","Qiao","Gao","Kuang","Ei","Zou","Zhuo","Wei","Yu",
                "Shen","Chan","Sui","Chen","Jian","Xue","Ye","E","Yu","Xuan","An","Di","Zi","Pian","Mo","Dang",
                "Su","Shi","Mi","Zhe","Jian","Zen","Qiao","Jue","Yan","Zhan","Chen","Dan","Jin","Zuo","Wu","Qian",
                "Jing","Ban","Yan","Zuo","Bei","Jing","Gai","Zhi","Nie","Zou","Chui","Pi","Wei","Huang","Wei","Xi",
                "Han","Qiong","Kuang","Mang","Wu","Fang","Bing","Pi","Bei","Ye","Di","Tai","Jia","Zhi","Zhu","Kuai",
                "Qie","Xun","Yun","Li","Ying","Gao","Xi","Fu","Pi","Tan","Yan","Juan","Yan","Yin","Zhang","Po",
                "Shan","Zou","Ling","Feng","Chu","Huan","Mai","Qu","Shao","He","Ge","Meng","Xu","Xie","Sou","Xie",
                "Jue","Jian","Qian","Dang","Chang","Si","Bian","Ben","Qiu","Ben","E","Fa","Shu","Ji","Yong","He",
                "Wei","Wu","Ge","Zhen","Kuang","Pi","Yi","Li","Qi","Ban","Gan","Long","Dian","Lu","Che","Di",
                "Tuo","Ni","Mu","Ao","Ya","Die","Dong","Kai","Shan","Shang","Nao","Gai","Yin","Cheng","Shi","Guo",
                "Xun","Lie","Yuan","Zhi","An","Yi","Pi","Nian","Peng","Tu","Sao","Dai","Ku","Die","Yin","Leng",
                "Hou","Ge","Yuan","Man","Yong","Liang","Chi","Xin","Pi","Yi","Cao","Jiao","Nai","Du","Qian","Ji",
                "Wan","Xiong","Qi","Xiang","Fu","Yuan","Yun","Fei","Ji","Li","E","Ju","Pi","Zhi","Rui","Xian",
                "Chang","Cong","Qin","Wu","Qian","Qi","Shan","Bian","Zhu","Kou","Yi","Mo","Gan","Pie","Long","Ba",
                "Mu","Ju","Ran","Qing","Chi","Fu","Ling","Niao","Yin","Mao","Ying","Qiong","Min","Tiao","Qian","Yi",
                "Rao","Bi","Zi","Ju","Tong","Hui","Zhu","Ting","Qiao","Fu","Ren","Xing","Quan","Hui","Xun","Ming",
                "Qi","Jiao","Chong","Jiang","Luo","Ying","Qian","Gen","Jin","Mai","Sun","Hong","Zhou","Kan","Bi","Shi",
                "Wo","You","E","Mei","You","Li","Tu","Xian","Fu","Sui","You","Di","Shen","Guan","Lang","Ying",
                "Chun","Jing","Qi","Xi","Song","Jin","Nai","Qi","Ba","Shu","Chang","Tie","Yu","Huan","Bi","Fu",
                "Tu","Dan","Cui","Yan","Zu","Dang","Jian","Wan","Ying","Gu","Han","Qia","Feng","Shen","Xiang","Wei",
                "Chan","Kai","Qi","Kui","Xi","E","Bao","Pa","Ting","Lou","Pai","Xuan","Jia","Zhen","Shi","Ru",
                "Mo","En","Bei","Weng","Hao","Ji","Li","Bang","Jian","Shuo","Lang","Ying","Yu","Su","Meng","Dou",
                "Xi","Lian","Cu","Lin","Qu","Kou","Xu","Liao","Hui","Xun","Jue","Rui","Zui","Ji","Meng","Fan",
                "Qi","Hong","Xie","Hong","Wei","Yi","Weng","Sou","Bi","Hao","Tai","Ru","Xun","Xian","Gao","Li",
                "Huo","Qu","Heng","Fan","Nie","Mi","Gong","Yi","Kuang","Lian","Da","Yi","Xi","Zang","Pao","You",
                "Liao","Ga","Gan","Ti","Men","Tuan","Chen","Fu","Pin","Niu","Jie","Jiao","Za","Yi","Lv","Jun",
                "Tian","Ye","Ai","Na","Ji","Guo","Bai","Ju","Pou","Lie","Qian","Guan","Die","Zha","Ya","Qin",
                "Yu","An","Xuan","Bing","Kui","Yuan","Shu","En","Chuai","Jian","Shuo","Zhan","Nuo","Sang","Luo","Ying",
                "Zhi","Han","Zhe","Xie","Lu","Zun","Cuan","Gan","Huan","Pi","Xing","Zhuo","Huo","Zuan","Nang","Yi",
                "Te","Dai","Shi","Bu","Chi","Ji","Kou","Dao","Le","Zha","A","Yao","Fu","Mu","Yi","Tai",
                "Li","E","Bi","Bei","Guo","Qin","Yin","Za","Ka","Ga","Gua","Ling","Dong","Ning","Duo","Nao",
                "You","Si","Kuang","Ji","Shen","Hui","Da","Lie","Yi","Xiao","Bi","Ci","Guang","Yue","Xiu","Yi",
                "Pai","Kuai","Duo","Ji","Mie","Mi","Zha","Nong","Gen","Mou","Mai","Chi","Lao","Geng","En","Zha",
                "Suo","Zao","Xi","Zuo","Ji","Feng","Ze","Nuo","Miao","Lin","Zhuan","Zhou","Tao","Hu","Cui","Sha",
                "Yo","Dan","Bo","Ding","Lang","Li","Shua","Chuo","Die","Da","Nan","Li","Kui","Jie","Yong","Kui",
                "Jiu","Sou","Yin","Chi","Jie","Lou","Ku","Wo","Hui","Qin","Ao","Su","Du","Ke","Nie","He",
                "Chen","Suo","Ge","A","En","Hao","Dia","Ai","Ai","Suo","Hei","Tong","Chi","Pei","Lei","Cao",
                "Piao","Qi","Ying","Beng","Sou","Di","Mi","Peng","Jue","Liao","Pu","Chuai","Jiao","O","Qin","Lu",
                "Ceng","Deng","Hao","Jin","Jue","Yi","Sai","Pi","Ru","Cha","Huo","Nang","Wei","Jian","Nan","Lun",
                "Hu","Ling","You","Yu","Qing","Yu","Huan","Wei","Zhi","Pei","Tang","Dao","Ze","Guo","Wei","Wo",
                "Man","Zhang","Fu","Fan","Ji","Qi","Qian","Qi","Qu","Ya","Xian","Ao","Cen","Lan","Ba","Hu",
                "Ke","Dong","Jia","Xiu","Dai","Gou","Mao","Min","Yi","Dong","Qiao","Xun","Zheng","Lao","Lai","Song",
                "Yan","Gu","Xiao","Guo","Kong","Jue","Rong","Yao","Wai","Zai","Wei","Yu","Cuo","Lou","Zi","Mei",
                "Sheng","Song","Ji","Zhang","Lin","Deng","Bin","Yi","Dian","Chi","Pang","Cu","Xun","Yang","Hou","Lai",
                "Xi","Chang","Huang","Yao","Zheng","Jiao","Qu","San","Fan","Qiu","An","Guang","Ma","Niu","Yun","Xia",
                "Pao","Fei","Rong","Kuai","Shou","Sun","Bi","Juan","Li","Yu","Xian","Yin","Suan","Yi","Guo","Luo",
                "Ni","She","Cu","Mi","Hu","Cha","Wei","Wei","Mei","Nao","Zhang","Jing","Jue","Liao","Xie","Xun",
                "Huan","Chuan","Huo","Sun","Yin","Dong","Shi","Tang","Tun","Xi","Ren","Yu","Chi","Yi","Xiang","Bo",
                "Yu","Hun","Zha","Sou","Mo","Xiu","Jin","San","Zhuan","Nang","Pi","Wu","Gui","Pao","Xiu","Xiang",
                "Tuo","An","Yu","Bi","Geng","Ao","Jin","Chan","Xie","Lin","Ying","Shu","Dao","Cun","Chan","Wu",
                "Zhi","Ou","Chong","Wu","Kai","Chang","Chuang","Song","Bian","Niu","Hu","Chu","Peng","Da","Yang","Zuo",
                "Ni","Fu","Chao","Yi","Yi","Tong","Yan","Ce","Kai","Xun","Ke","Yun","Bei","Song","Qian","Kui",
                "Kun","Yi","Ti","Quan","Qie","Xing","Fei","Chang","Wang","Chou","Hu","Cui","Yun","Kui","E","Leng",
                "Zhui","Qiao","Bi","Su","Qie","Yong","Jing","Qiao","Chong","Chu","Lin","Meng","Tian","Hui","Shuan","Yan",
                "Wei","Hong","Min","Kang","Ta","Lv","Kun","Jiu","Lang","Yu","Chang","Xi","Wen","Hun","E","Qu",
                "Que","He","Tian","Que","Kan","Jiang","Pan","Qiang","San","Qi","Si","Cha","Feng","Yuan","Mu","Mian",
                "Dun","Mi","Gu","Bian","Wen","Hang","Wei","Le","Gan","Shu","Long","Lu","Yang","Si","Duo","Ling",
                "Mao","Luo","Xuan","Pan","Duo","Hong","Min","Jing","Huan","Wei","Lie","Jia","Zhen","Yin","Hui","Zhu",
                "Ji","Xu","Hui","Tao","Xun","Jiang","Liu","Hu","Xun","Ru","Su","Wu","Lai","Wei","Zhuo","Juan",
                "Cen","Bang","Xi","Mei","Huan","Zhu","Qi","Xi","Song","Du","Zhuo","Pei","Mian","Gan","Fei","Cong",
                "Shen","Guan","Lu","Shuan","Xie","Yan","Mian","Qiu","Sou","Huang","Xu","Pen","Jian","Xuan","Wo","Mei",
                "Yan","Qin","Ke","She","Mang","Ying","Pu","Li","Ru","Ta","Hun","Bi","Xiu","Fu","Tang","Pang",
                "Ming","Huang","Ying","Xiao","Lan","Cao","Hu","Luo","Huan","Lian","Zhu","Yi","Lu","Xuan","Gan","Shu",
                "Si","Shan","Shao","Tong","Chan","Lai","Sui","Li","Dan","Chan","Lian","Ru","Pu","Bi","Hao","Zhuo",
                "Han","Xie","Ying","Yue","Fen","Hao","Ba","Bao","Gui","Dang","Mi","You","Chen","Ning","Jian","Qian",
                "Wu","Liao","Qian","Huan","Jian","Jian","Zou","Ya","Wu","Jiong","Ze","Yi","Er","Jia","Jing","Dai",
                "Hou","Pang","Bu","Li","Qiu","Xiao","Ti","Qun","Kui","Wei","Huan","Lu","Chuan","Huang","Qiu","Xia",
                "Ao","Gou","Ta","Liu","Xian","Lin","Ju","Xie","Miao","Sui","La","Ji","Hui","Tuan","Zhi","Kao",
                "Zhi","Ji","E","Chan","Xi","Ju","Chan","Jing","Nu","Mi","Fu","Bi","Yu","Che","Shuo","Fei",
                "Yan","Wu","Yu","Bi","Jin","Zi","Gui","Niu","Yu","Si","Da","Zhou","Shan","Qie","Ya","Rao",
                "Shu","Luan","Jiao","Pin","Cha","Li","Ping","Wa","Xian","Suo","Di","Wei","E","Jing","Biao","Jie",
                "Chang","Bi","Chan","Nu","Ao","Yuan","Ting","Wu","Gou","Mo","Pi","Ai","Pin","Chi","Li","Yan",
                "Qiang","Piao","Chang","Lei","Zhang","Xi","Shan","Bi","Niao","Mo","Shuang","Ga","Ga","Fu","Nu","Zi",
                "Jie","Jue","Bao","Zang","Si","Fu","Zou","Yi","Nu","Dai","Xiao","Hua","Pian","Li","Qi","Ke",
                "Zhui","Can","Zhi","Wu","Ao","Liu","Shan","Biao","Cong","Chan","Ji","Xiang","Jiao","Yu","Zhou","Ge",
                "Wan","Kuang","Yun","Pi","Shu","Gan","Xie","Fu","Zhou","Fu","Chu","Dai","Ku","Hang","Jiang","Geng",
                "Xiao","Ti","Ling","Qi","Fei","Shang","Gun","Duo","Shou","Liu","Quan","Wan","Zi","Ke","Xiang","Ti",
                "Miao","Hui","Si","Bian","Gou","Zhui","Min","Jin","Zhen","Ru","Gao","Li","Yi","Jian","Bin","Piao",
                "Man","Lei","Miao","Sao","Xie","Liao","Zeng","Jiang","Qian","Qiao","Huan","Zuan","Yao","Ji","Chuan","Zai",
                "Yong","Ding","Ji","Wei","Bin","Min","Jue","Ke","Long","Dian","Dai","Po","Min","Jia","Er","Gong",
                "Xu","Ya","Heng","Yao","Luo","Xi","Hui","Lian","Qi","Ying","Qi","Hu","Kun","Yan","Cong","Wan",
                "Chen","Ju","Mao","Yu","Yuan","Xia","Nao","Ai","Tang","Jin","Huang","Ying","Cui","Cong","Xuan","Zhang",
                "Pu","Can","Qu","Lu","Bi","Zan","Wen","Wei","Yun","Tao","Wu","Shao","Qi","Cha","Ma","Li",
                "Pi","Miao","Yao","Rui","Jian","Chu","Cheng","Cong","Xiao","Fang","Pa","Zhu","Nai","Zhi","Zhe","Long",
                "Jiu","Ping","Lu","Xia","Xiao","You","Zhi","Tuo","Zhi","Ling","Gou","Di","Li","Tuo","Cheng","Kao",
                "Lao","Ya","Rao","Zhi","Zhen","Guang","Qi","Ting","Gua","Jiu","Hua","Heng","Gui","Jie","Luan","Juan",
                "An","Xu","Fan","Gu","Fu","Jue","Zi","Suo","Ling","Chu","Fen","Du","Qian","Zhao","Luo","Chui",
                "Liang","Guo","Jian","Di","Ju","Cou","Zhen","Nan","Zha","Lian","Lan","Ji","Pin","Ju","Qiu","Duan",
                "Chui","Chen","Lv","Cha","Ju","Xuan","Mei","Ying","Zhen","Fei","Ta","Sun","Xie","Gao","Cui","Gao",
                "Shuo","Bin","Rong","Zhu","Xie","Jin","Qiang","Qi","Chu","Tang","Zhu","Hu","Gan","Yue","Qing","Tuo",
                "Jue","Qiao","Qin","Lu","Zun","Xi","Ju","Yuan","Lei","Yan","Lin","Bo","Cha","You","Ao","Mo",
                "Cu","Shang","Tian","Yun","Lian","Piao","Dan","Ji","Bin","Yi","Ren","E","Gu","Ke","Lu","Zhi",
                "Yi","Zhen","Hu","Li","Yao","Shi","Zhi","Quan","Lu","Zhe","Nian","Wang","Chuo","Zi","Cou","Lu",
                "Lin","Wei","Jian","Qiang","Jia","Ji","Ji","Kan","Deng","Gai","Jian","Zang","Ou","Ling","Bu","Beng",
                "Zeng","Pi","Po","Ga","La","Gan","Hao","Tan","Gao","Ze","Xin","Yun","Gui","He","Zan","Mao",
                "Yu","Chang","Ni","Qi","Sheng","Ye","Chao","Yan","Hui","Bu","Han","Gui","Xuan","Kui","Ai","Ming",
                "Tun","Xun","Yao","Xi","Nang","Ben","Shi","Kuang","Yi","Zhi","Zi","Gai","Jin","Zhen","Lai","Qiu",
                "Ji","Dan","Fu","Chan","Ji","Xi","Di","Yu","Gou","Jin","Qu","Jian","Jiang","Pin","Mao","Gu",
                "Wu","Gu","Ji","Ju","Jian","Pian","Kao","Qie","Suo","Bai","Ge","Bo","Mao","Mu","Cui","Jian",
                "San","Shu","Chang","Lu","Pu","Qu","Pie","Dao","Xian","Chuan","Dong","Ya","Yin","Ke","Yun","Fan",
                "Chi","Jiao","Du","Die","You","Yuan","Guo","Yue","Wo","Rong","Huang","Jing","Ruan","Tai","Gong","Zhun",
                "Na","Yao","Qian","Long","Dong","Ka","Lu","Jia","Shen","Zhou","Zuo","Gua","Zhen","Qu","Zhi","Jing",
                "Guang","Dong","Yan","Kuai","Sa","Hai","Pian","Zhen","Mi","Tun","Luo","Cuo","Pao","Wan","Niao","Jing",
                "Yan","Fei","Yu","Zong","Ding","Jian","Cou","Nan","Mian","Wa","E","Shu","Cheng","Ying","Ge","Lv",
                "Bin","Teng","Zhi","Chuai","Gu","Meng","Sao","Shan","Lian","Lin","Yu","Xi","Qi","Sha","Xin","Xi",
                "Biao","Sa","Ju","Sou","Biao","Biao","Shu","Gou","Gu","Hu","Fei","Ji","Lan","Yu","Pei","Mao",
                "Zhan","Jing","Ni","Liu","Yi","Yang","Wei","Dun","Qiang","Shi","Hu","Zhu","Xuan","Tai","Ye","Yang",
                "Wu","Han","Men","Chao","Yan","Hu","Yu","Wei","Duan","Bao","Xuan","Bian","Tui","Liu","Man","Shang",
                "Yun","Yi","Yu","Fan","Sui","Xian","Jue","Cuan","Huo","Tao","Xu","Xi","Li","Hu","Jiong","Hu",
                "Fei","Shi","Si","Xian","Zhi","Qu","Hu","Fu","Zuo","Mi","Zhi","Ci","Zhen","Tiao","Qi","Chan",
                "Xi","Zhuo","Xi","Rang","Te","Tan","Dui","Jia","Hui","Nv","Nin","Yang","Zi","Que","Qian","Min",
                "Te","Qi","Dui","Mao","Men","Gang","Yu","Yu","Ta","Xue","Miao","Ji","Gan","Dang","Hua","Che",
                "Dun","Ya","Zhuo","Bian","Feng","Fa","Ai","Li","Long","Zha","Tong","Di","La","Tuo","Fu","Xing",
                "Mang","Xia","Qiao","Zhai","Dong","Nao","Ge","Wo","Qi","Dui","Bei","Ding","Chen","Zhou","Jie","Di",
                "Xuan","Bian","Zhe","Gun","Sang","Qing","Qu","Dun","Deng","Jiang","Ca","Meng","Bo","Kan","Zhi","Fu",
                "Fu","Xu","Mian","Kou","Dun","Miao","Dan","Sheng","Yuan","Yi","Sui","Zi","Chi","Mou","Lai","Jian",
                "Di","Suo","Ya","Ni","Sui","Pi","Rui","Sou","Kui","Mao","Ke","Ming","Piao","Cheng","Kan","Lin",
                "Gu","Ding","Bi","Quan","Tian","Fan","Zhen","She","Wan","Tuan","Fu","Gang","Gu","Li","Yan","Pi",
                "Lan","Li","Ji","Zeng","He","Guan","Juan","Jin","Ga","Yi","Po","Zhao","Liao","Tu","Chuan","Shan",
                "Men","Chai","Nv","Bu","Tai","Ju","Ban","Qian","Fang","Kang","Dou","Huo","Ba","Yu","Zheng","Gu",
                "Ke","Po","Bu","Bo","Yue","Mu","Tan","Dian","Shuo","Shi","Xuan","Ta","Bi","Ni","Pi","Duo",
                "Kao","Lao","Er","You","Cheng","Jia","Nao","Ye","Cheng","Diao","Yin","Kai","Zhu","Ding","Diu","Hua",
                "Quan","Ha","Sha","Diao","Zheng","Se","Chong","Tang","An","Ru","Lao","Lai","Te","Keng","Zeng","Li",
                "Gao","E","Cuo","Lve","Liu","Kai","Jian","Lang","Qin","Ju","A","Qiang","Nuo","Ben","De","Ke",
                "Kun","Gu","Huo","Pei","Juan","Tan","Zi","Qie","Kai","Si","E","Cha","Sou","Huan","Ai","Lou",
                "Qiang","Fei","Mei","Mo","Ge","Juan","Na","Liu","Yi","Jia","Bin","Biao","Tang","Man","Luo","Yong",
                "Chuo","Xuan","Di","Tan","Jue","Pu","Lu","Dui","Lan","Pu","Cuan","Qiang","Deng","Huo","Zhuo","Yi",
                "Cha","Biao","Zhong","Shen","Cuo","Zhi","Bi","Zi","Mo","Shu","Lv","Ji","Fu","Lang","Ke","Ren",
                "Zhen","Ji","Se","Nian","Fu","Rang","Gui","Jiao","Hao","Xi","Po","Die","Hu","Yong","Jiu","Yuan",
                "Bao","Zhen","Gu","Dong","Lu","Qu","Chi","Si","Er","Zhi","Gua","Xiu","Luan","Bo","Li","Hu",
                "Yu","Xian","Ti","Wu","Miao","An","Bei","Chun","Hu","E","Ci","Mei","Wu","Yao","Jian","Ying",
                "Zhe","Liu","Liao","Jiao","Jiu","Yu","Hu","Lu","Guan","Bing","Ding","Jie","Li","Shan","Li","You",
                "Gan","Ke","Da","Zha","Pao","Zhu","Xuan","Jia","Ya","Yi","Zhi","Lao","Wu","Cuo","Xian","Sha",
                "Zhu","Fei","Gu","Wei","Yu","Yu","Dan","La","Yi","Hou","Chai","Lou","Jia","Sao","Chi","Mo",
                "Ban","Ji","Huang","Biao","Luo","Ying","Zhai","Long","Yin","Chou","Ban","Lai","Yi","Dian","Pi","Dian",
                "Qu","Yi","Song","Xi","Qiong","Zhun","Bian","Yao","Tiao","Dou","Ke","Yu","Xun","Ju","Yu","Yi",
                "Cha","Na","Ren","Jin","Mei","Pan","Dang","Jia","Ge","Ken","Lian","Cheng","Lian","Jian","Biao","Chu",
                "Ti","Bi","Ju","Duo","Da","Bei","Bao","Lv","Bian","Lan","Chi","Zhe","Qiang","Ru","Pan","Ya",
                "Xu","Jun","Cun","Jin","Lei","Zi","Chao","Si","Huo","Lao","Tang","Ou","Lou","Jiang","Nou","Mo",
                "Die","Ding","Dan","Ling","Ning","Guo","Kui","Ao","Qin","Han","Qi","Hang","Jie","He","Ying","Ke",
                "Han","E","Zhuan","Nie","Man","Sang","Hao","Ru","Pin","Hu","Qian","Qiu","Ji","Chai","Hui","Ge",
                "Meng","Fu","Pi","Rui","Xian","Hao","Jie","Gong","Dou","Yin","Chi","Han","Gu","Ke","Li","You",
                "Ran","Zha","Qiu","Ling","Cheng","You","Qiong","Jia","Nao","Zhi","Si","Qu","Ting","Kuo","Qi","Jiao",
                "Yang","Mou","Shen","Zhe","Shao","Wu","Li","Chu","Fu","Qiang","Qing","Qi","Xi","Yu","Fei","Guo",
                "Guo","Yi","Pi","Tiao","Quan","Wan","Lang","Meng","Chun","Rong","Nan","Fu","Kui","Ke","Fu","Sou",
                "Yu","You","Lou","You","Bian","Mou","Qin","Ao","Man","Mang","Ma","Yuan","Xi","Chi","Tang","Pang",
                "Shi","Huang","Cao","Piao","Tang","Xi","Xiang","Zhong","Zhang","Shuai","Mao","Peng","Hui","Pan","Shan","Huo",
                "Meng","Chan","Lian","Mie","Li","Du","Qu","Fou","Ying","Qing","Xia","Shi","Zhu","Yu","Ji","Du",
                "Ji","Jian","Zhao","Zi","Hu","Qiong","Po","Da","Sheng","Ze","Gou","Li","Si","Tiao","Jia","Bian",
                "Chi","Kou","Bi","Xian","Yan","Quan","Zheng","Jun","Shi","Gang","Pa","Shao","Xiao","Qing","Ze","Qie",
                "Zhu","Ruo","Qian","Tuo","Bi","Dan","Kong","Wan","Xiao","Zhen","Kui","Huang","Hou","Gou","Fei","Li",
                "Bi","Chi","Su","Mie","Dou","Lu","Duan","Gui","Dian","Zan","Deng","Bo","Lai","Zhou","Yu","Yu",
                "Chong","Xi","Nie","Nv","Chuan","Shan","Yi","Bi","Zhong","Ban","Fang","Ge","Lu","Zhu","Ze","Xi",
                "Shao","Wei","Meng","Shou","Cao","Chong","Meng","Qin","Niao","Jia","Qiu","Sha","Bi","Di","Qiang","Suo",
                "Jie","Tang","Xi","Xian","Mi","Ba","Li","Tiao","Xi","Zi","Can","Lin","Zong","San","Hou","Zan",
                "Ci","Xu","Rou","Qiu","Jiang","Gen","Ji","Yi","Ling","Xi","Zhu","Fei","Jian","Pian","He","Yi",
                "Jiao","Zhi","Qi","Qi","Yao","Dao","Fu","Qu","Jiu","Ju","Lie","Zi","Zan","Nan","Zhe","Jiang",
                "Chi","Ding","Gan","Zhou","Yi","Gu","Zuo","Tuo","Xian","Ming","Zhi","Yan","Shai","Cheng","Tu","Lei",
                "Kun","Pei","Hu","Ti","Xu","Hai","Tang","Lao","Bu","Jiao","Xi","Ju","Li","Xun","Shi","Cuo",
                "Dun","Qiong","Xue","Cu","Bie","Bo","Ta","Jian","Fu","Qiang","Zhi","Fu","Shan","Li","Tuo","Jia",
                "Bo","Tai","Kui","Qiao","Bi","Xian","Xian","Ji","Jiao","Liang","Ji","Chuo","Huai","Chi","Zhi","Dian",
                "Bo","Zhi","Jian","Die","Chuai","Zhong","Ju","Duo","Cuo","Pian","Rou","Nie","Pan","Qi","Chu","Jue",
                "Pu","Fan","Cu","Zhu","Lin","Chan","Lie","Zuan","Xie","Zhi","Diao","Mo","Xiu","Mo","Pi","Hu",
                "Jue","Shang","Gu","Zi","Gong","Su","Zhi","Zi","Qing","Liang","Yu","Li","Wen","Ting","Ji","Pei",
                "Fei","Sha","Yin","Ai","Xian","Mai","Chen","Ju","Bao","Tiao","Zi","Yin","Yu","Chuo","Wo","Mian",
                "Yuan","Tuo","Zhui","Sun","Jun","Ju","Luo","Qu","Chou","Qiong","Luan","Wu","Zan","Mou","Ao","Liu",
                "Bei","Xin","You","Fang","Ba","Ping","Nian","Lu","Su","Fu","Hou","Tai","Gui","Jie","Wei","Er",
                "Ji","Jiao","Xiang","Xun","Geng","Li","Lian","Jian","Shi","Tiao","Gun","Sha","Huan","Ji","Qing","Ling",
                "Zou","Fei","Kun","Chang","Gu","Ni","Nian","Diao","Shi","Zi","Fen","Die","E","Qiu","Fu","Huang",
                "Bian","Sao","Ao","Qi","Ta","Guan","Yao","Le","Biao","Xue","Man","Min","Yong","Gui","Shan","Zun",
                "Li","Da","Yang","Da","Qiao","Man","Jian","Ju","Rou","Gou","Bei","Jie","Tou","Ku","Gu","Di",
                "Hou","Ge","Ke","Bi","Lou","Qia","Kuan","Bin","Du","Mei","Ba","Yan","Liang","Xiao","Wang","Chi",
                "Xiang","Yan","Tie","Tao","Yong","Biao","Kun","Mao","Ran","Tiao","Ji","Zi","Xiu","Quan","Jiu","Bin",
                "Huan","Lie","Me","Hui","Mi","Ji","Jun","Zhu","Mi","Qi","Ao","She","Lin","Dai","Chu","You",
                "Xia","Yi","Qu","Du","Li","Qing","Can","An","Fen","You","Wu","Yan","Xi","Qiu","Han","Zha"
           };
        #endregion 二级汉字
        #region 变量定义
        // GB2312-80 标准规范中第一个汉字的机内码.即"啊"的机内码
        private const int firstChCode = -20319;
        // GB2312-80 标准规范中最后一个汉字的机内码.即"齄"的机内码
        private const int lastChCode = -2050;
        // GB2312-80 标准规范中最后一个一级汉字的机内码.即"座"的机内码
        private const int lastOfOneLevelChCode = -10247;
        // 配置中文字符
        //static Regex regex = new Regex("[\u4e00-\u9fa5]$");

        #endregion
        #endregion

        /// <summary>
        /// 取拼音第一个字段
        /// </summary>        
        /// <param name="ch"></param>        
        /// <returns></returns>        
         public static String GetFirst(Char ch)   
         {
             var rs = Get(ch);  
             if (!String.IsNullOrEmpty(rs)) rs = rs.Substring(0, 1); 
                             
             return rs;   
         }

        /// <summary>
        /// 取拼音第一个字段
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
         public static String GetFirst(String str)
         {
             if (String.IsNullOrEmpty(str)) return String.Empty; 

             var sb = new StringBuilder(str.Length + 1); 
             var chs = str.ToCharArray(); 

             for (var i = 0; i < chs.Length; i++) 
             { 
                 sb.Append(GetFirst(chs[i]));
             } 
             
             return sb.ToString();
         }
        
        /// <summary>
         /// 获取单字拼音
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
         public static String Get(Char ch)
         {
             // 拉丁字符            
             if (ch <= '\x00FF') return ch.ToString();

             // 标点符号、分隔符            
             if (Char.IsPunctuation(ch) || Char.IsSeparator(ch)) return ch.ToString();

             // 非中文字符            
             if (ch < '\x4E00' || ch > '\x9FA5') return ch.ToString();

             var arr = Encoding.GetEncoding("gb2312").GetBytes(ch.ToString());
             //Encoding.Default默认在中文环境里虽是GB2312，但在多变的环境可能是其它
             //var arr = Encoding.Default.GetBytes(ch.ToString()); 
             var chr = (Int16)arr[0] * 256 + (Int16)arr[1] - 65536;

             //***// 单字符--英文或半角字符  
             if (chr > 0 && chr < 160) return ch.ToString();
             #region 中文字符处理

             // 判断是否超过GB2312-80标准中的汉字范围
             if (chr > lastChCode || chr < firstChCode)
             {
                 return ch.ToString();;
             }
             // 如果是在一级汉字中
             else if (chr <= lastOfOneLevelChCode)
             {
                 // 将一级汉字分为12块,每块33个汉字.
                 for (int aPos = 11; aPos >= 0; aPos--)
                 {
                     int aboutPos = aPos * 33;
                     // 从最后的块开始扫描,如果机内码大于块的第一个机内码,说明在此块中
                     if (chr >= pyValue[aboutPos])
                     {
                         // Console.WriteLine("存在于第 " + aPos.ToString() + " 块,此块的第一个机内码是: " + pyValue[aPos * 33].ToString());
                         // 遍历块中的每个音节机内码,从最后的音节机内码开始扫描,
                         // 如果音节内码小于机内码,则取此音节
                         for (int i = aboutPos + 32; i >= aboutPos; i--)
                         {
                             if (pyValue[i] <= chr)
                             {
                                 // Console.WriteLine("找到第一个小于要查找机内码的机内码: " + pyValue[i].ToString());
                                 return pyName[i];
                             }
                         }
                         break;
                     }
                 }
             }
             // 如果是在二级汉字中
             else
             {
                 int pos = Array.IndexOf(otherChinese, ch.ToString());
                 if (pos != decimal.MinusOne)
                 {
                     return otherPinYin[pos];
                 }
             }
             #endregion 中文字符处理

             //if (chr < -20319 || chr > -10247) { // 不知道的字符  
             //    return null;  
         
             //for (var i = pyValue.Length - 1; i >= 0; i--)
             //{                
             //    if (pyValue[i] <= chr) return pyName[i];//这只能对应数组已经定义的           
             //}             
             
             return String.Empty;
         }

        /// <summary>
         /// 把汉字转换成拼音(全拼)
        /// </summary>
         /// <param name="str">汉字字符串</param>
         /// <returns>转换后的拼音(全拼)字符串</returns>
         public static String GetPinYin(String str)
         {
             if (String.IsNullOrEmpty(str)) return String.Empty; 
             
             var sb = new StringBuilder(str.Length * 10); 
             var chs = str.ToCharArray(); 
             
             for (var j = 0; j < chs.Length; j++) 
             { 
                 sb.Append(Get(chs[j])); 
             } 
             
             return sb.ToString();
         }
        #endregion

        #region  获取网页的HTML内容
         // 获取网页的HTML内容，指定Encoding
        public static string GetHtml(string url, Encoding encoding)
         {
             byte[] buf = new WebClient().DownloadData(url);
             if (encoding != null) return encoding.GetString(buf);
             string html = Encoding.UTF8.GetString(buf);
             encoding = GetEncoding(html);
             if (encoding == null || encoding == Encoding.UTF8) return html;
             return encoding.GetString(buf);
         }
         // 根据网页的HTML内容提取网页的Encoding
        public static Encoding GetEncoding(string html)
         {
             string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
             string charset = Regex.Match(html, pattern).Groups["charset"].Value;
             try { return Encoding.GetEncoding(charset); }
             catch (ArgumentException) { return null; }
         }
        #endregion
    }
}
