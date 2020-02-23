using System;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.JsonSerialization
{
    public class ClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Claim));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);

            // Read the properties which will be used as constructor parameters
            string _issuer, _originalIssuer, _type, _value, _valueType;
            if (jo.ContainsKey("Issuer"))
            { _issuer = (string)jo["Issuer"]; }
            else { _issuer = (string)jo["issuer"]; }
            
            if (jo.ContainsKey("OriginalIssuer"))
            { _originalIssuer = (string)jo["OriginalIssuer"]; }
            else { _originalIssuer = (string)jo["originalIssuer"]; }
            
            if (jo.ContainsKey("Type"))
            { _type = (string)jo["Type"]; }
            else { _type = (string)jo["type"]; }
            
            if (jo.ContainsKey("Value"))
            { _value = (string)jo["Value"]; }
            else { _value = (string)jo["value"]; }
            
            if (jo.ContainsKey("ValueType"))
            { _valueType = (string)jo["ValueType"]; }
            else { _valueType = (string)jo["valueType"]; }

            // Construct the Result object using the non-default constructor
            Claim claim = new Claim(
                issuer: _issuer,
                originalIssuer: _originalIssuer,
                type: _type,
                value: _value,
                valueType: _valueType
                );

            // (If anything else needs to be populated on the result object, do that here)

            // Return the result
            return claim;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}