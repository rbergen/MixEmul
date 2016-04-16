using MixLib.Type;

namespace MixLib.Instruction
{
	/// <summary>
	/// Instances of this class describe the characteristics of the fieldspec for a specific MIX instruction.
	/// </summary>
	public class MetaFieldSpec
	{
        /// <summary>
        /// Default value for the fieldspec described by this instance, or null if there is none
        /// </summary>
        public FieldSpec DefaultFieldSpec { get; private set; }

        /// <summary>
        /// True if the field is a range, like (0:5), false if it is a value, like (19)
        /// </summary>
        public bool FieldIsRange { get; private set; }

        /// <summary>
        /// Indicates if the fieldspec described by this instance is Forbidden, Optional or Mandatory
        /// </summary>
        public Presences Presence { get; private set; }

        /// <summary>
        /// Creates a MetaFieldSpec that describes that a fieldspec is forbidden
        /// </summary>
        public MetaFieldSpec()
			: this(Presences.Forbidden, false, null)
		{
		}

		/// <summary>
		/// Creates a MetaFieldSpec with a specified presence (Optional, Mandatory or Forbidden, of which only the first two make sense).
		/// It can also specifies if the fieldspec is a range, like (0:5), or a value, like (19).
		/// </summary>
		public MetaFieldSpec(Presences presence, bool fieldIsRange)
			: this(presence, fieldIsRange, null)
		{
		}

		/// <summary>
		/// Creates a MetaFieldSpec that describes an Optional fieldspec, with a specified default value.
		/// It can also specifies if the fieldspec is a range, like (0:5), or a value, like (19).
		/// </summary>
		/// <param name="fieldIsRange"></param>
		/// <param name="defaultFieldSpec"></param>
		public MetaFieldSpec(bool fieldIsRange, FieldSpec defaultFieldSpec)
			: this(Presences.Optional, fieldIsRange, defaultFieldSpec)
		{
		}

		private MetaFieldSpec(Presences presence, bool fieldIsRange, FieldSpec defaultFieldSpec)
		{
			Presence = presence;
			FieldIsRange = fieldIsRange;
			DefaultFieldSpec = defaultFieldSpec;
		}

		public enum Presences
		{
			Forbidden,
			Optional,
			Mandatory
		}
	}
}
