using Opc.Ua;
using System;

namespace OPC.Helpers
{
    public static class OpcUaBuiltInTypeForDataItem
    {
        /// <summary>
        /// Changes the value in the text box to the data type required for the write operation.
        /// </summary>
        /// <returns>A value with the correct type.</returns>
        public static object Execute(BuiltInType builtInType, string value)
        {
           object valueObj;

            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        valueObj = Convert.ToBoolean(value);
                        break;
                    }

                case BuiltInType.SByte:
                    {
                        valueObj = Convert.ToSByte(value);
                        break;
                    }

                case BuiltInType.Byte:
                    {
                        valueObj = Convert.ToByte(value);
                        break;
                    }

                case BuiltInType.Int16:
                    {
                        valueObj = Convert.ToInt16(value);
                        break;
                    }

                case BuiltInType.UInt16:
                    {
                        valueObj = Convert.ToUInt16(value);
                        break;
                    }

                case BuiltInType.Int32:
                    {
                        valueObj = Convert.ToInt32(value);
                        break;
                    }

                case BuiltInType.UInt32:
                    {
                        valueObj = Convert.ToUInt32(value);
                        break;
                    }

                case BuiltInType.Int64:
                    {
                        valueObj = Convert.ToInt64(value);
                        break;
                    }

                case BuiltInType.UInt64:
                    {
                        valueObj = Convert.ToUInt64(value);
                        break;
                    }

                case BuiltInType.Float:
                    {
                        valueObj = Convert.ToSingle(value);
                        break;
                    }

                case BuiltInType.Double:
                    {
                        valueObj = Convert.ToDouble(value);
                        break;
                    }

                default:
                    {
                        valueObj = value;
                        break;
                    }
            }

            return valueObj;
        }
    }
}
