using System;
using System.Collections.Generic;
using System.Text;

namespace LothianProductions.VoIP.Monitor {
    public class DeviceException : ApplicationException {
        public DeviceException( String message )
            : base( message ) {
        }

        public DeviceException( String message, Exception e )
            : base( message, e ) {
        }
    }

    public class DeviceConfigurationException : DeviceException {
        public DeviceConfigurationException( String message )
            : base( message ) {
        }

        public DeviceConfigurationException( String message, Exception e )
            : base( message, e ) {
        }
    }
    
    public class DeviceAccessUnauthorizedException : DeviceException {
        public DeviceAccessUnauthorizedException( String message, Exception e )
            : base( message, e ) {
        }
    }

    public class DeviceNotRespondingException : DeviceException {
        public DeviceNotRespondingException( String message, Exception e )
            : base( message, e ) {
        }
    }
}
