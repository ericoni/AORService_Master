using FTN.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSDB.Model
{
	public class HistorianItem
	{
		/// <summary>
		/// Id koji baza sama generise
		/// </summary>
		private int id;

		/// <summary>
		/// Gid
		/// </summary>
		private long globalId;

		/// <summary>
		/// Vrednost akvtivne ili reaktivne snage
		/// </summary>
		private float value;

        /// <summary>
        /// value + derFlex
        /// </summary>
        private float increase;

        /// <summary>
        /// value - derFlex
        /// </summary>
        private float decrease;

		/// <summary>
		/// Tip - da li se radi o aktivnoj ili reaktivnoj snazi
		/// </summary>
		private PowerType type;

		/// <summary>
		/// Vreme kada je nastalo merenje
		/// </summary>
		private long timestamp;

		/// <summary>
		/// Operacija - da li je insert, update ili delete
		/// </summary>
		private Operation operation;

		/// <summary>
		/// Get/Set za id
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id
		{
			get
			{
				return id;
			}

			set
			{
				id = value;
			}
		}

		/// <summary>
		/// Get/Set za value
		/// </summary>
		public float Value
		{
			get
			{
				return value;
			}

			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// Get/Set za tip
		/// </summary>
		public PowerType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}

		/// <summary>
		/// Get/Set za timestamp
		/// </summary>
		public long Timestamp
		{
			get
			{
				return timestamp;
			}

			set
			{
				timestamp = value;
			}
		}

		/// <summary>
		/// Get/Set za operaciju
		/// </summary>
		public Operation Operation
		{
			get
			{
				return operation;
			}

			set
			{
				operation = value;
			}
		}

		/// <summary>
		/// Get/Set za gid
		/// </summary>
		public long GlobalId
		{
			get
			{
				return globalId;
			}

			set
			{
				globalId = value;
			}
		}

        /// <summary>
        /// Get/Set
        /// </summary>
        public float Increase
        {
            get
            {
                return increase;
            }

            set
            {
                increase = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// </summary>
        public float Decrease
        {
            get
            {
                return decrease;
            }

            set
            {
                decrease = value;
            }
        }
    }
}
