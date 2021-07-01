using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitInitTest.Entities.Models
{
    public class SysConfig
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SysConfigId { get; set; }

        //Default Properties
        public string AppName { get; set; }

        public string DeveloperName { get; set; }
        public string DeveloperEmail { get; set; }
        public string BusinessOwnerName { get; set; }
        public string BusinessOwnerEmail { get; set; }

        public string AppFromName { get; set; }
        public string AppFromEmail { get; set; }

        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }

        //Custom properties
        public string UserAdministratorName { get; set; }

        public string UserAdministratorEmail { get; set; }
        public bool IsDebug { get; set; }
    }
}