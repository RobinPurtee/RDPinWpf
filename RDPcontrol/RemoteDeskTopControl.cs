using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace RDPcontrol
{
    public partial class RemoteDeskTopControl : UserControl
    {


        #region out going events and support classes
        /// <summary>
        /// class to hold the disconnect reason for the disconnect event
        /// </summary>
        public class DisconnectEventArgs : EventArgs
        {
            public int Reason;
            public DisconnectEventArgs(int reason)
            {
                Reason = reason;
            }             
        }
        /// <summary>
        /// OnDisconnect handler delegate
        /// </summary>
        /// <param name="sender">the object that fired the event</param>
        /// <param name="args">the reason for the disconnect</param>
        public delegate void DisconnectedHandler(object sender, DisconnectEventArgs args);

        /// <summary>
        /// Fired on disconnecting from the remote host
        /// </summary>
        public event DisconnectedHandler OnDisconnected;



        #endregion

        #region Public properties
        private const short RDP_CLIENT_DISCONNECTED = 0;
        private const short RDP_CLIENT_CONNECTED = 1;
        private const short RDP_CLIENT_CONNECTING = 2; 
        /// <summary>
        /// Test if there is currently a connected session
        /// </summary>
        public bool IsConnected { get { return axRdpClient != null && axRdpClient.Connected == RDP_CLIENT_CONNECTED; } } 

        /// <summary>
        /// Test if a session is currently being connected 
        /// </summary>
        public bool IsConnecting { get { return axRdpClient != null && axRdpClient.Connected == RDP_CLIENT_CONNECTED; } }


        #endregion


        /// <summary>
        /// default constructor
        /// </summary>
        public RemoteDeskTopControl()
        {
            InitializeComponent();

            axRdpClient.OnConnecting += new System.EventHandler(axRdpClient_OnConnecting);
            axRdpClient.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(axRdpClient_OnDisconnected);

        }

        /// <summary>
        /// Initiates a connection using the properties currently set on the control.
        /// </summary>
        public void Connect()
        {
            axRdpClient.Connect();
        }

        /// <summary>
        /// Reconnects to the remote session with the new desktop width and height.
        /// </summary>
        /// <param name="width">The new desktop width, in pixels. </param>
        /// <param name="height">The new desktop height, in pixels. </param>
        public void Reconnect(uint width, uint height)
        {
            axRdpClient.Reconnect(width, height);
        }

        /// <summary>
        /// Requests a graceful shutdown of the Remote Desktop ActiveX control.
        /// </summary>
        /// <returns>
        /// true if the control has shutdown gracefully.
        /// false if you need to wait for a 
        /// </returns>
        public bool RequestClose()
        {

            MSTSCLib.ControlCloseStatus status = axRdpClient.RequestClose();
            return status == MSTSCLib.ControlCloseStatus.controlCloseCanProceed;
            {

            }
        }
        
        /// <summary>
        /// Disconnects the active connection.
        /// </summary>
        /// <remarks>Will throw if there is not currently a connection</remarks>
        public void Disconnect()
        {
            axRdpClient.Disconnect();
        }

        /// <summary>
        /// Retrieves the error description for the session disconnect events
        /// </summary>
        /// <param name="disconnectReason">The disconnect reason.</param>
        /// <param name="extendedDisconnectReason">Provides detailed information about why a disconnect was initiated.</param>
        /// <returns></returns>
        public string GetErrorDescription(uint disconnectReason, uint extendedDisconnectReason)
        {
            return axRdpClient.GetErrorDescription(disconnectReason, extendedDisconnectReason);
        }

        /// <summary>
        /// Retrieves the status text for the specified status code.
        /// </summary>
        /// <param name="statusCode">A unsigned int that specifies the status code to retrieve the text for.</param>
        /// <returns></returns>
        public string GetStatusText(uint statusCode)
        {
            return axRdpClient.GetStatusText(statusCode);
        }


        // Note: Virtiual channels will not be supported by this control

        // Creates a client-side virtual channel object for each specified virtual channel name.
        //    public void CreateVirtualChannels(string newVChannels)


        // Retrieves the options set for a virtual channel.
        //   GetVirtualChannelOptions
        // Sends data to the RD Session Host server over a virtual channel that was created previously by using the CreateVirtualChannels method.
        //   SendOnVirtualChannel

        // Causes an action to be performed in the remote session.
        //   SendRemoteAction

        // Sets the virtual channel options for the Remote Desktop ActiveX control.
        //   SetVirtualChannelOptions




        #region RDP client event handlers
        /// <summary>
        /// Event handler for OnConnecting events from the RDP client control
        /// </summary>
        /// <param name="sender">The RDP client control</param>
        /// <param name="e"> unused parameter</param>
        private void axRdpClient_OnConnecting(object sender, EventArgs e)
        {
            Debug.WriteLine("RemoteDeskTopControl.OnConnecting: EvnetArgs type {0}", e.ToString());
        }


        private void axRdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Debug.WriteLine("OnDisconnected event");
            OnDisconnected(this, new DisconnectEventArgs(e.discReason));
        }
        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if(IsConnected)
            {


            }
        }

        private void axRdpClient_OnConnecting_1(object sender, EventArgs e)
        {

        }
    }
}
