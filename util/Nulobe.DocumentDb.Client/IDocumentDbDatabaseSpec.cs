﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulobe.DocumentDb.Client
{
    public interface IDocumentDbDatabaseSpec
    {
        string DatabaseName { get; set; }
    }
}