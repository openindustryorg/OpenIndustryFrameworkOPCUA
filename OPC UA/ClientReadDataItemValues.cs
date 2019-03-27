using Models;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;

namespace OPC
{
    public static class ClientReadDataItemValues
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
        public static DataValueCollection Execute(List<DataItem> OPCItems, Session session)
        {
            log.Info("Getting DataItem Values.");
            DataValueCollection results = null;

            try
            {
                ReadValueIdCollection nodesToRead = CreateReadValueIdCollection(OPCItems);
                DiagnosticInfoCollection diagnosticInfos = null;

                session.Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);
            }
            catch (Exception exception)
            {
                //ClientUtils.HandleException("Error Writing Value", exception);
            }

            return results;
        }

        private static ReadValueIdCollection CreateReadValueIdCollection(IEnumerable<DataItem> OPCItems)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            foreach (DataItem di in OPCItems)
            {
                ReadValueId nodeToRead = new ReadValueId();
                nodeToRead.NodeId = di.Tag;
                nodeToRead.AttributeId = Attributes.Value;

                nodesToRead.Add(nodeToRead);
            }
            return nodesToRead;
        }
    }
}
