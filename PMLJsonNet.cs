using System;
using System.Collections;
using System.IO;
using Aveva.Core.PMLNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PMLJsonNet
{
    /// <summary>
    /// Provides an interface to process JSON in PML.
    /// </summary>
    [PMLNetCallable()]
    public class PMLJsonNet
    {
        private JToken token;

        [PMLNetCallable()]
        public PMLJsonNet()
        {
            token = JValue.CreateNull();
        }

        public PMLJsonNet(JToken t)
        {
            token = t;
        }

        /// <summary>
        /// Initializes a new instance with the specified JSON string.
        /// </summary>
        /// <param name="json">JSON string</param>
        [PMLNetCallable()]
        public PMLJsonNet(string json)
        {
            Parse(json);
        }

        [PMLNetCallable()]
        public void Assign(PMLJsonNet that)
        {
            token = that.token;
        }

        public override string ToString()
        {
            return token.ToString(Formatting.None, null);
        }

        /// <summary>
        /// Loads data from a JSON string.
        /// </summary>
        /// <param name="json">JSON string</param>
        [PMLNetCallable()]
        public void Parse(string json)
        {
            try
            {
                token = JToken.Parse(json);
            }
            catch (JsonReaderException ex)
            {
                throw new PMLNetException(1000, 1, ex.Message);
            }
        }

        /// <summary>
        /// Reads data from a JSON file.
        /// </summary>
        /// <param name="filename">File name</param>
        [PMLNetCallable()]
        public void ReadFile(string filename)
        {
            try
            {
                using (var sr = new StreamReader(Environment.ExpandEnvironmentVariables(filename)))
                using (var reader = new JsonTextReader(sr))
                {
                    token = JToken.ReadFrom(reader);
                }
            }
            catch (JsonReaderException ex)
            {
                throw new PMLNetException(1000, 2, ex.Message);
            }
        }

        /// <summary>
        /// Writes data to the specified file with the specified indentation width.
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="indent">Indentation width</param>
        [PMLNetCallable()]
        public void WriteFile(string filename, double indent)
        {
            using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
            using (var writer = new JsonTextWriter(sw))
            {
                if (indent >= 1)
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = (int)indent;
                }
                token.WriteTo(writer);
            }
        }

        /// <summary>
        /// Returns the data type.
        /// </summary>
        /// <returns>Data type</returns>
        [PMLNetCallable()]
        public string GetValueType()
        {
            return token.Type.ToString();
        }

        /// <summary>
        /// Returns the data value.
        /// </summary>
        /// <returns>Data value</returns>
        [PMLNetCallable()]
        public string GetValue()
        {
            if (token is JValue)
            {
                return token.ToString();
            }
            return token.ToString(Formatting.None, null);
        }

        /// <summary>
        /// Sets the data value as string.
        /// </summary>
        /// <param name="val">Data value</param>
        [PMLNetCallable()]
        public void SetValue(string val)
        {
            token = new JValue(val);
        }

        /// <summary>
        /// Sets the data value as bool.
        /// </summary>
        /// <param name="val">Data value</param>
        [PMLNetCallable()]
        public void SetValue(bool val)
        {
            token = new JValue(val);
        }

        /// <summary>
        /// Sets the data value as double or int.
        /// </summary>
        /// <param name="val">Data value</param>
        [PMLNetCallable()]
        public void SetValue(double val)
        {
            if ((int)val == val)
            {
                token = new JValue((int)val);
            }
            else
            {
                token = new JValue(val);
            }
        }

        /// <summary>
        /// Returns the number of the array items.
        /// </summary>
        /// <returns>Number of array items</returns>
        [PMLNetCallable()]
        public double GetArraySize()
        {
            if (token.Type == JTokenType.Null)
            {
                return 0;
            }
            if (token is JArray arr)
            {
                return arr.Count;
            }
            throw new PMLNetException(1000, 11, "This object data is not Array.");
        }

        /// <summary>
        /// Returns the array item with the specified index.
        /// </summary>
        /// <param name="idx">Array index</param>
        /// <returns>A new <see cref="PMLJsonNet"/> instance</returns>
        [PMLNetCallable()]
        public PMLJsonNet GetArrayItem(double idx)
        {
            if (token is JArray arr)
            {
                return new PMLJsonNet(arr[(int)idx - 1]);
            }
            throw new PMLNetException(1000, 12, "This object data is not Array.");
        }

        /// <summary>
        /// Adds the specified data as the array item.
        /// </summary>
        /// <param name="that"><see cref="PMLJsonNet"/> instance</param>
        [PMLNetCallable()]
        public void AddArrayItem(PMLJsonNet that)
        {
            if (token.Type == JTokenType.Null)
            {
                token = new JArray();
            }
            if (token is JArray arr)
            {
                arr.Add(that.token);
                return;
            }
            throw new PMLNetException(1000, 13, "This object data is not Array.");
        }

        /// <summary>
        /// Returns the names of the object properties.
        /// </summary>
        /// <returns>Names of object properties</returns>
        [PMLNetCallable()]
        public Hashtable GetPropertyNames()
        {
            var result = new Hashtable();
            if (token.Type == JTokenType.Null)
            {
                return result;
            }
            if (token is JObject obj)
            {
                double idx = 1;
                foreach (var p in obj.Properties())
                {
                    result.Add(idx++, p.Name);
                }
                return result;
            }
            throw new PMLNetException(1000, 21, "This object data is not Object.");
        }

        /// <summary>
        /// Returns the object property with the specified name.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns>A new <see cref="PMLJsonNet"/> instance</returns>
        [PMLNetCallable()]
        public PMLJsonNet GetProperty(string name)
        {
            if (token is JObject obj)
            {
                return new PMLJsonNet(obj[name]);
            }
            throw new PMLNetException(1000, 22, "This object data is not Object.");
        }

        /// <summary>
        /// Adds the specified data as the object property.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="that"><see cref="PMLJsonNet"/> instance</param>
        [PMLNetCallable()]
        public void AddProperty(string name, PMLJsonNet that)
        {
            if (token.Type == JTokenType.Null)
            {
                token = new JObject();
            }
            if (token is JObject obj)
            {
                obj.Add(name, that.token);
                return;
            }
            throw new PMLNetException(1000, 23, "This object data is not Object.");
        }
    }
}