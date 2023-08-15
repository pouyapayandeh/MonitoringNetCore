using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MonitoringNetCore.Common.Utils;

public class ObjectYamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(object);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            object value;
            if (parser.Current is Scalar scalar)
            {
                value = ParseScalar(scalar);
            }
            else
            {
                throw new InvalidOperationException(parser.Current?.ToString());
            }

            parser.MoveNext();
            return value;
        }

        private static object ParseScalar(Scalar scalar)
        {
            if (scalar.Value == null)
            {
                return null;
            }

            if (scalar.IsQuotedImplicit)
            {
                return scalar.Value;
            }

            if(string.IsNullOrEmpty(scalar.Value))
            {
                return null;
            }

            if (bool.TryParse(scalar.Value, out var booleanValue))
            {
                return booleanValue;
            }
            
            if (!scalar.IsQuotedImplicit && scalar.Value.Equals("no"))
            {
                return false;
            }
            
            if (!scalar.IsQuotedImplicit && scalar.Value.Equals("yes"))
            {
                return true;
            }
            
            if (decimal.TryParse(scalar.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                return decimalValue;
            }

            if (DateTime.TryParse(scalar.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                out var dateTimeValue))
            {
                return dateTimeValue;
            }
            return scalar.Value;
            
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }
    }