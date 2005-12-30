using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.State {

	// FIXME how to enable some lines but not others?
	public class PropertyChangeBehaviour {
	
		public PropertyChangeBehaviour(	bool showBubble, String bubbleText, bool systemTrayWarning,
										bool showApplication, String externalProcess, String warningCriteria, bool log ) {
			mShowBubble = showBubble;
			mBubbleText = bubbleText;
			mSystemTrayWarning = systemTrayWarning;
			mShowApplication = showApplication;
			mExternalProcess = externalProcess;
			mWarningCriteria = warningCriteria;
			mLog = log;
		}

		protected bool mShowBubble;
		public bool ShowBubble {
			get{ return mShowBubble; }
		}
		
		protected String mBubbleText;
		public String BubbleText {
			get{ return mBubbleText; }
		}

		protected bool mSystemTrayWarning;
		public bool SystemTrayWarning {
			get{ return mSystemTrayWarning; }
		}

		protected bool mShowApplication;
		public bool ShowApplication {
			get{ return mShowApplication; }
		}

		protected String mExternalProcess;
		public String ExternalProcess {
			get{ return mExternalProcess; }
		}
		
		protected String mWarningCriteria;
		public String WarningCriteria {
			get{ return mWarningCriteria; }
		}
		
		protected bool mLog;
		public bool Log {
			get{ return mLog; }
		}
	}
}
