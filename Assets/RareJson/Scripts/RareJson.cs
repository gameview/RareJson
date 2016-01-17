using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rare
{
    // json值 string 数值(int,float) bool(true,false) null
    public enum JsonValueType
    {
        JsonValueType_Null = 0,
        JsonValueType_String,
        JsonValueType_Int,
        JsonValueType_Float,
        JsonValueType_Double,
        JsonValueType_Bool,
        JsonValueType_Object,
        JsonValueType_Array,
    }

    public class JsonNode
    {
        public JsonValueType m_eType = JsonValueType.JsonValueType_Null;
        public string m_Data = "null";    // 对象值

        protected static string GenerateSpace(int nNum)
        {
            string strSpace = "";
            for (int i = 0; i < nNum; ++i)
            {
                strSpace += " ";
            }

            return strSpace;
        }

        public string Value
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public JsonValueType Type
        {
            get { return m_eType;  }
        }

        public bool IsNull()
        {
            return Type == JsonValueType.JsonValueType_Null;
        }

        public JsonNode()
        {
            m_eType = JsonValueType.JsonValueType_Null;
            m_Data = "null";
        }

        public JsonNode(string aData)
        {
            m_eType = JsonValueType.JsonValueType_String;
            m_Data = aData;
        }
        public JsonNode(float aData)
        {
            m_eType = JsonValueType.JsonValueType_Float;
            AsFloat = aData;
        }
        public JsonNode(double aData)
        {
            m_eType = JsonValueType.JsonValueType_Double;
            AsDouble = aData;
        }
        public JsonNode(bool aData)
        {
            m_eType = JsonValueType.JsonValueType_Bool;
            AsBool = aData;
        }
        public JsonNode(int aData)
        {
            m_eType = JsonValueType.JsonValueType_Int;
            AsInt = aData;
        }

        // 类型隐式转换
        static public implicit operator JsonNode(string v) { return new JsonNode(v); }
        static public implicit operator JsonNode(float v) { return new JsonNode(v); }
        static public implicit operator JsonNode(double v) { return new JsonNode(v); }
        static public implicit operator JsonNode(bool v) { return new JsonNode(v); }
        static public implicit operator JsonNode(int v) { return new JsonNode(v); }

        static public implicit operator string(JsonNode n) { return n.Value; }
        static public implicit operator float(JsonNode n) { return n.AsFloat; }
        static public implicit operator double(JsonNode n) { return n.AsDouble; }
        static public implicit operator bool(JsonNode n) { return n.AsBool; }
        static public implicit operator int(JsonNode n) { return n.AsInt; }

        public virtual JsonNode this[int nIndex] { get { return null; } set { } }
        public virtual JsonNode this[string strKey] { get { return null; } set { } }
        public virtual int Count { get { return 0; } }

        public virtual void Add(string strKey, JsonNode value) { }
        public virtual void Add(int nIndex, JsonNode value) { }
        public virtual void Add(JsonNode value) { }

        public virtual void Remove(string strKey) { }
        public virtual void Remove(int nIndex) { }

        public virtual JsonArray AsArray
        {
            get
            {
                return this as JsonArray;
            }
        }
        public virtual JsonObject AsObject
        {
            get
            {
                return this as JsonObject;
            }
        }

        public virtual void Serialize(StreamWriter writer,int nLevel = -1, bool bSpace = false)
        {
            if( this.Type == JsonValueType.JsonValueType_Object || this.Type == JsonValueType.JsonValueType_Array )
            {
                return;
            }

            if (this.Type == JsonValueType.JsonValueType_String)
            {
                if (nLevel == -1 || !bSpace )
                {
                    writer.Write("\"{0}\"", Value);
                }
                else
                {
                    writer.Write("{0}\"{1}\"", JsonNode.GenerateSpace(nLevel * 2), Value);
                }
            }
            else
            {
                if (nLevel == -1 || !bSpace)
                {
                    writer.Write(Value);
                }
                else
                {
                    writer.Write("{0}\"{1}\"", JsonNode.GenerateSpace(nLevel * 2), Value);
                }
            }
        }

        private int AsInt
        {
            get
            {
                int v = 0;
                if (int.TryParse(Value, out v))
                    return v;
                return 0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        private float AsFloat
        {
            get
            {
                float v = 0.0f;
                if (float.TryParse(Value, out v))
                    return v;
                return 0.0f;
            }
            set
            {
                Value = value.ToString();
            }
        }
        private double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        private bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }
    }

    // json对象
    public class JsonObject : JsonNode
    {
        private Dictionary<string, JsonNode> m_dicValus = new Dictionary<string,JsonNode>();    // 值列表，无序 

        public JsonObject()
        {
            m_eType = JsonValueType.JsonValueType_Object;
        }
        public override JsonNode this[string strKey]
        {
            get
            {
                if (m_dicValus.ContainsKey(strKey))
                {
                    return m_dicValus[strKey];
                }
                else
                {
                    return new JsonNode();
                }
            }
            set
            {
                if (m_dicValus.ContainsKey(strKey))
                    m_dicValus[strKey] = value;
                else
                    m_dicValus.Add(strKey,value);
            }
        }
        public override int Count
        {
            get { return m_dicValus.Count; }
        }


        public override void Add(string strKey,JsonNode value) 
        {
            this[strKey] = value;
        }

        public override void Remove(string strKey) 
        {
            m_dicValus.Remove(strKey);
        }

        public override void Serialize(StreamWriter writer, int nLevel = -1, bool bSpace = false)
        {
            if( m_dicValus.Count > 0 )
            {
                if (nLevel == -1)
                {
                    writer.Write("{");

                    int nIndex = 0;
                    foreach (KeyValuePair<string, JsonNode> v in m_dicValus)
                    {
                        writer.Write("\"{0}\":", new object[]{v.Key});
                        v.Value.Serialize(writer);
                        if (nIndex != m_dicValus.Count - 1)
                        {
                            writer.Write(",");
                        }

                        nIndex++;
                    }

                    writer.Write("}");
                }
                else
                {
                    string strSpace = JsonNode.GenerateSpace(nLevel * 2);
                    if (bSpace)
                    {
                        writer.Write(strSpace);
                    }
                    writer.Write("{\n");

                    string strSubSpace = JsonNode.GenerateSpace((nLevel+1) * 2);
                    int nIndex = 0;
                    foreach (KeyValuePair<string, JsonNode> v in m_dicValus)
                    {

                        writer.Write("{0}\"{1}\":", new object[]{strSubSpace, v.Key});
                        v.Value.Serialize(writer,nLevel+1);
                        if (nIndex != m_dicValus.Count - 1)
                        {
                            writer.Write(",\n");
                        }
                        else
                        {
                            writer.Write("\n");
                        }

                        nIndex++;
                    }

                    writer.Write(strSpace);
                    writer.Write("}");
                }
            }
        }
    }

    // json数组
    public class JsonArray : JsonNode
    {
        private List<JsonNode> m_lstValue = new List<JsonNode>();    // 数组序列，有序值

        public JsonArray()
        {
            m_eType = JsonValueType.JsonValueType_Array;
        }

        public override JsonNode this[int nIndex]
        {
            get
            {
                if (nIndex < 0 || nIndex >= m_lstValue.Count)
                {
                    return new JsonNode();
                }
                return m_lstValue[nIndex];
            }
            set
            {
                if (nIndex < 0 || nIndex >= m_lstValue.Count)
                {
                    m_lstValue.Add(value);
                }
                else
                {
                    m_lstValue[nIndex] = value;
                }
            }
        }

        public override int Count
        {
            get { return m_lstValue.Count; }
        }

        public override void Add(int nIndex, JsonNode value)
        {
            this[nIndex] = value;
        }

        public override void Add(JsonNode value)
        {
            m_lstValue.Add(value);
        }

        public override void Remove(int nIndex)
        {
            m_lstValue.RemoveAt(nIndex);
        }

        public override void Serialize(StreamWriter writer, int nLevel = -1, bool bSpace = false)
        {
            if (m_lstValue.Count > 0)
            {
                if( nLevel == -1 )
                {
                    writer.Write("[");

                    for (int i = 0; i < m_lstValue.Count; ++i)
                    {
                        m_lstValue[i].Serialize(writer);
                        if (i != m_lstValue.Count - 1)
                        {
                            writer.Write(",");
                        }
                    }

                    writer.Write("]");
                }
                else
                {
                    string strSpace = JsonNode.GenerateSpace(nLevel * 2);
                    writer.Write("[\n");

                    for (int i = 0; i < m_lstValue.Count; ++i)
                    {
                        m_lstValue[i].Serialize(writer, nLevel+1, true);
                        if (i != m_lstValue.Count - 1)
                        {
                            writer.Write(",\n");
                        }
                        else
                        {
                            writer.Write("\n");
                        }
                    }

                    writer.Write("{0}]", new object[]{strSpace});
                }

            }
        }
    }

    public class RareJson
    {
        /// <summary>
        /// 解析Json
        /// </summary>
        /// <param name="strJsonFile">json file name</param>
        /// <returns></returns>
        public static JsonNode ParseJsonFile(string strJsonFile)
        {
            StreamReader reader = new StreamReader(strJsonFile);
            if( reader != null )
            {
                string strJson = reader.ReadToEnd();
                reader.Close();
                return ParseJson(strJson);
            }
            return new JsonNode();
        }

        // Json解析，参考SimpJson
        public static JsonNode ParseJson(string strJson)
        {
            strJson = strJson.Trim();

            Stack<JsonNode> stack = new Stack<JsonNode>();
            JsonNode ctx = null;
            int i = 0;
            string Token = "";
            string TokenName = "";
            bool QuoteMode = false;
            while (i < strJson.Length)
            {
                switch (strJson[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token += strJson[i];
                            break;
                        }
                        stack.Push(new JsonObject());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JsonArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token += strJson[i];
                            break;
                        }

                        stack.Push(new JsonArray());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JsonArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token += strJson[i];
                            break;
                        }
                        if (stack.Count == 0)
                        {
                            throw new Exception("JSON Parse: Too many closing brackets");
                        }

                        stack.Pop();
                        if (Token != "")
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JsonArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token += strJson[i];
                            break;
                        }
                        TokenName = Token;
                        Token = "";
                        break;

                    case '"':
                        QuoteMode ^= true;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token += strJson[i];
                            break;
                        }
                        if (Token != "")
                        {
                            if (ctx is JsonArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token += strJson[i];
                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = strJson[i];
                            switch (C)
                            {
                                case 't': Token += '\t'; break;
                                case 'r': Token += '\r'; break;
                                case 'n': Token += '\n'; break;
                                case 'b': Token += '\b'; break;
                                case 'f': Token += '\f'; break;
                                case 'u':
                                    {
                                        string s = strJson.Substring(i + 1, 4);
                                        Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        i += 4;
                                        break;
                                    }
                                default: Token += C; break;
                            }
                        }
                        break;

                    default:
                        Token += strJson[i];
                        break;
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <param name="node">Json根节点</param>
        /// <param name="strJsonFile">Json文件名称</param>
        /// <param name="bFormat">是否启用格式化</param>
        public static void Serialize(JsonNode node, string strJsonFile, bool bFormat = false)
        {
            if( !(node is JsonArray) && !(node is JsonObject) )
            {
                throw new Exception("JSON Serialize: Json root node is not obj or array!");
            }

            StreamWriter writer = new StreamWriter(strJsonFile);
            if( writer == null )
            {
                return;
            }

            node.Serialize(writer, bFormat ? 0 : -1);
            writer.Close();
        }
    }
}
