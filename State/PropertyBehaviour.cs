using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.State {
	public class PropertyBehaviour {
		public PropertyBehaviour( String label, Dictionary<String, PropertyChangeBehaviour> changeBehaviours ) {
			mLabel = label;
			mPropertyChangeBehaviours = changeBehaviours;
		}
		
		protected String mLabel;
		public String Label {
			get{ return mLabel; }
		}
		
		protected Dictionary<String, PropertyChangeBehaviour> mPropertyChangeBehaviours;
		public Dictionary<String, PropertyChangeBehaviour> PropertyChangeBehaviours {
			get{ return mPropertyChangeBehaviours; }
			set{ mPropertyChangeBehaviours = value; }
		}
	}
}
