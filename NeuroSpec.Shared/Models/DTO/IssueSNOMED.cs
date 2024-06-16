﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeuroSpec.Shared.Models.DTO
{
    public class IssueSNOMED: Issue
    {
        public string SNOMEDID { get; set; }
        public string SNOMEDName { get; set; }
        public string SNOMEDDescription { get; set; }

    }
}
