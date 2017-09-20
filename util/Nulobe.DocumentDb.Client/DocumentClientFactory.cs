﻿using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulobe.DocumentDb.Client
{
    public class DocumentClientFactory : IDocumentClientFactory
    {
        public DocumentClient Create(string connectionString)
        {
            return DocumentClientAccount.Parse(connectionString);
        }

        public DocumentClient Create(IDocumentDbConnectionSpec connectionSpec)
        {
            return new DocumentClient(connectionSpec.ServiceEndpoint, connectionSpec.AuthorizationKey);
        }

        DocumentClient IDocumentClientFactory.Create(IDocumentDbConnectionSpec connectionSpec, bool readOnly)
        {
            return new DocumentClient(connectionSpec.ServiceEndpoint, connectionSpec.AuthorizationKey);
        }

        DocumentClient IDocumentClientFactory.Create(bool readOnly)
        {
            throw new NotImplementedException();
        }
    }
}
