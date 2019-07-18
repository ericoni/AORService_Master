using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
    public class Alarm : BindableBase
    {
        private string username;
        private string details;
        private DateTime timestamp;

        public Alarm(string username, string details)
        {
            this.Username = username;
            this.Details = details;
            this.Timestamp = timestamp;
        }

        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public string Details
        {
            get { return details; }
            set
            {
                if (details != value)
                {
                    details = value;
                    OnPropertyChanged("Details");
                }
            }
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set
            {
                if (timestamp != value)
                {
                    timestamp = value;
                    OnPropertyChanged("Timestamp");
                }
            }
        }
    }
}
