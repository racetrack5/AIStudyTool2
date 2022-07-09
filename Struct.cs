using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIStudyTool2
{
    public class USleepOutput
    {
        private string? p_Epoch { get; set; }
        public string? Epoch
        {
            get { return this.p_Epoch; }
            set
            {
                if (this.p_Epoch != value)
                {
                    this.p_Epoch = value;
                }
            }
        }
    }
}
