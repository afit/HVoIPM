/// ***************************************************************************
/// This file and its related files are protected by copyright and intellectual
/// property disclaimers. Use of it by you or your organisation must be covered
/// by prior agreement, and may involve NDA and other restriction. Duplication,
/// distribution or disclosure of any part of this file are likely prohibited,
/// and violations of the licensing terms will be pursued by law.
/// ***************************************************************************
/// Further details of the stock LothianProductionsCommon usage agreement are
/// available at: http://www.lothianproductions.co.uk/lpc/#license
/// ***************************************************************************
using System;
using System.Collections.Generic;
using System.Text;

using LothianProductions.VoIP.State;

namespace LothianProductions.VoIP.Monitor {

	/// <summary>
	/// Top level device monitor interface. See how the device monitor
	/// and its subclasses flesh it out.
	/// </summary>
	public interface IDeviceMonitor {
		void Run();
		Device GetDeviceState();
		
		String Name {
			get;
		}
	}
}
