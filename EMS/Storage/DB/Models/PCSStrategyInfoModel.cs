using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Storage.DB.Models
{
    public class PCSStrategyInfoModel
    {
        [Key]
        public int ID {  get; set; }
        public string Name { get; set; }
        public string Mode {  get; set; }
        public string Value {  get; set; }
        public string StartTime {  get; set; }

        public string EndTime { get; set; }
    }
}
