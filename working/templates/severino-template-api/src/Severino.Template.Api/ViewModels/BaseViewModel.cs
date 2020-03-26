using System;
using System.Collections.Generic;

namespace Severino.Template.Api.ViewModels
{
    public class BaseViewModel
    {
        public MetadataViewModel Meta { get; set; }
        public Array Records { get; set; }
        public Array Errors { get; set; }
    }

    public class MetadataViewModel
    {
        public string Server { get; set; } = Environment.MachineName;
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public int? RecordCount { get; set; }
    }
}