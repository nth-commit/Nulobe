﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nulobe.Utility.Validation
{
    public class StringNotNullOrEmptyAttribute : StringLengthAttribute
    {
        public StringNotNullOrEmptyAttribute() : base(int.MaxValue)
        {
            MinimumLength = 1;
        }
    }
}