using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.Monitor {
    public class DeviceNotRespondingException : ApplicationException {
        public DeviceNotRespondingException( String message, Exception e )
            : base( message, e ) {
        }
    }
}
