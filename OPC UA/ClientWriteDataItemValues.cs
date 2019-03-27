using Models;
using Opc.Ua;
using Opc.Ua.Client;
using OPC.Helpers;
using System;
using System.Collections.Generic;


namespace OPC
{
    public static class ClientWriteDataItemValues
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
        public static void Execute(IEnumerable<DataItem> OPCItems, Session session)
        {
            log.Info("Writing DataItem Values.");
         
            try
            {
                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                WriteValueCollection valuesToWrite = CreateWriteValueCollection(OPCItems);

                session.Write(
                    null,
                    valuesToWrite,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, valuesToWrite);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

                if (StatusCode.IsBad(results[0]))
                {
                    throw new ServiceResultException(results[0]);
                }

            }
            catch (Exception exception)
            {
                log.Error("OPC Error Writing Value", exception);
            }
        }

        private static WriteValueCollection CreateWriteValueCollection(IEnumerable<DataItem> dataItems)
        {
            WriteValueCollection valuesToWrite = new WriteValueCollection();

           foreach (DataItem di in dataItems) { 
                WriteValue valueToWrite = new WriteValue();
                valueToWrite.NodeId = di.Tag;
                valueToWrite.AttributeId = Attributes.Value;
                valueToWrite.Value.Value = OpcUaBuiltInTypeForDataItem.Execute((BuiltInType)Enum.Parse(typeof(BuiltInType), di.Type), di.Value);
                valueToWrite.Value.StatusCode = StatusCodes.Good;
                valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
                valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

                valuesToWrite.Add(valueToWrite);
            };
            
            return valuesToWrite;
        }
    }
}
