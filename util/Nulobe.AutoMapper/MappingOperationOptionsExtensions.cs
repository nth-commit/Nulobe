﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapper
{
    public static class MappingOperationOptionsExtensions
    {
        public static void AddServices(
            this IMappingOperationOptions opts,
            IServiceProvider serviceProvider)
        {
            opts.Items.Add(Constants.ServiceProviderKey, serviceProvider);
        }
    }
}
