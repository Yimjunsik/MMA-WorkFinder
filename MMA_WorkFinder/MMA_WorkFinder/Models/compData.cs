using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA_WorkFinder.Models
{
    internal class compData
    {
        private string name;
        private string id;

        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }

        public string GetId()
        {
            return id;
        }

        public void SetId(string value)
        {
            id = value;
        }
    }
}
