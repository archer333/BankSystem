﻿using System.ServiceModel;
using System.ServiceModel.Web;
using Service.Models;

namespace Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBankService" in both code and config file together.
    [ServiceContract]
    public interface IBankServiceWeb
    {      
        //======= Service - Service (REST) 
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/accounts/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //HttpResponseMessage ReceiveExternalTransfer(string id, int amount, string from, string title);
        bool ReceiveExternalTransfer(string id, ExternalOperation externalOperation);
    }
}
