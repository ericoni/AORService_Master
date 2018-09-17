using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORViewer.Model
{
    public class LBModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        string imagePath; 

        public LBModelBase(string name)
		{
			this.Name = name;
		}

		public LBModelBase(string name, string desc) // obrisi ovaj
		{
			this.Name = name;
			this.Description = desc;
		}

        public LBModelBase(string name, string desc, string imagePath)
        {
            this.Name = name;
            this.Description = desc;
            this.imagePath = imagePath;
        }


        public override string ToString()
		{
			return Name;
		}

        public string DisplayedImage
        {
            get
            {
                return imagePath;
            } //  path relative to the Main.exe file. //@"..\..\..\Images\moreAM.jpg"
            set
            {
                this.imagePath = value;
            }
        }
    }

	public enum LBType
	{
		Permissions,
		DNA_Authorities,
		AOR_Groups,
		AOR_Areas
	}
}
